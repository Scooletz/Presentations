# Performance That Pays Off

## Szymon Kulec

???

How fast is it? How fast could it be? How fast should it be?
No matter if you write software for cloud environments or on-premises, these questions might pop up.
Are there truly important? Should they always be answered? Let me give you a few hints.
My name is Szymon Kulec. I work for Particular Software and I encourage you to join me in a journey 
through various projects I was involved into.  

---

background-image: url(img/purr.jpg)
background-size: cover

## Purrfect Purrformance

---

## Purrfect Purrformance

```c#
var output = 1.ToString() + 2.ToString();
bestRepoEver.SlowestStoredProcedureEver();
```

```c#
Upload (items.Select(i=>i.ToString()));
Thread.Sleep(1000);
```

???

So you're lying on your couch. It's quite cosy and then an idea pops up. "If I removed this operation in line X" or "let's switch from a list to an array". I'm not talking about thoughts being a result of a long profiling session. I'm talking about these "ideas" that will CHANGE our applications' performance without any measurements.
This is a really bad case and probably most of you had it at least one time. When looking long enough in a piece of code, you'll finally notice an operation, that could be optimized. It can be adding to a list, calculating a thing multiple times or simply unrolling a loop by coping same statements multiple times. If that's a hot path, that is used a lot, you can gain something. Quite often, it will be a path that is not executed that frequently, but, hey. You can get back to lying on your couch, as you've introduced MOAR performance, right?

---

background-image: url(img/gains.jpg)
background-size: cover

## Measure you gains

???

Wrong.
The same rule that can be applied to a gym, can be applied to the performance. You should measure your gains. And if you want to measure your gains, you should first measure the current state. This could be done by running a profiler, but also, you can provide a benchmarking test, that will measure the current state of affairs. Moreover, you can put that into your CI environment to ensure, that once a threshold is reached, your build will be green again by failing on builds that breach the performance agreement.

We already know, that making purrfect guesses does not work and that we need to measure our stuff. Are there any hints that one could use to make their guesses more educated?

---

background-image: url(img/garbage.jpg)
background-size: cover

## Unallocating Managed World

???

It does not matter which managed language would you use: is it Java, Scala, C#, or F#. Or event VB.NET
Unfortunately for the creators, fortunately for developers, the list of optimizations that you should start with, after measuring a performance, will quite often cover the list of language features, which are: allocations, bytes and their copies.
This was the case in the following two cases I helped with. First, it was Marten, a .NET library providing a document db capabilities + event sourcing on top of PostgreSQL. The second, was strongly related with Akka.NET.

---

background-image: url(img/marten.png)
background-size: contain

## Unallocating Managed World

???

Let's start with Marten. Marten is a .NET library build on top of PostgreSQL. It uses extensively Postgre's support for JSON types (jsonB in Postgre) and capabilities provided by Npgql, the .NET driver/connection provider.
It's funny, that the creator Jeremy D. Miller came with the name, by googling for the natural Raven predator. You can connect the dots ;-)
Marten provides two main capabilities: the first is the document database built with the already mentioned jsonb support, the second - event sourcing store, with lots of goodness for projections, views and more.
If that's so cool, what could have been wrong with this library? We need to move a few months back, to version 1.2.

---

## Unallocating Managed World
### Marten

```c#
public interface ISerializer
{
  string ToJson(object document);
  string ToCleanJson(object document);

  // ...
}
```

???

As Marten's primary data format is JSON, you can imagine that one of the core components of this library is the serialization abstraction. In Marten, it's used for serializing any document, event, whatever. As you can see, in this version, every serialization resulted in a string creation. Yes, strings are cheap, but as the old performance proverb says "nothing is cheaper than anything". If you could pick up something not to do in here, what would it be? Of course, it'd be string allocations. Because the majority of JSON serializers are capable of writing to a TextWriter...

---

## Unallocating Managed World
### Marten

```c#
public interface ISerializer
{
  string ToJson(object document);
  string ToCleanJson(object document);
  
  void ToJson(object document, TextWriter writer);

  // ...
}
```

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


