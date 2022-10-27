using Photo.Album.Data.Models;
using System.Text.Json;

namespace Photo.Album.Engine.AlbumApiClient
{
    public class AlbumApiClient : IAlbumApiClient
    {
        private readonly HttpClient _httpClient;

        public AlbumApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<int>?> GetListOfPhotoAlbumIds()
        {
            var response = await _httpClient.GetAsync(string.Empty);

            if (response.IsSuccessStatusCode)
            {
                var listOfAllPhotos = JsonSerializer.Deserialize<List<PhotoMetaData>>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                var listOfAlbumIds = listOfAllPhotos?.Select(p => p.AlbumId).Distinct().ToList();
                return listOfAlbumIds;
            }

            return null;
        }

        public async Task<List<PhotoMetaData>?> GetListOfPhotosByAlbumId(int albumId)
        {
            var response = await _httpClient.GetAsync($"?albumId={albumId}");

            if (response.IsSuccessStatusCode)
            {
                var listOfPhotos = JsonSerializer.Deserialize<List<PhotoMetaData>>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return listOfPhotos;
            }

            return null;
        }

        public async Task<PhotoMetaData?> GetPhotoByAlbumIdAndPhotoId(int albumId, int photoId)
        {
            var response = await _httpClient.GetAsync($"?albumId={albumId}");

            if (response.IsSuccessStatusCode)
            {
                var listOfPhotos = JsonSerializer.Deserialize<List<PhotoMetaData>>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                var photo = listOfPhotos?.Where(p => p.Id == photoId).FirstOrDefault();
                return photo;
            }

            return null;
        }
    }
}