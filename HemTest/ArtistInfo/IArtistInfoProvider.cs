using System;
using System.Threading.Tasks;

namespace ArtistInfoRepository
{
    public interface IArtistInfoProvider
    {
        Task<ArtistInfo> GetArtistInfoAsync(Guid MBID);
    }
}