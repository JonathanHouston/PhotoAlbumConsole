using FluentAssertions;
using NSubstitute;
using Photo.Album.Console.Console;
using Photo.Album.Data.Models;
using Photo.Album.Engine.AlbumApiClient;

namespace Photo.Album.Console.Test
{
    [TestClass]
    public class AppTest
    {
        private readonly IAlbumApiClient _albumApiClient;
        private readonly IConsoleManager _consoleManager;
        private readonly App _app;

        public AppTest()
        {
            _albumApiClient = Substitute.For<IAlbumApiClient>();
            _consoleManager = Substitute.For<IConsoleManager>();
            _app = new App(_albumApiClient, _consoleManager);
        }

        [TestMethod]
        public void HelpDisplay_When_Called_AllHelpDisplayed()
        {
            //Act
            var result = _app.HelpDisplay();

            //Assert
            _consoleManager.Received().WriteLine("--allalbums will return all album ids");
            _consoleManager.Received().WriteLine("--album=id will return all photos in that album");
            _consoleManager.Received().WriteLine("--album=id*photo=id will return the photo requested from the album");
            result.Should().Be(true);   
        }

        [TestMethod]
        public async Task ListAllAlbumsDisplayAsync_When_NoIds_DisplayErrorMessage()
        {
            //arrange
            _albumApiClient.GetListOfPhotoAlbumIds().Returns(new List<int>());

            //Act
            var result = await _app.ListAllAlbumsDisplayAsync();

            //Assert
            _consoleManager.Received().WriteLine("There was an issue communicating to the resouce.  Please try your request again.");
            result.Should().Be(true);
        }

        [TestMethod]
        public async Task ListAllAlbumsDisplayAsync_When_AlbumIdsExist_AlbumIdsDisplayed()
        {
            //arrange
            var albumIds = new List<int> { 1, 2, 3 };
            _albumApiClient.GetListOfPhotoAlbumIds().Returns(albumIds);

            //Act
            var result = await _app.ListAllAlbumsDisplayAsync();

            //Assert
            albumIds.ForEach(id => _consoleManager.Received().WriteLine(id.ToString()));
            result.Should().Be(true);
        }

        [TestMethod]
        public async Task AlbumAndPhotoDisplayAsync_When_DataExists_PhotoDataDisplayed()
        {
            //arrange
            var testData = new PhotoMetaData { AlbumId = 1, Id = 1, Title = "Title1" };
            _albumApiClient.GetPhotoByAlbumIdAndPhotoId(Arg.Any<int>(), Arg.Any<int>()).Returns(testData);

            //Act
            var result = await _app.AlbumAndPhotoDisplayAsync($"--album={testData?.AlbumId}*photo={testData?.Id}");

            //Assert
            _consoleManager.Received().WriteLine($"Album: {testData?.AlbumId}");
            _consoleManager.Received().WriteLine($"\tPhotoId: {testData?.Id} Title: {testData?.Title}");
            result.Should().Be(true);
        }

        [TestMethod]
        public async Task AlbumAndPhotoDisplayAsync_When_PhotoDoesNotExist_ErrorDisplayed()
        {
            //arrange
            var badAlbumId = 2;
            var badPhotoId = 2;
            var testData = new PhotoMetaData { AlbumId = 1, Id = 1, Title = "Title1" };
            _albumApiClient.GetPhotoByAlbumIdAndPhotoId(testData.AlbumId, testData.Id).Returns(testData);

            //Act
            var result = await _app.AlbumAndPhotoDisplayAsync($"--album={badAlbumId}*photo={badPhotoId}");

            //Assert
            _consoleManager.Received().WriteLine($"There is no photo that matches AlbumId: {badAlbumId}  and PhotoId: {badPhotoId}");
            result.Should().Be(true);
        }

        [TestMethod]
        public async Task AlbumAndPhotoDisplayAsync_When_ArgsAreInvalid_ErrorDisplayed()
        {
            //arrange
            var badAlbumId = "notanumber";
            var badPhotoId = "#@!";
            var testData = new PhotoMetaData { AlbumId = 1, Id = 1, Title = "Title1" };
            _albumApiClient.GetPhotoByAlbumIdAndPhotoId(testData.AlbumId, testData.Id).Returns(testData);

            //Act
            var result = await _app.AlbumAndPhotoDisplayAsync($"--album={badAlbumId}*photo={badPhotoId}");

            //Assert
            _consoleManager.Received().WriteLine($"The value {badAlbumId} for AlbumId or the value for {badPhotoId} is invalid.  Please review and supply a number");
            result.Should().Be(true);
        }

        [TestMethod]
        public async Task PhotosInAlbumDisplayAsync_When_DataExists_PhotoDataDisplayed()
        {
            //arrange
            var albumId = 1;
            var testData = new List<PhotoMetaData>
            {
                new PhotoMetaData { AlbumId = albumId, Id = 1, Title = "Title1" },
                new PhotoMetaData { AlbumId = albumId, Id = 2, Title = "Title2" },
                new PhotoMetaData { AlbumId = albumId, Id = 3, Title = "Title3" },
            };
            _albumApiClient.GetListOfPhotosByAlbumId(albumId).Returns(testData);

            //Act
            var result = await _app.PhotosInAlbumDisplayAsync($"--album={albumId}");

            //Assert
            _consoleManager.Received().WriteLine($"Album: {albumId}");

            testData.ForEach(p => _consoleManager.Received().WriteLine($"\tPhotoId: {p.Id} Title: {p.Title}"));
            result.Should().Be(true);
        }

        [TestMethod]
        public async Task PhotosInAlbumDisplayAsync_When_ArgIsInvalid_ErrorDisplayed()
        {
            //arrange
            var albumId = "hgfsk@1";
            var testData = new List<PhotoMetaData>
            {
                new PhotoMetaData { AlbumId = 1, Id = 1, Title = "Title1" },
                new PhotoMetaData { AlbumId = 1, Id = 2, Title = "Title2" },
                new PhotoMetaData { AlbumId = 1, Id = 3, Title = "Title3" },
            };
            _albumApiClient.GetListOfPhotosByAlbumId(1).Returns(testData);

            //Act
            var result = await _app.PhotosInAlbumDisplayAsync($"--album={albumId}");

            //Assert
            _consoleManager.Received().WriteLine($"The value {albumId} is invalid.  Please supply a number");
            result.Should().Be(true);
        }

        [TestMethod]
        public void DefaultDisplay_When_Called_DefaultDisplayed()
        {
            //Act
            var result = _app.DefaultDisplay();

            //Assert
            _consoleManager.Received().WriteLine("Unrecognized command type. Use --h for list of available commands");
            result.Should().Be(true);
        }
    }
}