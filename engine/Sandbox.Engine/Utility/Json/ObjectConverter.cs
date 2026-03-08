using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sandbox;

internal sealed class ObjectConverter : JsonConverter<object>
{
	public override object Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
	{
		if ( reader.TokenType == JsonTokenType.Null )
		{
			return null;
		}

		if ( reader.TokenType == JsonTokenType.String )
		{
			return reader.GetString();
		}

		var json = Json.ParseToJsonObject( ref reader );

		if ( !json.TryGetPropertyValue( "t", out var t ) )
		{
			return null;
		}
		if ( !json.TryGetPropertyValue( "v", out var v ) )
		{
			return null;
		}

		var typeDesc = Game.TypeLibrary.GetType( t.GetValue<string>(), true );
		return v.Deserialize( typeDesc.TargetType, options );
	}

	public override void Write( Utf8JsonWriter writer, object value, JsonSerializerOptions options )
	{
		if ( value == null )
		{
			writer.WriteNullValue();
			return;
		}

		if ( value is string str )
		{
			writer.WriteStringValue( str );
			return;
		}

		var typeToConvert = value.GetType();

		writer.WriteStartObject();
		{
			writer.WriteString( "t", typeToConvert.FullName );
		}
		{
			writer.WritePropertyName( "v" );
			Json.Serialize( writer, value, typeToConvert );
		}
		writer.WriteEndObject();
	}
}
