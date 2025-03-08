using Audio;
using Dialogs;
using Effects;
using Game;
using Menus;
using UnityEngine;

namespace Services
{
    public class ServiceLocator : MonoBehaviour
    {
#region singleton
        private static readonly string _serviceLocatorName = "ServiceLocator";
        public static ServiceLocator Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.Log("Looking for ServiceLocator object in current scenes.");
                    var go = GameObject.Find(_serviceLocatorName);
                    _instance = go?.GetComponent<ServiceLocator>();
                    if (_instance == null)
                    {
                        Debug.Log("Loading ServiceLocator from Resources.");
                        var gobj = Resources.Load(_serviceLocatorName) as GameObject;
                        _instance = Instantiate(gobj).GetComponent<ServiceLocator>();
                    }
                }
                return _instance;
            }
        }
        
        private static ServiceLocator _instance;
#endregion

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

// List services here
        public GameManager GameManager;
        public MenuManager MenuManager;
        public DialogManager DialogManager;
        public SaveManager SaveManager;
        public AudioManager AudioManager;
        public ParticleManager ParticleManager;
    }
}