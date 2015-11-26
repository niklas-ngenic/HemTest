using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace ArtistInfoRepository
{
    public class MusicBrainzDataAggregator : IArtistInfoAggregator
    {
        private const string WIKIPEDIA_TYPE_IDENTIFIER = "wikipedia";
        private readonly IAlbumCoverArtClient _albumCoverArtClient;
        private readonly IArtistDescriptionClient _artistDescriptionClient;
        private readonly IArtistInfoClient _artistInfoClient;

        public MusicBrainzDataAggregator(IArtistInfoClient artistInfoClient,
            IArtistDescriptionClient artistDescriptionClient,
            IAlbumCoverArtClient albumCoverArtClient)
        {
            _artistInfoClient = artistInfoClient;
            _artistDescriptionClient = artistDescriptionClient;
            _albumCoverArtClient = albumCoverArtClient;
        }

        public async Task<ArtistInfo> GetArtistInfoAsync(Guid MBID)
        {
            var artistInfo = await _artistInfoClient.GetAsync(MBID);

            if (artistInfo == null)
            {
                throw new ArtistInfoNotFoundException();
            }

            Task descriptionTask = SetDescriptionAsync(artistInfo);
            IEnumerable<Task> albumArtTasks = artistInfo.ReleaseGroups.Select(x => SetAlbumArtUrlAsync(x));

            Task[] asyncTasks = new Task[] { descriptionTask }.Concat(albumArtTasks).ToArray();
            await Task.WhenAll(asyncTasks);

            return artistInfo;
        }

        private async Task SetDescriptionAsync(ArtistInfo artistInfo)
        {
            var wikipediaRelation = artistInfo.Relations.FirstOrDefault(x => x.Type == WIKIPEDIA_TYPE_IDENTIFIER);
            if (wikipediaRelation != null)
            {
                var wikipediaIdentifier = GetWikipediaIdentifier(wikipediaRelation.Url.Resource);
                var description = await _artistDescriptionClient.GetAsync(wikipediaIdentifier);
                if (description != null)
                {
                    artistInfo.Description = description;
                }
            }
            return;
        }

        private string GetWikipediaIdentifier(string wikipediaResource)
        {
            return wikipediaResource.Split('/').Last();
        }

        private async Task SetAlbumArtUrlAsync(ArtistInfoReleaseGroup releaseGroup)
        {
            var albumArtUrl = await _albumCoverArtClient.GetAsync(releaseGroup.Id);
            releaseGroup.CovertArtUrl = albumArtUrl;
            return;
        }
    }
}