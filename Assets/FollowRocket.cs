using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowRocket : MonoBehaviour
{
    // Start is called before the first frame update
    Transform rocketTransform;
    public bool SetCameraInitial = false;
    public Vector3 initialTransform;
    public Vector3 initailRotation;
    [SerializeField] public Vector3 followRocketUnitVector = Vector3.right;
    void Start()
    {
        var gameObjectShip = GameObject.Find("3Side Rocket Ship Variant");
        rocketTransform = gameObjectShip.transform;

    }

    // Update is called once per frame
    void Update()
    {
        if (SetCameraInitial == false)
        {
            FollowRocket_XYZ();
        }
        if (SetCameraInitial == true)
        {
            InchCameraToInitialValues();
        }

    }
    
    void InchCameraToInitialValues()
    {
        if (initialTransform.x > transform.localPosition.x)
        {
            transform.localPosition += 1 * Time.deltaTime * Vector3.right;
        }
        if (initialTransform.y > transform.localPosition.y)
        {
            transform.localPosition += 1 * Time.deltaTime * Vector3.up;
        }
        if (initialTransform.z > transform.localPosition.z)
        {
            transform.localPosition += 1 * Time.deltaTime * Vector3.forward;
        }
        if (initailRotation.x > transform.eulerAngles.x)
        {
            transform.eulerAngles += 1 * Time.deltaTime * Vector3.right;
        }
        if (initailRotation.y < transform.eulerAngles.y)
        {
            transform.eulerAngles += 2 * Time.deltaTime * Vector3.down;
            /// Fixing a negative value bug
            if (initailRotation.y < 0 && initailRotation.y + 360 < transform.eulerAngles.y)
            {
                initailRotation.y += 360;
            }
            //print(transform.eulerAngles);
        }
        if (initailRotation.z > transform.eulerAngles.z)
        {
            transform.eulerAngles += 1 * Time.deltaTime * Vector3.forward;
        }

        /// Transforms
        if (initialTransform.x <= transform.localPosition.x && initialTransform.y <= transform.localPosition.y && initialTransform.z <= transform.localPosition.z &&
                /// Rotations
                initailRotation.x <= transform.eulerAngles.x && initailRotation.y >= transform.eulerAngles.y && initailRotation.z <= transform.eulerAngles.z
            )
        {
            SetCameraInitial = false;
        }
    }

    void FollowRocket_XYZ()
    {
        if (followRocketUnitVector.x - 0.9 > float.Epsilon) FollowRocket_X();
        if (followRocketUnitVector.z - 0.9 > float.Epsilon) FollowRocket_Z();
    }
    void FollowRocket_X()
    {
        var magnitudeDifference_x = Mathf.Abs(rocketTransform.position.x - transform.position.x);
        if (magnitudeDifference_x > 4)
        {
            if (rocketTransform.position.x > transform.position.x)
            {
                transform.Translate(Vector3.right * Time.deltaTime * magnitudeDifference_x);
            }
            else
            {
                transform.Translate(Vector3.left * Time.deltaTime * magnitudeDifference_x);
            }
        }
    }
    void FollowRocket_Z()
    {
        var magnitudeDifference_z = Mathf.Abs(rocketTransform.position.z - transform.position.z);
        if (magnitudeDifference_z > 4)
        {
            //print("Rocket: " + rocketTransform.position.z);
            //print("Camera: " + transform.position.z);
            if (rocketTransform.position.z > transform.position.z)
            {
                //transform.Translate(Vector3.forward * Time.deltaTime * magnitudeDifference_z);
                transform.position += (Vector3.forward * Time.deltaTime * magnitudeDifference_z);
            }
            else
            {
                transform.position += (Vector3.back * Time.deltaTime * magnitudeDifference_z);
            }
        }
    }
}
