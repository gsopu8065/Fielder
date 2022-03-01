using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FielderMovement : MonoBehaviour
{

    CharacterController controller;

    [SerializeField] float speed;
    [SerializeField] GameObject ball;
    [SerializeField] GameObject playerBody;



    private Vector3 velocity;
    private bool isGrounded;
    private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundMask;
    private float gravity = -9.8f;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        //StartCoroutine(DoRotationAtTargetDirection(playerBody.transform));
    }

    // Update is called once per frame
    void Update()
    {
        if (!ball) return;

        isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (isGrounded)
        {
            Vector3 ballPosition = ball.transform.position - transform.position;
            controller.Move(ballPosition * speed * Time.deltaTime);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);


        transform.LookAt(ball.transform);

        // The step size is equal to speed times frame time.
        //var step = speed * Time.deltaTime;

        // Rotate our transform a step closer to the target's.
        //playerBody.transform.rotation = Quaternion.RotateTowards(playerBody.transform.rotation, ball.transform.rotation, step);

        /* if (ballPosition != Vector3.zero)
         {
             // Determine which direction to rotate towards
             Vector3 targetDirection = ballPosition - playerBody.transform.position;

             // The step size is equal to speed times frame time.
             float singleStep = 1.0f * Time.deltaTime;

             // Rotate the forward vector towards the target direction by one step
             Vector3 newDirection = Vector3.RotateTowards(playerBody.transform.forward, targetDirection, singleStep, 0.0f);

             // Draw a ray pointing at our target in
             Debug.DrawRay(playerBody.transform.position, newDirection, Color.red);

             // Calculate a rotation a step closer to the target and applies rotation to this object
             playerBody.transform.rotation = Quaternion.LookRotation(newDirection);
         }*/
    }

    IEnumerator DoRotationAtTargetDirection(Transform opponentPlayer)
    {
        Quaternion targetRotation = Quaternion.identity;
        do
        {
            Debug.Log("do rotation");

            Vector3 targetDirection = (opponentPlayer.position - transform.position).normalized;
            targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime);

            yield return null;

        } while (Quaternion.Angle(transform.rotation, targetRotation) > 0.01f);
    }
}
