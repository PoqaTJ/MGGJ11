using System;
using System.Collections.Generic;
using Services;
using UnityEditor;
using UnityEngine;

namespace Game
{
    public class SaveManager: MonoBehaviour
    {
        private SaveGame _save;
        private static readonly string _playerPrefsKey = "save_game_rc_2";
        private HashSet<string> _collected = new ();

        private void Awake()
        {
            if (!PlayerPrefs.HasKey(_playerPrefsKey))
            {
                _save = new SaveGame();
            }
            else
            {
                try
                {
                    string stringSave = PlayerPrefs.GetString(_playerPrefsKey);
                    _save = JsonUtility.FromJson<SaveGame>(stringSave);
#if UNITY_EDITOR
                    if (_resetOnPlay)
                    {
                        _save.QuipsCollected.Clear();
                        _save.McGuffinsCollected.Clear();
                        _save.HazardsBroken.Clear();
                    }
#endif
                    UnpackCollections();
                }
                catch (Exception e)
                {
                    Debug.LogError($"Unable to parse save. {e}");
                    _save = new SaveGame();
                }
            }
        }

        private void UnpackCollections()
        {
            _collected.Clear();
            foreach (var id in _save.QuipsCollected)
            {
                _collected.Add(id);
            }
            foreach (var id in _save.McGuffinsCollected)
            {
                _collected.Add(id);
            }
            foreach (var id in _save.HazardsBroken)
            {
                _collected.Add(id);
            }
        }

        public bool WatchedIntro
        {
            get => _save.SeenIntro;
            set
            {
                _save.SeenIntro = value;
                Save();
            }
        }

        public string Spawner
        {
            get => _save.Spawner;
            set
            {
                _save.Spawner = value;
                Save();
            }
        }

        public bool UnlockedWallJump
        {
            get => _save.UnlockedWallJump;
            set
            {
                _save.UnlockedWallJump = value;
                Save();
            }
        }
        
        public bool UnlockedDoubleJump
        {
            get => _save.UnlockedDoubleJump;
            set
            {
                _save.UnlockedDoubleJump = value;
                Save();
            }
        }
        
        public bool UnlockedTripleJump
        {
            get => _save.UnlockedTripleJump;
            set
            {
                _save.UnlockedTripleJump = value;
                Save();
            }
        }
        
        public int Level
        {
            get => _save.Level;
            set
            {
                _save.Level = value;
                Save();
            }
        }

        public bool UnlockedBreakHazard
        {
            get => _save.UnlockedBreakHazard;
            set
            {
                _save.UnlockedBreakHazard = value;
                Save();
            }
        }

        public bool UnlockedRespawn
        {
            get => _save.UnlockedRespawn;
            set
            {
                _save.UnlockedRespawn = value;
                Save();
            }
        }  
        
        public void CollectHazard(string id)
        {
            Debug.Log("Collected " + id);

            if (HasBeenCollected(id))
            {
                Debug.LogError("Attempting to collect collectable that has already been collected.");
                return;
            }

            _collected.Add(id);
            _save.HazardsBroken.Add(id);
            Save();
        }
        
        public void CollectQuip(string id)
        {
            Debug.Log("Collected " + id);

            if (HasBeenCollected(id))
            {
                Debug.LogError("Attempting to collect collectable that has already been collected.");
                return;
            }

            _collected.Add(id);
            _save.QuipsCollected.Add(id);
            Save();
        }
        
        public void CollectMcGuffin(string id)
        {
            Debug.Log("Collected " + id);

            if (HasBeenCollected(id))
            {
                Debug.LogError("Attempting to collect collectable that has already been collected.");
                return;
            }

            _collected.Add(id);
            _save.McGuffinsCollected.Add(id);
            Save();
        }
        
        public bool HasBeenCollected(string id)
        {
            return _collected.Contains(id);
        }
        
        
        public bool FoundAkari
        {
            get => _save.FoundAkari;
            set
            {
                _save.FoundAkari = value;
                Save();
            }
        }

        public int McGuffinCount => _save.McGuffinsCollected.Count;

        public void Save()
        {
            PlayerPrefs.SetString(_playerPrefsKey, JsonUtility.ToJson(_save));
        }

        public void Destroy()
        {
            _save = new SaveGame();
            Save();
        }

#if UNITY_EDITOR
        [MenuItem("Save/Delete Save (only when stopped)", false, 100)]
        private static void ClearSave()
        {
            PlayerPrefs.DeleteKey(_playerPrefsKey);
        }

        [MenuItem("Save/Delete Save (only when stopped)", true)]
        private static bool ClearSaveValidate()
        {
            return !EditorApplication.isPlaying && PlayerPrefs.HasKey(_playerPrefsKey);
        }

        [MenuItem("Save/EnableWallJump", false, 1)]
        private static void DebugEnableWallJump()
        {
            ServiceLocator.Instance.SaveManager.UnlockedWallJump = !ServiceLocator.Instance.SaveManager.UnlockedWallJump;
        }
        
