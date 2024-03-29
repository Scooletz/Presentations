﻿background-image: url(img/Ethereum.png)
background-size: cover

# Faster Ether

## a.k.a War Stories from optimizing Ethereum

## Szymon Kulec @Scooletz

---

background-image: url(img/logos.png)
background-size: cover

---

background-image: url(img/flight-to-mars.jpg)
background-size: cover

## Discovering New Worlds

---

## Discovering New Worlds

1. You select a new planet... 🪐

--

1. You pay for the fuel... ⛽

--

1. Your ship starts... 🚀

--

1. An Ethereum node suffers a memory leak 💥

--

This is what happened with **Dark Forest** game, which is

> an MMO space-conquest game where players discover and conquer planets in an infinite, procedurally-generated, cryptographically-specified universe!

---

## Discovering New Worlds - more

To solve the mystery, a usual setup was required:

--

1. having a **reproduction scenario**:

    - loading the game against a locally setup node
--

1. using a **measurement tool**

    - dotMemory/dotTrace from JetBrains
--

1. finding a **point** that makes a difference

    - a.k.a. WHAT causes it

???

We need to go deeper!

---

## Discovering New Worlds - EVM

- Ethereum has an internal Virtual Machine (EVM)

--

- EVM is stack based (as .NET Runtime) and uses `OpCodes` (.NET IL)

--

- **32 bytes** - a native size word

--

- accessing top 16 elements in the stack is cheap and easy:
  - `DUP_N` - duplicates specific item on the top
  - `SWAP_N` - swaps specific item with the top
  - `PUSH_N` - pushes an item on the stack

--

- when random memory is needed:
  - `MLOAD` - pops an `offset` and loads a slice of memory from `offset` to `offset + 32`
  - `MSTORE offset` - pops an `offset` and a `value` and stores it under `offset`

---

## Discovering New Worlds - more EVM

- stack is cheap

--

- high offsets of memory are pricey
  - the cost of offset `n` is correlated with `O(n*n)`

--

- apps' creators tend to optimize for their memory usage

--

- all costs are paid in **gas** which actual price depends on the demand

---

## Discovering New Worlds - discovery

```csharp
var memory = ArrayPool<byte>.Rent(int minimumLength) // hot line 🔥

// used in the EVM memory
public class EvmPooledMemory
{
    public const int WordSize = 32;
    static readonly ArrayPool<byte> Pool = ArrayPool<byte>.Shared;
}
```

--

- `ArrayPool<T>.Shared` - the default pool for reusable arrays `T[]`

--

- here, `Rent` was responsible for all the CPU + allocations

--

- what's wrong with this default pool anyway?

---

## Discovering New Worlds - reasoning

- there's nothing wrong with `ArrayPool<T>.Shared`

--

- **Dark Forest** app was requesting more than 1MB of memory

--

- EVM memory was requesting more than 1MB `byte[]` from the pool

--

- which is fine for `ArrayPool<T>.Shared`....

--

- but for `size > 1MB` it will allocate an array every single time and won't pool it

--

**Let's do it!**

---

## Discovering New Worlds - solution

