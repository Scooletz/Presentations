background-image: url(img/gentle.jpg)
background-size: cover

# A gentle introduction to low-level concurrency in .NET

## Szymon Kulec @Scooletz

---

background-image: url(img/logos.png)
background-size: cover

---

background-image: url(img/async_expert.png)
background-size: cover

---

background-image: url(img/fire.jpg)
background-size: cover

## Volatile experience

1. blowing up EventStore with not so [Better Queues PR](https://github.com/EventStore/EventStore/pull/1043) 💥

1. a journey project of [Ramp Up](https://github.com/scooletz/rampup)

1. optimizing various projects (not necessarily OSS)

1. repeatedly saying _it worked under debugger..._ 😭

---

background-image: url(img/fire.jpg)
background-size: cover

## A word of caution

> Most engineers reach for atomic operations in an attempt to produce some lock-free mechanism. Furthermore, **programmers enjoy the intellectual puzzle** of using atomic operations. Both of these lead to clever implementations which are **almost always ill-advised and often incorrect**. Algorithms involving atomic operations are extremely subtle. (...) Almost all **programmers make mistakes when they attempt** direct use of atomic operations. Even when they don’t make mistakes, the resulting **code is hard for others to maintain**.

> Atomic operations should be used only in a handful of low-level data structures which are **written by a few experts** and then reviewed and tested thoroughly. Unfortunately, many people attempt to write lock-free code, and this is almost always a mistake. Please do not fall into this trap: **do not use atomic operations**. If you do, you will make mistakes, and those will cost the owners of that code time and money.

from [Atomic Danger](https://abseil.io/docs/cpp/atomic_danger) by Dmitry Vyukov (Intel Black Belt) et al.

---

background-image: url(img/fire.jpg)
background-size: cover

## A word of caution

1. **Dmitry Vyukov**:
  
  1. inspired the modern implementation of the [ConcurrentQueue](https://github.com/dotnet/runtime/blob/8f33fd5cac1133a187bb27602f940608b8080758/src/libraries/System.Private.CoreLib/src/System/Collections/Concurrent/ConcurrentQueueSegment.cs#L21)
  1. publishes [1024 cores](https://www.1024cores.net)

--

1. it looks like writing code using `volatile` and `Interlocked` should be left to experts

--

1. but nobody said that we can't:
  1. read it
  1. understand it
  1. have some fun

--

Right👁️👁️? 

---

background-image: url(img/basket.jpg)
background-size: cover

## A volatile shopping basket

Let's consider an shopping example with two actors:

1. a person 🧑 who adds items to a shopping basket 🧺
1. a casher 🧑‍💼 who wants to checkout the basket 🧺

--

The scenario:

1. 🧑: adds 🥕 to 🧺
1. 🧑: adds 🥔 to 🧺
1. 🧑: says that 🧺 is ready to checkout 💰
1. 🧑‍💼: checkout 💸

---

background-image: url(img/basket.jpg)
background-size: cover

## A volatile shopping basket

What if 🧑 was run on a modern CPU? 
The sequence of operations could be perceived by an external observer as follows:

1. 🥕, 🥔, 🧺
1. 🥕, 🧺, 🥔
1. 🧺, 🥕, 🥔
1. 🧺, 🥔, 🥕
1. 🥔, 🧺, 🥕
1. 🥔, 🥕, 🧺

--

After all, at the end all the properties are matched:

- 🥕 is in
- 🥔 is in
- 🧺 is ready

---

background-image: url(img/basket.jpg)
background-size: cover

## A volatile shopping basket

What if 🧑‍💼 was run as another thread? What are the cases that would make it **work correctly**?

--

Only these, where 🧺 is checked out **after** adding 🥕 and 🥔 in **any order**

--

1. 🥕, 🥔, 🧺
1. ~~🥕, 🧺, 🥔~~
1. ~~🧺, 🥕, 🥔~~
1. ~~🧺, 🥔, 🥕~~
1. ~~🥔, 🧺, 🥕~~
1. 🥔, 🥕, 🧺

---

background-image: url(img/basket.jpg)
background-size: cover

## A volatile shopping basket

🧑 & 🧑‍💼 should work together to make sure that:

- 🧺 is marked as ready but...

- **only after** 🥕 and 🥔 are in it.

---

background-image: url(img/basket.jpg)
background-size: cover

## A volatile shopping basket

Let's introduce ➡️ operator to ensure some **ordering for external observers**:

🥕, 🥔, ➡️🧺

which means that for an external observer:

- 🧺 seen as **not ready** - they **might** observe some items in it

- 🧺 seen as **ready** - they **must** see all the items in it

--

➡️ - this is how `volatile` works.

---

background-image: url(img/basket.jpg)
background-size: cover

## A volatile shopping basket

A writer 🧑 and a reader 🧑‍💼 can use `volatile` ➡️ to create this coordination point:

- a writer uses a `volatile field` or a `Volatile.Write`

- a reader uses a `volatile field` or a `Volatile.Read`

--


Effectively, if you see a value written with volatile 👉 you'll see writes that happened before this write

---

background-image: url(img/crying-baby.jpg)
background-size: cover

## This ain't a gentle introduction... 😭

---

background-image: url(img/belt.jpg)
background-size: cover

## A volatile conveyor belt

Let's revisit the 🧑 & 🧑‍💼 example but modify it a little:

--

1. 🧑 should be able to put an item on the belt

1. 🧑 should make it sure that 🧑‍💼 is informed

1. 🧑‍💼 collect an item once it's there

--

This would allow 🧑 & 🧑‍💼 to work in a **producer-consumer** fashion where items are operated on one by one.

How could this be done?

---

background-image: url(img/belt.jpg)
background-size: cover

## A volatile conveyor belt

- belt:   🔳🔳
- count:  0️⃣

--

🧑: 🥕🔳, 1️⃣ - put an item on the belt, then set count to 1

--

It could result in the same error as before!

We know how to make sure that 1️⃣ is visible only after 🥕🔳 happens!

---

background-image: url(img/belt.jpg)
background-size: cover

## A volatile conveyor belt

1. 🧑: 🥕🔳, ➡️1️⃣

1. 🧑: 🥕🥔, ➡️2️⃣


--

How 🧑‍💼 as an external observer sees the belt?

--

1. if 1️⃣ is observed, then the 1st slot is set

1. if 2️⃣ is observed, then the 2nd slot is set

If 🧑‍💼 remembers last processed count, 🧑 & 🧑‍💼 can put and process items at the same time!

---

background-image: url(img/queue.jpg)
background-size: cover

## ConcurrentQueue - high-level view

--

* **unbounded MPMC** - Multi Producer Multi Consumer unbounded

--

* concurrent (it's Multi on both ends, it must be thread-safe)

--

* **FIFO** - first in-first out

--

* built from bounded MPMC **Segments**

--

* operations:

 * `Enqueue` - adds an item
 * `TryDequeue` - tries to dequeue an item
 * `TryPeek` - tries to return an item from the beginning of the queue without dequeueing it
 * `Count` - counts items at the specific moment
 * `IsEmpty` - a fast check for being empty at the specific moment

---

### ConcurrentQueue - design

.center[![img](img\concurrentqueue.png)]

---

background-image: url(img/queue.jpg)
background-size: cover

### ConcurrentQueue - design

Is a simple **wrap** for `_head` and `_tail`:

* `_head` - first segment, used to `TryDequeue`

* `_tail` - last segment, used to `Enqueue`

* **single segment** only at the beginning: `_head` == `_tail`

* more segments between `_head` and `_tail` if needed

---
background-image: url(img/queue.jpg)
background-size: cover

### ConcurrentQueue - design - segment

Segment:

* is a queue on its own, but it's **bounded**
* is a node in the list - has a ~~pointer~~ reference to the next segment
* uses **padded struct** to ensure that head and tail are on separate cache lines

```csharp
sealed class ConcurrentQueueSegment<T>
{
  readonly Slot[] _slots;

  PaddedHeadAndTail _headAndTail; // head & tail of a segment

  ConcurrentQueueSegment<T> _nextSegment; // the link to the next

  // ... more
}
```

---
background-image: url(img/queue.jpg)
background-size: cover

### ConcurrentQueue - design - slot

A simple structure that contains:

- the `Item`, the actual value stored in a queue

- the `SequenceNumber`, the number used for synchronization

```csharp
struct Slot
{
  public T Item;

  public int SequenceNumber;
}
```

---
background-image: url(img/queue.jpg)
background-size: cover

### ConcurrentQueue - operations

```csharp
public bool TryEnqueue(T item) {
  // ...
  slots[i].Item = item;
  Volatile.Write(ref slots[i].SequenceNumber, t + 1);
  return true;
  // ...
}
```

--

```csharp
public bool TryDequeue(out T item) {
  // ...
  item = slots[i].Item!;
  slots[i].Item = default;
  Volatile.Write(ref slots[i].SequenceNumber, head + slots.Length);
  return true;
  // ...
}
```

---

background-image: url(img/queue.jpg)
background-size: cover

### ConcurrentQueue - operations

1. Enqueuer 🧑:
  1. reads sequence #️⃣ from a slot
  1. if the sequence is multiple size of the segment, ready to dequeue 🔳
  1. 🥕,➡️#️⃣+1

1. Dequeuer:
  1. reads sequence #️⃣ from a slot
  1. if the sequence is equal to the head, ready to dequeue 🥕
  1. 🔳,➡️#️⃣ head + SIZE

--

**Pure awesomeness:** the sequence is always increasing! This ensures that it's not `true`/`false` which can be read with a stale ordering. If you get a bigger number, you know that the value is set properly or the slot is ready to accept the value!

---

background-image: url(img/queue.jpg)
background-size: cover

### ConcurrentQueue - questions

**Question**: Why `ConcurrentQueue` does not use a single `count` for a whole segment?

--

**Answer**: A single count with a volatile access is ok for a single producer. `ConcurrentQueue` is multi producer!

--

**Question**: Why the `sequence` is always increasing for each slot?

--

**Answer**: Otherwise `volatile` would not help much, as it only ensures **happened before** semantics. Stale reads are possible, but ordering is preserved!

---

background-image: url(img/football.jpg)
background-size: cover

## An interlocked football

---

background-image: url(img/gentle.jpg)
background-size: cover

# A gentle introduction to low-level concurrency in .NET

## Szymon Kulec @Scooletz