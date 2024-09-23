# Мини-система управления заказами

Эта мини-система представляет собой упрощенную микросервисную архитектуру для управления заказами, использующую .NET8 и RabbitMQ.

## Структура проекта

Проект состоит из следующих микросервисов:

- **Order Service**: Управляет созданием и получением заказов.
- **Notification Service**: Отправляет уведомления о новых заказах.
- **RabbitMQ**: Шина сообщений для обработки событий.

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

### 4. Проверка работы API
После успешного запуска микросервисов вы можете протестировать API. Например, для получения всех заказов используйте следующий запрос:
```bash
curl -X GET http://localhost:8080/api/get-orders -H "x-api-key: your_api_key_here"
```
В окружении `Developement` доступен `swagger`:
```bash
curl -X GET http://localhost:8080/swagger -H "x-api-key: your_api_key_here"
```
