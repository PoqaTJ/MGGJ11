using System;
using Services;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInputController: MonoBehaviour
    {
        [SerializeField] private PlayerController _playerController;

        private InputAction _moveAction;
        private InputAction _jumpAction;
        private InputAction _respawnAction;
        private InputAction _pauseAction;

        private void Start()
        {
            _moveAction = InputSystem.actions.FindAction("Move");
            _jumpAction = InputSystem.actions.FindAction("Jump");
            _respawnAction = InputSystem.actions.FindAction("Respawn");
            _pauseAction = InputSystem.actions.FindAction("Pause");
        }

        private void Update()
        {
            bool jumpTriggered =  _jumpAction.triggered;

            bool jumpReleased = _jumpAction.WasReleasedThisFrame();

            _playerController.OnUpdate(jumpTriggered, jumpReleased, _moveAction.ReadValue<Vector2>().x);

            if (_respawnAction.triggered)
            {
                _playerController.Respawn();
            }

            if (_pauseAction.triggered)
            {
                ServiceLocator.Instance.GameManager.Pause();
            }
        }

        private void FixedUpdate()
        {
            float xDir = _moveAction.ReadValue<Vector2>().x;
            _playerController.OnFixedUpdate(xDir);
        }
    }
}