using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace ArtistInfoRepository
{
    public class ArtistInfoProvider : IArtistInfoProvider
    {
        private readonly IAlbumCoverArtClient _albumCoverArtClient;
        private readonly IArtistDescriptionClient _artistDescriptionClient;
        private readonly IArtistInfoClient _artistInfoClient;

        public ArtistInfoProvider(IArtistInfoClient artistInfoClient,
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

            Task descriptionTask = SetDescriptionAsync(artistInfo);
            IEnumerable<Task> albumArtTasks = artistInfo.ReleaseGroups.Select(x => SetAlbumArtUrlAsync(x));

            Task[] asyncTasks = new Task[] { descriptionTask }.Concat(albumArtTasks).ToArray();
            await Task.WhenAll(asyncTasks);

            return artistInfo;
        }

        private async Task SetDescriptionAsync(ArtistInfo artistInfo)
        {
            //TODO: Must extract wikipedia name to be used in api call
            var descriptionUrl = artistInfo.Relations.FirstOrDefault(x => x.Type == "wikipedia").Url.Resource;
            var description = await _artistDescriptionClient.GetAsync(descriptionUrl);
            artistInfo.Description = description;
            return;
        }

        private async Task SetAlbumArtUrlAsync(ArtistInfoReleaseGroup releaseGroup)
        {
            var albumArtUrl = await _albumCoverArtClient.GetAsync(releaseGroup.Id);
            releaseGroup.CovertArtUrl = albumArtUrl;
            return;
        }
    }
}