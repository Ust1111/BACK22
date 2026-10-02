# ArticulationAPI + ArticulationAPI.TranscriptionService

- **ArticulationAPI.TranscriptionService** — gRPC-сервер, выполняет фонетическую
  транскрипцию слов (русский язык — по словарю и правилам редукции; английский,
  немецкий, французский — через Python-библиотеку `g2p-en`).
- **ArticulationAPI** — HTTP API с авторизацией (JWT), базой данных PostgreSQL
  и разделом "избранное". При обработке запроса на транскрипцию обращается
  к TranscriptionService по gRPC.
- **ArticulationAPI.StatsService** — публикация уведомлений о добавлении в избранное, подсчёт языков.

## Требования к окружению

- .NET 9 SDK
- PostgreSQL
- Python 3.13
- брокер NATS JetStream

## Шаг 1. База данных

Создайте пустую базу PostgreSQL (само приложение создаст в ней таблицы
автоматически при первом запуске — `db.Database.Migrate()` в `Program.cs`):

```bash
psql -U postgres -c "CREATE DATABASE articulation;"
```

## Шаг 2. Строка подключения и настройки

Откройте `ArticulationAPI/appsettings.json` и поправьте
`ConnectionStrings:Default` под вашу локальную PostgreSQL (логин, пароль, хост):

`Transcription:GrpcUrl` должен указывать на адрес, на котором будет слушать
TranscriptionService — по умолчанию `http://localhost:5099`

## Шаг 3. Python-окружение для TranscriptionService

Нужно только для проверки английского (`en`), немецкого (`de`) и французского
(`fr`) языков (немецкий и французский не реализованы). Для базового сценария по методичке (русский язык, обработка
ошибок, таймаут, недоступность сервиса) Python не требуется — соответствующий
код в `RussianTranscriptionService.cs` работает на встроенных правилах без
внешних зависимостей.

Если хотите проверить и латинские языки:

```bash
cd ArticulationAPI.TranscriptionService
python3 -m venv .venv
./.venv/bin/pip install -r requirements.txt
```

`requirements.txt` лежит в архиве отдельным файлом — положите его в корень
`ArticulationAPI.TranscriptionService/` перед выполнением команды выше.

Если Python не установлен или `g2p-en` не устанавливается — русский сценарий
всё равно будет работать полностью, просто `language: "en"` в этом случае
завершится ошибкой на этапе выполнения, это не является дефектом основного
сценария лабы.

Запустить терминалы cd /Users/ustinabeseleva/Desktop/ArticulationAPI.TranscriptionService и cd ArticulationAPI

## Шаг 4. Запуск брокера и приложения

В терминале для брокера и оставить слушать

```bash
./nats-server -js
```

Запускаем ArticulationAPI.StatsService
