// PlayerController.cs
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public Camera myCamera;
    public float normalSpeed = 3f;
    public float fastSpeed = 10f;
    public float mouseSpeed = 2f;
    public float maxUpDownAngle = 75f;
    public float smoothValue = 5f;

    private Vector3 howToMove = Vector3.zero;
    private float upDownLook = 0;
    private float leftRightLook = 0;
    private bool playerCanMove = true;
    private CharacterController myController;
    private float initialCameraHeight;

    void Start()
    {
        myController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Store the initial camera height relative to player
        initialCameraHeight = myCamera.transform.localPosition.y;
    }

    void Update()
    {
        // Ensure camera maintains consistent height
        Vector3 cameraPos = myCamera.transform.localPosition;
        if (cameraPos.y != initialCameraHeight)
        {
            cameraPos.y = initialCameraHeight;
            myCamera.transform.localPosition = cameraPos;
        }

        Vector3 whereIsForward = transform.TransformDirection(Vector3.forward);
        Vector3 whereIsRight = transform.TransformDirection(Vector3.right);

        bool isPlayerRunning = Input.GetKey(KeyCode.LeftShift);

        float forwardSpeed = 0;
        float sideSpeed = 0;

        if(playerCanMove)
        {
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

            howToMove = (whereIsForward * forwardSpeed) + (whereIsRight * sideSpeed);
            myController.Move(howToMove * Time.deltaTime);

            // Camera rotation
            float mouseUpDown = Input.GetAxis("Mouse Y") * mouseSpeed;
            upDownLook = upDownLook - mouseUpDown;
            upDownLook = Mathf.Clamp(upDownLook, -maxUpDownAngle, maxUpDownAngle);

            float mouseLeftRight = Input.GetAxis("Mouse X") * mouseSpeed;
            leftRightLook = leftRightLook + mouseLeftRight;

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

    public void SetControlsEnabled(bool enabled)
    {
        playerCanMove = enabled;
    }

    // Store current look angles
    public void StoreCurrentRotation()
    {
        upDownLook = myCamera.transform.localRotation.eulerAngles.x;
        leftRightLook = transform.rotation.eulerAngles.y;
    }
}