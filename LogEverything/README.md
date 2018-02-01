# How to log everything
How to log everything, war stories from building a highly opitimized logging service with serverless architecture

# Jak zalogować wszystko
Jak zalogować wszystko, czyli historia usługi przetwarzającej miliardy logów dziennie.

- Pamiętasz szukanie w logach informacji, która nie została zapisana z jakiegoś powodu?
- Pamiętasz wybory pomiędzy poziomami logowania, co jest informacją dla trybu DEBUG, a co ostrzeżeniem WARN?
- Pamiętasz usuwanie logowania niektórych operacji z powodu "braku miejsca"?

Sam znam aż za dobrze powyższe problemy. Dlatego chcę podzielić się z Tobą historią usługi stworzonej w podejściu serverless, która adresuje powyższe punkty. Podczas prezentacji omówię podejścia i wyzwania związane ze sprawnym logowaniem. Dodatkowo, opowiem w jaki sposób efektywnie przetwarzać dane w architekturze serverless, tak, aby nie płacić kilkucyfrowego rachunku za chmurę. Zapraszam do podróży w krainę danych, gdzie logujemy każdą operację!

# How to log everything

How to log everything, or a story of a logging service processing billion entries per day. Do you remember:

- searching for a lot entry that was not stored because of a configuration setting?
- choosing between WARN and INFO, and then dropping some vital information on the production?
- dropping some entries because of hitting the space limit and then worrying again?

I was recalling all of this problems much to often. That's why I want to share with you a story, about a service created in a truly serverless way, which addressed all of this points. During the talk I'll discuss approaches and challenges connected with a well-performing and trusted logging. Additionally, I'll share some tricks how to efficiently use serverless and don't pay countless $$ for you application. Are you ready for blistering 45 of serverless logging?
