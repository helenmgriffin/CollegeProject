using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace CollegeProject.Models
{
    public partial class Ticket
    {
        [Key]
        [JsonProperty("TicketGuid")]
        public Guid TicketGuid { get; set; }

        [Display(Name = "#")]
        [JsonProperty("TicketNumber")]
        public int TicketNumber { get; set; }

        [Required(ErrorMessage = "A summary is required.")]
        [StringLength(50)]
        [Display(Name = "Summary")]
        [JsonProperty("Summary")]
        public string Summary { get; set; }

        [Required(ErrorMessage = "A description is required.")]
        [StringLength(1000)]
        [Display(Name = "Description")]
        [JsonProperty("Description")]
        public string Description { get; set; }

        [Display(Name = "Created")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        [JsonProperty("CreationDate")]
        public DateTime CreationDate { get; set; }

        [Required]
        [StringLength(25)]
        [JsonProperty("Creator")]
        public string Creator { get; set; }

        [Display(Name = "Closed")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        [JsonProperty("ClosedDate")]
        public Nullable<DateTime> ClosedDate { get; set; }

        [Display(Name = "Priority")]
        [Required(ErrorMessage = "A priority status is required.")]
        [JsonProperty("TicketPriority")]
        [JsonConverter(typeof(StringEnumConverter))]
        public TicketPriority TicketPriority { get; set; }

        [Display(Name = "Closing Comments")]
        [StringLength(1000)]
        [JsonProperty("ClosingComments")]
        public string ClosingComments { get; set; }
    }
    /// <summary>
    /// Custom DateTime JSON serializer/deserializer
    /// </summary>
    public class CustomDateTimeConverter : DateTimeConverterBase
    {
        /// <summary>
        /// DateTime format
        /// </summary>
        private const string Format = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// Writes value to JSON
        /// </summary>
        /// <param name="writer">JSON writer</param>
        /// <param name="value">Value to be written</param>
        /// <param name="serializer">JSON serializer</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((DateTime)value).ToString(Format));
        }

        /// <summary>
        /// Reads value from JSON
        /// </summary>
        /// <param name="reader">JSON reader</param>
        /// <param name="objectType">Target type</param>
        /// <param name="existingValue">Existing value</param>
        /// <param name="serializer">JSON serialized</param>
        /// <returns>Deserialized DateTime</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
            {
                return null;
            }

            var s = reader.Value.ToString();
            DateTime result;
            if (DateTime.TryParseExact(s, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                return result;
            }

            return null;
        }
    }
}
