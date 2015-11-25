using System.Threading.Tasks;

namespace ArtistInfoRepository
{
    public interface IArtistDescriptionClient
    {
        Task<string> GetAsync(string uri);
    }
}