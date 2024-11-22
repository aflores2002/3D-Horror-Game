using UnityEngine;

// Need this to make player move
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // Drag and drop the Main Camera here in Unity
    public Camera myCamera;

    // How fast the player moves
    public float normalSpeed = 3f;
    public float fastSpeed = 10f;

    // Mouse and camera stuff
    public float mouseSpeed = 2f;
    public float maxUpDownAngle = 75f;
    public float smoothValue = 5f;

    // These keep track of player movement
    private Vector3 howToMove = Vector3.zero;
    private float upDownLook = 0;
    private float leftRightLook = 0;
    private bool playerCanMove = true;

    // This moves the player
    private CharacterController myController;

    void Start()
    {
        // Get the component that moves the player
        myController = GetComponent<CharacterController>();

        // Make the mouse disappear
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Movement
        // Figure out which way is forward and right
        Vector3 whereIsForward = transform.TransformDirection(Vector3.forward);
        Vector3 whereIsRight = transform.TransformDirection(Vector3.right);

        // Check if shift is being held down to run
        bool isPlayerRunning = Input.GetKey(KeyCode.LeftShift);

        // Figure out how fast to move
        // W and S keys
        float forwardSpeed = 0;
        if(playerCanMove)
        {
            if(isPlayerRunning)
            {
                forwardSpeed = fastSpeed * Input.GetAxis("Vertical");
            }
            else
            {
                forwardSpeed = normalSpeed * Input.GetAxis("Vertical");
            }
        }

        // A and D keys
        float sideSpeed = 0;
        if(playerCanMove)
        {
            if(isPlayerRunning)
            {
                sideSpeed = fastSpeed * Input.GetAxis("Horizontal");
            }
            else
            {
                sideSpeed = normalSpeed * Input.GetAxis("Horizontal");
            }
        }

        // Move the player
        howToMove = (whereIsForward * forwardSpeed) + (whereIsRight * sideSpeed);
        myController.Move(howToMove * Time.deltaTime);

        // Camera
        if(playerCanMove)
        {
            // Move camera up/down with mouse
            float mouseUpDown = Input.GetAxis("Mouse Y") * mouseSpeed;
            upDownLook = upDownLook - mouseUpDown;

            // Don't let player look too far up or down
            if(upDownLook > maxUpDownAngle)
            {
                upDownLook = maxUpDownAngle;
            }
            if(upDownLook < -maxUpDownAngle)
            {
                upDownLook = -maxUpDownAngle;
            }

            // Move camera left/right with mouse
            float mouseLeftRight = Input.GetAxis("Mouse X") * mouseSpeed;
            leftRightLook = leftRightLook + mouseLeftRight;

            // Make the camera movement smooth
            Quaternion newUpDown = Quaternion.Euler(upDownLook, 0, 0);
            Quaternion newLeftRight = Quaternion.Euler(0, leftRightLook, 0);

            // Actually move the camera
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
}