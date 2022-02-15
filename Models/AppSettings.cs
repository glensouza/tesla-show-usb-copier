namespace TeslaLightShow.Models;

public partial class AppSettings
{
    [JsonProperty("SourceFolder", NullValueHandling = NullValueHandling.Ignore)]
    public string? SourceFolder { get; set; }

    [JsonProperty("DestinationDrive", NullValueHandling = NullValueHandling.Ignore)]
    public string? DestinationDrive { get; set; }

    [JsonProperty("FormatDrive", NullValueHandling = NullValueHandling.Ignore)]
    [JsonConverter(typeof(AppSettingsParseStringConverter))]
    public bool? FormatDrive { get; set; }

    [JsonProperty("ConvertWav", NullValueHandling = NullValueHandling.Ignore)]
    [JsonConverter(typeof(AppSettingsParseStringConverter))]
    public bool? ConvertWav { get; set; }
}

public partial class AppSettings
{
    public static AppSettings? FromJson(string json) => JsonConvert.DeserializeObject<AppSettings>(json, AppSettingsConverter.Settings);
}

public static class SerializeAppSettings
{
    public static string ToJson(this AppSettings self) => JsonConvert.SerializeObject(self, AppSettingsConverter.Settings);
}

internal static class AppSettingsConverter
{
    public static readonly JsonSerializerSettings? Settings = new()
    {
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        DateParseHandling = DateParseHandling.None,
        Converters = { new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal } },
        Formatting = Formatting.Indented
    };
}

internal class AppSettingsParseStringConverter : JsonConverter
{
    public override bool CanConvert(Type t) => t == typeof(bool) || t == typeof(bool?);

    public override object? ReadJson(JsonReader reader, Type t, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return null;
        }

        string? value = serializer.Deserialize<string>(reader);
        if (bool.TryParse(value, out bool b))
        {
            return b;
        }

        throw new Exception("Cannot unmarshal type bool");
    }

    public override void WriteJson(JsonWriter writer, object? untypedValue, JsonSerializer serializer)
    {
        if (untypedValue == null)
        {
            serializer.Serialize(writer, null);
            return;
        }

        bool value = (bool)untypedValue;
        string boolString = value ? "true" : "false";
        serializer.Serialize(writer, boolString);
        return;
    }

    public static readonly AppSettingsParseStringConverter Singleton = new();
}
