using System.Text.Json.Serialization;

namespace Domain.Entities.Concretes
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Role
    {
        Admin,
        Basic
    }
}