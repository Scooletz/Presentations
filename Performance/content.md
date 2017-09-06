# Performance That Pays Off

## Szymon Kulec @Scooletz

### Particular Software

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

So you're lying on your couch. It's quite cosy and then an idea pops up. "If I removed this string concatenation" or "let's switch from a list to an array and skip linq". I'm not talking about thoughts being a result of a long profiling session. I'm talking about these "ideas" that will CHANGE our applications' performance without any measurements.
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
This was the case in the following two cases I helped with. First, it was Marten, a .NET library providing a document db capabilities + event sourcing on top of PostgreSQL. The second, was strongly related with Akka.NET.

---

background-image: url(img/marten.png)
background-size: contain

## Unallocating Managed World

???

Let's start with Marten. Marten is a .NET library build on top of PostgreSQL. It uses extensively Postgre's support for JSON types (jsonB in Postgre) and capabilities provided by Npgql, the .NET driver/connection provider.
It's funny, that the creator Jeremy D. Miller came with the name, by googling for the natural Raven predator. You can connect the dots ;-)
Marten provides two main capabilities: the first is the document database built with the already mentioned jsonb support, the second - event sourcing store, with lots of goodness for projections, views and more.
If that's so cool, what could have been wrong with this library? We need to move a few months back, to version 1.2.

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

As Marten's primary data format is JSON, you can imagine that one of the core components of this library is the serialization abstraction. In Marten, it's used for serializing any document, event, whatever. As you can see, in this version, every serialization resulted in a string creation. Yes, strings are cheap, but as the old performance proverb says "nothing is cheaper than anything". If you could pick up something not to do in here, what would it be? Of course, it'd be string allocations. Because the majority of JSON serializers are capable of writing to a TextWriter...

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

???
The choice was simple. Introduce an overload of a method that accepts a TextWriter, and write a custom TextWriter that can be reused, to skip the allocations. Then, just go through all the places that were using them, and change their behavior. This would have been simpler, if the call sites hadn't been generated with expression trees...

---

## Unallocating Managed World
### Marten

```c#
public override Expression CompileUpdateExpression(
  EnumStorage enumStorage, 
  ParameterExpression call, 
  ParameterExpression doc, 
  ParameterExpression updateBatch,
  ParameterExpression mapping,
  ParameterExpression currentVersion,
  ParameterExpression newVersion, 
  bool useCharBufferPooling)
{
  // you can imagine the body of it :)
}
```

???

It took a bit longer to make it.

---

## Unallocating Managed World
### Marten

| Method | Mean | Allocated |
| --- | --- | --- | --- |
| AppendEvents | 43ms | 3.19MB |
| AppendEvents (after)| 41ms | 1.88MB |
| InsertDocuments | 44ms | 6.67MB |
| InsertDocuments (after) | 39ms | 4.62MB |
| BulkInsertDocuments | 230ms | 27.37MB |
| BulkInsertDocuments (after) | 168ms | 7.74MB |

???

As you can see, the amount of allocated memory dropped significantly.
The performance is better, especially for bulk inserts.
Before coming to a conclusion let's go through another project

---

## Unallocating Managed World
### Wire/Hyperion

> A high performance polymorphic serializer for the .NET framework.

???

This project could be used to present what a software licence is for. There was a small war over internet when it changed it's license from a permisive one to GNU, but let's keep our focus on the performance aspect.
It shows how much you can achive when writing a protocol from the beginnig, having a special case in mind. In this case it was a high performance without dropping the way to pass the schema with messages. The initial test cases were showing some potential so took a closer look. By the way, have you ever tried to convert an int to bytes?

---

## Unallocating Managed World
### Wire/Hyperion

```c#
public static class BitConverter 
{
  public unsafe static byte[] GetBytes(int value)
  {
    byte[] bytes = new byte[4];
    fixed(byte* b = bytes)
      *((int*)b) = value;
    return bytes;
  }
}
```

???

Finally some pointers! Admit you've already lost hope! Let's skip the beautiful star in there. This method is responsible for obtaining an array of bytes for an intiger value. It could be useful if you were passing these bytes forward. What about scenario, when we write them immiediately to the wire/stream? Then we could postpone these allocations and think use a shared array, released at the end of writing process. That's what I've done in Wire.

---

## Unallocating Managed World
### Wire/Hyperion

```c#
public static class NoAllocBitConverter
{
  public static unsafe byte[] GetBytes(short value, 
      SerializerSession session)
  {
    const int length = 2;

    var bytes = session.GetBuffer(length);
    fixed (byte* b = bytes)
      *((short*) b) = value;
          return bytes;
  }
}
```

???

Wire uses the notion of a serializer session. An object context that is passed to every call. With this change, I was able to claim a buffer from a session just to write to it. As the resulting bytes were written down immiediately, the buffer could be reused by another writer in a following call. This small change made a significant improvement.
Again, as lots of codes were generated dynamically (OpCodes, Reflection), this change required a few more files to be updated, to get it done.

---

## Unallocating Managed World
### Do's and don'ts

- as always measure first
- apply for libs/frameworks/shared
- lower gains in higher levels

???

Now, after loosing faith in "purrfect purrformance" and knowing that we need to measure our gains, we know a trick or two how to optimize .NET code in some aspects. It' about time to tell when it should be applied. My rule is to do it only for libraries, shared components or frameworks. It's likely that once it's addressed on the lower level it will make it transparent for the user or will make the user to follow the rules of the component they use.
Don't throw this into every controller you write or every service you create. Choose wisely.

---

background-image: url(img/formats.jpg)
background-size: cover

## Data format

???

How many serializers do you know? How many data formats are you aware of? Have you ever thought about storing some binary data in a column of Azure Storage just to make it more dense, therefore cheaper. To make it more version tolerant?
We all know JSON, we all hate XML. Have you learnt what Google Protocol Buffers are? Simple Binary Encoding. There are huge differences between these formats. Even when at the end you provide a simple service providing REST API, maybe it's worth to consider not using JSON inside? Maybe there's something more. I always compare these to a toolbox. You should know what's inside it, but choose wisely.

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
