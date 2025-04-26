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
```bash
Canteen/
├── ApiDomain/                  # Основные модели и репозитории
│   ├── Configurations/         # Конфигурации (связи между сущностями БД)
│   ├── Enums/                  # Перечисления (для отображения статусов заказа и корзины)
│   ├── Models/                 # Классы моделей
│   ├── ApiListContext.cs/      # DB Context
├── Api/                        # Веб-приложение 
│   ├── Controllers/            # Контроллеры API
│   ├── DTO/                   # Data Transfer Objects
│   ├── Services/               # Сервисы работы с авторизацией
│   ├── Repositories/           # Репозитории (CRUD-операции)
│   ├── Program.cs/             # Сборщик всего проекта
├── media/                      # Скриншоты API и диаграммы
│   ├── ERDiagrams.png          # ER-диаграмма базы данных
│   ├── swaggerFirst.png        # Swagger UI
│   ├── swaggerSecond.png       # Swagger UI
└── README.md                   # Документация проекта
```
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

https://www.figma.com/design/SMQj2yHzQJvgIGv9vpnJLL/canteen-maket?node-id=0-1&p=f&t=W4vIYsz2OeOkd4hF-0

# Реализованный интерфейс


[в разработке]

---

## 🔬 Тестирование API при помощи **Postman**

1. Post-запросы

   ![📌 Успешное выполнение POST-запроса контроллера Order](https://github.com/11qfour/Canteen/raw/main/media/SuccessPostOrder.png)

   ![📌 Успешное выполнение POST-запроса контроллера Cart](https://github.com/11qfour/Canteen/raw/main/media/SuccessPostCart.png)

   ![📌 Успешное выполнение POST-запроса контроллера Dish](https://github.com/11qfour/Canteen/raw/main/media/SuccessPostDish.png)

   ![📌 Ошибочное выполнение POST-запроса контроллера Cart, нарушение связи Один к Одному](https://github.com/11qfour/Canteen/raw/main/media/ExampleErrorPostCartOneToOne.png)

   ![📌 Ошибочное выполнение POST-запроса контроллера Dish, нарушение условий полей сущностей](https://github.com/11qfour/Canteen/raw/main/media/Example400ErrorPostDish.png)


    ```bash
    https://localhost:7168/api/<название сущности>

2. Get-запросы


   ![📌 Успешное выполнение GET-запроса контроллера Category](https://github.com/11qfour/Canteen/raw/main/media/SuccessGetCategory.png)

   ![📌 Успешное выполнение GET-запроса контроллера Dish](https://github.com/11qfour/Canteen/raw/main/media/SuccessGetDish.png)

   ![📌 Успешное выполнение GET-запроса контроллера Order](https://github.com/11qfour/Canteen/raw/main/media/SuccessGetOrder.png)


    ```bash
    https://localhost:7168/api/<название сущности> //вывод всей информации
    https://localhost:7168/api/<название сущности>/{id} //вывод информации по ID
        
3. PUT-запросы

    ![📌 Успешное выполнение PUT-запроса контроллера Customer](https://github.com/11qfour/Canteen/raw/main/media/SuccessPutCustomer.png)

    ![📌 Успешное выполнение PUT-запроса контроллера Order](https://github.com/11qfour/Canteen/raw/main/media/SuccessPutOrder.png)

    ![📌 Успешное выполнение PUT-запроса контроллера Cart](https://github.com/11qfour/Canteen/raw/main/media/SuccessPutCart.png)

    ![📌 Успешное выполнение PUT-запроса контроллера Dish](https://github.com/11qfour/Canteen/raw/main/media/SuccessPutDish.png)
    
   ```bash
    https://localhost:7168/api/<название сущности>/{id}

4. PUT-запросы изменений статусов Cart и Order

   ![📌 Успешное выполнение PUT-запроса статуса Cart](https://github.com/11qfour/Canteen/raw/main/media/SuccessPutCartStatus.png)

   ![📌 Успешное выполнение PUT-запроса статуса Order](https://github.com/11qfour/Canteen/raw/main/media/SuccessPutOrderStatus.png)

   ![📌 Ошибочное выполнение PUT-запроса статуса Order, неверный переход между статусами, перескочили через другой этап](https://github.com/11qfour/Canteen/raw/main/media/Example400ErrorPutOrderStatus.png)


   ```bash
   https://localhost:7168/api/order/{id}/status
   https://localhost:7168/api/cart/{id}/status
   ```

5. Delete-запросы

   ![📌 Успешное выполнение DELETE-запроса по id контроллера Cart](https://github.com/11qfour/Canteen/raw/main/media/DeleteSuccessPostman.png)
   
   ![📌 Успешное выполнение DELETE-запроса по id контроллера Order](https://github.com/11qfour/Canteen/raw/main/media/SuccessDeleteOrder.png)
   
    ```bash
    https://localhost:7168/api/<название сущности>/{id}

---

## 🛠 Тестирование авторизации и аунтефикации через JWT токен при помощи **Postman**
   📌 Успешное выполнение выполнение регистрации

   ![📌 Успешное выполнение выполнение регистрации](https://github.com/11qfour/Canteen/raw/main/media/SuccessPostRegister.png)

   📌 Успешное выполнение входа

   ![📌 Успешное выполнение входа](https://github.com/11qfour/Canteen/raw/main/media/SuccessLogin.png)

   📌 Успешное обновление токена

   ![📌 Успешное обновление токена](https://github.com/11qfour/Canteen/raw/main/media/SuccessRefresh.png)

   📌 Успешный Get-запрос с аунтефикацией

   ![📌 Успешный Get-запрос с аунтефикацией](https://github.com/11qfour/Canteen/raw/main/media/SuccessGetCustomerWithAuth.png)

   📌 Успешное добавление пользователя после регистрации в БД Customer

   ![📌 Успешное добавление пользователя после регистрации в БД Customer](https://github.com/11qfour/Canteen/raw/main/media/SuccessAddAuthUsersInCustomers.png)

   📌 Ошибка 401, неверный токен

   ![📌 Ошибка 401, неверный токен](https://github.com/11qfour/Canteen/raw/main/media/Example401AuthentcationFailed.png)

   📌 Ошибка 401, неверный токен

   ![📌 Ошибка 401, неверный токен](https://github.com/11qfour/Canteen/raw/main/media/Example401AuthentcationFailed.png)

   📌 Ошибка 401 при обновлении токена, невозможно обновить старый или невалидный

   ![📌 Ошибка 401 при обновлении токена, невозможно обновить старый или невалидный](https://github.com/11qfour/Canteen/raw/main/media/ExampleErrorOldToken.png)

   📌 📌 Ошибка 403, обычный пользователь не может добавить новое блюдо

   ![📌 Ошибка 403, обычный пользователь не может добавить новое блюдо](https://github.com/11qfour/Canteen/raw/main/media/Example403ErrorUserDon'tAddDish.png)

---
## 📦 Структура DTO
В проекте используются **Data Transfer Objects** (DTO):
------------------------------------------------------
 Тип DTO            ->         Назначение

<name>CreateDto     ->        Создание (Request)

<name>UpdateDto     ->        Обновление (Request)

<name>Dto           ->      Вывод (Response)

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

📧Email: elevenfourprod@yandex.ru

🐙GitHub: @11qfour
