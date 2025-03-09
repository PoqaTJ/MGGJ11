using Game;
using Services;
using TMPro;
using UnityEngine;

namespace Menus.MenuTypes
{
    public class PauseMenuController: MenuController
    {
        public override MenuType GetMenuType() => MenuType.PauseMenu;
        
        [SerializeField] private TMP_Text _doubleJumpText;
        [SerializeField] private TMP_Text _tripleJumpText;
        [SerializeField] private TMP_Text _wallJumpText;
        [SerializeField] private TMP_Text _returnText;

        protected override void OnShow()
        {
            base.OnShow();
        }

        public void ClickMainMenu()
        {
            var context = new PopupMenuTwoButton.PopupMenuTwoButtonContext();

            context.titleLocString = "dialog-return-mm-title";
            context.bodyLocString = "dialog-return-mm-body";
            context.buttonLeftLocString = "dialog-return-mm-left";
            context.buttonRightLocString = "dialog-return-mm-right";
            context.OnButtonRightAction = () =>
            {
                ServiceLocator.Instance.MenuManager.HideTop();
                ServiceLocator.Instance.GameManager.SetState(State.MainMenu);
            };

            ServiceLocator.Instance.MenuManager.Show(MenuType.PopupTwoButtons, context);
        }

        public void ClickSettings()
        {
            ServiceLocator.Instance.MenuManager.Show(MenuType.SettingsMenu, null);
        }

        public void Close()
        {
            ServiceLocator.Instance.MenuManager.HideTop();
        }

        protected override void OnHide()
        {
            ServiceLocator.Instance.GameManager.Unpause();
            base.OnHide();
        }
    }
}