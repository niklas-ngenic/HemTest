using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ArtistInfoRepository
{
    public class MusicBrainzClient : IArtistInfoClient
    {
        private const string MUSICBRAINZ_URI_FORMAT = @"http://musicbrainz.org/ws/2/artist/{0}?&fmt=json&inc=url-rels+release-groups";
        private readonly HttpClient _httpClient;

        public MusicBrainzClient()
        {
            _httpClient = new HttpClient();
        }

        public async Task<ArtistInfo> GetAsync(Guid MBID)
        {
            ArtistInfo artistInfo = null;
            var uri = string.Format(MUSICBRAINZ_URI_FORMAT, MBID);

            var response = await _httpClient.GetAsync(uri);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                artistInfo = JsonConvert.DeserializeObject<ArtistInfo>(jsonContent);
            }

            return artistInfo;
        }
    }
}
