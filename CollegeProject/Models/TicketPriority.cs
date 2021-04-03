using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CollegeProject.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TicketPriority
    {
        Low = 1,
        Normal = 2,
        High = 3
    }
}
