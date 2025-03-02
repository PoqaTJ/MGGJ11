using System;
using Services;
using UnityEngine;
using UnityEngine.Events;

namespace Dialogs
{
    public class ShowDialogOnTrigger: MonoBehaviour
    {
        [SerializeField] private ConversationDefinition _conversation;
        [SerializeField] private UnityEvent _onDoneEvent;

        private bool _showing = false;
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (_showing)
            {
                return;
            }

            if (col.CompareTag("Player"))
            {
                ShowConversation();
            }
        }

        private void ShowConversation()
        {
            _showing = true;

            ServiceLocator.Instance.DialogManager.StartConversation(_conversation, () =>
            {
                _onDoneEvent?.Invoke();                
            });
            Destroy(gameObject);
        }
    }
}