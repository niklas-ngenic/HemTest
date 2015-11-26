using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using Newtonsoft.Json;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace ArtistInfoRepository.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        MusicBrainzDataAggregator target;
        Mock<IArtistInfoClient> artistInfoClient;
        Mock<IArtistDescriptionClient> artistDescriptionClient;
        Mock<IAlbumCoverArtClient> albumCoverArtClient;

        [TestInitialize]
        public void Initialize()
        {
            artistInfoClient = new Mock<IArtistInfoClient>();
            artistDescriptionClient = new Mock<IArtistDescriptionClient>();
            albumCoverArtClient = new Mock<IAlbumCoverArtClient>();
            target = new MusicBrainzDataAggregator(artistInfoClient.Object,
                artistDescriptionClient.Object,
                albumCoverArtClient.Object);
        }

        [TestMethod]
        public async Task Returns_ArtistInfo_With_Description_And_AlbumCoverArtUrls()
        {
            // Arrange
            Guid artistGuid = Guid.NewGuid();
            Guid album1Id = Guid.NewGuid();
            Guid album2Id = Guid.NewGuid();
            string artistDescriptionUrl = "descriptionUrl";
            ArtistInfo artistInfo = new ArtistInfo()
            {
                Id = artistGuid,
                Description = string.Empty,
                Relations = new List<ArtistInfoRelation>()
                {
                    new ArtistInfoRelation() {
                    Type = "wikipedia",
                    Url = new ArtistInfoRelationUrl() {
                        Id = Guid.NewGuid(),
                        Resource = artistDescriptionUrl
                        }
                    }
                },
                ReleaseGroups = new List<ArtistInfoReleaseGroup>()
                {
                    new ArtistInfoReleaseGroup()
                    {
                        Id = album1Id,
                        CovertArtUrl = string.Empty, 
                        PrimaryType = "Album",
                        Title = "Album1"
                    }, 
                    new ArtistInfoReleaseGroup()
                    {
                        Id = album2Id,
                        CovertArtUrl = string.Empty,
                        PrimaryType = "Album",
                        Title = "Album2"
                    }
                }
            };

            artistInfoClient.Setup(x => x.GetAsync(It.Is<Guid>(y => y == artistGuid)))
                .Returns(Task.FromResult(artistInfo));

            artistDescriptionClient.Setup(x => x.GetAsync(It.Is<string>(y => y == artistDescriptionUrl)))
                .Returns(Task.FromResult("artistDescription"));

            albumCoverArtClient.Setup(x => x.GetAsync(It.Is<Guid>(y => y == album1Id)))
                .Returns(Task.FromResult("album1CoverUrl"));
            albumCoverArtClient.Setup(x => x.GetAsync(It.Is<Guid>(y => y == album2Id)))
                .Returns(Task.FromResult("album2CoverUrl"));

            // Act
            var result = await target.GetArtistInfoAsync(artistGuid);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Description, "artistDescription");
            Assert.AreEqual(2, result.ReleaseGroups.Count());
            Assert.IsTrue(result.ReleaseGroups.Any(x => x.CovertArtUrl == "album1CoverUrl"));
            Assert.IsTrue(result.ReleaseGroups.Any(x => x.CovertArtUrl == "album2CoverUrl"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArtistInfoNotFoundException))]
        public async Task Throws_NotFoundException_If_ArtistInfoClient_Returns_Null()
        {
            // Arrange
            artistInfoClient.Setup(x => x.GetAsync(It.IsAny<Guid>())).Returns(Task.FromResult<ArtistInfo>(null));

            // Act
            await target.GetArtistInfoAsync(Guid.NewGuid());
        }

        [TestMethod]
        public async Task Do_Nothing_If_ArtistInfoDescriptionClient_Returns_Null()
        {
            // Arrange
            var artistInfo = new ArtistInfo()
            {
                Relations = new List<ArtistInfoRelation>() {
                    new ArtistInfoRelation() {
                        Type = "wikipedia",
                        Url = new ArtistInfoRelationUrl() {
                            Id = Guid.NewGuid(),
                            Resource = "resource"
                        }
                    }
                }
            };
            artistInfoClient.Setup(x => x.GetAsync(It.IsAny<Guid>())).Returns(Task.FromResult(artistInfo));
            artistDescriptionClient.Setup(x => x.GetAsync(It.IsAny<string>())).Returns(Task.FromResult<string>(null));

            // Act
            var result = await target.GetArtistInfoAsync(Guid.NewGuid());

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task Do_Nothin_If_AlbumCoverArtClient_Returns_Null()
        {
            // Arrange
            var artistInfo = new ArtistInfo() {
                ReleaseGroups = new List<ArtistInfoReleaseGroup>() {
                    new ArtistInfoReleaseGroup() {
                        Id = Guid.NewGuid()
                    }
                }
            };

            artistInfoClient.Setup(x => x.GetAsync(It.IsAny<Guid>())).Returns(Task.FromResult(artistInfo));
            artistDescriptionClient.Setup(x => x.GetAsync(It.IsAny<string>())).Returns(Task.FromResult("Description"));
            albumCoverArtClient.Setup(x => x.GetAsync(It.IsAny<Guid>())).Returns(Task.FromResult<string>(null));

            // Act
            var result = await target.GetArtistInfoAsync(Guid.NewGuid());

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
