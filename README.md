🍽 Canteen API

![C#](https://img.shields.io/badge/C%23-7.0-purple?style=flat-square&logo=csharp)
![Entity Framework Core](https://img.shields.io/badge/EF%20Core-9.0.2-green?style=flat-square&logo=ef)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15-blue?style=flat-square&logo=postgresql)

---

## 📚 Описание проекта

**Canteen** — это веб-приложениее для управления заказами в столовой. 
**ApiDomain** содержит основной слой Api. Этот слой позволяет:
- Работать с клиентами, заказами, блюдами и корзинами.
- Управлять категориями блюд.
- Использовать Swagger для тестирования API.

API разработано на **ASP.NET Core** с использованием **Entity Framework Core** и **PostgreSQL** для хранения данных.

---

## 📊 ER-диаграмма базы данных

Ниже представлена схема структуры базы данных:

![📌 CanteenDB](https://github.com/11qfour/Canteen/tree/main/media/ERDiagrams.png)

---

## 🛠️ Технологии

- **Backend**: ASP.NET Core 7, Entity Framework Core.
- **Frontend**: React (планируется).
- **База данных**:  PostgreSQL.
- **Инструменты разработки**: Visual Studio, Visual Studio Code, Figma, PgAdmin.

---

## 📂 Структура проекта
* Canteen/
* ├── ApiDomain/                  # Основные модели и репозитории
* │   ├── Configurations/         # Конфигурации (связи между сущностями БД)
* │   ├── Enums/                  # Перечисления (для отображения статусов заказа и корзины)
* │   ├── Models/                 # Классы моделей
* │   ├── Repositories/           # Репозитории (CRUD-операции)
* │   ├── Services/               # Сервисы
* │   ├── ApiListContext.cs/      # DB Context
* ├── Api/                        # Веб-приложение 
* │   ├── Controllers/            # Контроллеры API
* │   ├── DTO/                   # Data Transfer Objects
* │   ├── Program.cs/             # Сборщик всего проекта
* ├── media/                      # Скриншоты API и диаграммы
* │   ├── ERDiagrams.png          # ER-диаграмма базы данных
* │   ├── swaggerFirst.png        # Swagger UI
* │   ├── swaggerSecond.png       # Swagger UI
* └── README.md                   # Документация проекта

---

## 🔧 Установка и запуск

1. **Склонируйте репозиторий**:
   ```bash
   git clone https://github.com/your-repository-url.git
   cd canteen-api

2. **Настройте подключение к базе данных** в appsettings.json
3. **Примените миграции:**
    ```bash
    dotnet ef database update
4. **Запустите приложение:**
    ```bash
     dotnet run
5. **Откройте Swagger UI:** 👉 https://localhost:7168/swagger/index.html


## 🖼️ Интерфейс 

# Макет интерфейса

* https://www.figma.com/design/SMQj2yHzQJvgIGv9vpnJLL/canteen-maket?node-id=0-1&p=f&t=W4vIYsz2OeOkd4hF-0

# Реализованный интерфейс

* [в разработке]

---

## 🧪 Тестирование API при помощи **Swagger**

* Post-запросы

![📌 Успешное выполнение POST-запроса контроллера Dish](https://github.com/11qfour/Canteen/tree/main/media/postSuccess.png)

![📌 Ошибочное выполнение POST-запроса контроллера Dish, не найден CategoryId](https://github.com/11qfour/Canteen/tree/main/media/postError1.png)

![📌 Ошибочное выполнение POST-запроса контроллера Cart, нарушение связи Один к Одному](https://github.com/11qfour/Canteen/tree/main/media/postError2.png)

* Get-запросы

![📌 Успешное выполнение GET-запроса контроллера Employee](https://github.com/11qfour/Canteen/tree/main/media/getSuccess1.png)

![📌 Успешное выполнение GET-запроса по id контроллера Employee](https://github.com/11qfour/Canteen/tree/main/media/getSuccess2.png)

![📌 Ошибочное выполнение GET-запроса контроллера Employee, не найден EmployeeId](https://github.com/11qfour/Canteen/tree/main/media/getError.png)

* Put-запросы

![📌 Успешное выполнение PUT-запроса контроллера Cart](https://github.com/11qfour/Canteen/tree/main/media/putSuccess1.png)

![📌 Ошибочное выполнение PUT-запроса контроллера Order](https://github.com/11qfour/Canteen/tree/main/media/putError.png)

![📌 Исправление выполнение PUT-запроса контроллера Order](https://github.com/11qfour/Canteen/tree/main/media/putChange.png)

![📌Результат выполнения PUT-запроса -> GET-запрос контроллера Order (статус заказа - Ready)](https://github.com/11qfour/Canteen/tree/main/media/putSuccess2.png)

* Delete-запросы

![📌 Получение всех сотрудников](https://github.com/11qfour/Canteen/tree/main/media/deleteCheck.png)

![📌 Успешное выполнение DELETE-запроса по id контроллера Employee](https://github.com/11qfour/Canteen/tree/main/media/deleteSuccess.png)

---

## 🔬 Тестирование API при помощи **Postman**

1. Post-запросы

    ```bash
    https://localhost:7168/api/<название сущности>

    *[📌 Успешное выполнение POST-запроса контроллера Order](https://github.com/11qfour/Canteen/tree/main/media/PostSuccessPostman.png)
    *[📌 Ошибочное выполнение POST-запроса контроллера Cart, нарушение связи Один к Одному](https://github.com/11qfour/Canteen/tree/main/media/PostErrorPostman.png)

2. Get-запросы

    ```bash
    https://localhost:7168/api/<название сущности> //вывод всей информации
    https://localhost:7168/api/<название сущности>/{id} //вывод информации по ID

    ![📌 Успешное выполнение GET-запроса контроллера Category](https://github.com/11qfour/Canteen/tree/main/media/GetSuccessPostman.png)
    ![📌 Ошибочное выполнение GET-запроса контроллера Order, не найден OrderId](https://github.com/11qfour/Canteen/tree/main/media/GetErrorPostman.png)
    
3. PUT-запросы

    ```bash
    https://localhost:7168/api/<название сущности>/{id}

    ![📌 Успешное выполнение PUT-запроса контроллера Employee](https://github.com/11qfour/Canteen/tree/main/media/PutSuccessPostman1.png)

    ![📌 Успешное выполнение PUT-запроса контроллера Customer](https://github.com/11qfour/Canteen/tree/main/media/PutSuccessPostman2.png)

    ![📌 Ошибочное выполнение PUT-запроса контроллера Order, такой позиции в перечислении статуса нет, выходит за границы enum](https://github.com/11qfour/Canteen/tree/main/media/PutErrorPostman.png)

4. Delete-запросы

    ```bash
    https://localhost:7168/api/<название сущности>/{id}

    ![📌 Получение Cart](https://github.com/11qfour/Canteen/tree/main/media/GetDeleteCheckPostman.png)

    ![📌 Успешное выполнение DELETE-запроса по id контроллера Cart](https://github.com/11qfour/Canteen/tree/main/media/DeleteSuccessPostman.png)

    ![📌 Проверка Cart](https://github.com/11qfour/Canteen/tree/main/media/DeleteCheckPostman.png)

---
## 📦 Структура DTO
В проекте используются **Data Transfer Objects** (DTO):
# Тип DTO             ->        Назначение
* <name>CreateDto     ->        Создание (Request)
* <name>UpdateDto     ->        Обновление (Request)
* <name>Dto           ->      Вывод (Response)

---

## 🤝 Контрибьюция
Буду рад любым предложениям и пул-реквестам!
1. **Форкните**  этот репозиторий
2. Создайте **новую ветку** 
    ```bash
    git checkout -b feature-branch
3. Внесите изменения и сделайте коммит
    ```bash
    git commit -m 'add ...'
4. Отправьте изменения (git push origin feature-branch)
    ```bash
    git push origin feature-branch
5. Создайте Pull Request 🚀

---

## ✉️ Контакты

* 📧Email: elevenfourprod@yandex.ru
* 🐙GitHub: @11qfour