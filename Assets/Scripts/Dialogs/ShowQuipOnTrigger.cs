using System;
using Game;
using Services;
using UnityEngine;

namespace Dialogs
{
    public class ShowQuipOnTrigger: MonoBehaviour
    {
        [SerializeField] private QuipDefinition _quip;
        [SerializeField] private DestroyOnStartIfCollected _collected;
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player"))
            {
                _collected.Collect();
                ServiceLocator.Instance.DialogManager.ShowQuip(_quip);
                Destroy(gameObject);
            }
        }
    }
}