﻿using System.Text.Json;

namespace servers_api.messaginghandlers.formatting
{
	public interface IMessageFormatter
	{
		string DecodeUnicodeEscape(string input);
		string FormatJson(string json);
		void WriteFormattedJson(JsonElement element, Utf8JsonWriter writer);
	}
}
