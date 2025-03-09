using System;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Menus.MenuTypes
{
    public class PopupMenuOneButton : MenuController
    {
        [SerializeField] private TMP_Text titleString;
        [SerializeField] private TMP_Text bodyString;
        [SerializeField] private TMP_Text buttonString;

        
        public override MenuType GetMenuType() => MenuType.PopupOneButton;
        
        private PopupMenuOneButtonContext context;

        protected override void OnSetup(DialogContext context)
        {
            this.context = context as PopupMenuOneButtonContext;

            if (this.context.UseLocSystem)
            {
                titleString.text = LocalizationSettings.StringDatabase.GetLocalizedString("Launch", this.context.titleLocString);
                bodyString.text = LocalizationSettings.StringDatabase.GetLocalizedString("Launch", this.context.bodyLocString);
                buttonString.text = LocalizationSettings.StringDatabase.GetLocalizedString("Launch", this.context.buttonLocString);;               
            }
            else
            {
                titleString.text = this.context.titleLocString;
                bodyString.text = this.context.bodyLocString;
                buttonString.text = this.context.buttonLocString;
            }
        }

        protected override void OnShow()
        {
            base.OnShow();
        }

        public void OnClick()
        {
            ServiceLocator.Instance.MenuManager.HideTop();
        }
        
        protected override void OnHide()
        {
            this.context.OnCloseAction?.Invoke();
            base.OnHide();
        }

        public class PopupMenuOneButtonContext : Menus.MenuController.DialogContext
        {
            public bool UseLocSystem = true;
            public Action OnCloseAction;
            public string titleLocString;
            public string bodyLocString;
            public string buttonLocString;
        }
    }
}