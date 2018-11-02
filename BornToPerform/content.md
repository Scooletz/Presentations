background-image: url(img/born.jpg)
background-size: cover

# Born to Perform

## Szymon Kulec @Scooletz

???

It’s a bird. It’s a plane. No, it’s .NET Core! This sentence once attributed to Superman, now surely can be used for .NET Core.
In this talk, I'll guide you through the features that I, personally, find most interesting and promising. And as we're getting into the performance world, we need to first answer the question, how .NET Core ever came to life.

---

background-image: url(img/laboratory.jpg)
background-size: cover

## No for secret labs, yes for Open Source ?

???

- not only Windows
- yes to Open Source
- no more secret projects

---

background-image: url(img/spaceship.jpg)
background-size: cover

## Spaceship from another planet ?

???

The theory says that it was sent in a space ship from another planet

---

background-image: url(img/space.jpg)
background-size: cover

## Memory&lt;Krypton&gt;

--

- lowering allocation needs

--

- wrapping any kind of memory

--

- splitting in an alloc-free way

--

- pinning

--

- pooling support

---

background-image: url(img/space.jpg)
background-size: cover

## Memory&lt;Krypton&gt; - before

```c#
// allocation
var bytes = new byte[32];
```
--
```c#
// other kinds of memory
var memory = new byte[somePtr]; // 💥💥💥
```
--
```c#
// splitting
var bytes = new otherBytes[16];
Array.Copy(...);
```

---

background-image: url(img/space.jpg)
background-size: cover

## Memory&lt;Krypton&gt; - before

```c#
// pinning
fixed(byte* ptr = bytes)
{
}
```
--
```c#
// other pinning
GCHandle.Alloc(...);
```
--
```c#
// pooling
ArrayPool.Shared.Rent(...);
```

---

background-image: url(img/space.jpg)
background-size: cover

## Memory&lt;Krypton&gt; - now

```c#
// allocation
var memory = new Memory<byte>(array);
```
--
```c#
// other kinds of memory - pinned
var memory = MemoryMarshal
    .CreateFromPinnedArray(...);
```
--
```c#
// other kinds of memory - external
var memory = MemoryManager<T>
    .CreateMemory(...);
```
--
```c#
// splitting
var first4 = memory.Slice(0, 4);
```
---

background-image: url(img/space.jpg)
background-size: cover

## Memory&lt;Krypton&gt; - now

```c#
// pinning
using(var pin = memory.Pin())
{
    pin.Pointer; // 😍😍😍
}
```
--
```c#
// pooling
using(var owner = pool.Rent(32))
{    
    Do(owner.Memory);   
}
```
---

background-image: url(img/space.jpg)
background-size: cover

## Memory&lt;Krypton&gt; - usage

- Kestrel

--

- IO methods in .NET Core

--

- your app

---

background-image: url(img/space.jpg)
background-size: cover

## Memory&lt;Krypton&gt; - summary

- no leaking, how it was allocated

--

- splitting in an alloc-free way

--

- efficient pinning

--

- pooling support

---


# Benchmark.NET - Lease/Return

|          Method |      Mean |     Error |    StdDev |
|---------------- |----------:|----------:|----------:|
| Rent_1_Thruster |  73.17 ns | 1.4890 ns | 2.9391 ns |
|   Rent_1_Shared |  89.66 ns | 1.7079 ns | 1.6774 ns |
|  Rent_1_Kestrel |  54.00 ns | 0.7190 ns | 0.6725 ns |
| Rent_2_Thruster | 130.78 ns | 1.8649 ns | 1.5573 ns |
|   Rent_2_Shared | 253.82 ns | 4.1537 ns | 3.6822 ns |
|  Rent_2_Kestrel | 108.60 ns | 2.2828 ns | 6.7309 ns |

---

background-image: url(img/born.jpg)
background-size: cover

# Born to Perform

## Szymon Kulec @Scooletz