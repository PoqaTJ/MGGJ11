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
                if (ServiceLocator.Instance.SaveManager.UnlockedBreakHazard)
                {
                    _collectable.Collect(CollectableType.Hazard);
                    Destroy(gameObject);
                }
                else
                {
                    PlayerController pc = col.GetComponent<PlayerController>();
                    pc.TakeDamage(_damage, transform.position);                    
                }
            }
        }
    }
}