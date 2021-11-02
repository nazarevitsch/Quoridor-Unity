using System.IO;

namespace AStar.models
{
    public class SimpleLogger
    {
        public static void LogToFile(string data)
        {
            File.AppendAllText(Path.GetFullPath("./inputLog.log"), data);
        }
    }
}