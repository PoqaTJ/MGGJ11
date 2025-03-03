using System;
using Player;
using UnityEngine;

namespace Baddy
{
    public class DamageOnHit: MonoBehaviour
    {
        [SerializeField] private int _damage = 1;
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player") || col.CompareTag("Akari"))
            {
                PlayerController pc = col.GetComponent<PlayerController>();
                pc.TakeDamage(_damage, transform.position);                
            }
        }
    }
}