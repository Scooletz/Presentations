background-image: url(img/Ethereum.png)
background-size: cover

# Faster Ether

## a.k.a War Stories from optimizing Ethereum

## Szymon Kulec @Scooletz

---

background-image: url(img/flight-to-mars.jpg)
background-size: cover

## Chapter 1: Discovering New Worlds

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

**We have it!**

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

background-image: url(img/Ethereum.png)
background-size: cover

# Faster Ether

## Szymon Kulec @Scooletz

### Thank you!