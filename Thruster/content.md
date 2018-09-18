background-image: url(img/thruster.jpg)
background-size: cover

# Thruster, or There and Back Again

## Szymon Kulec @Scooletz

???

Nazywam się Szymon Kulec, w sieci posługuję się nickiem Scooletz i muszę się Wam do czegoś przyznać. Stwierdzono u mnie NKDO...

---

background-image: url(img/nkdo.jpg)
background-size: cover

## NKDO

--

- Nieanonimowy
- Kontrybutor
- Do
- Open Source

???

NKDO, czyli...
Zazwyczaj na spotkaniach witamy nowo przybyłego mówiąc "Cześć Szymon". Dziękuję za wsparcie, wiele dla mnie znaczy.
Chciałbym podzielić się historią mojego ostatniego przypadku Open Source'owego, który miał silne połączenie z .NET Core

---

background-image: url(img/question.jpg)
background-size: cover

## .NET Core? 
### Ktoś? 
### Coś?

???

- Open Source
- dużo kontrybucji
- Windows i Linux

---

background-image: url(img/core_more.jpg)
background-size: cover

## Feature'y .NET Core

???

- cała masa
- portowane wstecz pakietami, System.Memory
- porty częściowe - zależą od frameworka
- Span, Memory - nie do końca działają tak samo

---

## System.Memory

- Span
- Memory
- IMemoryPool < T >
- IMemoryOwner < T >

---

## Bytes - po staremu

```c#
var bytes = new byte[64];
```

--

```c#
var segment = new ArraySegment<byte>(bytes, 0, 64);
```

--

```c#
await file.ReadAsync(bytes, 0, 32);
```

--

```c#
var something = SuperUnsafeAllocMagic(64);
await file.ReadAsync (something);

// AZURE STOPS, WORLD EXPLODES, UNIVERSE COLLAPSES
```

---

## A co jeśli by tak... mieć nowy typ

- który opakuje []

--

- który opakuje pamięci innego sortu

--

- który można dzielić

--

- który można przypiąć (pin)

--

- który obsłuży pamięć z puli

---

## System.Memory&lt;T&gt;

--

- który opakuje []
```c#
var memory = new Memory<byte>(bytes, 0 , 32);
```
--

- który opakuje pamięci innego sortu (wskaźniki)
```c#
var memory = MemoryMarshal
        .CreateFromPinnedArray(pinnedBytes, 0, 32);
```

---

## System.Memory&lt;T&gt;

- który można dzielić
```c#
var first2 = memory.Slice(0, 2);
```

--

- który można przypiąć (pin)
```c#
var almostPointer = memory.Pin();
```

--

- który obsłuży pamięć z puli
```c#
using(var owner = pool.Rent(32))
{    Do(owner.Memory);   }
```

---

background-image: url(img/thruster.jpg)
background-size: cover

# Thruster

---

# Thruster - plan ataku

- pula pamięci szybsza niż *Default*

--

- dowolna wielkość
--
, tzn. od 1 do 63 elementów :D

--

- benchmarki i porównanie z rynkiem

---

# Thruster - 63 elementy

```c#
Rent(int size)
{
    // TODO
}
```
--
```c#
long mask = (1 << size) - 1; 

// 1 -> 1
// 2 -> 3
// 3 -> 7
```

---

# Thruster - 63 elementy

```c#
for (var i = 0; i < 64 - size; i++)
{
    if (lease & mask == 0)
    { 
        lease |= mask;
        return i; // position found
    }

    mask <<= 1; // move by one bit
}
return -1;
```

---

# Thruster - 63 elementy
## Co z wieloma wątkami?

--
```c#
lock(pleaseDont)
{
    
}
```
--
albo:

```c#
Interlocked.CompareExchange
    (ref location, newValue, condition);
```

???

Interlocked:
- warunkowe podstawienie
- zawozi dzieci do przedszkola, gotuje obiad
- atomowe
- jedna operacja ASM

---

# Thruster - 63 elementy
## Co z wieloma wątkami?

