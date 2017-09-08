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
This is a really bad case and probably most of you had it at least one time. When looking long enough in a piece of code, you'll finally notice an operation, that could be optimized. But that's a local optimization. You don't know if it affects performance at all. If that's a hot path, that is used a lot, you can gain something. Quite often, it will be a path that is not executed that frequently, but, hey. You can get back to lying on your couch, as you've introduced MOAR performance, right?

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

## Unallocating The Managed World

???

It does not matter which managed language would you use: is it Java, Scala, C#, or F#. Or event VB.NET
Unfortunately for the creators, fortunately for developers, the list of optimizations that you should start with, after measuring a performance, will quite often cover the list of language features, which are: allocations, bytes and their copies.
This was the case in the following two cases I helped with. First, it was Marten, a .NET library providing a document db capabilities + event sourcing on top of PostgreSQL. The second, was strongly related with Akka.NET.

---

background-image: url(img/marten.png)
background-size: contain

## Unallocating The Managed World

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
- low level gains make a difference for the performance of the whole

???

Now, after loosing faith in "purrfect purrformance" and knowing that we need to measure our gains, we know a trick or two how to optimize .NET code in some aspects. It' about time to tell when it should be applied. My rule is to do it only for libraries, shared components or frameworks. It's likely that once it's addressed on the lower level it will make it transparent for the user or will make the user to follow the rules of the component they use.
Don't throw this into every controller you write or every service you create. Choose wisely.

---

background-image: url(img/formats.jpg)
background-size: cover

## Data format

???

After talking a look at our code, it's time to blame the others'. Let's take a look at the way we store our data.
It's quite common that we just use JSON.NET, the most popular serialization library in .NET. There's nothing wrong with it. Actually, it's quite cool to have a standard, but, when using JSON do we always need to use JSON.NET?

---

## Data format

```js
{onlyJSON: true}
```

- JSON.NET
- SimpleJSON
- Jil

???

There are some disadvantages of being a popular library. For instance, when you use two libs that depen on different version of JSON.NET. Yes, you can use il-repack or il-merge, but are there other choices?

SimpleJSON is a quite interesting library. It takes only 2000 lines of code and is filled with compilations flags. You can configure it and make it behaving as you want (probably). And it's a single file.

And there's Jil. My favourite one ;-) This this is crazy fast. For instance, it takes into consideration the layout of fields in a class to make it even faster. Underneath it uses a Sigil, another library for writing fast MSIL.

Ok, so we know that we've got choices when dealing with JSON. Do we always need to?

---

## Data format

```js
{onlyJSON: false}
```

- protobuf-net (Google Protocol Buffers)
- Wire/Hyperion
- Bond

???

There's lots, and I mean, lots of serializers beside the JSON. Most of them, are binary, some of them include some schema, some of them - not. If you don't know anything beside JSON, it might be a good thing to learn a one or two.

There's protobuf-net, provided by Marc Gravell. A .NET implementation of Google's protocol. It's quite handy when dealing with simple schema changes. Also, it's fast and sometimes allows two versions to work on the same piece of data.

There's mentioned Wire, used by AKKA.NET.

There's Bond, Microsoft's Bond. Another tool.

Having all the choices, you need to what do you want to use the serilizer for?

---

## Data format

### What?

- internal/external usage?
- db/queue/framework requirements?
- many protocols, content negotiation?

???

The first and most important question: what for do you want to use it? Is it for a client facing API or internal structures? 

What about DB? Do you need a serializer for this or do you use SQL? If you use a framework for you application/service, maybe they are some extension points you could use. 

The last but not least. Some frameworks and libraries enable content negotiation or at least are able to consume multiple formats. You could use a so-called multiserialization: JSON for pages using your serives, and other, for service-to-service communication.

Data format is just an example. You can think of different components that you use to create systems/apps. Think about your choices and choose wisely.

---

background-image: url(img/async.jpg)
background-size: cover

## Async By Design

---

## Async By Design

### Async history

