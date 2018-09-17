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

# Thruster, or There and Back Again

## Szymon Kulec @Scooletz