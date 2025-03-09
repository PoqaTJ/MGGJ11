using Services;
using UnityEngine;

namespace Game
{
    public class McGuffin: Collectable
    {
        [SerializeField] private DestroyOnStartIfCollected _collected;

        protected override void OnContact()
        {
            
            _collected.Collect(CollectableType.McGuffin);
            ServiceLocator.Instance.GameManager.FindMecGuffin();
            ServiceLocator.Instance.AudioManager.PlayCollectSFX();
            base.OnContact();
        }
    }
}