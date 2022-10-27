using System.Diagnostics.CodeAnalysis;

namespace Photo.Album.Data.Models
{
    [ExcludeFromCodeCoverage]
    public class Settings
    {
        public string BaseAlbumUrl { get; set; } = null!;
        public string ApiRetryCount { get; set; } = null!;
    }
}