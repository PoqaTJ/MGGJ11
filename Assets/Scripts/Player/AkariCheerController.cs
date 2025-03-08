using System;
using System.Collections.Generic;
using Dialogs;
using Services;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Player
{
    public class AkariCheerController: MonoBehaviour
    {
        [SerializeField] private List<QuipDefinition> _quipsToPlay;
        private List<QuipDefinition> _quipsRemaining;
        [SerializeField] private float _secondsBetweenQuipsMin = 4;
        [SerializeField] private float _secondsBetweenQuipsMax = 6;

        [SerializeField] private PlayerMover _mover;

        private float _nextQuipTime;
        private GameObject _tomoya;
        private bool _tomoyaInRange => _tomoya != null;
        
        private void Start()
        {
            RefreshQuips();
        }

        private void RefreshQuips()
        {
            _quipsRemaining = new List<QuipDefinition>(_quipsToPlay);
        }

        private void PlayQuip()
        {
            if (_quipsRemaining.Count == 0)
            {
                RefreshQuips();
            }

            int choice = Random.Range(0, _quipsRemaining.Count - 1);
            ServiceLocator.Instance.DialogManager.ShowQuip(_quipsRemaining[choice]);
            _quipsRemaining.RemoveAt(choice);
            
            _nextQuipTime = Time.timeSinceLevelLoad + Random.Range(_secondsBetweenQuipsMin, _secondsBetweenQuipsMax);

        }

        private void Update()
        {
            if (_tomoyaInRange)
            {
                if (Time.timeSinceLevelLoad > _nextQuipTime)
                {
                    PlayQuip();
                }

                if (_tomoya.transform.position.x < transform.position.x)
                {
                    _mover.Face(PlayerMover.Direction.LEFT);
                }
                else
                {
                    _mover.Face(PlayerMover.Direction.RIGHT);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player"))
            {
                _tomoya = col.gameObject;
            }
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if (col.CompareTag("Player"))
            {
                _tomoya = null;
            }
        }
    }
}