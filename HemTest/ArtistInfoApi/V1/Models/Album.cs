using System;

namespace ArtistInfoApi.V1.Models
{
    public class Album
    {
        public string Title { get; set; }

        public Guid Id { get; set; }

        public string Image { get; set; }
    }
}