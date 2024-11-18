using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MagicOrbwalker1.Essentials.API
{
    class API
    {
        private readonly HttpClient httpClient;
        private readonly string baseUrl = "https://127.0.0.1:2999/liveclientdata/";

        public API()
        {
            var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
            };

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            httpClient = new HttpClient(httpClientHandler);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "API");
        }

        public async Task<JObject?> GetActivePlayerDataAsync()
        {
            try
            {
                var response = await httpClient.GetStringAsync(baseUrl + "activeplayer");
                return JObject.Parse(response);
            }
            catch (Exception e) when (e is HttpRequestException || e is JsonReaderException)
            {
                Console.WriteLine($"Error: {e.Message}");
                return null;
            }
        }

        public async Task<float> GetAttackSpeedAsync()
        {
            var data = await GetActivePlayerDataAsync();

            if (data == null)
            {
                Console.WriteLine("Failed to retrieve the value of attackSpeed.");
                return -1;
            }

            float? attackSpeed = data["championStats"]?["attackSpeed"]?.Value<float>();

            if (attackSpeed == null)
            {
                Console.WriteLine("Failed to retrieve the value of attackSpeed.");
                return -1;
            }

            return attackSpeed.Value;
        }

        public async Task<float> GetAttackRangeAsync()
        {
            var data = await GetActivePlayerDataAsync();

            if (data == null)
            {
                Console.WriteLine("Failed to retrieve the value of attackRange.");
                return -1;
            }

            float? attackRange = data["championStats"]?["attackRange"]?.Value<float>();

            if (attackRange == null)
            {
                Console.WriteLine("Failed to retrieve the value of attackRange.");
                return -1;
            }

            return attackRange.Value;
        }

        public async Task<string> GetChampionNameAsync()
        {
            try
            {
                var response = await httpClient.GetStringAsync(baseUrl + "playerlist");
                var playerList = JArray.Parse(response);

                if (playerList.Count > 0)
                {
                    var championName = playerList[0]?["championName"]?.ToString();
                    if (championName == null)
                    {
                        Console.WriteLine("Unknown Champion");
                        return "Unknown Champion";
                    }
                    return championName;
                }
                else
                {
                    Console.WriteLine("No player data found.");
                    return "No Player Data";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return "Error occurred";
            }
        }
    }
}
