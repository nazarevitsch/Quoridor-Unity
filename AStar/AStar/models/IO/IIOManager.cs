namespace AStar.models.IO
{
    public interface IIoManager
    {
        public string Read();
        public void Write(string msg);
    }
}