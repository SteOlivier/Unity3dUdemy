using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketZY : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] bool MoveCameraOnEnable = false;
    [SerializeField] Vector3 CameraTransformOnEnable = Vector3.zero;
    [SerializeField] Vector3 CameraRotationOnEnable = Vector3.zero;
    [SerializeField] float Z_DepthIncrease = 14;


    [SerializeField] float rcsThrust = 10f;
    [SerializeField] float boostThrust = 60f;
    [SerializeField] float rcsThrustIncrease = 45f;
    [SerializeField] float maxBoostVelocity = 10f;
    [SerializeField] float levelLoadDelay = 6f;

    float rcsExtraThrust = 0f;
    float thrustExtraForce = 0f;
    float thrustExtraForceX = 0f;

    FollowRocket followRocket = null;
    Rigidbody rigidBody;
    private bool _enable = false;//{ get; set; }
    public bool Enabled
    {
        get
        {
            return _enable;
        }
        set
        {
            if (_enable == false && value == true && MoveCameraOnEnable && followRocket != null)
            {
                followRocket.initailRotation = CameraRotationOnEnable;
                followRocket.initialTransform = CameraTransformOnEnable;
                followRocket.SetCameraInitial = true;
                followRocket.followRocketUnitVector = Vector3.forward;
            }
            _enable = value;
        }
    }
    
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        followRocket = GetComponent<FollowRocket>();
        if (followRocket == null)
        {
            var camera = GameObject.Find("Main Camera");
            if (camera != null) followRocket = camera.GetComponent<FollowRocket>();
            //print(camera);
        }
        //print(followRocket);
    }

    // Update is called once per frame
    void Update()
    {
        if(Enabled) //Keys
        {
            if (followRocket != null && followRocket.SetCameraInitial == false)
            {
                if (Mathf.Abs(Z_DepthIncrease) > float.Epsilon)
                {
                    int direction = Z_DepthIncrease >= 0 ? 1 : -1;
                    transform.position += direction * Vector3.forward * Time.deltaTime;

                    Z_DepthIncrease -= direction * Time.deltaTime;
                    int direction2 = Z_DepthIncrease >= 0 ? 1 : -1;
                    if (direction != direction2)
                    {
                        Z_DepthIncrease = 0f;
                        // Set once of constraints for different directions
                        rigidBody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                    }
                }
                else // Setup is ready, start flying in Z directionals
                {
                    ProcessInput();
                }
            }
           // print("Hardieharhar");
        }
    }
    void ProcessInput()
    {
        Thrust();
    }
    void Thrust()
    {
        //print(rigidBody.velocity);
        ExtraThrust();

        float forcePerFrame = (boostThrust * thrustExtraForce) * Time.deltaTime;
        float forcePerFrameX = (boostThrust * thrustExtraForceX) * Time.deltaTime;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space))
        {
            //print("up: " + forcePerFrame);

            rigidBody.AddForce((Vector3.up * forcePerFrame * Mathf.Cos(rigidBody.rotation.eulerAngles.z * 0.01745329251f)) + (Vector3.left * forcePerFrameX * Mathf.Sin(rigidBody.rotation.eulerAngles.z * 0.01745329251f)));

            //if (!thrusterSound.isPlaying && playerState == Rocket_State.alive)
            {
                //thrusterSound.Play();
                //mainEngineParticles.Play();
            }

        }
        else
        {
            //thrusterSound.Stop();
            //mainEngineParticles.Stop();
        }
    }

    private void ExtraThrust()
    {
        var maxDirectionalVelocity = Max(rigidBody.GetRelativePointVelocity(Vector3.zero).y);
        var maxDirectionalVelocityX = Max(rigidBody.GetRelativePointVelocity(Vector3.zero).z, -rigidBody.GetRelativePointVelocity(Vector3.zero).z);
        //print(maxDirectionalVelocityX + "<->" + rigidBody.GetRelativePointVelocity(Vector3.zero).x);


        if (maxBoostVelocity > maxDirectionalVelocity) thrustExtraForce = maxBoostVelocity - maxDirectionalVelocity;
        else thrustExtraForce = 0;

        if (maxBoostVelocity * 2 > maxDirectionalVelocityX) thrustExtraForceX = maxBoostVelocity * 2 - maxDirectionalVelocityX;
        else thrustExtraForceX = 0;
        //print("dir: " + thrustExtraForceX);

        int angle = (int)rigidBody.rotation.eulerAngles.x;

        angle = angle - (angle / 360) * 360;
        if (angle < 0) angle = angle + 360;
        var quadrant = (angle / 90) % 4;
        //var positive = (quadrant == 3 || quadrant == 2);
        //if (positive) print(angle);

        if (!(sameSign((quadrant == 3 || quadrant == 2) ? 1 : -1, rigidBody.GetRelativePointVelocity(Vector3.zero).z)) || (int)maxDirectionalVelocityX == 0)
        {
            thrustExtraForceX = maxBoostVelocity * 2;
            //if ((int)maxDirectionalVelocityX != 0) print("quad: " + quadrant); 
        }
    }

    bool sameSign(float num1, float num2)
    {
        return num1 >= 0 && num2 >= 0 || num1 < 0 && num2 < 0;
    }

    float Max(params float[] values)
    {
        float maxValue = 0f;
        foreach (var item in values)
        {
            if (item > maxValue) maxValue = item;
        }
        return maxValue;
    }
}
