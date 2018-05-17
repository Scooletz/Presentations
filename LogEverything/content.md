background-image: url(img/fields.jpg)
background-size: cover

# Zalogować wszystko

## Szymon Kulec @Scooletz

???

Kilkanaście miesięcy temu pracowałem nad fragmentem kodu. Komponent ten zawierał kilka kilkanaście linijek związanych z logowaniem.

---

background-image: url(img/logs.jpg)
background-size: cover

## Logowanie, trace'owanie

- biblioteki
  - log4j, log4net, log4py, log4you, log4me
  - logback, NLog, Serilog

- systemowe
  - Windows: Performance Counters, Event Tracing
  - Linux: LTTng

???

- choroba wspólnego mianownika (logowanie do wszystkiego)
- ręczne przesuwanie bitów
- co pomiędzy?
- co z audytem?

---

background-image: url(img/money.jpg)
background-size: cover

## Logowanie w SaaS

|Ile | Retencja | Cena |
| ---: |---:| ---:|
|1 GB/dzień | 30 dni|$79|
|3mln/miesiąc | 180 dni|$499|
|1 GB/dzień | - |$79|

---

background-image: url(img/horses.jpg)
background-size: cover

## Szybsze konie

> “If I had asked people what they wanted, they would have said faster horses.”

Henry Ford (niby)

---

background-image: url(img/musk.jpg)
background-size: cover

## ~~Szybsze konie~~ Tesla

A co jeśli za 50$ miesięcznie zalogowałbym:

--
- 60.000.000.000 wpisów miesięcznie

--
- 2.000.000.000 dziennie

--
- 83.000.000 na godzinę

--
- 1.300.000 na minutę

---

background-image: url(img/musk-smile.jpg)
background-size: cover

## ~~Szybsze konie~~ Tesla

---

background-image: url(img/writing.jpg)
background-size: cover

## API - semantyczne logowanie

```c#
log.Info(
  "Sent {count} from {sender} to {receiver}", 
  total, you, me);
```

???

- format, szablon, template
- wartości przekazane jako parametry
- może być różnie wydrukowany, zależnie od systemu z tyłu

---

background-image: url(img/wall.jpg)
background-size: cover

## Kategorie ~~problemów~~ wyzwań

- klient
  - wielkość wpisu
  - przepustowość
  - audyt
- przetwarzanie danych
  - chmura (co wybrać)
  - input output (prędkość)

---

background-image: url(img/size.jpg)
background-size: cover

## Klient - wielkość wpisu

```c#
log.Info(
  "Sent {count} from {sender} to {receiver}",
  total, you, me);
```

--

```js
{
 "template" : "Sent ...",
 "total" : 4098,
 "sender" : "Amber Gold",
 "receiver" : "Kajmany"
}
```

???

JSON FTW!!! - WRONG!

---

background-image: url(img/size.jpg)
background-size: cover

## Klient - wielkość wpisu

```c#
var templateId = Sha1("Sent ...");
var bytes = Serialize(total, sender, receiver);

Send(templateId, bytes);
```

???

- Sha1 generowane raz i wysyłane na początku
- dane serializowane binarnie (bez nazw)

---

background-image: url(img/pipe.jpg)
background-size: cover

## Klient - przepustowość

- liczba wpisów na sekundę (_throughput_)
- czas pojedynczej operacji (_service time_)

--
- ~~wysyłania pojedynczych wpisów~~
- ~~czekania na wysłanie wszystkiego~~
- ~~serializacji przy wysyłce~~

---

background-image: url(img/pipe.jpg)
background-size: cover

## Klient - przepustowość

### Smart batching

- akumulacja wpisów do:
  - upłynięcia _k_ ms
  - zbudowania paczki o rozmiarze _m_ kb

--
- wysłanie paczek:
  - równoległe
  - sensownie limitowane

--
- struktura danych:
   - cykliczny bufor
   - współbieżny (_0 locków_)
   - wielu producentów

---

background-image: url(img/pipe.jpg)
background-size: cover

## Klient - przepustowość

### Cykliczny bufor

```
00.....08.....16.....24.....
[wpis0][wpis1][wpis2]
                     ^
                     |
─────────────────────┘
```
--
```
00.....08.....16.....24.....
[wpis4][wpis5]
              ^
              |
──────────────┘
```

---

background-image: url(img/check.jpg)
background-size: cover   

## Klient - audyt

```c#
await CallImportantBusinessMethod();

await log.Audit("Money sent {amount}", amount);

await CallOtherBusinessMethod();
```

---

background-image: url(img/check.jpg)
background-size: cover   

## Klient - audyt

### Pozycja w logu

```
00.....08.....16.....24.....
[wpis0][wpis1][wpis2][wpis3]
      ^
      |
──────┘

```

???

- zapis -> pozycja w buforze
- oczekiwanie, aż pozycja wysłana

---

background-image: url(img/check.jpg)
background-size: cover   

## Klient - audyt

### Pozycja w logu

```
00.....08.....16.....24.....
[wpis0][wpis1][wpis2][wpis3]
             ^
             |
─────────────┘

```

---

background-image: url(img/musk-smile.jpg)
background-size: cover

## Bla, bla, pokaż liczby!

- 10 mln zdarzeń
- wiele wątków (narzut)
- symulacja z serializacją (narzut)
- czas netto, bez transportu

--

|Biblioteka | Czas
| ---: |---:|
| Serilog | 00:00:08.3470107
| X |  00:00:02.1679472

???

- 10 mln zdarzeń
- każde zserializowane do formy do przesłania
- żadne nie przesłane
- narzut asynchronicznego wywołania dodany sztucznie

---

background-image: url(img/wall.jpg)
background-size: cover

## Kategorie ~~problemów~~ wyzwań

- [x] klient
  - [x] wielkość wpisu
  - [x] przepustowość
  - [x] audyt
- [ ] przetwarzanie danych
  - [ ] chmura (co wybrać)
  - [ ] input output (prędkość)

---

# Zalogować wszystko

## Szymon Kulec @Scooletz