using ArtistInfoApi.V1.Controllers;
using ArtistInfoApi.V1.Models;
using ArtistInfoRepository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;

namespace ArtistInfoApi.UnitTests.V1
{
    [TestClass]
    public class ArtistInfoControllerTests
    {
        ArtistInfoController target;
        Mock<IArtistInfoAggregator> artistInfoProvider;

        [TestInitialize]
        public void Initialize()
        {
            artistInfoProvider = new Mock<IArtistInfoAggregator>();
            target = new ArtistInfoController(artistInfoProvider.Object);
        }

        [TestMethod]
        public async Task Get_Returns_ArtistInfoModel()
        {
            // Arrange
            var artistInfo = new ArtistInfo() {
                Description = "Description",
                Id = Guid.NewGuid(),
                Name = "Name",
                ReleaseGroups = new List<ArtistInfoReleaseGroup>() {
                    new ArtistInfoReleaseGroup() {
                        CovertArtUrl = "url1",
                        Id = Guid.NewGuid(),
                        PrimaryType = "Album",
                        Title = "Album1"
                    }, new ArtistInfoReleaseGroup() {
                        CovertArtUrl = "url2",
                        Id = Guid.NewGuid(),
                        PrimaryType = "Album",
                        Title = "Album2"
                    }
                }
            };
            artistInfoProvider.Setup(x => x.GetArtistInfoAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult<ArtistInfo>(artistInfo));

            // Act
            var result = await target.Get(artistInfo.Id) as OkNegotiatedContentResult<ArtistInfoModel>;

            // Assert
            Assert.IsNotNull(result);

            var content = result.Content;
            Assert.AreEqual(2, content.Albums.Count());
        }

        [TestMethod]
        public async Task Returns_404NotFound_If_AristInfo_Throws_ArtistNotFoundException()
        {
            // Arrange
            artistInfoProvider.Setup(x => x.GetArtistInfoAsync(It.IsAny<Guid>()))
                .Throws(new ArtistInfoNotFoundException());

            // Act
            var result = await target.Get(Guid.NewGuid()) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task Returns_500InternalServerError_For_All_Other_Exceptions()
        {
            // Arrange
            artistInfoProvider.Setup(x => x.GetArtistInfoAsync(It.IsAny<Guid>()))
                .Throws(new Exception());

            // Act
            var result = await target.Get(Guid.NewGuid()) as InternalServerErrorResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
