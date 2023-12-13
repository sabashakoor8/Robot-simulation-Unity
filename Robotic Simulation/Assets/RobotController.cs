using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    // naming constraints do not change
    [SerializeField] private WheelCollider FLWC;
    [SerializeField] private WheelCollider FRWC;
    [SerializeField] private WheelCollider BLWC;
    [SerializeField] private WheelCollider BRWC;

    [SerializeField] private Transform FLWT;
    [SerializeField] private Transform FRWT;
    [SerializeField] private Transform BLWT;
    [SerializeField] private Transform BRWT;

    [SerializeField] private Transform SFR;
    [SerializeField] private Transform SL1;
    [SerializeField] private Transform SL2;
    [SerializeField] private Transform SL3;
    [SerializeField] private Transform SR1;
    [SerializeField] private Transform SR2;
    [SerializeField] private Transform SR3;
    [SerializeField] private Transform SOR;

    [SerializeField] private float maxSteeringAngle = 20;
    [SerializeField] private float motorForce = 10;
    [SerializeField] private float brakeForce;
    
    private Rigidbody rbody;

    [SerializeField] private float angle_x;
    [SerializeField] private float angle_y;
    [SerializeField] private float angle_z;
    [SerializeField] private float velocity;

   // [SerializeField] LayerMask mask;

    private float horizontalInput;
    private float verticalInput;
    private float steerAngle;
    private bool isBreaking;

    private float s1_distance = 6;
    private float s2_distance = 6;
    private float s3_distance = 12;
    //private float s0_distance = 5;
    
    private void AdjustSensors(Transform sensor, float x_angle, float y_angle, float z_angle)
    {
       sensor.transform.Rotate(x_angle, y_angle, z_angle);
    }

    private void HandleMotor()
    {
        FLWC.motorTorque = motorForce;
        FRWC.motorTorque = motorForce;
        BRWC.motorTorque = motorForce;
        BLWC.motorTorque = motorForce;

        brakeForce = isBreaking ? 3000f : 0f;
       // FRWC.brakeTorque

        FRWC.brakeTorque = brakeForce;
        FLWC.brakeTorque = brakeForce;
        BLWC.brakeTorque = brakeForce;
        BRWC.brakeTorque = brakeForce;
    }

    private void Start()
    {
        rbody = GetComponent<Rigidbody>();
        float s1x = 6; float s1y = 12; float s1z = 0;
        float s2x = 6; float s2y = 18; float s2z = 0;
        float s3x = 11; float s3y = 75; float s3z = 0;
        AdjustSensors(SFR, 6, 0, 0);
        AdjustSensors(SL1, s1x, -s1y, s1z);
        AdjustSensors(SL2, s2x, -s2y, s2z);
        AdjustSensors(SL3, s3x, -s3y, s3z);
        AdjustSensors(SR1, s1x, s1y, s1z);
        AdjustSensors(SR2, s2x, s2y, s2z);
        AdjustSensors(SR3, s3x, s3y, s3z);
        AdjustSensors(SOR, 60, 0, 0);
    }

    private void FixedUpdate()
    {
        StaysOnRoad();
        IgnoreObstacles();
        FrontSensor();
        SpeedAdjustment();
       HandleMotor();
        UpdateWheels();
        angle_x = SOR.eulerAngles.x;
        angle_y = SOR.eulerAngles.y;
        angle_z = SOR.eulerAngles.z;
        velocity = rbody.velocity.magnitude;
        //Console.WriteLine("ABc");
    }
    private void UpdateWheelPos(WheelCollider wheelCollider, Transform transform)
    {
        Vector3 position;
        Quaternion rotation;
        wheelCollider.GetWorldPose(out position, out rotation);
        transform.rotation = rotation;
        transform.position = position;
    }
    private void UpdateWheels()
    {
        UpdateWheelPos(FLWC, FLWT);
        UpdateWheelPos(FRWC, FRWT);
        UpdateWheelPos(BLWC, BLWT);
        UpdateWheelPos(BRWC, BRWT);
    }


    private void SteeringHandle(float direction)
    {
        steerAngle = maxSteeringAngle * direction;
        FLWC.steerAngle = steerAngle;
        FRWC.steerAngle = steerAngle;
    }
    
    private bool SenseFunc(Transform sensor, float distance)
    {
        //int mask = 5 << LayerMask.NameToLayer("Ignore Raycast");
        //LayerMask mask = LayerMask.GetMask("road");
        // if (Physics.Raycast(sensor.position, sensor.TransformDirection(Vector3.forward), distance, LayerMask.NameToLayer("Nothing")))
        // {
        //    Debug.DrawRay(sensor.position, sensor.TransformDirection(Vector3.forward) * distance, Color.yellow);
        //    return true;
        // }
        // else
        // {
        //    Debug.DrawRay(sensor.position, sensor.TransformDirection(Vector3.forward) * distance, Color.white);
        //    return false;
        // }
         RaycastHit hit;
         LayerMask mask = LayerMask.GetMask("Road", "Obstacles");
      //  LayerMask mask2 = LayerMask.GetMask("Obstacles");
        if (Physics.Raycast(sensor.position,sensor.TransformDirection(Vector3.forward), out hit,distance, mask))
        {
            Debug.DrawRay(sensor.position, sensor.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            return true;
        }
        else
        {
            Debug.DrawRay(sensor.position, sensor.TransformDirection(Vector3.forward) * distance, Color.white);
            return false;
        }
    }

    private void FrontSensor()
    {
       if (SenseFunc (SFR,6))
       {
        SteeringHandle(0);
       }
    }
    private void StaysOnRoad()
    {
        if (!SenseFunc(SL3,s3_distance) || !SenseFunc(SR3, s3_distance))
        {
            if (!SenseFunc(SL3, s3_distance) )
            {
                SteeringHandle(1);
            }
            if (!SenseFunc(SR3,s3_distance))
            {
                SteeringHandle(-1);
            }
        }
        else
        {
            SteeringHandle(0);
        }
    }
    private void SpeedAdjustment()
    {
        if (velocity < 2 & motorForce < 85)
        {
            motorForce = motorForce + 110f;
        }
        if (velocity > 4.5 & motorForce > 0)
        {
            motorForce = motorForce - 5.5f;
        }
        // if (velocity < 2 & motorForce == 110 & angle_y > 90 & angle_y < 130)
        // {
        //     //Debug.Log("Check curve");
        //     motorForce = motorForce + 150.5f;
        // }
        // if (velocity < 2 & motorForce >= 105.5 & angle_y > 230 & angle_x < 65)
        // {
        //      motorForce = 650.5f;
        // }
        if (angle_x <= 59 & angle_y > 225)
        {
             motorForce = motorForce + 2f;
        }
        if (angle_x <= 59 & angle_y > 65)
        {
             motorForce = motorForce + 2f;
        }
        if (angle_x >= 59 & angle_y < 170)
        {
             motorForce = motorForce - 2f;
        }
        if (angle_x <= 56 & angle_y < 64)
        {
             motorForce = motorForce + 0.5f;
        }

    }
    private void IgnoreObstacles()
    {
        //if (SenseFunc(SFR, 4) || SenseFunc(SL1, s1_distance) || SenseFunc(SR1, s1_distance))
     //  {
            if (SenseFunc(SL1, s1_distance) || SenseFunc(SL2, s2_distance) || SenseFunc(SFR, s2_distance))
            {
                SteeringHandle(1);
            }
            if (SenseFunc(SR1, s1_distance) || SenseFunc(SR2, s2_distance) || SenseFunc(SFR, s2_distance))
            {
                SteeringHandle(-1);
            }
    }
}