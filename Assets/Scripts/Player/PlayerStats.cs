using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "Stats", menuName = "ScriptableObjects/PlayerStats", order = 1)]
    public class PlayerStats: ScriptableObject
    {
        // stats - move to scriptableobject for fast swap between power ups?
        public float Acceleration = 3f;
        public float Deceleration = 5f;
        public float AirAcceleration = 3f;
        public float AirDeceleration = 5f;
        public float WalledMaxSpeed = 3f;
        public float MaxSpeed = 10f;

        // jump
        public float JumpVelocity = 10f;
        public float JumpShort = 5f;
        public float CoyoteDuration = 0.2f;
        
        // wall jump
        public float WallJumpVelocityX = 2.5f;
        public float WallJumpVelocityY = 8f;
    }
}