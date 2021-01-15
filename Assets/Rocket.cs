using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 10f;
    [SerializeField] float boostThrust = 60f;
    [SerializeField] float rcsThrustIncrease = 45f;
    [SerializeField] float maxBoostVelocity = 10f;
    
    [SerializeField] AudioClip audioDie;
    Rigidbody rigidBody;
    //[SerializeField]
    AudioSource thrusterSound;
    //AudioSource playAudio;
    float rcsExtraThrust = 0f;
    float thrustExtraForce = 0f;
    float thrustExtraForceX = 0f;
    Rocket_State playerState;
    enum Rocket_State { alive, dead };
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        if (thrusterSound == null)
        {
            thrusterSound = GetComponent<AudioSource>();
        }
        playerState = Rocket_State.alive;
    }
    
    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        if (playerState != Rocket_State.dead)
        {
            Thrust();
            Rotate();
        }
        //ResetRotation();
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

    bool sameSign(float num1, float num2)
    {
        return num1 >= 0 && num2 >= 0 || num1 < 0 && num2 < 0;
    }

    private void ExtraThrust()
    {
        var maxDirectionalVelocity = Max(rigidBody.GetRelativePointVelocity(Vector3.zero).y);
        var maxDirectionalVelocityX = Max(rigidBody.GetRelativePointVelocity(Vector3.zero).x, -rigidBody.GetRelativePointVelocity(Vector3.zero).x);
        //print(maxDirectionalVelocityX + "<->" + rigidBody.GetRelativePointVelocity(Vector3.zero).x);


        if (maxBoostVelocity > maxDirectionalVelocity) thrustExtraForce = maxBoostVelocity - maxDirectionalVelocity;
        else thrustExtraForce = 0;

        if (maxBoostVelocity * 2 > maxDirectionalVelocityX) thrustExtraForceX = maxBoostVelocity * 2 - maxDirectionalVelocityX;
        else thrustExtraForceX = 0;
        //print("dir: " + thrustExtraForceX);

        int angle = (int)rigidBody.rotation.eulerAngles.z;

        angle = angle - (angle / 360) * 360;
        if (angle < 0) angle = angle + 360;
        var quadrant = (angle / 90) % 4;
        //var positive = (quadrant == 3 || quadrant == 2);
        //if (positive) print(angle);

        if (!(sameSign((quadrant == 3 || quadrant == 2) ? 1 : -1, rigidBody.GetRelativePointVelocity(Vector3.zero).x)) || (int)maxDirectionalVelocityX == 0)
        {
            thrustExtraForceX = maxBoostVelocity * 2;
            //if ((int)maxDirectionalVelocityX != 0) print("quad: " + quadrant); 
        }
    }


    private void Thrust()
    {
        //print(rigidBody.velocity);
        ExtraThrust();

        float forcePerFrame = (boostThrust * thrustExtraForce) * Time.deltaTime;
        float forcePerFrameX = (boostThrust * thrustExtraForceX) * Time.deltaTime;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space))
        {
            //print("up: " + forcePerFrame);
            
            rigidBody.AddForce((Vector3.up*forcePerFrame * Mathf.Cos(rigidBody.rotation.eulerAngles.z * 0.01745329251f)) + (Vector3.left * forcePerFrameX * Mathf.Sin(rigidBody.rotation.eulerAngles.z * 0.01745329251f)));

            if (!thrusterSound.isPlaying && playerState == Rocket_State.alive)
            {
                thrusterSound.Play();
            }
 
        }
        else
        {
            thrusterSound.Stop();
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                print("You and me together we'll be forewver you'll see");
                break;
            case "Fuel":
                break;
            case "Finnish":
                break;
            default:
                print("dead");
                if (playerState != Rocket_State.dead)
                {
                    Invoke("DeadlySound", 0.05f);
                    
                }
                break;
        }
    }

    void DeadlySound()
    {
        playerState = Rocket_State.dead;
        thrusterSound.Stop();
        thrusterSound.PlayOneShot(audioDie);
        //thrusterSound.Stop();

        // audioDie.PlayOneShot();
    }

    private void Rotate()
    {
        
        float rotationPerFrame = Time.deltaTime * rcsThrust* (1+rcsExtraThrust);
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationPerFrame );
            rcsExtraThrust += rcsThrustIncrease * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.back * rotationPerFrame);
            rcsExtraThrust += rcsThrustIncrease * Time.deltaTime;
        }
        else
        {
            rcsExtraThrust = 0f;
        }
        
    }
}
