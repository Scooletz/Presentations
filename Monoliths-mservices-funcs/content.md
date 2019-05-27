background-image: url(img/monolith.jpg)
background-size: cover

# Monolity, mikroserwisy, funkcje i...dalsze kroki w nieznane

## Szymon Kulec @Scooletz

???

- cześć, czy dobrze mnie słychać?
- nazywam się...
- a to są Monolity, mikroserwisy, funkcje i dalsze kroki w nieznane

---

background-image: url(img/cursor.gif)
background-size: cover

???

- Nowootwarty projekt
- bez narzutów
- bez poprzednich decyzji
- idealny green field
- jakie to uczucie?

- byłem w tym miejscu
- jak to zrobić?

---

background-image: url(img/message.jpg)
background-size: cover

## Ścieżka a'la Simon Wardley

.mermaid[graph LR
    Genesis((Genesis))
    Custom((Custom Built))
    Product((Product))
    Commodity((Commodity))
    Genesis ==> Custom
    Custom ==> Product
    Product ==> Commodity
]

---

background-image: url(img/2001.png)
background-size: cover

## IT 2001: Monolit

- mój system jest samodzielny

- po co mu integracje

- mogę objąć wszystko transakcją

---

background-image: url(img/message.jpg)
background-size: cover

## Serwisy - mikro, nano, piko - te złe

- używające innych serwisów do każdej czynności

--

- oczekujące na zakończenie wywołania

--

- zmniejszające swoje SLA ile się tylko da

---

background-image: url(img/message.jpg)
background-size: cover

## Serwisy - mikro, nano, piko - te złe

.mermaid[sequenceDiagram
    participant Client
    participant YourApp
    participant DB
    participant A
    participant B
    Client->>+YourApp: POST form
    YourApp->>DB: update
    YourApp->>+A: call
    A->>+B: call
    B->>-A: ok
    A->>-YourApp: ok
    YourApp->>-Client: OK
]

???

- jak to wyglądało przed

---

background-image: url(img/message.jpg)
background-size: cover

## Serwisy - mikro, nano, piko - te dobre

- używające własnych danych do wykonania własnych operacji

--

- komunikujące się z innymi asynchronicznie

--

- dbające o SLA

---

background-image: url(img/message.jpg)
background-size: cover

## Serwisy - mikro, nano, piko - te dobre

.mermaid[sequenceDiagram
    participant Client
    participant YourApp
    participant DB
    participant A
    participant B
    Client->>+YourApp: POST form
    YourApp->>DB: update
    YourApp-->>A: msg
    YourApp-->>B: msg
    YourApp->>-Client: OK
]

---

background-image: url(img/message.jpg)
background-size: cover

## Gdzie jesteśmy - Simon Wardley maps

.mermaid[graph LR
    Genesis((Genesis))
    Custom((Custom Built))
    Product((Product))
    Commodity((Commodity))
    Genesis ==> Custom
    Custom ==> Product
    Product ==> Commodity
]

---

background-image: url(img/commodity.jpg)
background-size: cover

## Funkcje - computation as commodity

- brak zarządzania

- brak informacji gdzie wykonywane

- automatyczne skalowane

- opłata za ilość => 0 gdy nic nie jest wykonywane

???

- opłata za ilość = jak z mąką, za kilogram

- fajnie, ale co Szymon, co chcesz zbudować

---

background-image: url(img/question.jpg)
background-size: cover

## _Prosty_ _serwis_ do kursów

1. nagrania

1. autoryzacja

1. oznaczanie obejrzanych nagrań

???

- co może być trudnego z zrobieniu serwisu do hostowania kursów?
- prosty serwis oznaczone kursywą - żart.

---

background-image: url(img/question.jpg)
background-size: cover

## _Prosty_ _serwis_ do kursów - dodatkowe

1. zainteresowanie i oglądalność

1. ilość/wartość pracy

1. przetestowanie _bycia bliżej commodity_

???

- ludzie często oglądają o określonych porach - po wypuszczeniu kursu

- ile mi to zajmie?

- ile pracy muszę wykonać?

---

background-image: url(img/video.jpg)
background-size: cover

# Nagrania - zrób to sam

- Azure Storage -> blobs

- wystarczająco tanie

- skalowalne, dobra wydajność

???

- nieustruktyryzowane dane w blobach

- Playr - player javascript

---

background-image: url(img/video.jpg)
background-size: cover

# Nagrania - zrób to sam 2

- dane w chmurze / dane z chmury

