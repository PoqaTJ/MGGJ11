using Menus;
using Menus.MenuTypes;
using Services;

namespace Game
{
    public class PowerupDoubleJump: Collectable
    {
        void Start()
        {
            if (ServiceLocator.Instance.SaveManager.UnlockedDoubleJump)
            {
                Destroy(this.gameObject);
            }
        }
        
        protected override void OnContact()
        {
            var context = new PopupMenuOneButton.PopupMenuOneButtonContext();
            context.titleLocString = "dialog-powerup-double-jump-title";
            context.bodyLocString = "dialog-powerup-double-jump-body";
            context.buttonLocString = "dialog-powerup-double-jump-button";
            ServiceLocator.Instance.MenuManager.Show(MenuType.PopupOneButton, context);
            
            ServiceLocator.Instance.GameManager.UnlockDoubleJump();

            base.OnContact();
        }
    }
}