using System;
using Dialogs;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace Menus.MenuTypes
{
    public class ConversationMenu: MenuController
    {
        public override MenuType GetMenuType() => MenuType.ConversationMenu;
        
        [SerializeField] private Image _leftImage;
        [SerializeField] private Image _rightImage;
        [SerializeField] private TMP_Text _leftName;
        [SerializeField] private TMP_Text _rightName;
        [SerializeField] private TMP_Text dialogText;

        private ConversationMenuContext _context;
        
        private int _index = 0;
        
        protected override void OnSetup(DialogContext context)
        {
            _context = context as ConversationMenuContext;
            _leftImage.enabled = false;
            _rightImage.enabled = false;
            _leftName.enabled = false;
            _rightName.enabled = false;
            ShowDialog();
        }

        private void ShowDialog()
        {
            DialogDefinition dialog = _context.Conversation.Dialogs[_index];
            DialogCharacter character = dialog.Character;
            DialogSide side = dialog.Side;

            switch (side)
            {
                case DialogSide.Left:
                    _leftImage.enabled = true;
                    _leftName.enabled = true;
                    _leftImage.color = new Color(1f, 1f, 1f, 1f);
                    _rightImage.color = new Color(1f, 1f, 1f, 0.5f);
                    _leftImage.sprite = GetImage(character, dialog.Expression);
                    _leftName.text = GetName(character);
                    break;
                case DialogSide.Right:
                    _rightImage.enabled = true;
                    _rightName.enabled = true;
                    _rightImage.color = new Color(1f, 1f, 1f, 1f);
                    _leftImage.color = new Color(1f, 1f, 1f, 0.5f);
                    _rightImage.sprite = GetImage(character, dialog.Expression);
                    _rightName.text = GetName(character);
                    break;
            }

            dialogText.text = dialog.Text;
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

        private string GetName(DialogCharacter character)
        {
            switch (character)
            {
                case DialogCharacter.Akari:
                case DialogCharacter.MagicalAkari:
                    return "Akari";
                    break;
                case DialogCharacter.Tomoya:
                case DialogCharacter.MagicalTomoya:
                    return "Tomoya";
                    break;
                case DialogCharacter.Butterfly:
                    return "Prism";
            }

            return "";
        }

        public void OnProgress()
        {
            if (++_index < _context.Conversation.Dialogs.Count)
            {
                ShowDialog();
            }
            else
            {
                ServiceLocator.Instance.MenuManager.HideTop();
                _context.OnFinish?.Invoke();
            }
        }
    }

    public class ConversationMenuContext : MenuController.DialogContext
    {
        public Action OnFinish;
        public ConversationDefinition Conversation;
    }

    public enum SpriteFace
    {
        Normal = 0,
        SideEye,
        WideEyed,
        PikachuFace,
        Crying,
        Happy,
        Glad,
        Mischievous
    }
}