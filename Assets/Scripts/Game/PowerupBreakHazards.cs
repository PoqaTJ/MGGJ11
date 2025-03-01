using Menus;
using Menus.MenuTypes;
using Services;

namespace Game
{
    public class PowerupBreakHazards: Collectable
    {
        void Start()
        {
            if (ServiceLocator.Instance.SaveManager.UnlockedBreakHazard)
            {
                Destroy(gameObject);
            }
        }
        
        protected override void OnContact()
        {
            var context = new PopupMenuOneButton.PopupMenuOneButtonContext();
            context.titleLocString = "dialog-powerup-break-hazards-title";
            context.bodyLocString = "dialog-powerup-break-hazards-body";
            context.buttonLocString = "dialog-powerup-break-hazards-button";
            ServiceLocator.Instance.MenuManager.Show(MenuType.PopupOneButton, context);
            
            ServiceLocator.Instance.SaveManager.UnlockedBreakHazard = true;
            base.OnContact();
        }
    }
}