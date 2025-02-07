local function execute_application(port)
    -- Убедимся, что порт передан
    if not port then
        print("Ошибка: Укажите порт.")
        print("Пример: lua run.lua 5001")
        return
    end

    -- Уникальный идентификатор запуска
    local identifier = "lua_script"

    -- Путь к вашему приложению
    local application_path = "C:\\projects\\protei\\dynamicgate\\api\\servers-api\\bin\\Debug\\net8.0\\servers-api.dll"
		
    -- Формируем команду для выполнения
    local command = string.format('dotnet "%s" --port=%s', application_path, port)

    -- Запускаем приложение
    local result = os.execute(command)
    if result == 0 then
        print("Приложение успешно запущено.")
    else
        print("Не удалось запустить приложение. Код возврата: " .. tostring(result))
    end
end -- Здесь завершается функция execute_application

-- Получаем порт из командной строки
local port = arg[1]

-- Выполняем запуск приложения
execute_application(port)
