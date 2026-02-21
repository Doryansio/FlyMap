using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Cache;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FlyMap.Services
{
    public static class FournisseurAeroport
    {
        public static async Task<ReponseApi> GetAeroportsAsync(HttpClient httpClient)
        {
            string key = await File.ReadAllTextAsync("api_key.txt"); ;
            string connexion = $"http://api.aviationstack.com/v1/airports?access_key={key}";
            var response = await httpClient.GetFromJsonAsync<ReponseApi>(connexion);
            return response; 
                
            
        }
    }
}