```c#
var v = 1;
var result = Interlocked
            .CompareExchange(ref v, 2, 1);

v = ?
result = ?
```
--
```c#
var v = 1;
var result = Interlocked
            .CompareExchange(ref v, 2, 0);

v = ?
result = ?
```
---

# Thruster - 63 elementy

```c#
if (lease & mask == 0)
{ 
    lease |= mask;
    return i;
}
```
--
```c#
var v = lease | mask;
var newLease = CAS(ref this.lease, v, lease);
if (newLease == lease)
{
    return i;
}

lease = newLease;
i--;
```

---

# Thruster - Więcej wątków ?

--

|CPU1 | CPU2  | CPU3 | CPU4 |
|---|---|---|---|
| 0  | 1  | 2  | 3  |
| 1  | 2  | 3  | 0  |
| 2  | 3  | 0  | 2  |
| 3  | 0  | 1  | 1  |

---

# Thruster - Więcej wątków ?

```c#
[MethodImpl(MethodImplOptions.AggressiveInlining)]
int GetBucketId()
{
#if NETCOREAPP2_1
    return Thread.GetCurrentProcessorId() 
            % processorCount;
#else
    return Environment.CurrentManagedThreadId 
            % processorCount;
#endif
}
```

---

# Thruster - Więcej wątków ?

- to samo `bucketId` dla pobierania/zwracania

--

- nawet po `async/await`

--

- dwie operacje CAS na 1 Memory

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

# Benchmark.NET - Parse ASP.NET

|                           Method |     Mean |    Error |   StdDev |
|--------------------------------- |---------:|---------:|---------:|
| Tasks_Thruster | 311.9 ns | 3.705 ns | 3.466 ns |
|   Tasks_Shared | 315.6 ns | 4.070 ns | 3.608 ns |
|  Tasks_Kestrel | 318.6 ns | 6.361 ns | 9.323 ns |
|   Inline_Thruster | 402.1 ns | 4.979 ns | 4.657 ns |
|     Inline_Shared | 434.4 ns | 3.357 ns | 3.140 ns |
|    Inline_Kestrel | 394.9 ns | 7.844 ns | 9.921 ns |


???

- ASP.NET pipeline handling from Kestrel
- syntetyczny test parsowania

---

# Kestrel - MemoryPool

```c#
internal class SlabMemoryPool : MemoryPool<byte>
{
    ConcurrentQueue<MemoryPoolBlock> _blocks 
    ConcurrentStack<MemoryPoolSlab> _slabs 

    private MemoryPoolBlock Lease()
    {
        if (_blocks.TryDequeue(out var block))
        {
            block.Lease();
            return block;
        }
        
        block = AllocateSlab();
        block.Lease();
        return block;
    }
} 
```

---

# Kestrel - MemoryPool
## ConcurrentQueue.TryDequeue

```c#
public bool TryDequeue(out T item)
{
    // ...
    int currentHead = Volatile.Read(
        ref _headAndTail.Head);

    // ...
    if (Interlocked.CompareExchange(
        ref _headAndTail.Head, 
        currentHead + 1, 
        currentHead)
         == currentHead)
    // ..
```

---

# Kestrel - MemoryPool
## ConcurrentQueue.TryEnqueue

```c#
public bool TryEnqueue(T item)
{
    // ...
    int currentTail = Volatile.Read(
        ref _headAndTail.Tail);

    // ...
    if (Interlocked.CompareExchange(
        ref _headAndTail.Tail, 
        currentTail + 1, 
        currentTail) 
        == currentTail)
    // ...
```

---

# Thruster - wnioski

--
- Kestrel jest szybki

--
- .NET Core jest szybki

--
- specyficzny szybszy od generycznego

--
- benchmarkowanie jest dobre
--
,choć wolne ;)

--
- lock-free, jest szybkie

---

background-image: url(img/nkdo.jpg)
background-size: cover

# NKDO

---

background-image: url(img/dotnetos.png)
background-size: contain

# Konferencja !

---

background-image: url(img/thruster.jpg)
background-size: cover

# Thruster, or There and Back Again

## Szymon Kulec @Scooletz