Pull Request: [LargeArrayPool introduced #2493](https://github.com/NethermindEth/nethermind/pull/2493)

```csharp
public sealed class LargerArrayPool : ArrayPool<byte>
{
    static readonly LargerArrayPool s_instance = new LargerArrayPool();
    public static new ArrayPool<byte> Shared => s_instance;
    public const int LargeBufferSize = 8 * 1024 * 1024;
}
```

--

- pools a large 8MB per one core only

--

- defaults to `ArrayPool<T>.Shared` for smaller ones

--

- aligned with modern cloud `SKU`s

---

## Discovering New Worlds - summary

- **source**: a problematic scenario with CPU + memory overhead

- **tools**: profiling with dotTrace & dotMemory

- **solution**: an implementation of a custom `ArrayPool` for big arrays (more than 1MB)

---

background-image: url(img/math.jpg)
background-size: cover

## Math with uint256

---

## Math with uint256 - details

- EVM uses **32 bytes** (256 bits) long words

--

- EVM is **Turing complete**, it performs regular arithmetics

--

- EVM provides usual arithmetic operations:

  - `ADD`  _a + b_
  - `SUB`  _a - b_
  - `MUL`  _a * b_
  - `DIV`  _a / b_
  - `MOD`  _a % b_
  - `EXP`  _a<sup>b</sup>_

--

- but also binary operations
  - `AND` _a & b_
  - `XOR` _a ^ b_
  - `NOT` _~a_

---

## Math with uint256 - opportunities

- **pure CPU** computations without allocations

--

- use **BenchmarkDotNet** to provide benchmarks

--

- do low level work in a specific location...

---

## Math with uint256 - struct

```csharp
public readonly struct UInt256 : IComparable, IComparable<UInt256>
{
  public readonly ulong u0, u1, u2, u3;
  
  public static bool operator==(in UInt256 a, in UInt256 b)=>a.Equals(b);
  
  public bool Equals(UInt256 other)
  {
      return u0 == other.u0 && 
            u1 == other.u1 && 
            u2 == other.u2 && 
            u3 == other.u3;
  }
  // and many more methods...
}

```

---

## Math with uint256 - IsZero

```csharp
public readonly struct UInt256
{
  public static readonly UInt256 Zero = new (0UL, 0UL, 0UL, 0UL);

* public bool IsZero => this == Zero;
}
```

- used in many checks

--

- includes multiple comparisons

--

- `.IsZero` is true only when **all fields** are 0

--

- if all fields are 0, then their ORed value should be 0 as well...

---

## Math with uint256 - IsZero - optimized

Pull Request: [Optimized IsZero and IsOne #12](https://github.com/NethermindEth/int256/pull/12)

```csharp
public readonly struct UInt256
{
  public bool IsZero => this == Zero; // old

* public bool IsZero => (u0 | u1 | u2 | u3) == 0;
}
```

---

## Math with uint256 - IsZero - results

Before & after

| Method |                Value |      Mean |     Error |    StdDev |
|------- |--------------------- |----------:|----------:|----------:|
| **IsZero** before |     **0** | **1.3511 ns** | **0.0703 ns** | **0.1074 ns** |
| **IsZero** after |      **0** | **0.0506 ns** | **0.0322 ns** | **0.0301 ns** |
| **IsZero** before |     **1** | **0.7594 ns** | **0.0538 ns** | **0.0552 ns** |
| **IsZero** after |      **1** | **0.0517 ns** | **0.0330 ns** | **0.0308 ns** |

---

## Math with uint256 - IsZero - summary

- **source**: noticing potential gains with low work overhead

- **tools**: BenchmarkDotNet

- **solution**: a binary operator based implementation

---

## Math with uint256 - Multiplication

- not that simple

--

- requires dealing with carry over

--

- requires multiple `ulong` multiplications

---

## Math with uint256 - Multiplication - impl

```csharp
public static void Multiply(in UInt256 x, in UInt256 y, out UInt256 res){
  ulong carry, res1, res2, res3, r0, r1, r2, r3;
  
  (carry, r0) = Multiply64(x[0], y[0]);
  UmulHop(carry, x[1], y[0], out carry, out res1);
  UmulHop(carry, x[2], y[0], out carry, out res2);
  res3 = x[3] * y[0] + carry;
  
  UmulHop(res1, x[0], y[1], out carry, out r1);
  UmulStep(res2, x[1], y[1], carry, out carry, out res2);
  res3 = res3 + x[2] * y[1] + carry;
  
  UmulHop(res2, x[0], y[2], out carry, out r2);
  res3 = res3 + x[1] * y[2] + carry;
  
  r3 = res3 + x[0] * y[3]; res = new UInt256(r0, r1, r2, r3);
}
```

---

## Math with uint256 - Multiplication - optimized

Pull Request: [UInt256 Multiply speed up #11](https://github.com/NethermindEth/int256/pull/11)

```csharp
UmulHop(carry, x[1], y[0], out carry, out res1);

*UmulHop(carry, Unsafe.Add(ref rx, 1), ry, out carry, out res1);
```

Replaced indexing with direct access to a specific part

---

## Math with uint256 - Multiplication - results

|           Method |      Mean |    Error |   StdDev |   Code Size |
|----------------- |----------:|---------:|---------:|---------:|---------:|
| **Multiply_UInt256** before | **62.56 ns** | **1.203 ns** | **3.169 ns** | **1223 B** |
| **Multiply_UInt256** after | **27.05 ns** | **0.399 ns** | **0.373 ns** |  **920 B** |

Twice faster than before!

---

## Math with uint256 - Multiplication - summary

- **source**: noticing indexing logic in a compute heavy component

- **tools**: BenchmarkDotNet

- **solution**: indexer turned into a direct access

---

## Math with uint256 - Multiply64

```csharp
[MethodImpl(MethodImplOptions.AggressiveInlining)]
internal static (ulong high, ulong low) Multiply64(ulong a, ulong b)
{
  ulong a0 = (uint)a;
  ulong a1 = a >> 32;
  ulong b0 = (uint)b;
  ulong b1 = b >> 32;
  ulong carry = a0 * b0;
  uint r0 = (uint)carry;
  carry = (carry >> 32) + a0 * b1;
  ulong r2 = carry >> 32;
  carry = (uint)carry + a1 * b0;
  var low = carry << 32 | r0;
  var high = (carry >> 32) + r2 + a1 * b1;
* return (high, low);           // 128bit ulong returned in 2 parts
}
```

---

## Math with uint256 - Multiply64 - optimized

Pull Request: [Multiply64 optimized with Math.BigMul #15](https://github.com/NethermindEth/int256/pull/15)

```csharp
[MethodImpl(MethodImplOptions.AggressiveInlining)]
internal static (ulong high, ulong low) Multiply64(ulong a, ulong b)
{
  // non-pro move: just use a new Math.BigMul method 😂
* ulong high = Math.BigMul(a, b, out ulong low);
  return (high, low);
}
```

--

Could you discuss the implementation **a bit more**? 🙄

--

Sure. 😜

---

## Math with uint256 - Multiply64 - Math.cs

```csharp
public static unsafe ulong BigMul(ulong a, ulong b, out ulong low)
{
* if (Bmi2.X64.IsSupported)
  {
    ulong tmp;
    ulong high = Bmi2.X64.MultiplyNoFlags(a, b, &tmp);
    low = tmp;
    return high;
  }
* else if (ArmBase.Arm64.IsSupported)
  {
    low = a * b;
    return ArmBase.Arm64.MultiplyHigh(a, b);
  }
* return SoftwareFallback(a, b, out low);
}
```

---

## Math with uint256 - Multiply64 - results

|           Method |      Mean |    Error |   StdDev |
|----------------- |----------:|---------:|---------:|
| **Multiply_UInt256** before| **28.50 ns** | **0.537 ns** | **0.996 ns** |
| **Multiply_UInt256** after | **19.08 ns** | **0.266 ns** | **0.236 ns** |

- 2 times faster than before!

- **4 times faster** than the initial multiplication!

---

## Math with uint256 - Multiply64 - summary

- **source**: looking for potential Math counterparts in .NET5

- **tools**: BenchmarkDotNet

- **solution**: replacing a custom implementation with a call to .NET BCL

---

background-image: url(img/starks.jpg)
background-size: cover

## STARKs

---

## STARKs - current state & near future

- Ethereum version 1 works with **Proof of Work** which

  - consumes electricity ⚡
  - requires generic purpose GPUs for computing 🖥️

--

- It results in people repeatedly sharing information about
  - high impact on environment (_C0<sub>2</sub>_ argument)
  - shortage of good GPUs (_I can't play games_ argument)

--

- It will be addressed by **Ethereum 2**
  - scales up Ethereum by introducing **Shards**
  - replaces the Proof of Work with **Proof of Stake** (no GPUs)
  - C0<sub>2</sub> attention will go again to cars, planes, meat
  - GPUs will be available

---

## STARKs - Ethereum current execution model

- all transactions and contracts are executed on **all nodes**

--

- for _N_ nodes and _T_ execution steps, the total cost of **computation is _N * T_**

--

- are there any global optimizations possible? 🧠

--

- is it possible to **compute more by computing less**? 😐

--

It's not the best question to answer. Let's rephrase.

---

## STARKs - the question

A better question, splitting **computation** from **verification**

> Is it possible to compute something outside of Ethereum and have its execution verified on-chain, inside Ethereum?

and

> Would that be more efficient?

--

**Yes**, using **STARKs**!

---

## STARKs - introduction

Consider two types of entities

- **prover** - running a program and generating a **proof** of computation that can be verified

- **verifier** - verifying the proof and ensuring that computation happened with the specific result

--

Consider _T_ as the number of steps to be executed, it's possible with STARKs to create...

--

- **prover** that runs in _O(T log(T))_

- **verifier** that runs in _O(log<sup>K</sup>(T))_, a.k.a polylog

--

This makes the **verifier exponentially cheaper** than _T_ 🤩

---

## STARKs - the beauty and the math

Given:

- _O(T log(T))_ - the cost of running **a single prover** somewhere, even on your machine

- _O(N * log<sup>K</sup>(T))_ - the cost of **running verification** on _N_ Ethereum nodes

--

We just **saved a lot** of computation, because:

<center><em>O(N * log<sup>K</sup>(T))</em> is less than <em>O(N * T)</em> </center>

---

## STARKs - the beauty and more math

This means that:

- **Ethereum can be used to verify** of the computation

- computation itself can be run on a machine that potentially is malicious

--

With SNARKs a malicious proof will be found and discarded.

**You can't cheat it! 💥**

---

## STARKs - summary

- **source**: looking for huge scalability wins, embracing Ethereum nature

- **tools**: so-called **moon math**

- **solution**: splitting the computation into two different parts: running (proving) and verification

---

## Faster Ether - summary

We saw that optimizations and performance work can be done on multiple levels:

- a failing scenario with a memory leak from `ArrayPool<byte>.Shared`

--

- benchmark based, computing optimizations with **IsZero** and **multiplication**

--

- replacing a custom implementation with BCL component call with **Multiply64**

--

- leveraging the architecture assumptions with **STARKs** and split between computing and verifying

---

## Accounts to follow

- [@dotnetosorg](https://twitter.com/dotnetosorg/) for general .NET awesomeness

--

- [@nethermindeth](twitter.com/nethermindeth) for Ethereum related news with .NET hats on

--

- [@StarkWareLtd](https://twitter.com/StarkWareLtd) for STARKs related news

---

background-image: url(img/Ethereum.png)
background-size: cover

# Faster Ether

## Szymon Kulec @Scooletz

### Thank you!