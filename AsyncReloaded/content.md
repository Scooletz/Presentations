# Async Reloaded

Szymon Kulec @Scooletz

Lead Developer Advocate @ [RavenDB](https://ravendb.net)

---

## Intro

1. `async` + `await` + `Task` = ❤️
1. scope: hot paths 🔥
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

<img src="/AsyncReloaded/assets/pass-the-state.webp" style="max-width: 80%">

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

### ConcurrentDictionary

It just works!

```csharp
ConcurrentDictionary<string, int> dict;

// Get
return dict[key];

// Set
dict[key] = value;
```

--

### Counters

Let's build a counter! (A terrible example)

1. Isn't set - assign 1
1. Is set - assign value + 1

--

### Counters

```csharp
ConcurrentDictionary<string, int> counters;

// The wrong way 💣
if (!counters.TryGetValue("counters", out var value))
{
    value = 0
}

counters["counter"] = value + 1;
```

--

### Counters

```csharp
ConcurrentDictionary<string, int> counters;

// The right way 🎉
counters.AddOrUpdate("counters",
  (key) => 1,             // add factory
  (key, prev) => prev + 1 // update factory
);
```

--

### Counters - easy-peasy

1. It was easy really easy.
1. No external state dependency.
1. Let's introduce one.

--

### Hands on: Bench1 + Bench2

--

### Hands on - summary

`ConcurrentDictionary` supports:

1. mutli-operations like `AddOrUpdate`
1. methods with an additional state

Maybe others do as well?

--

### Pass the State - Cancellation

--

### Cancellation

1. `CancellationTokenSource` 
   1. allows cancelling
   1. provides `.Token`
1. `CancellationToken`
   1. `.CanBeCancelled`
   1. `.IsCancellationRequested`
   1. `.Register(...)` 👀

--

### CancellationToken.Register

```csharp
CancellationTokenRegistration registration = token.Register(
    Action callback);

registration.Dispose();
```

--

### CancellationToken.Register - state

```csharp
CancellationTokenRegistration registration = token.Register(
    static s => callback(s), state);
registration.Dispose();
```

--

### Hands on: Bench3

--

### Hands on - summary

`CancellationToken` supports a method with an **external state**

Maybe others do as well?

--

### Task.ContinueWith

```csharp
Task task;
Task continuation = task.ContinueWith(
    static t => callback(t));
```

--

### Task.ContinueWith - state

```csharp
Task task;
Task continuation = task.ContinueWith(
    static (t, s) => callback(t, s), state);
```

--

### Hands on: Bench4

--

### Hands on - summary

1. `Task` supports a method with an **external state**
1. It can be used to pass some state machine
1. It can be used by you to pass any state (when writing low level code)

--

### Pass the State - examples

[Prefer CancellationToken.Register with state over closure allocating](https://github.com/ravendb/ravendb/pull/21205)

<img src="/AsyncReloaded/assets/PassTheState1.png">

--

### Pass the State - examples

[Prefer CancellationToken.Register with state over closure allocating](https://github.com/ravendb/ravendb/pull/21205)

<img src="/AsyncReloaded/assets/PassTheState2.png">

--

### Pass the State - examples

[Use Task.ContinueWith with the state where possible](https://github.com/ravendb/ravendb/pull/21234)

<img src="/AsyncReloaded/assets/PassTheState3.png">

--

### Pass the State - summary

1. Async friends allow passing the **external state**
1. The external state is an `object`
1. Designing to use just one `object` is sometimes hard.

---

## Red, Blue, and Purple

<img src="/AsyncReloaded/assets/red-blue-purple.webp" style="max-width: 80%">

--

### Red, Blue, and Purple

---

## Async in .NET 10

--

### Async in .NET 10

---

## Feedback & QA

https://forms.gle/uovE8xZeEzLUP4ox8

<img src="/AsyncReloaded/assets/feedback.png" alt="https://forms.gle/uovE8xZeEzLUP4ox8" style="max-width: 50%">