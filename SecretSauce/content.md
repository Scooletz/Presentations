background-image: url(img/dotnetos.png)
background-size: contain
background-position: bottom

# Sekretny Sos Serializacji &#127798;

## Szymon Kulec @Scooletz

---

## Skala Scoville’a (SHU)

| Wartość | Papryka | Uwagi |
| ---: | --- | --- | --- |
| 0 | zwyczajna | dla babci i dziadka |
| 5000 | Jalapeño | dla mamy i taty |
| 200000 | Habanero | starter do kolacji |
| 900000 | Dorset Naga | now we're talkin'! |
| 1500000 | Carolina Reaper | yummy |
| 5000000 | gaz pieprzowy | dla prawdziwych tygrysów |

---

## Menu degustacyjne

- XML
- JSON
- Google Protocol Buffers
- custom protocols
- deser

Ostrość od &#127798; do &#127798;&#127798;&#127798;

???
- każda pozycja menu podsumowana w kilku kategoriach
- i co dalej (stack alloc)

---

## XML & WS*

```xml
<?xml version="1.0"?>
<nope>
    <niet id="-1">
      <nein>I tak nie użyję.</nein>
      <pleaseno>Czy to lata 90?</pleaseno>
    </niet>
</nope>
```
---

## XML - what if

- HTTP: `Accept` multiple
- Use XML only where needed
- Multi serialization (accept many)

---

## XML

| Cecha | Ocena |
| --- | --- |
| Enterprise ready | &#127798;&#127798;&#127798; |
| Nowoczesność | |
| Czytelność | &#127798;&#127798;&#127798;
| Schemat | &#127798;&#127798;&#127798; 
| Wydajność | &#127798; |
| Ocena ostateczna | &#127798; |

???

- CORBA, Java, .NET interop
- WS* - gwiazdka śmierci

---

## JSON

```json
{
    id: 1,
    title: "A green book",
    price: 12.50,
    tags: ["home", "green"],
    details : {
      description : "some"
    } 
}
```
---

## JSON.NET

```c#
[JsonObject(MemberSerialization.OptIn)]
public class Book
{
    [JsonProperty]
    public string Title { get; set; }

    [JsonProperty]
    public decimal Price { get; set; }

    [JsonProperty]
    public string[] Tags { get; set; }

    // not serialized because mode is opt-in
    public string TempNotes { get; set; }
}
```

---

## JSON.NET

```c#
var computer = new Computer
{
    Cpu = "Intel",
    Memory = 32,
    Drives = new List<string>
    {
        "DVD",
        "SSD"
    }
};

var o = (JObject)JToken.FromObject(computer);
var a = (JArray)JToken.FromObject(computer.Drives);
var i = (JValue)JToken.FromObject(computer.Cpu);
```

---

## JSON.NET vs JIL

```c#
// JSON.NET writing Guids

public override void WriteValue(Guid value)
{
  InternalWriteValue(JsonToken.String);

  _writer.Write(_quoteChar);
  _writer.Write(value.ToString("D"));
  _writer.Write(_quoteChar);
}
```

---

## JSON.NET vs JIL

```c#
static void _WriteGuid(TextWriter writer, Guid guid,
  char[] buffer)
{
  // get all the dashes in place
  buffer[8] = '-'; buffer[13] = '-';
  buffer[18] = '-'; buffer[23] = '-';

  var visibleMembers = new GuidStruct(guid);

  // bytes are in different order
  // bytes[0]
  var b = visibleMembers.B00 * 2;
  buffer[6] = WriteGuidLookup[b];
  buffer[7] = WriteGuidLookup[b + 1];
  // ...
  
  writer.Write(buffer, 0, 36);
}

```

---

## JSON.NET & Jil

| Cecha | Ocena ( JSON.NET) | Ocena (Jil) |
| --- | --- |
| Enterprise ready | &#127798;&#127798;&#127798; |-|
| Nowoczesność | &#127798;&#127798; | &#127798;&#127798;&#127798; |
| Czytelność | &#127798;&#127798;&#127798; | &#127798;&#127798;&#127798;
| Wydajność | &#127798; | &#127798;&#127798; |
| Schemat | &#127798; | &#127798;
| Ocena ostateczna | &#127798; | &#127798;&#127798; |

