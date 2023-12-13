using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    // naming constraints do not change
    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    [SerializeField] private Transform SensorFR;
    [SerializeField] private Transform SensorL1;
    [SerializeField] private Transform SensorL2;
    [SerializeField] private Transform SensorL3;
    [SerializeField] private Transform SensorR1;
    [SerializeField] private Transform SensorR2;
    [SerializeField] private Transform SensorR3;
    [SerializeField] private Transform SensorOR;

    [SerializeField] private float maxSteeringAngle;
    [SerializeField] private float motorForce;
    [SerializeField] private float brakeForce;

    private Rigidbody rb;
    //display y angle x angle and velocity jiust display not going to use it its our job to do it on our course work
    [SerializeField] private float angle_x;
    [SerializeField] private float angle_z;
    [SerializeField] private float velocity;

    private float steerAngle;
    private bool isBreaking;
    // how far sensis will reach, left and right symetrically set it, like sensor l1,L2
    private float s1dist = 5; //let say sensose 1
    private float s3dist = 4;
    //sensors set x axxies y axis and z axis
    private void AdjustSensors(Transform sensor,float x_angle,float y_angle,float z_angle)//sensor is game objetct it slef and other are valuues
    {
        sensor.transform.Rotate(x_angle, y_angle, z_angle);
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        //how musch we rotate the sensos
        float s1x = 0;
        float s1y = 10;
        float s1z = 0;

        float s3x = 16;
        float s3y = 50;
        float s3z = 0;

        //we need to utilize the sensor function,, orrientation opf game objetect change
        AdjustSensors(SensorFR, 20, 0, 0);
        AdjustSensors(SensorL1, s1x, -s1y, s1z); //we make y - becz move opostie direction
        AdjustSensors(SensorL3, s3x, -s3y, s3z);
        AdjustSensors(SensorR1, s1x, s1y, s1z);
        AdjustSensors(SensorR3, s3x, s3y, s3z);
        AdjustSensors(SensorOR, 50, 180, 0); // ag game object mifddle of object like gradient is more in one side then it will give power 
    }

    private void FixedUpdate()
    {
        StayOnROad();
        AvoidObstacles();
        AdjustSpeed();
        HandleMotor();
        UpdateWheels();

        angle_x = SensorOR.eulerAngles.x;
        angle_z = SensorOR.eulerAngles.z;

        velocity = rb.velocity.magnitude;
    }
    private void HandleMotor()
    {
        // car is moving without oressing any button
        frontLeftWheelCollider.motorTorque = motorForce;
        frontRightWheelCollider.motorTorque = motorForce;
        rearLeftWheelCollider.motorTorque = motorForce;
        rearRightWheelCollider.motorTorque = motorForce;

        brakeForce = isBreaking ? 3000f : 0f;
        frontLeftWheelCollider.brakeTorque = brakeForce;
        frontRightWheelCollider.brakeTorque = brakeForce;
        rearLeftWheelCollider.brakeTorque = brakeForce;
        rearRightWheelCollider.brakeTorque = brakeForce;
    }
    private void UpdateWheelsPos(WheelCollider wheelCollider, Transform trans)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        trans.rotation = rot;
        trans.position = pos;
    }
    private void UpdateWheels()
    {
        UpdateWheelsPos(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateWheelsPos(frontRightWheelCollider, frontRightWheelTransform);
        UpdateWheelsPos(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateWheelsPos(rearRightWheelCollider, rearRightWheelTransform);
    }

    private void HandleSteering(float direction)
    {
        steerAngle = maxSteeringAngle * direction;
        frontLeftWheelCollider.steerAngle = steerAngle;
        frontRightWheelCollider.steerAngle = steerAngle;
    }
    //visiuyaliize sensor
    private bool sense(Transform sensor, float dist)
    {
        if (Physics.Raycast(sensor.position, sensor.TransformDirection(Vector3.forward), dist))
        {
            Debug.DrawRay(sensor.position, sensor.TransformDirection(Vector3.forward) * dist, Color.red);//red for more clear
            return true;
        }
        else
        {
            Debug.DrawRay(sensor.position, sensor.TransformDirection(Vector3.forward) * dist, Color.white);
            return false;
        }
    }
        //don't come from road
        private void StayOnROad()
        {
            if (!sense(SensorL3,s3dist) || !sense(SensorR3, s3dist))
            {
                if (!sense(SensorL3,s3dist))
                {
                    HandleSteering(1);
                }
                if (!sense(SensorR3, s3dist))
                {
                    HandleSteering(-1);
                }
                else
                {
                    HandleSteering(0);
                }
            }
        }
    private void AdjustSpeed()
    {
        if(velocity < 2 & motorForce < 50)
        {
            motorForce = motorForce + 0.5f;
        }
        if (velocity > 4 & motorForce > 0)
        {
            motorForce = motorForce - 0.5f;
        }
    }

    private void AvoidObstacles()
    {
        if (sense(SensorL1, s1dist))
        {
            HandleSteering(1);
        }
        if (sense(SensorR1, s1dist))
        {
            HandleSteering(-1);
        }
    }
   
}