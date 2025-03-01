using UnityEngine;

namespace Game
{
    public class McGuffin: Collectable
    {
        [SerializeField] private DestroyOnStartIfCollected _collected;

        protected override void OnContact()
        {
            _collected.Collect();
            base.OnContact();
        }
    }
}