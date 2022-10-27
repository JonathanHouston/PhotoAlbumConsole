namespace Photo.Album.Console.Console
{
    public abstract class ConsoleManagerBase : IConsoleManager
    {
        public abstract void Write(string value);
        public abstract void WriteLine(string value);
    }
}