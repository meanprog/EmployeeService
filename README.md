# Тестовое задание
## Описание решения
Реализован RESTful веб-сервис для управления сотрудниками на .NET Core с использованием:
- **Dapper** в качестве ORM
- **MySQL** как СУБД
- **Docker** для контейнеризации

##  Требования

| Требование | Реализация | Особенности |
|------------|------------|-------------|
| Добавление сотрудников | `POST /api/employees` | Возвращает ID созданного сотрудника |
| Удаление по ID | `DELETE /api/employees/{id}` | Идемпотентная операция |
| Список по компании | `GET /api/employees/company/{id}` | Полная информация о сотрудниках |
| Список по отделу | `GET /api/employees/company/{id}/department/{name}` | Двойная фильтрация |
| Частичное обновление | `PUT /api/employees/{id}` | Обновляет только переданные поля |


Сервис будет доступен на http://localhost:8080

Добавление
![создал.PNG](ScreenShots%2F%F1%EE%E7%E4%E0%EB.PNG)
Список по компании
![сотрудники 2.PNG](ScreenShots%2F%F1%EE%F2%F0%F3%E4%ED%E8%EA%E8%202.PNG)
![сотрудники 1.PNG](ScreenShots%2F%F1%EE%F2%F0%F3%E4%ED%E8%EA%E8%201.PNG)
Список по отделу
![отдел разработки.PNG](ScreenShots%2F%EE%F2%E4%E5%EB%20%F0%E0%E7%F0%E0%E1%EE%F2%EA%E8.PNG)
Частичное обновление
![обновили.PNG](ScreenShots%2F%EE%E1%ED%EE%E2%E8%EB%E8.PNG)
![результат обновления.PNG](ScreenShots%2F%F0%E5%E7%F3%EB%FC%F2%E0%F2%20%EE%E1%ED%EE%E2%EB%E5%ED%E8%FF.PNG)
Удаление по ID
![удаление.PNG](ScreenShots%2F%F3%E4%E0%EB%E5%ED%E8%E5.PNG)
![результат удаления.PNG](ScreenShots%2F%F0%E5%E7%F3%EB%FC%F2%E0%F2%20%F3%E4%E0%EB%E5%ED%E8%FF.PNG)


## Что можно улучшить (из напрашивающегося для красоты)
Реализовать кеширование, добавить Swagger, написать тесты