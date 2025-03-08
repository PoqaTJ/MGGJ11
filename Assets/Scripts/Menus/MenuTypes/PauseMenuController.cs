using Services;

namespace Menus.MenuTypes
{
    public class PauseMenuController: MenuController
    {
        public override MenuType GetMenuType() => MenuType.PauseMenu;

        protected override void OnShow()
        {
            base.OnShow();
        }

        public void Close()
        {
            ServiceLocator.Instance.MenuManager.HideTop();
            ServiceLocator.Instance.GameManager.Unpause();
        }
    }
}