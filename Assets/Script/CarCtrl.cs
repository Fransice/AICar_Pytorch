using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCtrl : Agent
{
    public WheelCollider[] wheels;
    public Transform[] tran;
    public Rigidbody rigidbody;
    public Vector3 center;
    public float maxAngle;
    public float maxMotor;
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        rigidbody.centerOfMass = center;
    }
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        wheels[0].steerAngle = x * maxAngle;
        wheels[1].steerAngle = x * maxAngle;


        wheels[2].motorTorque = y * maxMotor;
        wheels[3].motorTorque = y * maxMotor;
        SetWc();
    }
    private void SetWc()
    {
        for (int i = 0; i < 4; i++)
        {
            Vector3 pos;
            Quaternion qua;
            wheels[i].GetWorldPose(out pos, out qua);
            tran[i].position = pos;
            tran[i].rotation = qua;
        }
    }
    double[] x = new double[2] { 0.1f, 0.2f };
    public override double[] GetInputs()
    {
        return x;
    }
}
