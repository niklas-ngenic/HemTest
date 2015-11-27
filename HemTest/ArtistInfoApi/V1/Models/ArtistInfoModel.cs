using System;
using System.Collections.Generic;

namespace ArtistInfoApi.V1.Models
{
    public class ArtistInfoModel
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public ICollection<Album> Albums { get; set; }
    }
}