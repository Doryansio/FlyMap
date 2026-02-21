using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Transactions;

namespace FlyMap.Modele
{
    public class AeroportApi
    {
        [JsonPropertyName("id")]
        public string Id {  get; set; }

        [JsonPropertyName("airport_name")]
        public String Name { get; set; }

        [JsonPropertyName("iata_code")]
        public String Code { get; set; }

        [JsonPropertyName("latitude")]
        public Double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public Double Longitude { get; set; }

        
    }
}
