using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowRocket : MonoBehaviour
{
    // Start is called before the first frame update
    Transform rocketTransform;
    void Start()
    {
        var gameObjectShip = GameObject.Find("3Side Rocket Ship Variant");
        rocketTransform = gameObjectShip.transform;

    }

    // Update is called once per frame
    void Update()
    {
        FollowRocket_X();

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
}
