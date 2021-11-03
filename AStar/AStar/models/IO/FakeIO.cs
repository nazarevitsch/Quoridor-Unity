using System;
using System.Collections.Generic;
using System.Threading;

namespace AStar.models.IO
{
    public class FakeIo : IIoManager
    {
        private Queue<string> FakeText { get; set; } = new();
        public string Read()
        {
            var text = FakeText.Dequeue();
            if (text == "STOP")
            {
                throw new ThreadInterruptedException();
            }

            return text;
        }

        public void Write(string msg)
        {
            if (msg.StartsWith("//"))
            {
                Console.WriteLine(msg);
                return;
            }
            FakeText.Enqueue(msg);
        }
    }
}