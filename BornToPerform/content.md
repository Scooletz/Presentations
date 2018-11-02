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

background-image: url(img/flying.jpg)
background-size: cover

## SuperSpan

--
- synchronous memory accessor

--

- `ref struct`, can leave only on stack

--

- a bit more efficient in .NET Core 

--

- removes `unsafe` for `stackalloc`

---

background-image: url(img/flying.jpg)
background-size: cover

## SuperSpan - how to get it?

```c#
// from memory
Span<byte> span = memory.Span;
```
--
```c#
// from stack
Span<byte> span = stackalloc byte[128];
```
--
```c#
// stack or alloc
Span<byte> span = size < 128 ?
         stackalloc byte[size] :
         new byte[size];
```
---

background-image: url(img/flying.jpg)
background-size: cover

## SuperSpan - what for?

```c#
// fast encoding
Base64.EncodeToUtf8(ReadOnlySpan<byte> bytes, 
                    Span<byte> utf8, ...)
```
--
```c#
// even better encoding
Base64.EncodeToUtf8InPlace(
                Span<byte> buffer, ...)
```
--
```c#
// and parsing
long.TryParse(ReadOnlySpan<char> s, 
    out long result);
```
---

background-image: url(img/flying.jpg)
background-size: cover

## SuperSpan - what for?

```c#
// copying
span.CopyTo(another);

// clearing
span.Clear();

// filling
span.Fill(value);
```

---

background-image: url(img/flying.jpg)
background-size: cover

## SuperSpan - abusing it

```c#
// what's wrong in here?

Span<byte> Serialize(...)
{
    Span<byte> s = stackalloc byte [16];
    return s;
}
```
???

- span is ref struct
- cannot be returned

---

background-image: url(img/flying.jpg)
background-size: cover

## SuperSpan - abusing it

&#11208; our code

&#11208;&#11208; Serialize

&#11208;&#11208;&#11208; our code

???

- stack can be passed down to the method
- if we could pass it to the lower level
- we could make it faster

---

background-image: url(img/flying.jpg)
background-size: cover

## SuperSpan - abusing it

```c#
delegate void Write(Span<byte> payload);
```
--
```c#
void Serialize(Write write)
{
    Span<byte> s = stackalloc byte [16];

    // serialize
    write(s);
}
```

???

- Enzyme
- new .NET serializer

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