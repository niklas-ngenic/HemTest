using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ArtistInfoRepository
{
    public class CoverArtArchiveClient : IAlbumCoverArtClient
    {
        private readonly string _uriFormat = @"http://coverartarchive.org/release-group/{0}";
        private readonly HttpClient _httpClient;

        public CoverArtArchiveClient()
        {
            _httpClient = new HttpClient(); 
        }

        public async Task<string> GetAsync(Guid albumGuid)
        {
            var uri = string.Format(_uriFormat, albumGuid);
            var response = await _httpClient.GetAsync(uri);
            string url = "";
            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jsonStr = await response.Content.ReadAsStringAsync();
                var content = JsonConvert.DeserializeObject<CovertArtArchiveResponse>(jsonStr);
                url =  content.Images.FirstOrDefault(x => x.Front).Uri;
            }
            return url;
        }

        internal class CovertArtArchiveResponse
        {
            public ICollection<CoverImage> Images { get; set; }
        }

        internal class CoverImage
        {
            public bool Front { get; set; }

            [JsonProperty("image")]
            public string Uri { get; set; }
        }
    }
}
