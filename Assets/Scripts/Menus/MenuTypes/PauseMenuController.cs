using Dialogs;
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
        [SerializeField] private TMP_Text _breakHazardText;

        [SerializeField] private GameObject _doubleJumpInfoButton;
        [SerializeField] private GameObject _tripleJumpInfoButton;
        [SerializeField] private GameObject _wallJumpInfoButton;
        [SerializeField] private GameObject _respawnInfoButton;
        [SerializeField] private GameObject _breakHazardInfoButton;
        protected override void OnShow()
        {
            base.OnShow();

            if (!ServiceLocator.Instance.SaveManager.UnlockedWallJump)
            {
                _wallJumpText.text = "Locked";
                _wallJumpInfoButton.SetActive(false);
            }
            if (!ServiceLocator.Instance.SaveManager.UnlockedRespawn)
            {
                _returnText.text = "Locked";
                _respawnInfoButton.SetActive(false);
            }
            if (!ServiceLocator.Instance.SaveManager.UnlockedDoubleJump)
            {
                _doubleJumpText.text = "Locked";
                _doubleJumpInfoButton.SetActive(false);
            }
            if (!ServiceLocator.Instance.SaveManager.UnlockedTripleJump)
            {
                _tripleJumpText.text = "Locked";
                _tripleJumpInfoButton.SetActive(false);
            }
            if (!ServiceLocator.Instance.SaveManager.UnlockedBreakHazard)
            {
                _breakHazardText.text = "Locked";
                _breakHazardInfoButton.SetActive(false);
            }
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

        private void ShowDialogOnTrigger(string title, string body)
        {
            var context = new PopupMenuOneButton.PopupMenuOneButtonContext();
            context.UseLocSystem = false;
            context.titleLocString = title;
            context.bodyLocString = body;
            context.buttonLocString = "Back";
            ServiceLocator.Instance.MenuManager.Show(MenuType.PopupOneButton, context);
        }
        
        public void ClickRespawnInfo()
        {
            ShowDialogOnTrigger("Respawn", "Press ENTER to return to the last spawner isntantly!");
        }
        
        public void ClickDoubleJumpInfo()
        {
            ShowDialogOnTrigger("Double Jump", "You can jump again in midair!");
        }

        public void ClickTripleJumpInfo()
        {
            ShowDialogOnTrigger("Triple Jump", "You can jump again in midair, again!!");
        }

        public void ClickWallJumpInfo()
        {
            ShowDialogOnTrigger("Wall Jump", "You can boost off of walls that you're facing!");
        }

        public void ClickBreakHazardInfo()
        {
            ShowDialogOnTrigger("Smash Hazards", "Hit purple hazards from below to break them!");
        }

        public void ClickPerfectEndingInfo()
        {
            ShowDialogOnTrigger("McGuffins", "Collect all the green orbs to unlock the perfect ending!");

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