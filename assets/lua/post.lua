local http = require("socket.http")
local ltn12 = require("ltn12")
local json = require("dkjson") -- JSON библиотека для Lua

-- Читаем JSON-данные из файла
local function readJsonFile(filename)
    local file = io.open(filename, "r")
    if not file then
        error("Не удалось открыть файл: " .. filename)
    end
    local content = file:read("*a")
    file:close()
    return json.decode(content)
end

-- Загрузка данных из файла
local jsonData = readJsonFile("data.json")

-- Преобразуем таблицу обратно в JSON-строку
local jsonString = json.encode(jsonData)

-- Отправляем POST-запрос
local response = {}
local _, code = http.request{
    url = "http://localhost:5106/api/servers/upload",
    method = "POST",
    headers = {
        ["Content-Type"] = "application/json",
        ["Content-Length"] = tostring(#jsonString)
    },
    source = ltn12.source.string(jsonString),
    sink = ltn12.sink.table(response)
}

-- Проверяем результат
if code == 200 then
    print("JSON успешно отправлен!")
else
    print("Ошибка отправки! Код ответа: " .. tostring(code))
end
