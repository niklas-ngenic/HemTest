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
        private const string COVERT_ART_ARCHIVE_URI_FORMAT = @"http://coverartarchive.org/release-group/{0}";
        private readonly HttpClient _httpClient;

        public CoverArtArchiveClient()
        {
            _httpClient = new HttpClient(); 
        }

        public async Task<string> GetAsync(Guid albumGuid)
        {
            string url = null;
            var uri = string.Format(COVERT_ART_ARCHIVE_URI_FORMAT, albumGuid);
            var response = await _httpClient.GetAsync(uri);

            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jsonStr = await response.Content.ReadAsStringAsync();
                var content = JsonConvert.DeserializeObject<CovertArtArchiveResponse>(jsonStr);
                url = content.Images.FirstOrDefault(x => x.Front).Uri;
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
