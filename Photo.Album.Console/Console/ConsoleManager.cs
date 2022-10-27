namespace Photo.Album.Console.Console
{
    public class ConsoleManager : ConsoleManagerBase
    {
        public override void Write(string value)
        {
            System.Console.Write(value);
        }
        public override void WriteLine(string value)
        {
            System.Console.WriteLine(value);
        }
    }
}