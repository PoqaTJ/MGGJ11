using System;
using Player;
using Services;
using TMPro;
using UnityEngine;

namespace Menus.HUD
{
    public class CollectableUI: MonoBehaviour
    {
        [SerializeField] private TMP_Text _counter;

        private void Start()
        {
            RefreshUI();
        }

        private void OnEnable()
        {
            ServiceLocator.Instance.GameManager.OnPlayerSpawn += OnPlayerSpawn;
            ServiceLocator.Instance.GameManager.OnMcGuffinFound += RefreshUI;
        }

        private void OnDisable()
        {
            ServiceLocator.Instance.GameManager.OnPlayerSpawn -= OnPlayerSpawn;
            ServiceLocator.Instance.GameManager.OnMcGuffinFound -= RefreshUI;
        }

        private void OnPlayerSpawn(PlayerController p)
        {
            RefreshUI();
        }

        private void RefreshUI()
        {
            _counter.text = "" + ServiceLocator.Instance.GameManager.McGuffinCount;
        }
    }
}