using UnityEngine;
using UnityEngine.InputSystem;

namespace Movement
{
    public class Movement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 100f;
        [SerializeField] private float jumpForce = 5f;
        
        private Rigidbody _rb;
        private bool _isGrounded = true;
        private Transform _cameraTransform;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            if (_rb == null)
            {
                _rb = gameObject.AddComponent<Rigidbody>();
            }
            _rb.freezeRotation = true;
            
            _cameraTransform = Camera.main != null ? Camera.main.transform : null;
        }

        private void Update()
        {
            HandleMovement();
            HandleRotation();
            HandleJump();
        }

        private void HandleMovement()
        {
            var input = Vector2.zero;

            var gamepad = Gamepad.current;
            if (gamepad != null)
            {
                input = gamepad.leftStick.ReadValue();
            }
            else if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) input.y += 1f;
                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) input.y -= 1f;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) input.x += 1f;
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) input.x -= 1f;
            }

            Vector3 movement;
            if (_cameraTransform is not null)
            {
                var cameraForward = _cameraTransform.forward;
                cameraForward.y = 0;
                cameraForward.Normalize();
                
                var cameraRight = _cameraTransform.right;
                cameraRight.y = 0;
                cameraRight.Normalize();
                
                movement = cameraRight * input.x + cameraForward * input.y;
            }
            else
            {
                movement = transform.right * input.x + transform.forward * input.y;
            }
            
            transform.position += movement * (moveSpeed * Time.deltaTime);
        }

        private void HandleRotation()
        {
            var rotation = 0f;

            var gamepad = Gamepad.current;
            if (gamepad != null)
            {
                rotation = gamepad.rightStick.ReadValue().x;
            }
            else if (Keyboard.current != null)
            {
                if (Keyboard.current.qKey.isPressed) rotation = -1f;
                else if (Keyboard.current.eKey.isPressed) rotation = 1f;
            }

            transform.Rotate(Vector3.up, rotation * rotationSpeed * Time.deltaTime);
        }

        private void HandleJump()
        {
            var gamepad = Gamepad.current;
            var jumpPressed = false;

            if (gamepad != null)
            {
                jumpPressed = gamepad.buttonSouth.wasPressedThisFrame; // A button
            }
            else if (Keyboard.current != null)
            {
                jumpPressed = Keyboard.current.spaceKey.wasPressedThisFrame;
            }

            if (jumpPressed && _isGrounded && _rb != null)
            {
                _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                _isGrounded = false;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                _isGrounded = true;
                Debug.Log("Landed on ground");
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                _isGrounded = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Collectible"))
            {
                Debug.Log("Triggered collectible: " + other.gameObject.name);
                Destroy(other.gameObject);
            }
        }
    }
}