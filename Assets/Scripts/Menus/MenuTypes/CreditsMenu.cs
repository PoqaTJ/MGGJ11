using Services;
using Unity.VisualScripting;
using State = Game.State;

namespace Menus.MenuTypes
{
    public class CreditsMenu: MenuController
    {
        public override MenuType GetMenuType() => MenuType.CreditsMenu;

        public void OnBack()
        {
            ServiceLocator.Instance.MenuManager.HideTop();
            ServiceLocator.Instance.GameManager.SetState(State.MainMenu);
        }
    }
}
