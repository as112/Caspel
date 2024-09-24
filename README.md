# Мини-система управления заказами

Эта мини-система представляет собой упрощенную микросервисную архитектуру для управления заказами, использующую .NET8 и RabbitMQ.

## Структура проекта

Проект состоит из следующих микросервисов:

- **Order Service**: Управляет созданием и получением заказов. Отправляет уведомления о созданных заказах в очередь. Для хранения данных используется In-Memory DataBase EFCore.
- **Notification Service**: Принимает сообщения из очереди. Отправляет уведомления о новых заказах по email (имитация в консоль).
- **RabbitMQ**: Шина сообщений для обработки событий. Взаимодействие организовано с помощью библиотеки MassTransit. Настроен один обменник типа `fanout`, к нему привязвна одна очередь.

- Для контейнеризации используется Docker.
- Для запуска используется `docker compose`

## Установка и запуск

### 1. Клонируйте репозиторий

```bash
git clone https://github.com/as112/Caspel.git
cd Caspel
```
### 2. Настройка конфигурации

- Откройте файл `docker-compose.yml`
- Добавьте параметры конфигурации RabbitMq через переменные окружения `RabbitMqOptions` для контейнеров `orderservice` и `notificationservice`
- Добавьте ключ для доступа к API через переменную `ApiKey`

 ### 3. Запуск с помощью Docker Compose
 
Запустите следующую команду для сборки и запуска контейнеров:

```bash
docker-compose up --build
```
Это создаст и запустит все необходимые контейнеры: `orderservice`, `notificationservice` и `rabbitmq`.

# Order Management API
Base URL:
`http://localhost:8080/api`

Authentication
API использует API Key для аутентификации. Ключ необходимо передавать в заголовке x-api-key для всех запросов.

Пример заголовка:
```
x-api-key: your_api_key
```
## Endpoints
### 1. Создание заказа:
- URL: `/create-order`
- Метод: `POST`
- Описание: Создает новый заказ и публикует событие в RabbitMQ.
- Тело запроса:
```
{
  "productName": "Pasta",
  "quantity": 1
}
```
- Пример запроса:
```
curl -X POST http://localhost:8080/create-order \
  -H "x-api-key: your_api_key" \
  -H "Content-Type: application/json" \
  -d '{"productName": "Pasta", "quantity": 10}'
```

Ответы:
- `200 OK`: Возвращает идентификатор созданного заказа.
- `500 Internal server error`: Не удалось сохранить заказ.

Пример успешного ответа:
```
{
  "id": "e7d2b1c3-dcbc-44c7-91fc-2d66c9a72b2e"
}
```

### 2. Получить заказ по ID
- URL: `/get-order/{id}`
- Метод: `GET`
- Описание: Возвращает данные о заказе по его идентификатору.
- Пример запроса:

```
curl -X GET http://localhost:8080/get-order/e7d2b1c3-dcbc-44c7-91fc-2d66c9a72b2e \
  -H "x-api-key: your_api_key"
```
Ответы:
- `200 OK`: Возвращает данные заказа.
```
{
  "id": "e7d2b1c3-dcbc-44c7-91fc-2d66c9a72b2e",
  "productName": "Widget",
  "quantity": 10,
  "createdAt": "2024-09-23T18:25:43.511Z"
}
```
- `404 Not Found`: Заказ не найден.

### 3. Получить все заказы
- URL: `/get-orders`
- Метод: `GET`
- Описание: Возвращает список всех заказов.
- Пример запроса:
```
curl -X GET http://localhost:8080/get-orders \
  -H "x-api-key: your_api_key"
```
- Ответы:
- `200 OK`: Возвращает массив заказов.
```
[
  {
    "id": "e7d2b1c3-dcbc-44c7-91fc-2d66c9a72b2e",
    "productName": "Pasta",
    "quantity": 2,
    "createdAt": "2024-09-23T18:25:43.511Z"
  }
]
```
- `404 Not Found`: Заказы не найдены.

## Ошибки
- `401 Unauthorized`: Отсутствует или неверный API Key.
- `404 Not Found`: Ресурс не найден.
- `500 Internal Server Error`: Ошибка на стороне сервера.
