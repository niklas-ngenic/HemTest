using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ArtistInfoRepository
{
    public class WikipediaArtistInfoClient : IArtistDescriptionClient
    {
        private const string WIKIPEDIA_URI_FORMAT = @"https://en.wikipedia.org/w/api.php?action=query&format=json&prop=extracts&exintro=true&redirects=true&titles={0}";
        private readonly HttpClient _httpClient;

        public WikipediaArtistInfoClient()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> GetAsync(string title)
        {
            string description = null;
            var uri = string.Format(WIKIPEDIA_URI_FORMAT, title);

            var result = await _httpClient.GetAsync(uri);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                var stringContent = await result.Content.ReadAsStringAsync();

                try
                {
                    description = (string)JObject.Parse(stringContent)["query"]["pages"].First.First["extract"];
                }
                catch (Exception)
                {
                    ; // Swallow exception
                }
            }

            return description;
        }
    }
}