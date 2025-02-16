﻿using System;
using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        #region events

        public Action OnDoublejumpUnlocked;
        public Action OnWalljumpUnlocked;
        public Action OnPlayerDied;
        public Action OnPlayerTakeDamage;
        public Action OnPlayerSpawn;

        #endregion
        
        public State CurrentState { get; private set; }

        private PlayerController _player;
        private PlayerSpawner _spawner;

        private void Start()
        {
            OnPlayerDied += OnPlayerDeath;
        }

        private void PlayerSpawned(PlayerController playerController)
        {
            _player = playerController;
        }

        private void OnPlayerDeath()
        {
            if (_spawner == null)
            {
                Debug.LogError("Player died but there is no active spawner so they will not respawn.");
                return;
            }

            StartCoroutine(SpawnPlayer());
        }

        private IEnumerator SpawnPlayer()
        {
            yield return new WaitForSeconds(0.5f);
            _player.transform.position = new Vector3(_spawner.transform.position.x, _spawner.transform.position.y,
                _player.transform.position.z);
            _player.Reset();
            _player.gameObject.SetActive(true);
        }

        public void SetState(State state)
        {
            if (CurrentState == state)
            {
                Debug.LogError($"Tried to set state to {state}, but that was already the current state.");
                return;
            }

            Debug.Log($"Setting state to {state}.");
            switch (state)
            {
                case State.MainMenu:
                    SceneManager.LoadSceneAsync("Main");
                    break;
                case State.Gameplay:
                    SceneManager.LoadSceneAsync("Gameplay");
                    break;
                case State.Debug:
                    SceneManager.LoadSceneAsync("Debug");
                    break;
            }
        }

        public void UnlockDoubleJump()
        {
            OnDoublejumpUnlocked?.Invoke();
        }

        public void UnlockWallJump()
        {
            OnWalljumpUnlocked?.Invoke();
        }

        public void ActivateSpawner(PlayerSpawner playerSpawner)
        {
            _spawner = playerSpawner;
        }

        public void RegisterPlayer(PlayerController playerController)
        {
            _player = playerController;
        }
    }

    public enum State
    {
        MainMenu,
        Gameplay,
        Debug
    }
}