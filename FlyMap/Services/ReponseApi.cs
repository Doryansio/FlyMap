using FlyMap.Modele;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlyMap.Services
{
    public class ReponseApi
    {
        [JsonPropertyName("data")]
        public List<AeroportApi> AeroportApis { get; set; }
    }
}
