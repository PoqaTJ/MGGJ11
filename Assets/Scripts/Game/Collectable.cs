using System;
using Services;
using UnityEngine;

namespace Game
{
    public class Collectable: MonoBehaviour
    {
        private bool _pickedUp = false;
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!_pickedUp && col.CompareTag("Player"))
            {
                _pickedUp = true;
                OnContact();
            }
        }

        protected virtual void OnContact()
        {
            Destroy(gameObject);
        }
    }
}