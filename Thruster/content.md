background-image: url(img/thruster.jpg)
background-size: cover

# Thruster, or There and Back Again

## Szymon Kulec @Scooletz

???

Nazywam się Szymon Kulec, w sieci posługuję się nickiem Scooletz i muszę się Wam do czegoś przyznać. Stwierdzono u mnie NKDO...

---

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

## .NET Core? 
### Ktoś? 
### Coś?

???

- Open Source
- dużo kontrybucji
- Windows i Linux

---

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

// COMPILER EXPLODES, AZURE STOPS
```

---

background-image: url(img/thruster.jpg)
background-size: cover

# Thruster, or There and Back Again

## Szymon Kulec @Scooletz