---

## Google Protocol Buffers

```proto
message SearchRequest {
  string query = 1;
  int32 page_number = 2;
  int32 result_per_page = 3;
  bytes payload = 4;
  oneof test_oneof {
    string more = 5;
    string more_more = 6;
  }
}
```

---

## Google Protocol Buffers

```js
[prefix1][value1]
[prefix2][value2]
[prefix3][value3]

[prefix] = (field_number << 3) | wire_type

```

- wire_type:
  - 0 - Varint
  - 1 - 64-bit
  - 2 - Length-delimited
  - 3 - Start group groups (deprecated)
  - 4 - End group groups (deprecated)
  - 5 - 32-bit
- field_number - varint
- value - varbinary

---

## Google Protocol Buffers

```c#
[StructLayout(LayoutKind.Explicit)]
public struct DiscriminatedUnion64
{
  [FieldOffset(0)] readonly int _discriminator; 

  [FieldOffset(8)] public readonly long Int64;
  [FieldOffset(8)] public readonly ulong UInt64;
  [FieldOffset(8)] public readonly int Int32;
  [FieldOffset(8)] public readonly uint UInt32;
  [FieldOffset(8)] public readonly bool Boolean;
  [FieldOffset(8)] public readonly float Single;
  [FieldOffset(8)] public readonly double Double;

  [FieldOffset(16)] public readonly object Object;
}
```

---

## Google Protocol Buffers

| Cecha | Ocena |
| --- | --- |
| Enterprise ready | &#127798;&#127798; |
| Nowoczesność | &#127798;&#127798; |
| Czytelność | &#127798;
| Schemat | &#127798;&#127798;&#127798; 
| Wydajność | &#127798;&#127798; |
| Ocena ostateczna | &#127798;&#127798; |

---

## Custom Protocols

Monitoring NServiceBus requires:
- metrics:
  - Critical Time
  - Processing Time
  - Retry Occurrence
- record every time it's measured (with date)
- record a message type

---

## Custom Protocols - JSON

```json
{
    metric: "CriticalTime",
    value: 1231231,
    messageType: "Your.Namespace.Your.Type.Very.Long"
    date: 123128321903
}
```

---

## Custom Protocols - Protocol Buffers

```proto
message Entry {
  Metric metric  = 1;
  int32 value = 2;
  string messageType = 3;
  int64 date = 4;
}
```

---

## Custom Protocols

- stały zbiór metryk (grupowanie per metryka)
- pomiary blisko siebie w czasie
- mała liczba typów wiadomości

```c#
DateTime date;
int messageType;
long measurement;
```
---

## Custom Protocols

```c#
long commonDate;

int dateDiff;
int messageType;
long measurement;
```
---

## Custom Protocols

```c#
Dictionary<int,string> messageTypes;
long commonDate;

int dateDiff;
int messageType;
long measurement;
```
???
- 16 byte'ów dla jednego wpisu
- stały narzut na cały raport (słownik i data)
- wydajne pamięciowo dla raportującego i przetwarzającego

---
## Custom Protocols

| Cecha | Ocena |
| --- | --- |
| Enterprise ready | &#127798; |
| Nowoczesność | &#127798;&#127798;&#127798; |
| Czytelność | &#127798;
| Schemat | &#127798;|
| Wydajność | &#127798;&#127798;&#127798;|
| Ocena ostateczna | &#127798;&#127798; |

---

## Inne

- Wire
- Simple Binary Encoding (zero copy)
- FlatBuffers / Cap'n'Proto (zero copy)

---

## Co dalej?

```c#
Span<byte> stackSpan = stackalloc byte[100];
```

```c#
pubic void Store(object obj)
{
  Span<byte> buffer = stackalloc byte[100];
  Serialize(obj, buffer);

  // ...
}
```

```c#
System.IO.Pipelines
```
---

## Wrapping up

- multi kulti
- tekst kontra byte'y
- szybsze wersje serializatorów
- Span

---

## Sekretny Sos Serializacji

.center[![image](img/qr.png)]

.center[[https://blog.scooletz.com](https://blog.scooletz.com)]

## Szymon Kulec @Scooletz