using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ArtistInfoRepository
{
    public class MusicBrainzClient : IArtistInfoClient
    {
        private readonly string _uriFormat = @"http://musicbrainz.org/ws/2/artist/{0}?&fmt=json&inc=url-rels+release-groups";
        private readonly HttpClient _httpClient;

        public MusicBrainzClient()
        {
            _httpClient = new HttpClient();
        }

        public async Task<ArtistInfo> GetAsync(Guid MBID)
        {
            var uri = string.Format(_uriFormat, MBID);
            var response = await _httpClient.GetAsync(uri);
            var jsonContent = await response.Content.ReadAsStringAsync();
            var artistInfo = JsonConvert.DeserializeObject<ArtistInfo>(jsonContent);
            return artistInfo;
        }
    }
}
