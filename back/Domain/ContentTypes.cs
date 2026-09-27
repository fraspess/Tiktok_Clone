using System.Text.Json.Serialization;

namespace Domain;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ContentTypes
{
    FallBack = 0,
    Video = 1,
    Comment = 2,
    User = 3
}