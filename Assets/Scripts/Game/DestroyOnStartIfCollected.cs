using Services;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class DestroyOnStartIfCollected: MonoBehaviour
    {
        [SerializeField] private string _prefix;
        
        private bool _collected = false;
        
        void Start()
        {
            if (ServiceLocator.Instance.SaveManager.HasBeenCollected(ID))
            {
                Destroy(gameObject);
            }
        }

        public void Collect(CollectableType type)
        {
            if (_collected)
            {
                return;
            }

            _collected = true;
            switch (type)
            {
                case CollectableType.Quip:
                    ServiceLocator.Instance.SaveManager.CollectQuip(ID);
                    break;
                case CollectableType.Hazard:
                    ServiceLocator.Instance.SaveManager.CollectHazard(ID);
                    break;
                case CollectableType.McGuffin:
                    ServiceLocator.Instance.SaveManager.CollectMcGuffin(ID);
                    break;
            }
        }
        
        private string ID => $"{SceneManager.GetActiveScene().name}-{_prefix}-{(int)transform.position.x},{(int)transform.position.y}";
    }

    public enum CollectableType
    {
        Quip,
        Hazard,
        McGuffin
    }
}