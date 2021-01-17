using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Occilator : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Vector3 movementVector;
    [SerializeField] Vector3 rotationVector;
    [SerializeField] float period = 4f;

    [Range(0,1)] float movementFactor; // 0 for not 1 for fully moved
    Vector3 startingPos;
    Vector3 startingRot;
    void Start()
    {
        startingPos = transform.position;
        startingRot = transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (period <= Mathf.Epsilon) return;
        float cycles = Time.time / period;
        const float tau = Mathf.PI * 2;
        float rawSign = Mathf.Sin(cycles * tau);
        //print(rawSign);

        movementFactor = rawSign / 2f + 0.5f;

        Vector3 offset = movementVector * movementFactor;
        Vector3 rotationOff = rotationVector * movementFactor;
        transform.position = startingPos + offset;

        transform.Rotate(startingRot + rotationOff - transform.rotation.eulerAngles);


    }
}
