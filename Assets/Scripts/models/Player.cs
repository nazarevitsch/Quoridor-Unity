using UnityEngine;

namespace models
{
    public class Player
    {
        public string Name { get; }
        public int StartY;
        public int FinishY;
        public int CurrentX;
        public int CurrentY;
        public int BlocksCount;
        
        public GameObject BlocksCountField;
        public GameObject BlocksUseButton;

        public Player(int startY, int finishY, int currentX, int currentY, int blocksCount, string name)
        {
            CurrentX = currentX;
            CurrentY = currentY;
            FinishY = finishY;
            StartY = startY;
            BlocksCount = blocksCount;
            Name = name;
        }
    }
}