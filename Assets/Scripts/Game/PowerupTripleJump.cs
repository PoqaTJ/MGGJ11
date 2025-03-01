using Menus;
using Menus.MenuTypes;
using Services;

namespace Game
{
    public class PowerupTripleJump: Collectable
    {
        void Start()
        {
            if (ServiceLocator.Instance.SaveManager.UnlockedTripleJump)
            {
                Destroy(this.gameObject);
            }
        }
        
        protected override void OnContact()
        {
            var context = new PopupMenuOneButton.PopupMenuOneButtonContext();
            context.titleLocString = "dialog-powerup-triple-jump-title";
            context.bodyLocString = "dialog-powerup-triple-jump-body";
            context.buttonLocString = "dialog-powerup-triple-jump-button";
            
            ServiceLocator.Instance.GameManager.UnlockTripleJump();
            ServiceLocator.Instance.MenuManager.Show(MenuType.PopupOneButton, context);
            base.OnContact();
        }
    }
}