using ArtistInfoRepository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtistInfoRepository.UnitTests
{
    [TestClass]
    public class ArtistInfoRepositoryIntegrationTests
    {
        Dictionary<string, string> TestTable = new Dictionary<string, string>() {
            { "5b11f4ce-a62d-471e-81fc-a69a8278c7da", "Nirvana" },
            { "76b2e842-5e85-4c97-ab62-d5bc315595b5", "Pulp" },
            { "4f416ed7-dda2-45de-a6f1-b986e861cf4b", "Deerhunter" },
            { "52074ba6-e495-4ef3-9bb4-0703888a9f68",  "Arcade Fire" }
        };

        [TestMethod]
        public async Task TestMusicBrainzDataAggregator()
        {
            IArtistInfoAggregator artistInfoProvider = new MusicBrainzDataAggregator(new MusicBrainzClient(),
                new WikipediaArtistInfoClient(),
                new CoverArtArchiveClient());

            var watch = Stopwatch.StartNew();
            foreach (var testEntity in TestTable)
            {
                var result = await artistInfoProvider.GetArtistInfoAsync(Guid.Parse(testEntity.Key));
                Assert.AreEqual(testEntity.Value, result.Name);
            }
            watch.Stop();
            Console.WriteLine(string.Format("Execution time {0} ms", watch.ElapsedMilliseconds / TestTable.Count()));
        }
    }
}
