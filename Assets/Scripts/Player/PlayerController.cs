using UnityEngine;
using System.Collections;

public class Playerc : MonoBehaviour
{
    private static readonly int IsSlidingHash = Animator.StringToHash("isSliding");
    private static readonly int IsGameStartedHash = Animator.StringToHash("isGameStarted");
    private static readonly int IsGroundedHash = Animator.StringToHash("isGrounded");
    private CharacterController controller;
    private Vector3 direction;
    public float forwardSpeed;
    public float maxSpeed;
    private bool isSliding = false;

    private int desiredLane = 1; //zuun=0 gol=1 baruun=2
    public float laneDistance = 4;
    public float jumpForce;
    public float Gravity = -20;

    public Animator animator;
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(forwardSpeed < maxSpeed)
        {
            forwardSpeed += 0.1f * Time.deltaTime;
        }

        direction.z = forwardSpeed;
        animator.SetBool(IsGroundedHash, controller.isGrounded);

        if (SwipeManager.swipeUp && controller.isGrounded) 
        {
            direction.y = -1;
            Jump();  
        }
        else
        {
            direction.y += Gravity * Time.deltaTime;
        }
        
        if (SwipeManager.swipeDown && !isSliding)
        {
            StartCoroutine(Slide());
        }

        if (SwipeManager.swipeRight) 
        {
            desiredLane++;
            if (desiredLane == 3)
            {
                desiredLane = 2;
            }
           
        }

        if (SwipeManager.swipeLeft)
        {
            desiredLane--;
            if (desiredLane == -1)
            {
                desiredLane = 0;
            }
           
        }



        Vector3 targetPosition = transform.position.z * transform.forward + transform.position.y * transform.up;

        if (desiredLane == 0)
        {
            targetPosition += Vector3.left * laneDistance;
        }
        else if (desiredLane == 2)
        {
            targetPosition += Vector3.right * laneDistance;
        }

        // transform.position = Vector3.Lerp(transform.position, targetPosition, 70 * Time.deltaTime);
        if (transform.position == targetPosition)
            return;
        Vector3 diff = targetPosition - transform.position;
        Vector3 moveDir = diff.normalized * 25 * Time.deltaTime;
        if (moveDir.sqrMagnitude < diff.sqrMagnitude)
            controller.Move(moveDir);
        else
            controller.Move(diff);
    }

    private void FixedUpdate()
    {
        if (!PlayerManager.isGameStarted)
            return;

        animator.SetBool(IsGameStartedHash, true);
        

        controller.Move(direction * Time.deltaTime);
    }

    private void Jump()
    {
        direction.y = jumpForce;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.tag == "Obstacle")
        {
            PlayerManager.gameOver = true;
        }
    }

    private IEnumerator Slide()
    {
        isSliding = true;
        animator.SetBool(IsSlidingHash, true);
        controller.center = new Vector3(0, -0.5f, 0);
        controller.height = 1;
        yield return new WaitForSeconds(1.3f);

        controller.center = new Vector3(0, 0, 0);
        controller.height = 2;
        animator.SetBool(IsSlidingHash, false);
        isSliding = false;
    }
}
