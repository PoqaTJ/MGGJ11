﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Player;
using Services;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        #region events

        public Action OnPlayerDied;
        public Action OnPlayerTakeDamage;
        public Action<PlayerController> OnPlayerSpawn;

        #endregion
        
        public State CurrentState { get; private set; }

        private PlayerController _player;

        [SerializeField] GameObject _akariNormalPrefab;
        [SerializeField] GameObject _akariTFPrefab;
        [SerializeField] GameObject _tomoyaNormalPrefab;
        [SerializeField] GameObject _tomoyaTFPrefab;
        [SerializeField] PlayerSpawner _defaultSpawner = null;
        private Dictionary<string, PlayerSpawner> _spawners = new();
        
        private void Start()
        {
            OnPlayerDied += OnPlayerDeath;

            StartCoroutine(LoadGameplay(SceneManager.GetActiveScene().name != "LevelOne"));
        }

        private void PlayerSpawned(PlayerController playerController)
        {
            _player = playerController;
        }

        private void OnPlayerDeath()
        {
            StartCoroutine(SpawnPlayer(true));
        }

        private IEnumerator SpawnPlayer(bool transformed)
        {
            PlayerSpawner spawner = ChooseSpawner();
            if (_player != null)
            {
                Destroy(_player.gameObject);                
            }

            GameObject prefab = transformed ? _tomoyaTFPrefab : _tomoyaNormalPrefab;
            GameObject playerObject = Instantiate(prefab);
            _player = playerObject.GetComponent<PlayerController>();
            var input = playerObject.GetComponent<PlayerInputController>();
            input.enabled = false;
            _player.transform.position = new Vector3(spawner.transform.position.x, spawner.transform.position.y + 1,
                _player.transform.position.z);
            FocusCameraOn(_player.transform);
            yield return new WaitForSeconds(1f);
            _player.Reset();
            input.enabled = true;
            OnPlayerSpawn?.Invoke(_player);
        }

        private PlayerSpawner ChooseSpawner()
        {
            string spawnerID = ServiceLocator.Instance.SaveManager.Spawner;
            if (spawnerID != null && _spawners.ContainsKey(spawnerID))
            {
                return _spawners[spawnerID];
            }

            return _defaultSpawner;
        }

        public void FocusCameraOn(Transform t)
        {
            GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>().Follow = t;
        }
        
        public void SetState(State state)
        {
            if (CurrentState == state)
            {
                Debug.LogError($"Tried to set state to {state}, but that was already the current state.");
                return;
            }

            StartCoroutine(SetStateRoutine(state));
        }

        private IEnumerator SetStateRoutine(State state)
        {
            Debug.Log($"Setting state to {state}.");
            
            _spawners.Clear();
            switch (state)
            {
                case State.MainMenu:
                    yield return SceneManager.LoadSceneAsync("Main");
                    break;
                case State.Credits:
                    yield return SceneManager.LoadSceneAsync("Credits");
                    break;
                case State.Intro:
                    yield return SceneManager.LoadSceneAsync("Intro");
                    break;
                case State.Gameplay:
                    int level = ServiceLocator.Instance.SaveManager.Level;
                    if (level == 0)
                    {
                        yield return SceneManager.LoadSceneAsync("LevelOne");
                        yield return LoadGameplay(false);
                    }
                    else
                    {
                        yield return SceneManager.LoadSceneAsync("Gameplay");
                        yield return LoadGameplay(true);
                    }
                    break;
            }
        }
        

        private IEnumerator LoadGameplay(bool transformed)
        {
            _player = null;
            GatherSpawners();
            if (_defaultSpawner != null)
            {
                yield return SpawnPlayer(transformed);
            }
        }
        
        public void UnlockDoubleJump()
        {
            ServiceLocator.Instance.SaveManager.UnlockedDoubleJump = true;
        }
        
        public void UnlockTripleJump()
        {
            ServiceLocator.Instance.SaveManager.UnlockedTripleJump = true;
        }

        public void UnlockWallJump()
        {
            ServiceLocator.Instance.SaveManager.UnlockedWallJump = true;
        }
        
        public void ActivateSpawner(PlayerSpawner playerSpawner)
        {
            ServiceLocator.Instance.SaveManager.Spawner = playerSpawner.ID;
        }

        public void RegisterPlayer(PlayerController playerController)
        {
            _player = playerController;
        }

        private IEnumerator PlayerDiesRoutine()
        {
            yield return null;
        }

        public void GatherSpawners()
        {
            PlayerSpawner[] spawners = GameObject.FindObjectsOfType<PlayerSpawner>();
            _defaultSpawner = null;
            foreach (var s in spawners)
            {
                _spawners[s.ID] = s;
                if (s.Default)
                {
                    if (_defaultSpawner)
                    {
                        Debug.LogError("Multiple default spawners. Using the first.");
                    }
                    else
                    {
                        _defaultSpawner = s;
                    }
                }
            }

            if (_defaultSpawner == null && _spawners.Count > 0)
            {
                Debug.LogError("No default spawners found. Using the first one.");
                _defaultSpawner = _spawners.ToArray()[0].Value;
            }
        }
    }

    public enum State
    {
        MainMenu,
        Gameplay,
        Intro,
        Credits
    }
}