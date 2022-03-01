using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Ball : MonoBehaviour
{


    [SerializeField]
    [Range(30, 35)]
    private float hitForce = 35;

    [SerializeField]
    [Range(3.5f, 4.25f)]
    private float upForce = 4;


    [SerializeField]
    private Vector2 _randomHitForceRange = new Vector2(30f, 35f);


    [SerializeField]
    private Vector2 _randomUpForceRange = new Vector2(3.5f, 4.25f);


    [SerializeField]
    private bool randomBall;


    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            ThrowBall();
        } 
    }

    private void ThrowBall()
    {
        _rigidbody.isKinematic = false;
        Vector3 step = new Vector3(0.370000005f, 0.340000004f, -10);
        Vector3 dir = step - transform.position; // get the direction to where we want to send the ball
        if (randomBall)
        {
            hitForce = Random.Range(_randomHitForceRange.x, _randomHitForceRange.y);
            upForce = Random.Range(_randomUpForceRange.x, _randomUpForceRange.y);
        }

        _rigidbody.velocity = dir.normalized * hitForce + new Vector3(0, upForce, 0);
    }
}