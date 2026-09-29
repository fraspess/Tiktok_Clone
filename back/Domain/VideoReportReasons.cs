using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Domain;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum VideoReportReasons
{
    Spam,
    InappropriateContent,
    HateSpeech, 
    Violence,
    Misinformation,
    Other
}