- 1 blob = 60Mb/s

- 1 video = setki megabyte'ów

???

- koszty wyprowadzania danych

- przepustowość bloba (asymetryczność) / uruchamianie kursów

- rozmiary video

- skalowalnie blobów

- NIE

---

background-image: url(img/video-azure.jpg)
background-size: contain

---

background-image: url(img/video.jpg)
background-size: cover

# Nagrania - użycie serwisu zewnętrznego

- użycie hosta nagrań (ale nie kursów)

- bezpieczeństwo

- Vimeo

???

- autoryzacja per domena
- prywatne wideo
- dobre możliwości uploadu
- sensowna cena
- brak limitu łącza

---

background-image: url(img/video-vimeo.jpg)
background-size: contain

---

background-image: url(img/security.jpg)
background-size: cover

## Autoryzacja - Azure Functions

- zarządzanie Identity

- brak wiedzy

- niejasne połączenie z serwisami zewnętrznymi

---

background-image: url(img/security.jpg)
background-size: cover

## Autoryzacja - zrób to sam

- może Identity Server?

- OAuth - implementacja?

- kto będzie konsumował?

- hostowanie?

???

- wiele niejasnych pytań

---

background-image: url(img/security.jpg)
background-size: cover

## Autoryzacja - Auth0

- OAuth

- developer friendly

- free tier

- dostępność pipeline'u

- wielu providerów

---

background-image: url(img/security.jpg)
background-size: cover

## Autoryzacja - Auth0 - Implicit Flow

.mermaid[sequenceDiagram
    participant YourSite
    participant Auth0
    participant IdentityProvider
    YourSite->>Auth0: redirect
    Auth0->>IdentityProvider: redirect
    IdentityProvider->>Auth0: redirect
    Auth0->>YourSite: redirect with anchor
]

???

- dla nieznających OAuth, OpenId Connect
- dla znających zatkać uszy
- znam Authorization Code Grant

---

background-image: url(img/auth-Auth0.jpg)
background-size: contain

???

- podsumuj: 2 elementy z listy gotowe:
  - nagrania
  - autoryzacja

- jest token z Auth0 autoryzując i token Id
- co oznaczaniem obejrzanych nagrań??

---

background-image: url(img/check.jpg)
background-size: cover

## Oznaczanie obejrzanych nagrań - func them all!

- _po prostu użyję funkcji jak aplikacji_

- użytkownik woła funkcję kiedy potrzebuje

???

- efektywnie: whostowanie w funkcje aplikacji

---

background-image: url(img/check.jpg)
background-size: cover

## Oznaczanie obejrzanych nagrań - func them all!

.mermaid[sequenceDiagram
    participant SPA
    participant Auth0
    participant IdentityProvider
    participant Function
    participant Tables
    SPA->>Auth0: redirect
    Auth0->>IdentityProvider: redirect
    IdentityProvider->>Auth0: redirect
    Auth0->>SPA: redirect with anchor
    SPA->>Function: mark as viewed
    Function->>Tables: update
]

---

background-image: url(img/check.jpg)
background-size: cover

## Oznaczanie obejrzanych nagrań - func them all!

```csharp
[FunctionName("UpdateVideo")]
public static async Task<IActionResult> Run(
    [HttpTrigger("post", Route = null, 
    /*somehow pass the token and claims*/)]
    HttpRequest req, ILogger log)
{
    // update database
}
```

???

- i tak dla każdej czynności użytkownika
- aplikacja rozbita na funkcyjną miazgę

---

background-image: url(img/views-funcapp.jpg)
background-size: contain

???

- opisz

- czy da się lepiej?

- czy musimy pisać aplikację jako funkcje?

- czy trzeba przeklepywać kod na taką makabrę?

- WRÓCMY do Azure

---

background-image: url(img/check.jpg)
background-size: cover

## Oznaczanie obejrzanych nagrań - Azure Storage Tables

|Partition Key |Row Key | Lesson 1  | Lesson 2   |
|---|---|---|---|
| user1  | course1   |  &#x2714;  | &#x2714;   |
| user1  | course2  |  &#x2714;   |   |
| user2  | course1  |   |    |
| user3  | course1  |   |  &#x2714; |

???

- przykładowe mapowanie danych

- PartitionKey + RowKey

- key-value mapowany do kolumn

- co da się zrobić?

---

background-image: url(img/check.jpg)
background-size: cover

## Oznaczanie obejrzanych nagrań - Azure Storage Tables

- _Security Access Signature_ token

