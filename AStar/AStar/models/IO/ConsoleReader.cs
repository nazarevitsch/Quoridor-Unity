using System;

namespace AStar.models.IO
{
    public class ConsoleReader : IIoManager
    {
        public string Read()
        {
            return Console.ReadLine();
        }

        public void Write(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}