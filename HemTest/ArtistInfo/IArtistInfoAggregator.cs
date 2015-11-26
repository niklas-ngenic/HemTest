using System;
using System.Threading.Tasks;

namespace ArtistInfoRepository
{
    public interface IArtistInfoAggregator
    {
        Task<ArtistInfo> GetArtistInfoAsync(Guid MBID);
    }
}