```c#
IAsyncResult BeginSend(AsyncCallback callback, 
  object state);

EndSend(IAsyncResult asyncResult);
```

```c#
Task Send ();

await Send ();
```

???

The whole async-await may seem as a revolution. For me, it's a language feature finally providing a tool to deal with underlying asynchronicity. Below Tasks, Being/End methods, used for IO operations on Windows, lie completion ports. This is a mechanism for registering your continuation to be executed after specific IO is done. Why is it so important to use?

---

## Async By Design

### Limited threads vs lots of work

```c#
await Send ("A");
await Send ("B");
```

```c#
ulitmateAppBuilder
  .Then(()=>Send("A"))
  .Then(()=>Send("B"))
```

???

The number of threads that can be executed efficiently on one machine is limited. Once, you breach some threshold the context switching kicks in, making your multithreaded app slower with every thread that is created. What if we left the creation of the threads and the whole management? What if that was handled by the framework?

That's exactly what async-await is all about. It allows to split your code in chunks that are IO-independent, and to register their execution as soon as IO is done. Effectively, it's your code executing, and executing again as soon as underlying operation is done. The thread that you was using, might be and will be scheduled to execute some other operation meanwhile.

It might look like a revolution, but actually, it's about embracing how the underlying hardware and operating system works.

We know that queues are helpful. What if we make it explicit...

---

background-image: url(img/performance.jpg)
background-size: cover

## Extreme performance

---

## Extreme performance

### Aeron, Aeron.NET, Ramp Up

```c#
var buffer = new UnsafeBuffer(new byte[256]);

using(var aeron = Aeron.Connect())
using(var publisher = 
  aeron.AddPublication(channel, streamId)) 
{
  var messageLength = buffer
    .PutStringWithoutLengthUtf8(0, "Hello World!");
  publisher.Offer(buffer, 0, messageLength);
}
```

???

we could push it to the extreme. By making adding to the work queue explicit, we can send 10mlns messages a second. It's not pleasant, requires lots of things to write but hey. You wanted performance, right? Additionally, some side effects might kick in...

---

## Extreme performance

### Side effects

```c#
unsafe
{
  Volatile.Write (ref data*, 1);
}
```

```c#
var slot = Interlocked.Add(ref counter);
data[slot].Id = 5;
Volatile.Write (ref data[slot].Barrier, 1);
```

???

Some code that you probably don't want to deal with. All the volatiles, interlocks and no locking at all.

We know that we can quite deep even in a managed language like c-sharp. Could we resurface now? It there anything on the higher level that could be helpful to make your systems more robust?

---

background-image: url(img/message.jpg)
background-size: cover

## Architecture for performance

???

I think there is and this is the thing that is quite frequently underestimated in terms of performance. This is is architecture.

---

## Architecture for performance

### Messaging

- async between processes = messaging
- Task.WhenAll = sagas / process managers
- await = command + event

???

It's 21st century and still, there are people that think that you can create a well performing system sitting in an ivory tower. This is rubbish.
I'd say totally opposite. If you provide strong foundations in your architecture, then, on the implementation level your systems will thrive. Let's take a look at this, a bit skewed but still, comparison.

By introducing messaging, you can actually make your processes asynchronous. One part is done and a message is sent. Another is done and another message is sent. Simple as this.

We all love Task.WhenAll, when we can register for the final completion of all the work items that have to be done. You could use a saga for it, or a process manager, whichever term you prefer. You gather responses and events and make the final step.

The last but not least. The await keyword. It's registering a continuation, like it was awaiting on an event marking an operation as completed.

You may say that I'm comparing apples to oranges but for sure you can see the analogy. The world is asynchro

---

background-image: url(img/gift.jpg)
background-size: cover

## Wrapping up

---

## Wrapping up

- measure your gains
- understand the managed world
- select right dependencies (serializers,...)
- async-ify
- push to extremes
- choose architecture wisely

---

# Performance That Pays Off

## Szymon Kulec @Scooletz

### Particular Software