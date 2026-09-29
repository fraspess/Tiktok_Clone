using System.Text.Json.Serialization;

namespace Domain;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ContentTypes
{
    Video,
    Comment,
    User
}