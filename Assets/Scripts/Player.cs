using UnityEngine;

namespace DefaultNamespace
{
    public class Player
    {
        public GameObject player;
        public int StartY;
        public int FinishY;
        public int CurrentX;
        public int CurrentY;
        public int BlocksCount;
        
        public GameObject BlocksCountField;
        public GameObject BlocksUseButton;

        public Player(GameObject player,int startY, int finishY, int currentX, int currentY, int blocksCount)
        {
            this.player = player;
            this.CurrentX = currentX;
            this.CurrentY = currentY;
            this.FinishY = finishY;
            this.StartY = startY;
            this.BlocksCount = blocksCount;
        }
    }
}