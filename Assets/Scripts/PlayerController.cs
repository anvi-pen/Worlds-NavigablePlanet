using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 * Modified from Sebastian Lague's First Person Controller: Spherical Worlds tutorial
 * Link: https://www.youtube.com/watch?v=TicipSVT-T8&t=1s
 */

public class PlayerController : MonoBehaviour
{
    // Toggle between first person view and third person view
    private bool m_firstPerson;
    public bool firstPerson
    {
        get
        {
            return m_firstPerson;
        }
        set
        {
            if (value == m_firstPerson)
                return;
            m_firstPerson = value;
            TogglePlayerView(m_firstPerson);
        }
    }

    // Used for ground detection
    [SerializeField] private float playerHeight = 2f;

    // Used for rotating camera
    public float mouseSensitivityX = 250f;
    public float mouseSensitivityY = 250f;

    public float walkSpeed = 8f;
    public float jumpForce = 220f;

    // Used for jumping / ground detection
    public LayerMask groundedMask;

    // Used for first person view and third person view
    [SerializeField] private Vector3 firstPosition = new Vector3(0, 0.4f, 0);
    [SerializeField] private Vector3 firstRotation = Vector3.zero;
    [SerializeField] private Vector3 thirdPosition = new Vector3(0, 8f, 0);
    [SerializeField] private Vector3 thirdRotation = new Vector3(90f, 0, 0);

    // Used for camera rotation
    Transform cameraT;
    float verticalLookRotation;

    // Used for player movement
    Vector3 moveDir;
    // Vector3 moveAmount;
    // Vector3 smoothMoveVelocity;

    // Used for camera rotation
    float rotateX = 0f;
    float rotateY = 0f;

    // Used for ground detection
    bool grounded;

    [SerializeField] HungerDigestionManager hungerDigestionManager;

    private bool summonRain = false;
    [SerializeField] GameObject rain;

    // Used for controlling player model
    [SerializeField] GameObject topLevelPlayerMesh;
    [SerializeField] SkinnedMeshRenderer playerMesh;
    [SerializeField] GameObject topLevelCam;

    [SerializeField] Animator playerAnimator;

    // Start is called before the first frame update
    void Start()
    {
        cameraT = Camera.main.transform;
        rain.SetActive(false);
        firstPerson = true;
    }

    // Update is called once per frame
    void Update()
    {
        // camera rotation
        topLevelCam.transform.Rotate(Vector3.up * rotateX * Time.deltaTime * mouseSensitivityX);
        if (firstPerson)
        {
            verticalLookRotation += rotateY * Time.deltaTime * mouseSensitivityY;
            verticalLookRotation = Mathf.Clamp(verticalLookRotation, -40, 40);
            cameraT.localEulerAngles = Vector3.left * verticalLookRotation;
        }

        // player movement
        // Vector3 targetMoveAmount = moveDir * walkSpeed;
        // moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, 0.15f);

        // player animation
        playerAnimator.SetFloat("speed", Vector3.Magnitude(moveDir.normalized) * walkSpeed);

        // rotate player model
        if (moveDir != Vector3.zero)
        {
            Vector3 targetPosition = topLevelPlayerMesh.transform.position + topLevelCam.transform.TransformDirection(moveDir);
            topLevelPlayerMesh.transform.LookAt(targetPosition, topLevelPlayerMesh.transform.up);
        }

        // if player moving, increase hunger
        if (moveDir != Vector3.zero)
            hungerDigestionManager.IncreaseHunger(HungerDigestionManager.RATE_TYPE.MOVE);

        // is player touching ground
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, (playerHeight / 2) + 0.1f, groundedMask))
        {
            grounded = true;
            playerAnimator.SetBool("onGround", true);
        }
        else
        {
            grounded = false;
            playerAnimator.SetBool("onGround", false);
        }
    }

    void FixedUpdate()
    {
        // player movement
        // GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);

        // player movement
        transform.position += (topLevelCam.transform.TransformDirection(moveDir) * walkSpeed * Time.deltaTime);
    }

    // Determine movement direction based on which keys are pressed
    private void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        float xMove = movementVector.x;
        float zMove = movementVector.y;

        moveDir = new Vector3(xMove, 0, zMove).normalized;
    }

    // Determine values for rotating camera
    private void OnRotate(InputValue rotateValue)
    {
        rotateX = rotateValue.Get<Vector2>().x;
        rotateY = rotateValue.Get<Vector2>().y;
    }

    // Jump based on if key is pressed, if player is on the ground, and if player has enough stamina
    private void OnJump(InputValue jumpValue)
    {
        if (jumpValue.Get<float>() > 0)
        {
            if (grounded)
            {
                if (hungerDigestionManager.stage == HungerDigestionManager.HUNGER_STAGE.HUNGRY)
                    return;

                GetComponent<Rigidbody>().AddForce(transform.up * jumpForce);
                // Play jump animation
                playerAnimator.SetTrigger("Jump");
                // Decrease stamina
                hungerDigestionManager.IncreaseHunger(HungerDigestionManager.RATE_TYPE.JUMP);
            }
        }
    }

    // When key is pressed, switch between first person view and third person view
    private void OnViewChange(InputValue viewValue)
    {
        if (viewValue.Get<float>() > 0)
            firstPerson = !firstPerson;
    }

    // When key is pressed, summon rain around player
    private void OnRain(InputValue rainValue)
    {
        if (rainValue.Get<float>() > 0)
        {
            summonRain = !summonRain;
            rain.SetActive(summonRain);
        }
    }

    // Adjusts camera to support first person view and third person view
    private void TogglePlayerView(bool firstPersonView)
    {
        if (firstPersonView)
        {
            cameraT.localPosition = firstPosition;
            cameraT.localRotation = Quaternion.Euler(firstRotation);

            playerMesh.enabled = false;
        }
        else
        {
            cameraT.localPosition = thirdPosition;
            cameraT.localRotation = Quaternion.Euler(thirdRotation);

            playerMesh.enabled = true;
        }
    }
}
