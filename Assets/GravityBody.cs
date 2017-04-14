using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class GravityBody : MonoBehaviour
{

    public GravityAttractor attractor;
    public int grounded;

    void Start ()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.WakeUp();
        rb.useGravity = false;
    }

    void OnCollisionEnter(Collision c)
    {
          ++grounded;
    }

   void OnCollisionExit(Collision c)
    {
          --grounded;
    }

    void FixedUpdate()
    {
        if(attractor)
        {
            attractor.Attract(this);
        }
    }
}