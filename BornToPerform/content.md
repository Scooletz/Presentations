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
- new experimental .NET serializer
- described on blog post
- will share links

---

background-image: url(img/zod.jpg)
background-size: cover

## Unsafe.As&lt;GeneralZod&gt;()

???
- a weapon for General Zod
- written purely in IL
- it's so much fun

---

background-image: url(img/zod.jpg)
background-size: cover

## Unsafe.As&lt;GeneralZod&gt;() - casting

--

```c#
// traditional casting
MyType my = (MyType) o;

// as
MyType my = o as MyType;
```
--
```c#
// unsafe, fast and furious
MyType my = Unsafe.As<MyType>(o);
```

---

background-image: url(img/zod.jpg)
background-size: cover

## Unsafe.As&lt;GeneralZod&gt;() - casting

```asm
.method public hidebysig static 
    !!T As<class T>(object o)
{
    .maxstack 1
    ldarg.0
    ret
}
```

---

background-image: url(img/zod.jpg)
background-size: cover

## Unsafe.As&lt;GeneralZod&gt;() - adding refs

--

```c#
int[] numbers = { 1, 2, 4 };
ref i = ref numbers[0];

// later on
*(i+1);
```

---

background-image: url(img/zod.jpg)
background-size: cover

## Unsafe.As&lt;GeneralZod&gt;() - adding refs

```asm
 .method public hidebysig static 
    !!T& Add<T>(!!T& source, int32 offset)
{
    .maxstack 3
    ldarg.0
    ldarg.1
    sizeof !!T
    conv.i
    mul
    add
    ret
}
```

---

background-image: url(img/source.jpg)
background-size: cover

## ValueTask&lt;DailyPlanet&gt;

--

```c#
public readonly struct ValueTask
{
    public ValueTask(IValueTaskSource source,
        short token)
    {
        // ...
    }
}
```

---

background-image: url(img/source.jpg)
background-size: cover

## ValueTask&lt;DailyPlanet&gt;

IValueTaskSource:

--

- Task-like abstraction

--

- can react to being finshed

--

- reusable/poolable

---

background-image: url(img/source.jpg)
background-size: cover

## ValueTask&lt;DailyPlanet&gt;

```c#
ValueTask DoSth()
{
    var source = pool.GetSource();
    // ...
    return new ValueTask(source, token++);
}
```
--
```c#
await o.DoSth(); // ValueTask(source, 1);
await o.DoSth(); // ValueTask(source, 2);
await o.DoSth(); // ValueTask(source, 3);
await o.DoSth(); // ValueTask(source, 4);
```

---

background-image: url(img/source.jpg)
background-size: cover

## ValueTask&lt;DailyPlanet&gt;

--

- it takes a while to implement `IValueTaskSource`

--

- if pooled properly, can greatly reduce allocations

--

- makes async-await less heavy

---

background-image: url(img/scooletz.png)
background-size: cover

## Interested? (In Polish)

---

background-image: url(img/born.jpg)
background-size: cover

# Born to Perform

## Szymon Kulec @Scooletz