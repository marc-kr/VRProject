using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonController : MonoBehaviour
{   
    [SerializeField] private Transform playerCamera;

    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float gravity = -20f;

    [SerializeField, Range(0.02f, 0.5f)] private float sensitivityX = 0.12f;
    [SerializeField, Range(0.02f, 0.5f)] private float sensitivityY = 0.12f;
    
    private CharacterController controller;
    private float verticalVelocity;
    private float cameraPitch;
    
    private void Awake(){
        controller = GetComponent<CharacterController>();
    }

    void Start()
    {
        LockCursor();
    }

    // Update is called once per frame
    void Update(){
        if (Keyboard.current == null || Mouse.current == null)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            UnlockCursor();

        if (Mouse.current.leftButton.wasPressedThisFrame &&
            Cursor.lockState != CursorLockMode.Locked)
            LockCursor();

        Move();

        if (Cursor.lockState == CursorLockMode.Locked)
            Look();
    }

    private void Move(){
        Vector2 input = Vector2.zero;

        if (Keyboard.current.wKey.isPressed) input.y += 1f;
        if (Keyboard.current.sKey.isPressed) input.y -= 1f;
        if (Keyboard.current.dKey.isPressed) input.x += 1f;
        if (Keyboard.current.aKey.isPressed) input.x -= 1f;

        Vector3 direction =
            transform.right * input.x + transform.forward * input.y;
        direction = Vector3.ClampMagnitude(direction, 1f);

        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 motion = direction * moveSpeed;
        motion.y = verticalVelocity;

        controller.Move(motion * Time.deltaTime);
    }

     private void Look()
    {
        Vector2 delta = Mouse.current.delta.ReadValue();

        transform.Rotate(Vector3.up, delta.x * sensitivityX);

        cameraPitch = Mathf.Clamp(
            cameraPitch - delta.y * sensitivityY,
            -85f,
            85f
        );

        playerCamera.localRotation =
            Quaternion.Euler(cameraPitch, 0f, 0f);
    }

    public void SetSensitivityX(float value)
    {
        sensitivityX = Mathf.Clamp(value, 0.02f, 0.5f);
    }

    public void SetSensitivityY(float value)
    {
        sensitivityY = Mathf.Clamp(value, 0.02f, 0.5f);
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
