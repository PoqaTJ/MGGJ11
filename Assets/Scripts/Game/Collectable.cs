using System;
using Services;
using UnityEngine;

namespace Game
{
    public class Collectable: MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.tag == "Player")
            {
                OnContact();
            }
        }

        protected virtual void OnContact()
        {
            Destroy(gameObject);
        }
    }
}