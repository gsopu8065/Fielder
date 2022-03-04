using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class FielderMovement : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 3;
    [SerializeField] private float movementSpeed = 1;
    [SerializeField] private LayerMask groundMask;

    [Header("Ball")]
    private Transform ballTransform;
    private Transform ballCheckTransform;
    private GameObject ballAfterCollect;

    [Header("Animations")] [SerializeField]
    private Transform highTester;

    [SerializeField] private Transform jumpHighTester;
    [SerializeField] private Transform lowTester;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform catchTransform;

    CharacterController controller;
    private bool isGrounded;
    private Transform _transform;
    #region Unity methods

    private void Awake()
    {
        _transform = transform; //cache transform for better performance!
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        //ballAfterCollect.SetActive(false);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Ball" || ballTransform == null)
            return;
        CopyBallRotationAndPosition();
        Destroy(ballTransform.gameObject);
        ballAfterCollect.SetActive(true);
        SelectProperAnimation();
    }

    private void FixedUpdate()
    {
        if(ballTransform == null)
        {
            GameObject ballRootObject = GameObject.FindGameObjectWithTag("Ball");
            if(ballRootObject != null)
            {
                Transform[] ts = ballRootObject.transform.GetComponentsInChildren<Transform>(true);
                foreach (Transform t in ts) {
                    if (t.gameObject.name == "Ball")
                    {
                        ballTransform = t;
                    }
                    if (t.gameObject.name == "BallGroundChecker")
                    {
                        ballCheckTransform = t;
                    }
                    if (t.gameObject.name == "BallAfterCatch")
                    {
                        ballAfterCollect = t.gameObject;
                    }
                }
            }
        }
        

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

    #endregion

    #region private methods

    private void CopyBallRotationAndPosition()
    {
        ballAfterCollect.transform.position = ballTransform.position;
        ballAfterCollect.transform.rotation = ballTransform.rotation;
    }

    //we have 3 normal animations
    //animation from above and animation picking from the ground
    private void SelectProperAnimation()
    {
        float ballTime = .1f;

        if (ballTransform.position.y > jumpHighTester.position.y)
        {
            animator.SetTrigger("highJump");
            ballTime = .2f;
        }
        else
        {
            if (ballTransform.position.y > highTester.position.y)
            {
                animator.SetTrigger("jump");
            }
            else if (ballTransform.position.y < lowTester.position.y)
            {
                animator.SetTrigger("pickup");
            }
            else
            {
                animator.SetInteger("collect", Random.Range(1, 4));
                Invoke(nameof(ResetCollector), .5f); //need to reset to 0 - do not replay same animation all over again
            }
        }


        Invoke(nameof(MoveBallToCatch), ballTime); //if we dont want to use DoTween
    }

    private void MoveBallToCatch()
    {
        ballAfterCollect.transform.SetParent(catchTransform);
        ballAfterCollect.transform.DOLocalMove(Vector3.zero, .1f).SetEase(Ease.Linear);
        //ballAfterCollect.transform.localPosition = Vector3.zero; //use that if you dont want to use dotween
    }

    private void ResetCollector()
    {
        animator.SetInteger("collect", 0);
    }


    private void SetAnimation()
    {
        var clamp = Math.Clamp(controller.velocity.magnitude, 0f, 3f);
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

    #endregion
}