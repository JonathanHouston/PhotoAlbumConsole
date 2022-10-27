using Photo.Album.Data.Models;

namespace Photo.Album.Engine.AlbumApiClient
{
    public interface IAlbumApiClient
    {
        Task<List<int>?> GetListOfPhotoAlbumIds();
        Task<List<PhotoMetaData>?> GetListOfPhotosByAlbumId(int albumId);
        Task<PhotoMetaData?> GetPhotoByAlbumIdAndPhotoId(int albumId, int photoId);
    }
}