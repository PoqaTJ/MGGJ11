using System;
using System.Collections;
using System.Collections.Generic;

namespace Game
{
    [Serializable]
    public class SaveGame
    {
        public bool SeenIntro;
        public string Spawner;
        public int Level;
        public bool UnlockedWallJump;
        public bool UnlockedDoubleJump;
        public bool UnlockedTripleJump;
        public bool UnlockedBreakHazard;
        public List<string> Collected = new List<string>();
    }
}
