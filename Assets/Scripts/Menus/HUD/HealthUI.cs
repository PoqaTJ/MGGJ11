using System;
using System.Collections.Generic;
using Player;
using Services;
using UnityEngine;

namespace Menus.HUD
{
    public class HealthUI: MonoBehaviour
    {
        [SerializeField] private List<GameObject> _hearts = new();

        private void Start()
        {
            RefreshUI();
        }

        private void OnEnable()
        {
            ServiceLocator.Instance.GameManager.OnPlayerHeal += RefreshUI;
            ServiceLocator.Instance.GameManager.OnPlayerSpawn += OnPlayerSpawn;
            ServiceLocator.Instance.GameManager.OnPlayerTakeDamage += RefreshUI;
        }

        private void OnDisable()
        {
            ServiceLocator.Instance.GameManager.OnPlayerHeal -= RefreshUI;
            ServiceLocator.Instance.GameManager.OnPlayerSpawn -= OnPlayerSpawn;
            ServiceLocator.Instance.GameManager.OnPlayerTakeDamage -= RefreshUI;
        }

        private void OnPlayerSpawn(PlayerController p)
        {
            RefreshUI();
        }
        
        private void RefreshUI()
        {
            int currentHealth = ServiceLocator.Instance.GameManager.PlayerHealth;
            for (var i = 0; i < _hearts.Count; i++)
            {
                _hearts[i].SetActive(currentHealth > i);
            }
        }
    }
}