using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ArtistInfoRepository
{
    public class ArtistInfo
    {
        private ICollection<ArtistInfoReleaseGroup> _releaseGroups;

        private ICollection<ArtistInfoRelation> _relations;

        public Guid Id { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }

        public ICollection<ArtistInfoRelation> Relations
        {
            get { return _relations ?? (_relations = new List<ArtistInfoRelation>()); }
            set { _relations = value; }
        }

        [JsonProperty("release-groups")]
        public ICollection<ArtistInfoReleaseGroup> ReleaseGroups
        {
            get { return _releaseGroups ?? (_releaseGroups = new List<ArtistInfoReleaseGroup>()); }
            set { _releaseGroups = value; }
        }
    }

    public class ArtistInfoRelation
    {
        public string Type { get; set; }

        public ArtistInfoRelationUrl Url { get; set; }
    }

    public class ArtistInfoRelationUrl
    {
        public Guid Id { get; set; }

        public string Resource { get; set; }
    }

    public class ArtistInfoReleaseGroup
    {
        public Guid Id { get; set; }

        [JsonProperty("primary-type")]
        public string PrimaryType { get; set; }

        public string Title { get; set; }

        public string CovertArtUrl { get; set; }
    }
}