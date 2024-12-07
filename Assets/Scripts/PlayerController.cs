using UnityEngine;

// Handles player movement and camera controls using CharacterController
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public Camera myCamera;
    public float normalSpeed = 3f;
    public float fastSpeed = 10f;
    public float mouseSpeed = 2f;
    public float maxUpDownAngle = 75f; // Maximum angle player can look up/down
    public float smoothValue = 5f; // Controls how smoothly camera rotation occurs

    private Vector3 howToMove = Vector3.zero;
    private float upDownLook = 0;    // Vertical camera rotation
    private float leftRightLook = 0; // Horizontal camera rotation
    private bool playerCanMove = true;
    private CharacterController myController;
    private float initialCameraHeight; // Stores original camera height to prevent unwanted changes

    void Start()
    {
        myController = GetComponent<CharacterController>();

        // Lock and hide cursor for first-person control
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        initialCameraHeight = myCamera.transform.localPosition.y;
    }

    void Update()
    {
        // Maintain consistent camera height relative to player
        Vector3 cameraPos = myCamera.transform.localPosition;
        if (cameraPos.y != initialCameraHeight)
        {
            cameraPos.y = initialCameraHeight;
            myCamera.transform.localPosition = cameraPos;
        }

        // Get local directional vectors for movement
        Vector3 whereIsForward = transform.TransformDirection(Vector3.forward);
        Vector3 whereIsRight = transform.TransformDirection(Vector3.right);

        bool isPlayerRunning = Input.GetKey(KeyCode.LeftShift);

        float forwardSpeed = 0;
        float sideSpeed = 0;

        if(playerCanMove)
        {
            // Calculate movement speeds based on running state
            if(isPlayerRunning)
            {
                forwardSpeed = fastSpeed * Input.GetAxis("Vertical");
                sideSpeed = fastSpeed * Input.GetAxis("Horizontal");
            }
            else
            {
                forwardSpeed = normalSpeed * Input.GetAxis("Vertical");
                sideSpeed = normalSpeed * Input.GetAxis("Horizontal");
            }

            // Apply movement
            howToMove = (whereIsForward * forwardSpeed) + (whereIsRight * sideSpeed);
            myController.Move(howToMove * Time.deltaTime);

            // Handle camera vertical rotation with angle clamping
            float mouseUpDown = Input.GetAxis("Mouse Y") * mouseSpeed;
            upDownLook = upDownLook - mouseUpDown;
            upDownLook = Mathf.Clamp(upDownLook, -maxUpDownAngle, maxUpDownAngle);

            // Handle camera horizontal rotation
            float mouseLeftRight = Input.GetAxis("Mouse X") * mouseSpeed;
            leftRightLook = leftRightLook + mouseLeftRight;

            // Apply smooth camera rotation
            Quaternion newUpDown = Quaternion.Euler(upDownLook, 0, 0);
            Quaternion newLeftRight = Quaternion.Euler(0, leftRightLook, 0);

            myCamera.transform.localRotation = Quaternion.Slerp(
                myCamera.transform.localRotation,
                newUpDown,
                Time.deltaTime * smoothValue
            );

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                newLeftRight,
                Time.deltaTime * smoothValue
            );
        }
    }

    // Enables/disables player movement and camera control
    public void SetControlsEnabled(bool enabled)
    {
        playerCanMove = enabled;
    }

    // Stores current camera rotation angles for restoration later
    public void StoreCurrentRotation()
    {
        upDownLook = myCamera.transform.localRotation.eulerAngles.x;
        leftRightLook = transform.rotation.eulerAngles.y;
    }
}