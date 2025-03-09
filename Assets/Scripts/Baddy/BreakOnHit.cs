using System;
using Game;
using Player;
using Services;
using UnityEngine;

namespace Baddy
{
    public class BreakOnHit : MonoBehaviour
    {
        [SerializeField] private int _damage = 1;
        [SerializeField] private DestroyOnStartIfCollected _collectable;
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player") || col.CompareTag("Akari"))
            {
                PlayerController pc = col.GetComponent<PlayerController>();

                if (ServiceLocator.Instance.SaveManager.UnlockedBreakHazard &&
                    col == pc.BreakHazardCollider)
                {
                    ServiceLocator.Instance.AudioManager.PlayJumpSFX();
                    _collectable.Collect(CollectableType.Hazard);
                    Destroy(gameObject);
                }
                else
                {
                    pc.TakeDamage(_damage, transform.position);                    
                }
            }
        }
    }
}