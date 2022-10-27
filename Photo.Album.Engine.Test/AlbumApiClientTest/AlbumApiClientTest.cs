using FluentAssertions;
using Moq;
using Moq.Protected;
using Photo.Album.Data.Models;
using System.Net;
using System.Text.Json;

namespace Photo.Album.Engine.Test.AlbumApiClientTest
{
    [TestClass]
    public class AlbumApiClientTest
    {
        [TestMethod]
        public async Task GetListOfPhotoAlbumIds_When_ThreeRecords_ReturnsSameData()
        {
            //Arrange
            var testData = new List<PhotoMetaData>
            {
                new PhotoMetaData { AlbumId = 1 },
                new PhotoMetaData { AlbumId = 2 },
                new PhotoMetaData { AlbumId = 3 },
            };

            var apiReturn = JsonSerializer.Serialize(testData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var albumApiClient = new AlbumApiClient.AlbumApiClient(SetupHttpClient(apiReturn));

            //Act
            var result = await albumApiClient.GetListOfPhotoAlbumIds();

            //Assert
            result?.Count.Should().Be(3);
            result?[0].Should().Be(1);
            result?[1].Should().Be(2);
            result?[2].Should().Be(3);
        }

        [TestMethod]
        public async Task GetListOfPhotoAlbumIds_When_NoRecords_ReturnsZeroRecords()
        {
            //Arrange
            var testData = new List<PhotoMetaData>{};

            var apiReturn = JsonSerializer.Serialize(testData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var albumApiClient = new AlbumApiClient.AlbumApiClient(SetupHttpClient(apiReturn));

            //Act
            var result = await albumApiClient.GetListOfPhotoAlbumIds();

            //Assert
            result?.Count.Should().Be(0);

        }

        [TestMethod]
        public async Task GetListOfPhotoAlbumIds_When_StatusNot200_ReturnsNull()
        {
            //Arrange
            var testData = new List<PhotoMetaData> { };

            var apiReturn = JsonSerializer.Serialize(testData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var albumApiClient = new AlbumApiClient.AlbumApiClient(SetupHttpClient(apiReturn, HttpStatusCode.BadRequest));

            //Act
            var result = await albumApiClient.GetListOfPhotoAlbumIds();

            //Assert
            result.Should().BeNull();

        }

        [TestMethod]
        public async Task GetListOfPhotosByAlbumId_When_ThreeRecords_ReturnsSameData()
        {
            //Arrange
            var albumId = 1;

            var testData = new List<PhotoMetaData>
            {
                new PhotoMetaData { AlbumId = albumId, Id = 1, Title = "Title1" },
                new PhotoMetaData { AlbumId = albumId, Id = 2, Title = "Title2" },
                new PhotoMetaData { AlbumId = albumId, Id = 3, Title = "Title3" },
            };

            var apiReturn = JsonSerializer.Serialize(testData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var albumApiClient = new AlbumApiClient.AlbumApiClient(SetupHttpClient(apiReturn));

            //Act
            var result = await albumApiClient.GetListOfPhotosByAlbumId(albumId);

            //Assert
            result?.Count.Should().Be(3);

            result?[0].AlbumId.Should().Be(1);
            result?[0].Id.Should().Be(1);
            result?[0].Title.Should().Be("Title1");

            result?[1].AlbumId.Should().Be(1);
            result?[1].Id.Should().Be(2);
            result?[1].Title.Should().Be("Title2");

            result?[2].AlbumId.Should().Be(1);
            result?[2].Id.Should().Be(3);
            result?[2].Title.Should().Be("Title3");
        }

        [TestMethod]
        public async Task GetListOfPhotosByAlbumId_When_NoRecords_ReturnsZeroRecords()
        {
            //Arrange
            var albumId = 1;

            var testData = new List<PhotoMetaData>{};

            var apiReturn = JsonSerializer.Serialize(testData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var albumApiClient = new AlbumApiClient.AlbumApiClient(SetupHttpClient(apiReturn));

            //Act
            var result = await albumApiClient.GetListOfPhotosByAlbumId(albumId);

            //Assert
            result?.Count.Should().Be(0);
        }

        [TestMethod]
        public async Task GetListOfPhotosByAlbumId_When_StatusNot200_ReturnsNull()
        {
            //Arrange
            var albumId = 1;

            var testData = new List<PhotoMetaData> { };

            var apiReturn = JsonSerializer.Serialize(testData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var albumApiClient = new AlbumApiClient.AlbumApiClient(SetupHttpClient(apiReturn, HttpStatusCode.BadRequest));

            //Act
            var result = await albumApiClient.GetListOfPhotosByAlbumId(albumId);

            //Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetPhotoByAlbumIdAndPhotoId_When_SelectOneRecordThatDoesNotExist_ReturnsNull()
        {
            //Arrange
            var albumId = 1;

            var testData = new List<PhotoMetaData>
            {
                new PhotoMetaData { AlbumId = albumId, Id = 1, Title = "Title1" },
                new PhotoMetaData { AlbumId = albumId, Id = 2, Title = "Title2" },
                new PhotoMetaData { AlbumId = albumId, Id = 3, Title = "Title3" },
            };

            var apiReturn = JsonSerializer.Serialize(testData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var albumApiClient = new AlbumApiClient.AlbumApiClient(SetupHttpClient(apiReturn));

            //Act
            var result = await albumApiClient.GetPhotoByAlbumIdAndPhotoId(albumId, 4);

            //Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetPhotoByAlbumIdAndPhotoId_When_StatusNot200_ReturnsNull()
        {
            //Arrange
            var albumId = 1;

            var testData = new List<PhotoMetaData>
            {
                new PhotoMetaData { AlbumId = albumId, Id = 1, Title = "Title1" },
                new PhotoMetaData { AlbumId = albumId, Id = 2, Title = "Title2" },
                new PhotoMetaData { AlbumId = albumId, Id = 3, Title = "Title3" },
            };

            var apiReturn = JsonSerializer.Serialize(testData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var albumApiClient = new AlbumApiClient.AlbumApiClient(SetupHttpClient(apiReturn, HttpStatusCode.BadRequest));

            //Act
            var result = await albumApiClient.GetPhotoByAlbumIdAndPhotoId(albumId, 4);

            //Assert
            result.Should().BeNull();
        }

        private static HttpClient SetupHttpClient(string content, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        {
            var httpMessageHandler = new Mock<HttpMessageHandler>();
            httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = httpStatusCode, Content = new StringContent(content) });

            return new HttpClient(httpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://google.com")
            };
        }

        [TestMethod]
        public async Task GetPhotoByAlbumIdAndPhotoId_When_SelectOneRecord_ReturnsSelectedRecordData()
        {
            //Arrange
            var albumId = 1;

            var testData = new List<PhotoMetaData>
            {
                new PhotoMetaData { AlbumId = albumId, Id = 1, Title = "Title1" },
                new PhotoMetaData { AlbumId = albumId, Id = 2, Title = "Title2" },
                new PhotoMetaData { AlbumId = albumId, Id = 3, Title = "Title3" },
            };

            var apiReturn = JsonSerializer.Serialize(testData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var albumApiClient = new AlbumApiClient.AlbumApiClient(SetupHttpClient(apiReturn));

            //Act
            var result = await albumApiClient.GetPhotoByAlbumIdAndPhotoId(albumId, 1);

            //Assert
            result?.AlbumId.Should().Be(1);
            result?.Id.Should().Be(1);
            result?.Title.Should().Be("Title1");
        }
    }
}