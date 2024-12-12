// PlayerController.cs
using UnityEngine;
using System.Collections;


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

    // Footstep Audio Variables
    public AudioClip[] footstepSounds; // Array of footstep sounds
    public Transform footstepAudioPosition; // Position of the audio source
    public AudioSource audioSource; // Audio source to play the sounds
    public float walkFootstepDelay = 0.5f; // Delay between footsteps when walking
    public float runFootstepDelay = 0.3f; // Delay between footsteps when running

    // These keep track of player movement
    private Vector3 howToMove = Vector3.zero;
    private float upDownLook = 0;
    private float leftRightLook = 0;
    private bool playerCanMove = true;
    private bool isWalking = false; // Whether the player is walking
    private bool isFootstepCoroutineRunning = false; // To track coroutine status

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

    void Update()
    {
        // Movement
        Vector3 whereIsForward = transform.TransformDirection(Vector3.forward);
        Vector3 whereIsRight = transform.TransformDirection(Vector3.right);

        bool isPlayerRunning = Input.GetKey(KeyCode.LeftShift);

        float forwardSpeed = 0;
        if (playerCanMove)
        {
            forwardSpeed = (isPlayerRunning ? fastSpeed : normalSpeed) * Input.GetAxis("Vertical");
        }

        float sideSpeed = 0;
        if (playerCanMove)
        {
            sideSpeed = (isPlayerRunning ? fastSpeed : normalSpeed) * Input.GetAxis("Horizontal");
        }

        howToMove = (whereIsForward * forwardSpeed) + (whereIsRight * sideSpeed);
        myController.Move(howToMove * Time.deltaTime);

        // Footstep sounds
        bool isPlayerMoving = howToMove.magnitude > 0.1f;
        isWalking = isPlayerMoving && playerCanMove;

        if (isWalking && !isFootstepCoroutineRunning)
        {
            float footstepDelay = isPlayerRunning ? runFootstepDelay : walkFootstepDelay;
            StartCoroutine(PlayFootstepSounds(footstepDelay));
        }

        // Camera logic (unchanged)
        if (playerCanMove)
        {
            float mouseUpDown = Input.GetAxis("Mouse Y") * mouseSpeed;
            upDownLook -= mouseUpDown;

            if (upDownLook > maxUpDownAngle)
                upDownLook = maxUpDownAngle;
            if (upDownLook < -maxUpDownAngle)
                upDownLook = -maxUpDownAngle;

            float mouseLeftRight = Input.GetAxis("Mouse X") * mouseSpeed;
            leftRightLook += mouseLeftRight;

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

    // Play footstep sounds with a delay based on movement speed
    IEnumerator PlayFootstepSounds(float footstepDelay)
    {
        isFootstepCoroutineRunning = true;

        while (isWalking)
        {
            if (footstepSounds.Length > 0)
            {
                int randomIndex = Random.Range(0, footstepSounds.Length);
                audioSource.transform.position = footstepAudioPosition.position;
                audioSource.clip = footstepSounds[randomIndex];
                audioSource.Play();
                yield return new WaitForSeconds(footstepDelay);
            }
            else
            {
                yield break;
            }
        }

        isFootstepCoroutineRunning = false;
    }
}