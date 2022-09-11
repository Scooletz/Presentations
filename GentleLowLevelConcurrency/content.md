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

## A word of caution, pt. 2

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

background-image: url(img/gentle.jpg)
background-size: cover

# A gentle introduction to low-level concurrency in .NET

## Szymon Kulec @Scooletz