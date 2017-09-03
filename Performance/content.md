# Performance That Pays Off

## Szymon Kulec

???

How fast is it? How fast could it be? How fast should it be?
No matter if you write software for cloud environments or on-premises, these questions might pop up.
Are there truly important? Should they always be answered? Let me give you a few hints.
My name is Szymon Kulec. I work for Particular Software and I encourage you to join me in a journey 
through various projects I was involved into.  

---

## Purrfect Purrformance

???

So you're lying on your couch. It's quite cosy and then an idea pops up. "If I removed this operation in line X" or "let's switch from a list to an array". I'm not talking about thoughts being a result of a long profiling session. I'm talking about these "ideas" that will CHANGE our applications' performance without any measurements.
This is a really bad case and probably most of you had it at least one time. When looking long enough in a piece of code, you'll finally notice an operation, that could be optimized. It can be adding to a list, calculating a thing multiple times or simply unrolling a loop by coping same statements multiple times. If that's a hot path, that is used a lot, you can gain something. Quite often, it will be a path that is not executed that frequently, but, hey. You can get back to lying on your couch, as you've introduced MOAR performance, right?

---

## Measure you gains

???

Wrong.
The same rule that can be applied to a gym, can be applied to the performance. You should measure your gains. And if you want to measure your gains, you should first measure the current state. This could be done by running a profiler, but also, you can provide a benchmarking test, that will measure the current state of affairs. Moreover, you can put that into your CI environment to ensure, that once a threshold is reached, your build will be green again by failing on builds that breach the performance agreement.

We already know, that making purrfect guesses does not work and that we need to measure our stuff. Are there any hints that one could use to make their guesses more educated?

---

## Unallocating Managed World

???

How many of you use managed languages/environments like Java, Scala, C#, F#?
Unfortunately for the creators, fortunately for developers, the list of optimizations that you should start with, after measuring a performance, will quite often cover the list of language features, which are: allocations, bytes and their copies.

- allocations, reuse of bytes
- Marten 1.3

---

## Async By Design

???

async-await, futures, promises

---

## Performance by design

???

Aeron messaging

---

## Architecture for performance

???

NServiceBus


