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
<img width="663" height="667" alt="создал" src="https://github.com/user-attachments/assets/57d0c451-affd-4199-9a2f-efbd197000cd" />

Список по компании
<img width="615" height="879" alt="сотрудники 1" src="https://github.com/user-attachments/assets/8f28d54e-2d2d-4217-ad96-45083be26eba" />

Список по отделу
<img width="797" height="889" alt="отдел разработки" src="https://github.com/user-attachments/assets/e820ecb5-18b2-480e-bdc7-c3db524b41f5" />

Частичное обновление
<img width="893" height="902" alt="результат обновления" src="https://github.com/user-attachments/assets/91a6d5d6-9db1-41bf-b588-6efd33cec445" />
<img width="786" height="708" alt="обновили" src="https://github.com/user-attachments/assets/f3a0d00b-f69f-4f8d-8463-50f3a48187da" />


Удаление по ID
<img width="882" height="604" alt="удаление" src="https://github.com/user-attachments/assets/5e239408-f952-4fb6-89ce-3a923efd564b" />
<img width="965" height="764" alt="результат удаления" src="https://github.com/user-attachments/assets/f366482f-fd81-454d-8ca1-09c1ec390843" />



## Что можно улучшить (из напрашивающегося для красоты)
Реализовать кеширование, добавить Swagger, написать тесты
