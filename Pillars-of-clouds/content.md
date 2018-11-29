background-image: url(img/athens.jpg)
background-size: cover

# Filary chmur

## Szymon Kulec @Scooletz

???

"Chmura to bzdura". Jeszcze kilka lat temu, takie zdanie można było słyszeć ode mnie w momencie, w którym ktoś wspominał chmurę publiczną. Nie widziałem sensu w tych gigantycznych centrach danych skolokowanych w jednym miejscu. Co jeśli to wszystko zawiedzie? Co jeśli zabraknie miejsca na dane? Co jeśli infrastruktura nie utrzyma obciążenia. Te i podobne pytania pojawiały się w mojej głowie co chwilę. Ostatecznie, zostałem przekonany, czy może przekonałem się sam do instytucji chmury publicznej. Co odpowiedziało na powyższe pytania? Co przekonało mnie, i może przekonać Ciebie? Co pokazało, że chmura faktycznie ma niemałe szanse działania? Dzisiaj chciałbym podzielić się z Tobą moją ścieżką, przez różne elementy chmury publicznej, u różnych prowiderów. Zapraszam do zgłębienia filarów na których stoi chmura.

Nazywam się Szymon Kulec, a to są Filary Chmur.

Co mnie nie przekonało do chmury...

---

background-image: url(img/hype.jpg)
background-size: cover

## Hype / moda / hashtagi

--

- mikroserwisy

--

- skalowalność

--

- każdy to robi

--

- posty a'la _ten najlepszy sposób na ..._

???

---

background-image: url(img/pallette.jpg)
background-size: cover

## Tyyyyle wyborów

--

- 10 baz danych

--

- 5 systemów kolejkowych

--

- 3 sposoby wyszukiwania

--

- 200 rodzajów VMek

---

background-image: url(img/papers.jpg)
background-size: cover

## Papierowe filary

---

background-image: url(img/storage.jpg)
background-size: cover

## Storage: Azure Storage Services

- Queues
- Tables
- Blobs
- Disks

---

background-image: url(img/storage.jpg)
background-size: cover

## Storage: Azure Storage Services (2)

--

- strong consistency

  - Twierdzenie CAP
  - optimistic concurrency

--

- globalna/skalowalna przestrzeń nazw

--

- Disaster Recovery

--

- multi-tenant

???

- twierdzenie CAP
- namespace'y
- wiele Data Center
- dawanie tych samych danych dla wielu użytkowników

---

background-image: url(img/storage.jpg)
background-size: cover

## Storage: Azure Storage Services - przestrzenie nazw

```html
http(s)://{account}.{service}.core.windows.net/{partition}/{object}
```

???

- account - Storage Account
- service - jeden z serwisów
- partition - partycja
- object - pojedynczy obiekt w partycji

- blobs & queues - bez obiektu
- tables - PartitionKey = obiekt

---

background-image: url(img/storage.jpg)
background-size: cover

## Storage: Azure Storage Services - architektura

--

- Storage  Stamp

  - 10 - 20  racków
  - 2PB - 30PB danych
  - wysycenie 70 %

--

- Location Service (LS)

  - zarządza SSami
  - aktualizuje DNSy

---

background-image: url(img/storage.jpg)
background-size: cover

## Storage: Azure Storage Services - architektura - trzy warstwy

--

- Stream Layer

  - "system plików"
  - "pliki" to listy wskaźników do extent'ów
  - extent składa się z bloków
  - replikuje i przechowuje dane bez zrozumienia

--

- Partition Layer

  - rozumie pojęcia: blob, kolejka, tabela
  - partycjonuje dane w Stampie
  - cache'uje dane

--

- Front-End (FE)

  - autentykacja
  - cache mapy partycji
  - może przeskoczyć Partition Layer, dla dużych obiektów

---

background-image: url(img/storage.jpg)
background-size: cover

## Storage: Azure Storage Services - przykład

```html
Request:  
POST https://myaccount.queue.core.windows.net/messages HTTP/1.1  

Headers:  
x-ms-version: 2011-08-18  
x-ms-date: Tue, 30 Aug 2011 01:03:21 GMT  
Authorization: ABC
Content-Length: 100  

Body:  
<QueueMessage>  
<MessageText>base64payload</MessageText>  
</QueueMessage>  
```

???

- wiadomość wysyłana na dany adres
- przechodzi przez Front-End, który znajduje partycję "myaccount->messages"
- partycja znajduje odpowiedni strumień i dodaje do niego wiadomość

---

background-image: url(img/storage.jpg)
background-size: cover

## Storage: Azure Storage Services - replikacja

--

- stream -> extent -> block

--

- do strumienia można tylko dodawać (_append-only_)

--

- _multi-block append_

--

- extent - jednostka replikacji

--

- replikacja zarządzana przez Stream Manager

---

background-image: url(img/storage.jpg)
background-size: cover

## Storage: Azure Storage Services - podsumowanie

--

- CP... ale duże A

--

- partycje, to wszytko partycje

--

- _append-only_

--

- Whitepaper: _Windows Azure Storage: A Highly Available Cloud Storage Service with Strong Consistency_

---

background-image: url(img/time.jpg)
background-size: cover

## Czas: Google TrueTime

--

- _Która godzina?_

???

Jest jedno pytanie, którego boją się wszyscy posiadający wiele DC. Która godzina.

--

- dryft zegara

--

- NTM - Network Time Protocol

???

Google zbudował znacznie lepszą odpowiedź na to.

---

background-image: url(img/time.jpg)
background-size: cover

## Czas: Google TrueTime - składowe

???

Każdy może zbudować w domu takie API

--

- serwer

--

- GPS

--

- zegar atomowy

---

background-image: url(img/time.jpg)
background-size: cover

## Czas: Google TrueTime - API

```c
TT.now() => [from, to]

TT.after(t) => bool

TT.before(t) => bool

const delta = 7ms
```

???

Nie da się określić jednej daty
Dokładność 7ms

---

background-image: url(img/time.jpg)
background-size: cover

## Czas: Google TrueTime - Spanner

- skalowalny

- rozproszony

- partycjonowany (Paxos)

- dwa rodzaje transakcji:

  - Read-Only

  - Read-Write

---

background-image: url(img/time.jpg)
background-size: cover

## Czas: Google TrueTime - Spanner - Read-Only

???

Transakcja odczytująca

--

- użycie `TT.now().latest` dla snapshotu

--

- obsługiwane przez dowolne repliki, które mają tę datę

---

background-image: url(img/time.jpg)
background-size: cover

## Czas: Google TrueTime - Spanner - Read-Write

--

- użycie `TT.now().latest` jako datę commitu

???

Transakcja zapisująca

--

- koordynator oczekuje `2 * delta` co zapewnia że commit jest w przeszłości

---

background-image: url(img/time.jpg)
background-size: cover

## Czas: Google TrueTime - Spanner - liniowość

```C
t1 < t2 < t3 < t4 ...
```

???

Efektywnie oznacza możliwość uliniowienia transakcji, bez wektor clocków itp

---

background-image: url(img/time.jpg)
background-size: cover

## Czas: Google TrueTime (TM) - podsumowanie

--

- super dokładny zegar

--

- umożliwia sortowanie transakcji

--

- liniowość operacji bez zewnętrznych struktur danych

--

- Whitepaper: _Spanner: Google’s Globally Distributed Database_

---

background-image: url(img/athens.jpg)
background-size: cover

# Filary chmur

## Szymon Kulec @Scooletz