- token - url + parametry

- granularność:
  - tabela
  - czas
  - IP
  - PK+RK `(starpk,startrk) =(user1,) (endpk, endrk)=(user1,zzzzzz)`

---

background-image: url(img/check.jpg)
background-size: cover

## Oznaczanie obejrzanych nagrań - jedyna funkcja naszej aplikacji

```csharp
[FunctionName("Authorize")]
public static async Task<IActionResult> Run(
    [HttpTrigger(/*omitted*/)] HttpRequestMessage req)
{
    var authorize = await req.Content.ReadAsAsync<Authorize>();
    var storage = GetStorageAccount();
    // retrieve table

    var updateToken = table.GetSharedAccessSignature(/*params*/);

    // a few more tokens

    return new OkObjectResult(tokens);
}
```

---

background-image: url(img/check.jpg)
background-size: cover

## Oznaczanie obejrzanych nagrań - flow przed zmianą

.mermaid[sequenceDiagram
    participant SPA
    participant Auth0
    participant IdentityProvider
    participant Function
    participant Tables
    SPA->>Auth0: redirect
    Auth0->>IdentityProvider: redirect
    IdentityProvider->>Auth0: redirect
    Auth0->>SPA: redirect with anchor
    SPA->>Function: mark as viewed
    Function->>Tables: update
]

???

flow przed zmianą

---

background-image: url(img/check.jpg)
background-size: cover

## Oznaczanie obejrzanych nagrań - flow po zmianie

.mermaid[sequenceDiagram
    participant SPA
    participant Auth0
    participant IdentityProvider
    participant Function
    SPA->>Auth0: redirect
    Auth0->>IdentityProvider: redirect
    IdentityProvider->>Auth0: redirect
    Auth0->>Function: get token
    Auth0->>SPA: redirect with anchor
    SPA->>Tables: update
]

???

flow po zmianie

---

background-image: url(img/views-token.jpg)
background-size: contain

???

- opisz

- ok ok co z oglądaniem nagrań?

---

background-image: url(img/tv.jpg)
background-size: cover

## Oglądanie nagrań

- _collaborative domain_ vs _non-collaborative_

- odczyt vs zapis

- wszyscy czytają to samo

???

- opisz domenę kolaboratywną

- zapis różnych danych - odczyt tych samych

- dodać kilka tokenów, które pozwolą odczytać urle

---

background-image: url(img/tv.jpg)
background-size: cover

## Oglądanie nagrań - tokeny

```csharp
[FunctionName("Authorize")]
public static async Task<IActionResult> Run(
    [HttpTrigger(/*omitted*/)] HttpRequestMessage req)
{
    var authorize = await req.Content.ReadAsAsync<Authorize>();
    var storage = GetStorageAccount();
    // retrieve table

    var updateToken = table.GetSharedAccessSignature(/*params*/);

    var viewTokens = GetTokensForBlobs();

    return new OkObjectResult(tokens);
}
```

???

przechodzą do odpowiedzi...

---

background-image: url(img/question.jpg)
background-size: cover

## _Prosty_ _serwis_ do kursów - odpowiedzi

1. nagrania - Vimeo

1. autoryzacja - Auth0

1. oznaczanie obejrzanych nagrań - SAS token + direct

???

- jeżeli chodzi o odpowiedzi
- a jeżeli chodzi o szkic mapy

---

background-image: url(img/question.jpg)
background-size: cover

## Co dalej?

- computation as commodity

- inercja - ruch na mapie

- rozwiązania:

  - Identity Management w Azure

  - Firebase

???

- INERTIA - przesuwa elementy w prawo

- ruch na mapie - warto zastanowić się gdzie następuje ruch i co robić

- Identity Management w Azure
  - coraz więcej serwisów, zasady dostępu
  - uruchamianie wykonania pod credentialami osoby, która kliknęła
- Firebase
  - może wszystko :P
  - pełne połączenie konta + bazy

---

background-image: url(img/views-token.jpg)
background-size: contain

???

- CO DALEJ?
  - commodity
  - INERTIA - przesuwa elementy w prawo

- brakuje jeszcze wielu elementów

- Inertia - rozpęd przesuwa w prawo

- czy warto to robić? zastanowienie

- mam nadzieję, że zaciekawiłem Was drogą z mapą

---

background-image: url(img/monolith.jpg)
background-size: cover

# Monolity, mikroserwisy, funkcje i...dalsze kroki w nieznane

## Szymon Kulec @Scooletz