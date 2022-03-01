using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class FielderMovement : MonoBehaviour
{
    CharacterController controller;

    [SerializeField] private float rotationSpeed = 3;
    [SerializeField] private float movementSpeed = 1;
    [SerializeField] private Transform ballTransform;
    [SerializeField] private Transform ballCheckTransform;


    [Header("Animations")] 
    [SerializeField] private Transform highTester;
    [SerializeField] private Transform jumpHighTester;
    [SerializeField] private Transform lowTester;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject ballAfterCollect;
    [SerializeField] private Transform catchTransform;


    private Vector3 velocity;
    private bool isGrounded;
    private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundMask;
    private float gravity = -9.8f;
    private Transform _transform;


    private void Awake()
    {
        _transform = transform; //cache transform for better performance!
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        ballAfterCollect.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Ball")
            return;
        ballAfterCollect.transform.position = ballTransform.position;
        ballAfterCollect.SetActive(true);
        Destroy(ballTransform.gameObject);
        SelectProperAnimation();
       
        
    }

    //we have 3 normal animations
    //animation from above and animation picking from the ground
    private void SelectProperAnimation()
    {
        float ballTime = .15f;

        if (ballTransform.position.y > jumpHighTester.position.y)
        {
            animator.SetTrigger("highJump");
            ballTime = .25f;
        }
        else
        {
            if (ballTransform.position.y > highTester.position.y)
            {
                animator.SetTrigger("jump");
                ballTime = .2f;
            }
            else if (ballTransform.position.y < lowTester.position.y)
            {
                animator.SetTrigger("pickup");
                ballTime = .2f;
            }
            else
            {
                animator.SetInteger("collect", Random.Range(1, 4));
                Invoke(nameof(ResetCollector), .5f);
            }    
        }
        Invoke(nameof(MoveBallToCatch),ballTime);
    }

    private void MoveBallToCatch()
    {
        ballAfterCollect.transform.SetParent(catchTransform);
        ballAfterCollect.transform.localPosition = Vector3.zero;
    }

    private void ResetCollector()
    {
        animator.SetInteger("collect", 0);
    }

    private void FixedUpdate()
    {
        if (ballTransform == null)
            return;
        isGrounded = CheckIfBallIsOnField();
    }


    // Update is called once per frame
    private void Update()
    {
        if (ballTransform != null)
        {
            if (isGrounded)
            {
                MoveToBall();
                RotateToBall();
            }
        }

        SetAnimation();
    }

    private void SetAnimation()
    {
        var clamp = Math.Clamp(controller.velocity.magnitude, 0f, 1f);
        animator.SetFloat("speed", clamp);
    }

    private bool IsControllerInMove()
    {
        return controller.velocity.magnitude > .5f;
    }

    private bool CheckIfBallIsOnField()
    {
        ballCheckTransform.position = ballTransform.position; //we need to eliminate rotation
        if (Physics.Raycast(ballCheckTransform.position, ballCheckTransform.TransformDirection(-Vector3.up),
                out var hit, Mathf.Infinity, groundMask))
        {
            Debug.DrawRay(ballCheckTransform.position,
                ballCheckTransform.TransformDirection(-Vector3.up) * hit.distance, Color.green);
            return true;
        }

        Debug.DrawRay(ballCheckTransform.position, ballCheckTransform.TransformDirection(-Vector3.up) * 1000,
            Color.red);
        return false;
    }

    private void MoveToBall()
    {
        Vector3 ballPosNoY = new Vector3(ballTransform.position.x, _transform.position.y, ballTransform.position.z);
        Vector3 ballPosition = ballPosNoY - _transform.position;
        controller.Move(ballPosition * movementSpeed * Time.deltaTime);
    }

    private void RotateToBall()
    {
        Vector3 relativePos = ballTransform.position - _transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        _transform.rotation = Quaternion.Slerp(_transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        _transform.localEulerAngles = new Vector3(0, _transform.localEulerAngles.y, 0);
    }
}