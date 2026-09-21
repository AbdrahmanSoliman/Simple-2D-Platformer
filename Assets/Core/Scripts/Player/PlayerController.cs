using UnityEngine;
using UnityEngine.InputSystem;

namespace Platformer.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float _moveSpeed = 8f;
        [SerializeField] private float _jumpForce = 12f;

        [Header("Ground Detection")]
        [SerializeField] private Transform _groundCheck;
        [SerializeField] private float _groundRadius = 0.2f;
        [SerializeField] private LayerMask _groundMask;

        private Rigidbody2D _rb;
        private PlayerInput _playerInput;
        private Vector2 _moveInput;
        private bool _isGrounded;
        private bool _jumpRequested;

        public bool IsGrounded => _isGrounded;
        public Vector2 MoveInput => _moveInput;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.freezeRotation = true;

            if (_rb.sharedMaterial == null)
            {
                _rb.sharedMaterial = new PhysicsMaterial2D("PlayerMovement")
                {
                    friction = 0f,
                    bounciness = 0f
                };
            }

            _playerInput = new PlayerInput();
        }

        private void OnEnable()
        {
            _playerInput.Player.Enable();
            _playerInput.Player.Move.performed += OnMovePerformed;
            _playerInput.Player.Move.canceled += OnMoveCanceled;
            _playerInput.Player.Jump.performed += OnJumpPerformed;
        }

        private void OnDisable()
        {
            _playerInput.Player.Move.performed -= OnMovePerformed;
            _playerInput.Player.Move.canceled -= OnMoveCanceled;
            _playerInput.Player.Jump.performed -= OnJumpPerformed;
            _playerInput.Player.Disable();
        }

        private void FixedUpdate()
        {
            CheckGrounded();
            ApplyMovement();
            ApplyJump();
        }

        #region Input Callbacks (Event-Driven)

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            _moveInput = Vector2.zero;
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            _jumpRequested = true;
        }

        #endregion

        private void CheckGrounded()
        {
            if (_groundCheck != null)
            {
                _isGrounded = Physics2D.OverlapCircle(_groundCheck.position, _groundRadius, _groundMask);
            }
            else
            {
                _isGrounded = false;
            }
        }

        private void ApplyMovement()
        {
            _rb.linearVelocity = new Vector2(_moveInput.x * _moveSpeed, _rb.linearVelocity.y);
        }

        private void ApplyJump()
        {
            if (_jumpRequested)
            {
                if (_isGrounded)
                {
                    _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce);
                }
                _jumpRequested = false;
            }
        }

        public void Bounce(float force)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, force);
        }

        public void Teleport(Vector2 position)
        {
            transform.position = position;
            _rb.linearVelocity = Vector2.zero;
        }
    }
}


