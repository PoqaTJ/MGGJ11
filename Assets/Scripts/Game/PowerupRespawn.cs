using Menus;
using Menus.MenuTypes;
using Services;

namespace Game
{
    public class PowerupRespawn: Collectable
    {
        void Start()
        {
            if (ServiceLocator.Instance.SaveManager.UnlockedRespawn)
            {
                Destroy(gameObject);
            }
        }
        
        protected override void OnContact()
        {
            var context = new PopupMenuOneButton.PopupMenuOneButtonContext();
            context.titleLocString = "dialog-powerup-respawn-title";
            context.bodyLocString = "dialog-powerup-respawn-body";
            context.buttonLocString = "dialog-powerup-respawn-button";
            ServiceLocator.Instance.MenuManager.Show(MenuType.PopupOneButton, context);
            
            ServiceLocator.Instance.SaveManager.UnlockedRespawn = true;
            base.OnContact();
        }
    }
}