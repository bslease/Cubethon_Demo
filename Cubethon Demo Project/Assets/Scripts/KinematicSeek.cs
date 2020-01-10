using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicSeek : MonoBehaviour
{
    private Vector3 linearVelocity;
    private float angularVelocity;
    public float maxSpeed;
    public Transform target;

    // Update is called once per frame
    void Update()
    {
        SteeringOutput steering = getSteering();
        transform.position += steering.linearVelocity * Time.deltaTime;
    }

    // function: return a maxspeed velocity toward our current target and snap our orientation to face it
    // cf. Millington p. 52 
    SteeringOutput getSteering()
    {
        SteeringOutput result = new SteeringOutput();

        // Get the direction to the target
        result.linearVelocity = target.position - this.transform.position;

        // Set velocity to be along this direction at maxspeed
        result.linearVelocity = result.linearVelocity.normalized * maxSpeed;

        // Face in the direction we want to move
        float angle = newOrientation(transform.eulerAngles.y, result.linearVelocity);
        angle *= Mathf.Rad2Deg;
        this.transform.eulerAngles = new Vector3(0, angle, 0);

        result.angularVelocity = 0;
        return result;
    }

    // function: returns an orientation in radians pointing in the direction of a given velocity
    // cf. Millington p. 51 
    float newOrientation(float currentOrientation, Vector3 velocity)
    {
        // return Mathf.Atan2(velocity.x, velocity.z); // result is (in radians?)
        if (velocity.magnitude > 0)
        {
            return Mathf.Atan2(velocity.x, velocity.z); // result is in radians
        }
        else
        {
            return currentOrientation;
        }
    }
}
