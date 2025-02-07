using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace servers_api.messaging.formatting
{
	public class MessageFormatter : IMessageFormatter
	{
		public string FormatJson(string json)
		{
			try
			{
				// Десериализуем JSON в объект
				using var doc = JsonDocument.Parse(json);
				var options = new JsonSerializerOptions
				{
					WriteIndented = true,
					Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
				};

				// Преобразуем объект в строку с отступами и декодированием unicode
				using var ms = new MemoryStream();
				using (var writer = new Utf8JsonWriter(ms, new JsonWriterOptions { Indented = true }))
				{
					WriteFormattedJson(doc.RootElement, writer);
				}

				// Преобразуем байты в строку
				var formattedJson = Encoding.UTF8.GetString(ms.ToArray());

				// Декодируем Unicode-escape в строках
				return DecodeUnicodeEscape(formattedJson);
			}
			catch
			{
				return json; // Если JSON невалидный, просто вернуть как есть
			}
		}

		public string DecodeUnicodeEscape(string input)
		{
			// Декодируем все Unicode escape символы
			return Regex.Replace(input, @"\\u([0-9a-fA-F]{4})", match =>
			{
				return char.ConvertFromUtf32(int.Parse(match.Groups[1].Value, System.Globalization.NumberStyles.HexNumber));
			});
		}

		public void WriteFormattedJson(JsonElement element, Utf8JsonWriter writer)
		{
			if (element.ValueKind == JsonValueKind.Object)
			{
				writer.WriteStartObject();
				foreach (var property in element.EnumerateObject())
				{
					writer.WritePropertyName(property.Name);
					if (property.Name.Equals("Timestamp", StringComparison.OrdinalIgnoreCase) &&
						property.Value.ValueKind == JsonValueKind.String &&
						DateTime.TryParse(property.Value.GetString(), out var timestamp))
					{
						writer.WriteStringValue(timestamp.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
					}
					else
					{
						WriteFormattedJson(property.Value, writer);
					}
				}
				writer.WriteEndObject();
			}
			else if (element.ValueKind == JsonValueKind.Array)
			{
				writer.WriteStartArray();
				foreach (var item in element.EnumerateArray())
				{
					WriteFormattedJson(item, writer);
				}
				writer.WriteEndArray();
			}
			else
			{
				element.WriteTo(writer);
			}
		}
	}
}
