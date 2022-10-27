using Photo.Album.Console.Console;
using Photo.Album.Engine.AlbumApiClient;

namespace Photo.Album.Console;

public class App
{
    private readonly IAlbumApiClient _albumApiClient;
    private readonly IConsoleManager _consoleManager;

    public App(IAlbumApiClient albumApiClient, IConsoleManager consoleManager)
    {
        _albumApiClient = albumApiClient;
        _consoleManager = consoleManager;
    }

    public async Task RunAsync(string[] args)
    {
        var commandExecuted = false;

        foreach (var arg in args)
        {
            bool exitFor;
            switch (arg.ToLower())
            {
                case { } when arg == "--h":
                    exitFor = HelpDisplay();
                    commandExecuted = exitFor;
                    break;
                case { } when arg == "--allalbums":
                    exitFor = await ListAllAlbumsDisplayAsync();
                    commandExecuted = exitFor;
                    break;
                case { } when (arg.StartsWith("--album=") && arg.Contains("*photo=")):
                    exitFor = await AlbumAndPhotoDisplayAsync(arg);
                    commandExecuted = exitFor;
                    break;
                case { } when (arg.StartsWith("--album=") && !arg.Contains("*photo=")):
                    exitFor = await PhotosInAlbumDisplayAsync(arg);
                    commandExecuted = exitFor;
                    break;
                default:
                    exitFor = DefaultDisplay();
                    break;
            }

            if (exitFor) break;
        }
        if (!commandExecuted) DefaultDisplay();
    }

    public bool HelpDisplay()
    {
        _consoleManager.WriteLine("--allalbums will return all album ids");
        _consoleManager.WriteLine("--album=id will return all photos in that album");
        _consoleManager.WriteLine("--album=id*photo=id will return the photo requested from the album");
        return true;
    }

    public async Task<bool> ListAllAlbumsDisplayAsync()
    {
        var albumIds = await _albumApiClient.GetListOfPhotoAlbumIds();

        if (!albumIds?.Any() ?? true)
        {
            _consoleManager.WriteLine("There was an issue communicating to the resouce.  Please try your request again.");
            return true;

        }

        albumIds?.ForEach(albumId =>
        {
            _consoleManager.WriteLine(albumId.ToString());
        });
        return true;
    }

    public async Task<bool> AlbumAndPhotoDisplayAsync(string arg)
    {
        var parameterSplit = arg.Split('*');
        var paramaterAlbumId = parameterSplit[0][8..];
        var paramaterPhotoId = parameterSplit[1][6..];

        var successAlbumId = int.TryParse(paramaterAlbumId, out var albumId);
        var successPhotoId = int.TryParse(paramaterPhotoId, out var photoId);

        if (!successAlbumId || !successPhotoId)
        {
            _consoleManager.WriteLine($"The value {paramaterAlbumId} for AlbumId or the value for {paramaterPhotoId} is invalid.  Please review and supply a number");
            return true;
        }

        var photo = await _albumApiClient.GetPhotoByAlbumIdAndPhotoId(albumId, photoId);

        if (photo == null)
        {
            _consoleManager.WriteLine($"There is no photo that matches AlbumId: {albumId}  and PhotoId: {photoId}");
            return true;
        }

        _consoleManager.WriteLine($"Album: {photo?.AlbumId}");
        _consoleManager.WriteLine($"\tPhotoId: {photo?.Id} Title: {photo?.Title}");

        return true;
    }

    public async Task<bool> PhotosInAlbumDisplayAsync(string arg)
    {
        var paramaterValue = arg[8..];

        var success = int.TryParse(paramaterValue, out var albumId2);

        if (!success)
        {
            _consoleManager.WriteLine($"The value {paramaterValue} is invalid.  Please supply a number");
            return true;
        }

        var albumPhotos = await _albumApiClient.GetListOfPhotosByAlbumId(albumId2);

        if (albumPhotos?.Any() ?? false)
        {
            var shouldDisplayAlbumInfo = true;
            albumPhotos?.ForEach(p =>
            {
                if (shouldDisplayAlbumInfo) _consoleManager.WriteLine($"Album: {p.AlbumId}");
                shouldDisplayAlbumInfo = false;
                _consoleManager.WriteLine($"\tPhotoId: {p.Id} Title: {p.Title}");
            });
        }
        return true;
    }

    public bool DefaultDisplay()
    {
        _consoleManager.WriteLine("Unrecognized command type. Use --h for list of available commands");
        return true;
    } 
}