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

namespace ArtistInfoRepository.Test
{
    [TestClass]
    public class UnitTest1
    {
        ArtistInfoProvider target;
        Mock<IArtistInfoClient> artistInfoClient;
        Mock<IArtistDescriptionClient> artistDescriptionClient;
        Mock<IAlbumCoverArtClient> albumCoverArtClient;

        [TestInitialize]
        public void Initialize()
        {
            artistInfoClient = new Mock<IArtistInfoClient>();
            artistDescriptionClient = new Mock<IArtistDescriptionClient>();
            albumCoverArtClient = new Mock<IAlbumCoverArtClient>();
            target = new ArtistInfoProvider(artistInfoClient.Object,
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
        public void Can_Parse_MusicQuizJsonResponse()
        {
            string json = @"{ ""relations"":[{""source-credit"":"""",""attribute-values"":{},""type"":""lyrics"",""url"":{""relations"":[],""resource"":""http://decoda.com/nirvana-lyrics"",""id"":""2759d810-5a97-446e-b168-09d2dec389a9""},""direction"":""forward"",""ended"":false,""begin"":null,""target-credit"":"""",""end"":null,""attributes"":[],""type-id"":""e4d73442-3762-45a8-905c-401da65544ed""},{""source-credit"":"""",""type"":""wikipedia"",""attribute-values"":{},""direction"":""forward"",""url"":{""relations"":[],""id"":""d99c5574-096b-45af-bf20-c3dc3e94fde5"",""resource"":""http://en.wikipedia.org/wiki/Nirvana_(band)""},""ended"":false,""begin"":null,""target-credit"":"""",""end"":null,""type-id"":""29651736-fa6d-48e4-aadc-a557c6add1cb"",""attributes"":[]},{""ended"":false,""url"":{""relations"":[],""id"":""eb2efdf0-05cf-4ea8-bc00-b1b0d2bcdcfb"",""resource"":""http://genius.com/artists/Nirvana""},""direction"":""forward"",""source-credit"":"""",""type"":""lyrics"",""attribute-values"":{},""attributes"":[],""type-id"":""e4d73442-3762-45a8-905c-401da65544ed"",""end"":null,""target-credit"":"""",""begin"":null},{""attributes"":[],""type-id"":""e4d73442-3762-45a8-905c-401da65544ed"",""end"":null,""target-credit"":"""",""begin"":null,""ended"":false,""url"":{""relations"":[],""resource"":""http://lyrics.wikia.com/Nirvana"",""id"":""47e69f92-e4b3-46ca-9f95-28cc11bedf34""},""direction"":""forward"",""source-credit"":"""",""type"":""lyrics"",""attribute-values"":{}},{""type-id"":""d94fb61c-fa20-4e3c-a19a-71a949fb2c55"",""attributes"":[],""end"":null,""target-credit"":"""",""begin"":null,""ended"":false,""direction"":""forward"",""url"":{""resource"":""http://musicmoz.org/Bands_and_Artists/N/Nirvana/"",""id"":""21daaa31-4c41-4fc9-b07d-e77b3f01d3e6"",""relations"":[]},""source-credit"":"""",""type"":""other databases"",""attribute-values"":{}},{""ended"":false,""url"":{""relations"":[],""resource"":""http://muzikum.eu/en/122-4216/nirvana/lyrics.html"",""id"":""740505c7-3a30-4482-8ca6-10183b37d308""},""direction"":""forward"",""attribute-values"":{},""source-credit"":"""",""type"":""lyrics"",""attributes"":[],""type-id"":""e4d73442-3762-45a8-905c-401da65544ed"",""end"":null,""target-credit"":"""",""begin"":null},{""attributes"":[],""type-id"":""769085a1-c2f7-4c24-a532-2375a77693bd"",""end"":null,""target-credit"":"""",""begin"":null,""ended"":false,""url"":{""id"":""f6a499eb-5959-4861-95f1-13caec960006"",""resource"":""http://open.spotify.com/artist/6olE6TJLqED3rqDCT0FyPh"",""relations"":[]},""direction"":""forward"",""source-credit"":"""",""type"":""streaming music"",""attribute-values"":{}},{""attribute-values"":{},""source-credit"":"""",""type"":""other databases"",""direction"":""forward"",""url"":{""relations"":[],""id"":""07a468c6-3ceb-4e44-b0b9-f46cca1151dc"",""resource"":""http://rateyourmusic.com/artist/nirvana""},""ended"":false,""begin"":null,""target-credit"":"""",""end"":null,""type-id"":""d94fb61c-fa20-4e3c-a19a-71a949fb2c55"",""attributes"":[]},{""type"":""discography"",""source-credit"":"""",""attribute-values"":{},""direction"":""forward"",""url"":{""id"":""cca2fe38-99c2-4f4d-a3e0-6d84945b7861"",""resource"":""http://sliver.it/nirvana/test/index.html"",""relations"":[]},""ended"":false,""begin"":null,""target-credit"":"""",""end"":null,""type-id"":""4fb0eeec-a6eb-4ae3-ad52-b55765b94e8f"",""attributes"":[]},{""attribute-values"":{},""source-credit"":"""",""type"":""VIAF"",""direction"":""forward"",""url"":{""relations"":[],""resource"":""http://viaf.org/viaf/138573893"",""id"":""421a959a-c50f-4a52-99e4-3c603dd37145""},""ended"":false,""begin"":null,""target-credit"":"""",""end"":null,""type-id"":""e8571dcc-35d4-4e91-a577-a3382fd84460"",""attributes"":[]},{""attributes"":[],""type-id"":""6b3e3c85-0002-4f34-aca6-80ace0d7e846"",""end"":null,""target-credit"":"""",""begin"":null,""ended"":false,""url"":{""id"":""4a425cd3-641d-409c-a282-2334935bf1bd"",""resource"":""http://www.allmusic.com/artist/mn0000357406"",""relations"":[]},""direction"":""forward"",""type"":""allmusic"",""source-credit"":"""",""attribute-values"":{}},{""attributes"":[],""type-id"":""d028a975-000c-4525-9333-d3c8425e4b54"",""end"":null,""target-credit"":"""",""begin"":null,""ended"":false,""url"":{""relations"":[],""id"":""627ce98c-0eef-41c7-b28f-cc3387b98aab"",""resource"":""http://www.bbc.co.uk/music/artists/5b11f4ce-a62d-471e-81fc-a69a8278c7da""},""direction"":""forward"",""attribute-values"":{},""source-credit"":"""",""type"":""BBC Music page""},{""attributes"":[],""type-id"":""04a5b104-a4c2-4bac-99a1-7b837c37d9e4"",""end"":null,""target-credit"":"""",""begin"":null,""ended"":false,""url"":{""id"":""81846eca-af41-43d0-bcae-b62dbf5cfa2f"",""resource"":""http://www.discogs.com/artist/125246"",""relations"":[]},""direction"":""forward"",""attribute-values"":{},""source-credit"":"""",""type"":""discogs""},{""type"":""official homepage"",""source-credit"":"""",""attribute-values"":{},""direction"":""forward"",""url"":{""relations"":[],""resource"":""http://www.hereisnirvana.com/"",""id"":""9e31ece5-9275-489c-a197-065a3c466474""},""ended"":false,""begin"":null,""target-credit"":"""",""end"":null,""type-id"":""fe33d22f-c3b0-4d68-bd53-a856badf2b15"",""attributes"":[]},{""type"":""IMDb"",""source-credit"":"""",""attribute-values"":{},""direction"":""forward"",""url"":{""id"":""85229dcd-cc79-4ce8-a3be-4b0539e9148a"",""resource"":""http://www.imdb.com/name/nm1110321/"",""relations"":[]},""ended"":false,""begin"":null,""target-credit"":"""",""end"":null,""type-id"":""94c8b0cc-4477-4106-932c-da60e63de61c"",""attributes"":[]},{""begin"":null,""target-credit"":"""",""end"":null,""attributes"":[],""type-id"":""08db8098-c0df-4b78-82c3-c8697b4bba7f"",""source-credit"":"""",""type"":""last.fm"",""attribute-values"":{},""url"":{""relations"":[],""id"":""36dc918b-2a58-4d31-9ccd-10af003e7386"",""resource"":""http://www.last.fm/music/Nirvana""},""direction"":""forward"",""ended"":false},{""ended"":false,""url"":{""id"":""73a6779b-8aaa-42ff-9833-e550ad974be4"",""resource"":""http://www.livenirvana.com/bootography/listing85a9.html?listingquery=all"",""relations"":[]},""direction"":""forward"",""source-credit"":"""",""attribute-values"":{},""type"":""discography"",""attributes"":[],""type-id"":""4fb0eeec-a6eb-4ae3-ad52-b55765b94e8f"",""end"":null,""target-credit"":"""",""begin"":null},{""url"":{""relations"":[],""resource"":""http://www.livenirvana.com/digitalnirvana/discography/index.html"",""id"":""aa2f9928-f2d0-4ce3-9714-af7566b9df94""},""direction"":""forward"",""ended"":false,""source-credit"":"""",""type"":""discography"",""attribute-values"":{},""end"":null,""attributes"":[],""type-id"":""4fb0eeec-a6eb-4ae3-ad52-b55765b94e8f"",""begin"":null,""target-credit"":""""},{""ended"":false,""url"":{""relations"":[],""resource"":""http://www.livenirvana.com/"",""id"":""74c7fc4f-cb3d-45ef-9c83-f7a1061f0272""},""direction"":""forward"",""source-credit"":"""",""type"":""fanpage"",""attribute-values"":{},""attributes"":[],""type-id"":""f484f897-81cc-406e-96f9-cd799a04ee24"",""end"":null,""target-credit"":"""",""begin"":null},{""begin"":null,""target-credit"":"""",""end"":null,""type-id"":""d94fb61c-fa20-4e3c-a19a-71a949fb2c55"",""attributes"":[],""attribute-values"":{},""source-credit"":"""",""type"":""other databases"",""direction"":""forward"",""url"":{""relations"":[],""resource"":""http://www.musik-sammler.de/artist/2597"",""id"":""e88eb245-f5f5-4173-afb7-378f950644c2""},""ended"":false},{""target-credit"":"""",""begin"":null,""type-id"":""4fb0eeec-a6eb-4ae3-ad52-b55765b94e8f"",""attributes"":[],""end"":null,""source-credit"":"""",""type"":""discography"",""attribute-values"":{},""ended"":false,""direction"":""forward"",""url"":{""relations"":[],""resource"":""http://www.nirvanaarchive.com/"",""id"":""d0498330-0679-4174-a145-273dd974e09e""}},{""ended"":false,""url"":{""id"":""e42476ce-e923-498e-8e98-d11ae200aebb"",""resource"":""http://www.nirvanaclub.com/"",""relations"":[]},""direction"":""forward"",""attribute-values"":{},""source-credit"":"""",""type"":""fanpage"",""attributes"":[],""type-id"":""f484f897-81cc-406e-96f9-cd799a04ee24"",""end"":null,""target-credit"":"""",""begin"":null},{""attribute-values"":{},""source-credit"":"""",""type"":""purevolume"",""ended"":false,""direction"":""forward"",""url"":{""relations"":[],""resource"":""http://www.purevolume.com/Nirvana109A"",""id"":""61139a11-0318-481d-b305-bf569e400e50""},""target-credit"":"""",""begin"":null,""type-id"":""b6f02157-a9d3-4f24-9057-0675b2dbc581"",""attributes"":[],""end"":null},{""source-credit"":"""",""attribute-values"":{},""type"":""secondhandsongs"",""ended"":false,""url"":{""id"":""5f33ae58-aa56-40bd-ad14-ab7db9b3d3fd"",""resource"":""http://www.secondhandsongs.com/artist/169"",""relations"":[]},""direction"":""forward"",""target-credit"":"""",""begin"":null,""attributes"":[],""type-id"":""79c5b84d-a206-4f4c-9832-78c028c312c3"",""end"":null},{""end"":null,""type-id"":""d94fb61c-fa20-4e3c-a19a-71a949fb2c55"",""attributes"":[],""begin"":null,""target-credit"":"""",""direction"":""forward"",""url"":{""relations"":[],""id"":""805e0346-cdfb-4eae-a8c4-f27937288cda"",""resource"":""http://www.whosampled.com/Nirvana/""},""ended"":false,""type"":""other databases"",""source-credit"":"""",""attribute-values"":{}},{""end"":null,""type-id"":""689870a4-a1e4-4912-b17f-7b2664215698"",""attributes"":[],""begin"":null,""target-credit"":"""",""direction"":""forward"",""url"":{""relations"":[],""id"":""1221730c-3a48-49fa-8001-beaa6e93c892"",""resource"":""http://www.wikidata.org/wiki/Q11649""},""ended"":false,""source-credit"":"""",""attribute-values"":{},""type"":""wikidata""},{""attributes"":[],""type-id"":""d94fb61c-fa20-4e3c-a19a-71a949fb2c55"",""end"":null,""target-credit"":"""",""begin"":null,""ended"":false,""url"":{""id"":""2127a8c5-befb-401c-a6f0-057b4f4a4581"",""resource"":""http://www.worldcat.org/wcidentities/lccn-n92-11111"",""relations"":[]},""direction"":""forward"",""source-credit"":"""",""attribute-values"":{},""type"":""other databases""},{""attributes"":[],""type-id"":""6a540e5b-58c6-4192-b6ba-dbc71ec8fcf0"",""end"":null,""target-credit"":"""",""begin"":null,""ended"":false,""url"":{""resource"":""http://www.youtube.com/user/NirvanaVEVO"",""id"":""c8d415be-b993-4ade-bd28-d3ab4806fcbd"",""relations"":[]},""direction"":""forward"",""attribute-values"":{},""source-credit"":"""",""type"":""youtube""},{""end"":null,""attributes"":[],""type-id"":""221132e9-e30e-43f2-a741-15afc4c5fa7c"",""begin"":null,""target-credit"":"""",""url"":{""resource"":""https://commons.wikimedia.org/wiki/File:Nirvana_around_1992.jpg"",""id"":""88867281-e540-4b37-9fce-870dda1bfd8b"",""relations"":[]},""direction"":""forward"",""ended"":false,""source-credit"":"""",""type"":""image"",""attribute-values"":{}},{""url"":{""id"":""706cb178-5d5c-49e0-a07f-149751b94043"",""resource"":""https://myspace.com/nirvana"",""relations"":[]},""direction"":""forward"",""ended"":false,""type"":""myspace"",""source-credit"":"""",""attribute-values"":{},""end"":null,""attributes"":[],""type-id"":""bac47923-ecde-4b59-822e-d08f0cd10156"",""begin"":null,""target-credit"":""""},{""source-credit"":"""",""attribute-values"":{},""type"":""soundcloud"",""direction"":""forward"",""url"":{""relations"":[],""resource"":""https://soundcloud.com/nirvana"",""id"":""14c6ec03-4bd8-4f12-bfef-f2450746adab""},""ended"":false,""begin"":null,""target-credit"":"""",""end"":null,""type-id"":""89e4a949-0976-440d-bda1-5f772c1e5710"",""attributes"":[]},{""begin"":null,""target-credit"":"""",""end"":null,""type-id"":""99429741-f3f6-484b-84f8-23af51991770"",""attributes"":[],""type"":""social network"",""source-credit"":"""",""attribute-values"":{},""direction"":""forward"",""url"":{""resource"":""https://twitter.com/Nirvana"",""id"":""f5cfd704-b99a-45fc-9aed-8747257cad03"",""relations"":[]},""ended"":false},{""end"":null,""type-id"":""99429741-f3f6-484b-84f8-23af51991770"",""attributes"":[],""begin"":null,""target-credit"":"""",""direction"":""forward"",""url"":{""relations"":[],""id"":""a9cec2d1-0544-4dc9-a4b2-640751654573"",""resource"":""https://www.facebook.com/Nirvana""},""ended"":false,""type"":""social network"",""source-credit"":"""",""attribute-values"":{}}],""gender"":null,""release-groups"":[{""id"":""01cf1391-141b-3c87-8650-45ade6e59070"",""first-release-date"":""1992-12-14"",""title"":""Incesticide"",""relations"":[],""secondary-types"":[""Compilation""],""primary-type"":""Album"",""disambiguation"":""""},{""primary-type"":""Album"",""disambiguation"":"""",""secondary-types"":[""Compilation""],""relations"":[],""first-release-date"":""1993"",""title"":""Wipeout"",""id"":""178b993e-fa9c-36d3-9d73-c5a8ba0c748d""},{""primary-type"":""Album"",""disambiguation"":"""",""relations"":[],""secondary-types"":[],""title"":""Verse Chorus Verse"",""first-release-date"":""1994"",""id"":""1a0edfef-ed8a-4664-8911-1ee69c39ae26""},{""id"":""1b022e01-4da6-387b-8658-8678046e4cef"",""title"":""Nevermind"",""first-release-date"":""1991-09-23"",""relations"":[],""secondary-types"":[],""disambiguation"":"""",""primary-type"":""Album""},{""first-release-date"":""1993-09-21"",""title"":""In Utero"",""id"":""2a0981fb-9593-3019-864b-ce934d97a16e"",""secondary-types"":[],""relations"":[],""primary-type"":""Album"",""disambiguation"":""""},{""disambiguation"":"""",""primary-type"":""Album"",""id"":""339ab911-1568-32cf-8997-f00a538208c9"",""first-release-date"":""1992"",""title"":""Down With Me"",""secondary-types"":[""Compilation""],""relations"":[]},{""id"":""37b1659c-7560-34cd-8946-d6f3c5c9ad92"",""title"":""It’s Better to Burn Out Than to Fade Away…"",""first-release-date"":""1994"",""relations"":[],""secondary-types"":[""Compilation""],""disambiguation"":"""",""primary-type"":""Album""},{""disambiguation"":"""",""primary-type"":""Album"",""secondary-types"":[""Compilation""],""relations"":[],""id"":""3f09f97d-3b18-336c-9760-9ebb7df3497e"",""first-release-date"":""1992"",""title"":""Seventh Heaven""},{""disambiguation"":"""",""primary-type"":""Album"",""title"":""Outcesticide II: The Needle & The Damage Done"",""first-release-date"":""1994"",""id"":""584b7e62-d8b5-378e-986c-dfc78e1fc06e"",""relations"":[],""secondary-types"":[""Compilation""]},{""primary-type"":""Album"",""disambiguation"":"""",""secondary-types"":[""Compilation""],""relations"":[],""id"":""5bcaeba6-a532-3fa0-b540-75c09f70f759"",""title"":""The Very Best"",""first-release-date"":""1994""},{""secondary-types"":[""Compilation""],""relations"":[],""first-release-date"":""1995"",""title"":""Outcesticide III: The Final Solution"",""id"":""5dba01ed-b4e9-394e-a875-c7c2ff052133"",""primary-type"":""Album"",""disambiguation"":""""},{""first-release-date"":""1995"",""title"":""B-Side Themselves"",""id"":""60c826fb-8853-3796-8ff2-16c35f362ec9"",""secondary-types"":[""Compilation""],""relations"":[],""disambiguation"":"""",""primary-type"":""Album""},{""id"":""85155291-222f-388a-a5f7-dcaa60b65afd"",""first-release-date"":""1992"",""title"":""Before We Ever Minded"",""secondary-types"":[""Compilation""],""relations"":[],""primary-type"":""Album"",""disambiguation"":""""},{""primary-type"":""Album"",""disambiguation"":"""",""secondary-types"":[""Compilation""],""relations"":[],""id"":""8648b043-3e5c-3cd7-8396-52fe9910f569"",""title"":""Unreleased Tracks"",""first-release-date"":""2000""},{""relations"":[],""secondary-types"":[""Compilation""],""title"":""In Utero Demos"",""first-release-date"":""2001"",""id"":""915dc49e-8203-31b8-bb19-ca5996a2b810"",""disambiguation"":"""",""primary-type"":""Album""},{""relations"":[],""secondary-types"":[""Compilation""],""id"":""9a198646-ff93-3459-b943-f39da399c270"",""first-release-date"":""2001"",""title"":""First Live Show"",""primary-type"":""Album"",""disambiguation"":""""},{""disambiguation"":"""",""primary-type"":""Album"",""secondary-types"":[""Compilation""],""relations"":[],""first-release-date"":""2000-02-14"",""title"":""Secret Songs: The Unreleased Album"",""id"":""9a3ed295-7823-374e-95fe-c8ce79b6ca2a""},{""disambiguation"":"""",""primary-type"":""Album"",""title"":""Outcesticide: In Memory of Kurt Cobain"",""first-release-date"":""1994"",""id"":""9f42e883-12a4-3790-b3c1-0e2982f2d832"",""relations"":[],""secondary-types"":[""Compilation""]},{""relations"":[],""secondary-types"":[""Compilation""],""first-release-date"":""1994"",""title"":""Heart Shaped Box, Volume 2"",""id"":""bcbe3ddf-7901-315c-8282-f0e9d5b88285"",""disambiguation"":"""",""primary-type"":""Album""},{""first-release-date"":""1994"",""title"":""The Eternal Legacy"",""id"":""c0fdd829-fa5d-4bf0-a227-39979375298d"",""relations"":[],""secondary-types"":[""Compilation""],""primary-type"":""Album"",""disambiguation"":""""},{""primary-type"":""Album"",""disambiguation"":"""",""relations"":[],""secondary-types"":[""Compilation""],""id"":""e602b067-c9eb-31eb-bed0-80e4245d5d54"",""title"":""Dark Emotion"",""first-release-date"":""1999""},{""disambiguation"":"""",""primary-type"":""Album"",""id"":""ee7695bc-fde5-3a84-8ebb-195fa5ee6c2d"",""first-release-date"":""1998"",""title"":""Outcesticide V: Disintegration"",""relations"":[],""secondary-types"":[""Compilation""]},{""id"":""f1afec0b-26dd-3db5-9aa1-c91229a74a24"",""first-release-date"":""1989-06-01"",""title"":""Bleach"",""secondary-types"":[],""relations"":[],""primary-type"":""Album"",""disambiguation"":""""},{""disambiguation"":"""",""primary-type"":""Album"",""secondary-types"":[""Compilation""],""relations"":[],""first-release-date"":""1994"",""title"":""Grunge Is Dead"",""id"":""fdeebee4-b749-31c4-8a9b-d8d25cd9a43a""},{""first-release-date"":""1995"",""title"":""Twilight of the Gods"",""id"":""ff9dec8b-3674-35a3-aa39-9f9ba3d30b71"",""secondary-types"":[""Compilation""],""relations"":[],""primary-type"":""Album"",""disambiguation"":""""}],""id"":""5b11f4ce-a62d-471e-81fc-a69a8278c7da"",""sort-name"":""Nirvana"",""begin_area"":{""iso_3166_3_codes"":[],""id"":""a640b45c-c173-49b1-8030-973603e895b5"",""iso_3166_2_codes"":[],""iso_3166_1_codes"":[],""disambiguation"":"""",""name"":""Aberdeen"",""sort-name"":""Aberdeen""},""disambiguation"":""90s US grunge band"",""end_area"":null,""type"":""Group"",""country"":""US"",""area"":{""name"":""United States"",""disambiguation"":"""",""sort-name"":""United States"",""iso_3166_2_codes"":[],""id"":""489ce91b-6658-3307-9877-795b68554c98"",""iso_3166_3_codes"":[],""iso_3166_1_codes"":[""US""]},""ipis"":[],""name"":""Nirvana"",""life-span"":{""ended"":true,""end"":""1994-04-05"",""begin"":""1988-01""}}";
            var artistInfo = JsonConvert.DeserializeObject<ArtistInfo>(json);
        }

        [TestMethod]
        public async Task Can_Get_AlbumArtCoverUrl()
        {
            CoverArtArchiveClient client = new CoverArtArchiveClient();
            await client.GetAsync(Guid.Parse("178b993e-fa9c-36d3-9d73-c5a8ba0c748d"));
        }

        [TestMethod]
        public async Task Test()
        {
            IArtistInfoProvider artistInfoProvider = new ArtistInfoProvider(new MusicBrainzClient(), null, new CoverArtArchiveClient());
            var watch = Stopwatch.StartNew();
            var content = await artistInfoProvider.GetArtistInfoAsync(Guid.Parse("5b11f4ce-a62d-471e-81fc-a69a8278c7da"));
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds / 1000);
        }
    }
}