        [MenuItem("Save/EnableWallJump", true)]
        private static bool DebugEnableWallJumpValidate()
        {
            if (EditorApplication.isPlaying)
            {
                Menu.SetChecked("Save/EnableWallJump", ServiceLocator.Instance.SaveManager.UnlockedWallJump);
                return true;
            }
            return false;
        }
        
        [MenuItem("Save/EnableDoubleJump", false, 2)]
        private static void DebugEnableDoubleJump()
        {
            ServiceLocator.Instance.SaveManager.UnlockedDoubleJump = !ServiceLocator.Instance.SaveManager.UnlockedDoubleJump;
        }
        
        [MenuItem("Save/EnableDoubleJump", true)]
        private static bool DebugEnableDoubleJumpValidate()
        {
            if (EditorApplication.isPlaying)
            {
                Menu.SetChecked("Save/EnableDoubleJump", ServiceLocator.Instance.SaveManager.UnlockedDoubleJump);
                return true;
            }
            return false;
        }
        
        [MenuItem("Save/SavedAkari", false, 10)]
        private static void DebugEnableSavedAkari()
        {
            ServiceLocator.Instance.SaveManager.FoundAkari = !ServiceLocator.Instance.SaveManager.FoundAkari;
        }
        
        [MenuItem("Save/SavedAkari", true)]
        private static bool DebugEnableSavedAkariValidate()
        {
            if (EditorApplication.isPlaying)
            {
                Menu.SetChecked("Save/SavedAkari", ServiceLocator.Instance.SaveManager.FoundAkari);
                return true;
            }
            return false;
        }
        
        [MenuItem("Save/EnableTripleJump", false, 3)]
        private static void DebugEnableTripleJump()
        {
            ServiceLocator.Instance.SaveManager.UnlockedTripleJump = !ServiceLocator.Instance.SaveManager.UnlockedTripleJump;
        }
        
        [MenuItem("Save/EnableTripleJump", true)]
        private static bool DebugEnableTripleJumpValidate()
        {
            if (EditorApplication.isPlaying)
            {
                Menu.SetChecked("Save/EnableTripleJump", ServiceLocator.Instance.SaveManager.UnlockedTripleJump);
                return true;
            }
            return false;
        }
        
        [MenuItem("Save/EnableRespawn", false, 8)]
        private static void DebugEnableRespawn()
        {
            ServiceLocator.Instance.SaveManager.UnlockedRespawn = !ServiceLocator.Instance.SaveManager.UnlockedRespawn;
        }
        
        [MenuItem("Save/EnableRespawn", true)]
        private static bool DebugEnableRespawnValidate()
        {
            if (EditorApplication.isPlaying)
            {
                Menu.SetChecked("Save/EnableRespawn", ServiceLocator.Instance.SaveManager.UnlockedRespawn);
                return true;
            }
            return false;
        }
        
        [MenuItem("Save/EnableBreakHazard", false, 4)]
        private static void DebugEnableBreakHazard()
        {
            ServiceLocator.Instance.SaveManager.UnlockedBreakHazard = !ServiceLocator.Instance.SaveManager.UnlockedBreakHazard;
        }
        
        [MenuItem("Save/EnableBreakHazard", true)]
        private static bool DebugEnableBreakHazardValidate()
        {
            if (EditorApplication.isPlaying)
            {
                Menu.SetChecked("Save/EnableBreakHazard", ServiceLocator.Instance.SaveManager.UnlockedBreakHazard);
                return true;
            }
            return false;
        }
        
        private static bool _resetOnPlay {
            get => EditorPrefs.GetBool("EditorResetOnPlay");
            set => EditorPrefs.SetBool("EditorResetOnPlay", value);
        }

        [MenuItem("Save/ResetCollectablesOnPlay", false, 11)]
        private static void DebugResetCollectables()
        {
            _resetOnPlay = !_resetOnPlay;
        }
        
        [MenuItem("Save/ResetCollectablesOnPlay", true)]
        private static bool DebugResetCollectablesValidate()
        {
            Menu.SetChecked("Save/ResetCollectablesOnPlay", _resetOnPlay);
            return true;
        }
        
        private static bool _startNearAkari {
            get => EditorPrefs.GetBool("EditorStartNearAkari");
            set => EditorPrefs.SetBool("EditorStartNearAkari", value);
        }


        [MenuItem("Save/StartNearAkari", false, 12)]
        private static void DebugStartNearAkari()
        {
            _startNearAkari = !_startNearAkari;
        }
        
        [MenuItem("Save/StartNearAkari", true)]
        private static bool DebugStartNearAkariValidate()
        {
            Menu.SetChecked("Save/StartNearAkari", _startNearAkari);
            return true;
        }
#endif
    }
}