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

```c#
var memoryOfKrypton = new Memory<Krypton>(...);
```

---

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