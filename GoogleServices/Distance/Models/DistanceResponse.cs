using Newtonsoft.Json;
using System.Collections.Generic;

namespace GoogleServices.Geolocation.Models
{
    public class DistanceResponse
    {
        [JsonProperty("rows")]
        public IEnumerable<RowsDto> Rows { get; set; }

        public class RowsDto
        {
            [JsonProperty("elements")]
            public IEnumerable<ElementsDto> Elements { get; set; }

            public class ElementsDto
            {
                [JsonProperty("distance")]
                public DistanceDto Distance { get; set; }

                public class DistanceDto
                {
                    [JsonProperty("text")]
                    public string Text { get; set; }

                    [JsonProperty("value")]
                    public int Value { get; set; }
                }

                [JsonProperty("duration")]
                public DurationDto Duration { get; set; }

                public class DurationDto
                {
                    [JsonProperty("text")]
                    public string Text { get; set; }

                    [JsonProperty("value")]
                    public int Value { get; set; }
                }
            }
        }
    }
}
