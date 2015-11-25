using System;
using System.Threading.Tasks;

namespace ArtistInfoRepository
{
    public interface IArtistInfoClient
    {
        Task<ArtistInfo> GetAsync(Guid MBID);
    }
}