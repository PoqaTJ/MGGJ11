using System;
using Player;
using Unity.VisualScripting;
using UnityEngine;

namespace Game
{
    public class HealPlayer: MonoBehaviour
    {
        [SerializeField] private int _health = 1;
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player"))
            {
                var playerController = col.GetComponent<PlayerController>();
                if (playerController)
                {
                    playerController.Heal(_health);                    
                }
            }
        }
    }
}