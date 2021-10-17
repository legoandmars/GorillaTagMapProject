using System;
using UnityEngine;
using Newtonsoft.Json;

public class ColorConverter : JsonConverter
{
	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof(Color);
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		Color color = (Color)value;
		writer.WriteStartObject();

		writer.WritePropertyName("r");
		serializer.Serialize(writer, color.r);

		writer.WritePropertyName("g");
		serializer.Serialize(writer, color.g);

		writer.WritePropertyName("b");
		serializer.Serialize(writer, color.b);

		writer.WritePropertyName("a");
		serializer.Serialize(writer, color.a);

		writer.WriteEndObject();
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		throw new NotImplementedException();
	}
}
