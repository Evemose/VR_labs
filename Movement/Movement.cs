using UnityEngine;
using UnityEngine.InputSystem;

namespace Movement
{
    public class Movement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 100f;

        private void Update()
        {
            HandleMovement();
            HandleRotation();
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

            var movement = transform.right * input.x + transform.forward * input.y;
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
    }
}