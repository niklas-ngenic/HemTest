using System;
using System.Threading.Tasks;

namespace ArtistInfoRepository
{
    public interface IAlbumCoverArtClient
    {
        Task<string> GetAsync(Guid albumGuid);
    }
}