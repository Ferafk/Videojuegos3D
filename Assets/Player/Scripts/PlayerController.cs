using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Gaming.FinalCharacterController
{
    [DefaultExecutionOrder(-1)]
    public class PlayerController : MonoBehaviour
    {
        #region Variables

        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Camera _playerCamera;
        public float RotationMismatch { get; private set; } = 0f;
        public bool IsRotatinToTarget { get; private set; } = false;

        [Header("Base Movement")]
        public float runAcceleration = 0.25f;
        public float runSpeed = 4f;
        public float drag = 0.1f;
        public float gravity = 9.81f;
        public float terminalVelocity = 53f;
        public float checksphereRadius = 1f;
        public float jumpSpeed = 1.0f;
        public LayerMask groundlayer;
        public LayerMask groundlayer2;

        [Header("Camera Settings")]
        public float lookSenseH = 0.1f;
        public float lookSenseV = 0.1f;
        public float lookLimitV = 89f;

        public bool tieneComida;

        [Header("Particle System")]
        public ParticleSystem jumpParticles;
        public ParticleSystem landingParticles;

        private PlayerLocomotionInput _playerLocomotionInput;
        private PlayerActions _playerActions;
        private Vector2 _cameraRotation = Vector2.zero;

        private bool isJumping = false;
        private bool isFalling = false;
        private bool isGrounded;
        private bool wasGrounded = true;
        private float _verticalVelocity = 0f;

        [Header("Camera Advanced Settings")]
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;
        [SerializeField] private float _rotationSmoothTime = 0.2f;
        [SerializeField] private float _movementThreshold = 0.1f;

        private Vector3 _currentRotationVelocity;
        private float _targetYaw;
        private float _currentYaw;
        private bool _isMoving;
        private Transform _cameraTarget;

        private Vector3 _lastMovementDirection;
        private float _currentRotationAngle;
        private float _rotationVelocity;

        #endregion

        private void Awake()
        {
            _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
            _playerActions = GetComponent<PlayerActions>();
            InitializeCamera();
        }

        private void Update()
        {
            isGrounded = IsGrounded();

            if (!wasGrounded && isGrounded)
            {
                PlayLandingParticles();
            }

            if (wasGrounded && !isGrounded)
            {
                PlayJumpParticles();
            }

            wasGrounded = isGrounded;

            HandleMovement();
            HandleVerticalMovement();
        }

        #region Movement

        private void HandleMovement()
        {
            Vector3 cameraForwardXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z).normalized;
            Vector3 cameraRightXZ = new Vector3(_playerCamera.transform.right.x, 0f, _playerCamera.transform.right.z).normalized;
            Vector3 movementDirection = cameraRightXZ * _playerLocomotionInput.MovementInput.x + cameraForwardXZ * _playerLocomotionInput.MovementInput.y;

            if (movementDirection.magnitude > 0.1f)
            {
                _lastMovementDirection = movementDirection;
                float targetAngle = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg;
                _currentRotationAngle = Mathf.SmoothDampAngle(_currentRotationAngle, targetAngle, ref _rotationVelocity, _rotationSmoothTime);
                transform.rotation = Quaternion.Euler(0f, _currentRotationAngle, 0f);
            }

            // Mantener la velocidad horizontal actual
            Vector3 horizontalVelocity = new Vector3(_characterController.velocity.x, 0f, _characterController.velocity.z);

            Vector3 movementDelta = movementDirection * runAcceleration * Time.deltaTime;
            Vector3 newHorizontalVelocity = horizontalVelocity + movementDelta;

            Vector3 currentDrag = newHorizontalVelocity.normalized * drag * Time.deltaTime;
            newHorizontalVelocity = (newHorizontalVelocity.magnitude > drag * Time.deltaTime)
                ? newHorizontalVelocity - currentDrag
                : Vector3.zero;

            newHorizontalVelocity = Vector3.ClampMagnitude(newHorizontalVelocity, runSpeed);

            // Combinar la velocidad horizontal con la vertical
            Vector3 newVelocity = new Vector3(
                newHorizontalVelocity.x,
                _verticalVelocity,
                newHorizontalVelocity.z
            );

            if (newHorizontalVelocity.magnitude > 1 && isGrounded)
            {
                _playerActions.isMoving = true;
                _playerActions.Walk();
            }
            else
            {
                _playerActions.isMoving = false;
            }

            _characterController.Move(newVelocity * Time.deltaTime);
        }

        /*private void HandleMovement()
        {
            Vector3 cameraForwardXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z).normalized;
            Vector3 cameraRightXZ = new Vector3(_playerCamera.transform.right.x, 0f, _playerCamera.transform.right.z).normalized;
            Vector3 movementDirection = cameraRightXZ * _playerLocomotionInput.MovementInput.x + cameraForwardXZ * _playerLocomotionInput.MovementInput.y;

            if (movementDirection.magnitude > 0.1f)
            {
                _lastMovementDirection = movementDirection;
                float targetAngle = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg;
                _currentRotationAngle = Mathf.SmoothDampAngle(_currentRotationAngle, targetAngle, ref _rotationVelocity, _rotationSmoothTime);
                transform.rotation = Quaternion.Euler(0f, _currentRotationAngle, 0f);
            }

            Vector3 movementDelta = movementDirection * runAcceleration * Time.deltaTime;
            Vector3 newVelocity = _characterController.velocity + movementDelta;

            Vector3 currentDrag = newVelocity.normalized * drag * Time.deltaTime;
            newVelocity = (newVelocity.magnitude > drag * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero;
            newVelocity = Vector3.ClampMagnitude(newVelocity, runSpeed);
            newVelocity.y += _verticalVelocity;

            if (newVelocity.magnitude > 1 && isGrounded)
            {
                _playerActions.isMoving = true;
                _playerActions.Walk();
            }
            else
            {
                _playerActions.isMoving = false;
            }

            _characterController.Move(newVelocity * Time.deltaTime);
        }
        
        private void HandleVerticalMovement()
        {


            if (!isGrounded && _characterController.velocity.y > 0f && !isJumping)
            {
                _playerActions.Jumping();
                isJumping = true;
                isFalling = false;
            }
            else if (!isGrounded && _characterController.velocity.y <= 0f && !isFalling)
            {
                _playerActions.Falling();
                isFalling = true;
                isJumping = false;
            }
            else if (isGrounded && !(isJumping || isFalling))
            {
                isJumping = false;
                isFalling = false;
            }

            if (isGrounded && _verticalVelocity < 0f)
                _verticalVelocity = 0f;

            _verticalVelocity -= gravity * Time.deltaTime;

            if (_playerLocomotionInput.JumpPressed && isGrounded)
            {
                _verticalVelocity += Mathf.Sqrt(jumpSpeed * 3 * gravity);
            }


        }*/

        private void HandleVerticalMovement()
{
    // Si está en el suelo, resetear la velocidad vertical
    if (isGrounded)
    {
        // Añadir un pequeño offset para mantener el personaje pegado al suelo
        if (_verticalVelocity < 0)
            _verticalVelocity = -0.1f;
        
        isJumping = false;
        isFalling = false;
    }
    else
    {
        // Aplicar gravedad de forma más natural
        // Multiplicar por un factor mayor para una caída más rápida
        _verticalVelocity -= gravity * 2f * Time.deltaTime;
    }

    // Gestión de estados de salto y caída
    if (!isGrounded && _characterController.velocity.y > 0f && !isJumping)
    {
        _playerActions.Jumping();
        isJumping = true;
        isFalling = false;
    }
    else if (!isGrounded && _characterController.velocity.y <= 0f && !isFalling)
    {
        _playerActions.Falling();
        isFalling = true;
        isJumping = false;
    }

    // Salto
    if (_playerLocomotionInput.JumpPressed && isGrounded)
    {
        // Usar una fórmula de salto más precisa
        _verticalVelocity = Mathf.Sqrt(jumpSpeed * 2f * gravity);
    }

    // Limitar la velocidad de caída para evitar caídas extremadamente rápidas
    _verticalVelocity = Mathf.Max(_verticalVelocity, -terminalVelocity);
}

        private void LateUpdate()
        {
            UpdateCameraRotation();
        }

        private void InitializeCamera()
        {
            GameObject cameraTargetObj = new GameObject("CameraTarget");
            _cameraTarget = cameraTargetObj.transform;
            _cameraTarget.parent = transform;
            _cameraTarget.localPosition = Vector3.zero;

            if (_virtualCamera == null)
            {
                Debug.LogWarning("Prof: ¡No olvides asignar la cámara virtual en el inspector! ??");
                _virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            }

            _virtualCamera.Follow = _cameraTarget;
            _virtualCamera.LookAt = transform;
        }

        private void UpdateCameraRotation()
        {
            _isMoving = _characterController.velocity.magnitude > _movementThreshold;

            _cameraRotation.x += _playerLocomotionInput.LookInput.x * lookSenseH;
            _cameraRotation.y = Mathf.Clamp(_cameraRotation.y + _playerLocomotionInput.LookInput.y * lookSenseV, -lookLimitV, lookLimitV);

            _targetYaw = _cameraRotation.x;
            _currentYaw = Mathf.SmoothDampAngle(_currentYaw, _targetYaw, ref _currentRotationVelocity.x, _rotationSmoothTime);

            _cameraTarget.rotation = Quaternion.Euler(-_cameraRotation.y, _currentYaw, 0f);
        }

        #endregion

        private void PlayJumpParticles()
        {
            if (jumpParticles != null)
            {
                jumpParticles.Play();
            }
        }

        private void PlayLandingParticles()
        {
            if (landingParticles != null)
            {
                landingParticles.Play();
            }
        }



        public void TakeDamage(int damage)
        {
            //health --;
            Debug.Log("Damage");
        }

        #region State Checks

        private bool IsGrounded()
        {
            return Physics.Raycast(transform.position, Vector3.down, checksphereRadius, groundlayer) || Physics.Raycast(transform.position, Vector3.down, checksphereRadius, groundlayer2);
        }

        void OnDrawGizmos()
        {
            // Dibuja un rayo para ver exactamente dónde estás detectando el suelo
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawRay(
                transform.position,
                Vector3.down * checksphereRadius  // Distancia de verificación
            );
        }

        #endregion

    }
}


