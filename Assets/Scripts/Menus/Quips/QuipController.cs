using System;
using System.Collections.Generic;
using Dialogs;
using Menus.MenuTypes;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menus.Quips
{
    public class QuipController: MonoBehaviour
    {
        [SerializeField] private GameObject _quipUI;
        [SerializeField] private Image _characterImage;
        [SerializeField] private TMP_Text _text;
        
        private const float _showDuration = 2f;
        private float hideTime = 0f;

        [SerializeField] private ConversationMenu _conversationMenu;

        private Queue<QuipDefinition> _pendingQuips = new();

        private bool showingQuip = false;
        
        public void Show(string text, Sprite image)
        {
            _characterImage.sprite = image;
            _text.text = text;
            _quipUI.SetActive(true);
            hideTime = Time.timeSinceLevelLoad + _showDuration;
        }

        private void Update()
        {
            if (_quipUI.activeInHierarchy && Time.timeSinceLevelLoad > hideTime)
            {
                showingQuip = false;
                _quipUI.SetActive(false);
                PollQueue();
            }
        }

        private void PollQueue()
        {
            if (_pendingQuips.Count == 0)
            {
                return;
            }
            var nextQuip = _pendingQuips.Dequeue();
            Show(nextQuip);
        }

        public void Show(QuipDefinition definition)
        {
            if (showingQuip)
            {
                _pendingQuips.Enqueue(definition);
            }
            else
            {
                Show(definition.dialog.Text, GetImage(definition.dialog.Character, definition.Expression));
                showingQuip = true;                
            }
        }
        
        private Sprite GetImage(DialogCharacter character, SpriteFace expression)
        {
            Sprite ret = null;
            int expressionInt = (int)expression;
            switch (character)
            {
                case DialogCharacter.Akari:
                    ret = ServiceLocator.Instance.MenuManager.AkariSprites[expressionInt];
                    break;
                case DialogCharacter.MagicalAkari:
                    ret = ServiceLocator.Instance.MenuManager.MagicalAkariSprites[expressionInt];
                    break;
                case DialogCharacter.Tomoya:
                    ret = ServiceLocator.Instance.MenuManager.TomoyaSprites[expressionInt];
                    break;
                case DialogCharacter.MagicalTomoya:
                    ret = ServiceLocator.Instance.MenuManager.MagicalTomoyaSprites[expressionInt];
                    break;
                case DialogCharacter.Butterfly:
                    ret = ServiceLocator.Instance.MenuManager.ButterflySprite;
                    break;
            }

            return ret;
        }
    }
}