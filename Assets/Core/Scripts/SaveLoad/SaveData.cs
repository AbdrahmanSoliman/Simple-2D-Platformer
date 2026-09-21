using System;

namespace Platformer.SaveLoad
{
    [Serializable]
    public class SaveData
    {
        public int playerHP;
        public float playerX;
        public float playerY;
        public int coinCount;
        public int[] collectedCoinIds;
        public int[] defeatedEnemyIds;
        public int lastCheckpointId;
        public float checkpointX;
        public float checkpointY;
    }
}
