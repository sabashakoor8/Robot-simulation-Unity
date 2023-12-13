using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    [SerializeField] private WheelCollider FLWC;
    [SerializeField] private WheelCollider FRWC;
    [SerializeField] private WheelCollider BLWC;
    [SerializeField] private WheelCollider BRWC;

    [SerializeField] private Transform LeftSensor;
    [SerializeField] private Transform RightSensor;
    [SerializeField] private Transform CentreSensor;

    public Transform FLWT;
    public Transform FRWT;
    public Transform BLWT;
    public Transform BRWT;

    [SerializeField] private float MAngle = 30f;
    [SerializeField] private float Mforce = 50f;
    [SerializeField] private float BForce = 0f;

    private float horizontalInput;
    private float verticalInput;
    private float steerAngle;
    private bool isBreaking;

    // Start is called before the first frame update
    void Start()
    {
        LeftSensor.transform.Rotate(0, -10, 0);
        RightSensor.transform.Rotate(0, 10, 0);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        GetInput();
        HandleSteering();
        HandleMotor();
        UpdateWheels();
        sense(CentreSensor, 5);
        sense(LeftSensor, 5);
        sense(RightSensor, 5);
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        isBreaking = Input.GetKey(KeyCode.Space);
    }

    private void HandleSteering()
    {
        steerAngle = MAngle * horizontalInput;

        if (sense(CentreSensor, 5) || sense(LeftSensor,5) || sense(RightSensor, 5))
        {
            if (sense(LeftSensor, 5))
            {
                steerAngle = MAngle;
            }
            if (sense(RightSensor, 5))
            {
                steerAngle = -MAngle;
            }
            
            
        }


        FLWC.steerAngle = steerAngle;
        FRWC.steerAngle = steerAngle;
    }

    private void HandleMotor()
    {
        FLWC.motorTorque = verticalInput * Mforce;
        FRWC.motorTorque = verticalInput * Mforce;

        BForce = isBreaking ? 3000f : 0f;
        FLWC.brakeTorque = BForce;
        FRWC.brakeTorque = BForce;
        BLWC.brakeTorque = BForce;
        BRWC.brakeTorque = BForce;
    }

    private void UpdateWheelPos(WheelCollider wheelCollider, Transform trans)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        trans.rotation = rot;
        trans.position = pos;
    }

    private void UpdateWheels()
    {
        UpdateWheelPos(FLWC, FLWT);
        UpdateWheelPos(FRWC, FRWT);
        UpdateWheelPos(BLWC, BLWT);
        UpdateWheelPos(BRWC, BRWT);
    }

    private bool sense(Transform sensor, float dist)
    {
        RaycastHit hit;
        if (Physics.Raycast(sensor.position, sensor.TransformDirection(Vector3.forward), out hit, dist))
        {
            Debug.DrawRay(sensor.position, sensor.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            return true;
        }
        else
        {
            Debug.DrawRay(sensor.position, sensor.TransformDirection(Vector3.forward) * dist, Color.white);
            return false;
        }
    }
}
