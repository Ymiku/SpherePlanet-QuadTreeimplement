using UnityEngine;
using System.Collections;

public class GravityAttractor : MonoBehaviour
{
    public float forceGravity = -10.0f;

    public void Attract ( GravityBody body )
    {
        Transform bodyTrans = body.transform;
        Rigidbody rbBody = body.GetComponent<Rigidbody>();

        Vector3 gravityUp = (bodyTrans.position - transform.position).normalized;

        //Apply the gravity
        rbBody.AddForce( gravityUp * forceGravity * rbBody.mass );
        rbBody.drag = body.grounded == 0 ? 0.1f : 1.0f;

        //For the player controller
        if(rbBody.freezeRotation)
        {
            Quaternion q = Quaternion.FromToRotation(bodyTrans.up, gravityUp) * bodyTrans.rotation;
            bodyTrans.rotation = Quaternion.Slerp(bodyTrans.rotation, q, 0.1f);
        }
    }
}