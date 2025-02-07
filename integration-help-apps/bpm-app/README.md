# Backend

В репе содержится BPM движок и карточная подсистема

| Name   | Status |
|--------|--------|
| Deploy | [![pipeline status](https://git.pit.protei.ru/safe-city/backend/badges/main/pipeline.svg)](https://git.pit.protei.ru/safe-city/backend/-/commits/main) |
| Code analysis | [![Quality Gate Status](https://sonar.pit.protei.ru/api/project_badges/measure?project=Safe-City&metric=alert_status&token=sqb_1b3d1df0574d190a89a093ec2f1d02fe8f62822a)](https://sonar.pit.protei.ru/dashboard?id=Safe-City)|

## Публикация
На данный момент по окончанию pipeline, образы докер отправляются в [Hub](https://reg.pit.protei.ru/). Из него данный образ можно скачать в любое место.

|  Образ   |          Сервис      |                 Адрес                       | Порт |
|----------|----------------------|---------------------------------------------|------|
|cardsystem| Каротчная подсистема | [Ссылка](https://card.stand.pit.protei.ru/swagger) | 5001 |
|bpm       | BPM движок           | [Ссылка](https://bpm.stand.pit.protei.ru/swagger) | 5002 |

## Развёртывание на сервере
После провождения pipeline контейнеры автоматически обновляются и перезапускаются. Руками ничего делать не надо.



