using Services;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class DestroyOnStartIfCollected: MonoBehaviour
    {
        [SerializeField] private string _prefix;

        void Start()
        {
            if (ServiceLocator.Instance.SaveManager.HasBeenCollected(ID))
            {
                Destroy(gameObject);
            }
        }

        public void Collect()
        {
            ServiceLocator.Instance.SaveManager.Collect(ID);
        }
        
        private string ID => $"{SceneManager.GetActiveScene().name}-{_prefix}-{(int)transform.position.x},{(int)transform.position.y}";
    }
}