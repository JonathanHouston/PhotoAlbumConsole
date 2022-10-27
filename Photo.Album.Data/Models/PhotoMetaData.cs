using System.Diagnostics.CodeAnalysis;

namespace Photo.Album.Data.Models
{
    [ExcludeFromCodeCoverage]
    public class PhotoMetaData
    {
        public int AlbumId { get; set; }
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Url { get; set; }
        public string? ThumbnailUrl { get; set; }
    }
}