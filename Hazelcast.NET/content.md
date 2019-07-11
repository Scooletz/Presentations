background-image: url(img/hazelnet.jpg)
background-size: cover

# Hazelnet

## Szymon Kulec @Scooletz

---

## Allocations in the client

Mostly

```csharp
var buffer = new byte [length]
```

---

## New approach for memory

```csharp
// reclaims a regular array from the pool
var arrayOfBytes = ArrayPool<byte>.Shared.Rent (size);
```

--

```csharp
// reclaims memory from the pool, that can be pinned, etc
var memory = MemoryPool<byte>.Shared.Rent (size);
```

--

```csharp
// allocates memory on the stack
Span<byte> span = new stackalloc byte[16];
```

---

## Removal of allocations

- replace `new byte[length]` with `ArrayPool` whenever possible
- replace some copying arrays with `ArraySegment<byte>`

---

## Tests

- testing `map.PutAll` and `map.GetAll` failed terribly due to not that many operations :-)
- `map.Put` and other atomic operations gained... something?

---

## Results

| Case  | Mean  | Avg deviation  |
|---|---|---|
| Less allocation  | 17.3380402  | 0.44  |
| More allocation  | 16.27301065  | 0.30  |

---

## Outcomes

- think about .NET Framework selection
- prepare more stresfull test (multiple threads etc.)
- understand client better :P

---

background-image: url(img/hazelnet.jpg)
background-size: cover

# Hazelnet

## Szymon Kulec @Scooletz