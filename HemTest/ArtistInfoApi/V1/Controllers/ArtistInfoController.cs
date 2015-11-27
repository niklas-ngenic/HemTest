using ArtistInfoApi.V1.Models;
using ArtistInfoRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace ArtistInfoApi.V1.Controllers
{
    [RoutePrefix("api/v1/artistinfo")]
    public class ArtistInfoController : ApiController
    {
        private IArtistInfoAggregator _artistInfoProvider;

        public ArtistInfoController(IArtistInfoAggregator artistInfoProvider)
        {
            _artistInfoProvider = artistInfoProvider;
        }

        [Route("{MBID:guid}")]
        public async Task<IHttpActionResult> Get(Guid MBID)
        {
            try
            {
                var artistInfo = await _artistInfoProvider.GetArtistInfoAsync(MBID);
                var model = new ArtistInfoModel()
                {
                    Id = MBID,
                    Description = artistInfo.Description,
                    Albums = artistInfo.ReleaseGroups.Where(x => x.PrimaryType == "Album").Select(x => new Album()
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Image = x.CovertArtUrl
                    }).ToList()
                };

                return Ok(model);
            }
            catch (ArtistInfoNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
    }
}