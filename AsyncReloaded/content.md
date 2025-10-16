# Async Reloaded

Szymon Kulec @Scooletz

Lead Developer Advocate @ [RavenDB](https://ravendb.net)

---

## Intro

1. async/await
1. scope: hot paths
1. hot paths:
   1. a database: 99%
   1. a library: 99%
   1. infrastructure/middleware: a lot

---

## Agenda

1. Pass the State
1. Red, Blue, and Purple
1. Async in .NET 10 (experimental)

---

## Pass the State

--

### ConcurrentDictionary - lock

```csharp
Dictionary<string, int> dict;

// Get
lock(_obj)
{
    return dict[key];
}

// Set
lock(_obj)
{
    dict[key] = value;
}
```

--

### ConcurrentDictionary - RWL

```csharp
Dictionary<string, int> dict;
ReaderWriterLockSlim rwl;

// Get
rwl.EnterReadLock();
try
{
    return dict[key];
}
finally
{
    rwl.ExitReadLock();
}
```

--

### ConcurrentDictionary - RWL

```csharp
Dictionary<string, int> dict;
ReaderWriterLockSlim rwl;

// Set
rwl.EnterWriteLock();
try
{
    dict[key] = value;
}
finally
{
    rwl.ExitWriteLock();
}
```

--

### ConcurrentDictionary - RWL

```csharp
ConcurrentDictionary<string, int> dict;

// Get
return dict[key];

// Set
dict[key] = value;
```

--

### Pass the State

---

## Red, Blue, and Purple

--

### Red, Blue, and Purple

---

## Async in .NET 10

--

### Async in .NET 10

---

## Feedback

https://forms.gle/uovE8xZeEzLUP4ox8

![QR](/assets/feedback.png)