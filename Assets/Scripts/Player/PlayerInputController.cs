using System;
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

        private void Start()
        {
            _moveAction = InputSystem.actions.FindAction("Move");
            _jumpAction = InputSystem.actions.FindAction("Jump");
            _respawnAction = InputSystem.actions.FindAction("Respawn");
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
        }

        private void FixedUpdate()
        {
            float xDir = _moveAction.ReadValue<Vector2>().x;
            _playerController.OnFixedUpdate(xDir);
        }
    }
}