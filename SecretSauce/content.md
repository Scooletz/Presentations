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
- Wire
- Simple Binary Encoding
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
| Schemat | &#127798; &#127798; &#127798; 
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



---

## JSON.NET & Jil

| Cecha | Ocena ( JSON.NET) | Ocena (Jil) |
| --- | --- |
| Enterprise ready | &#127798;&#127798;&#127798; |-|
| Nowoczesność | &#127798;&#127798; | &#127798;&#127798;&#127798; |
| Czytelność | &#127798;&#127798;&#127798; |
| Wydajność | &#127798; |
| Ocena ostateczna | &#127798; |

---

## Wrapping up

---

## Sekretny Sos Serializacji

.center[![image](img/qr.png)]

.center[[https://blog.scooletz.com](https://blog.scooletz.com)]

## Szymon Kulec @Scooletz