using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Comms;
using TMPro;

public class JetbotController : WheelDrive
{
    [Header("Remote Control")]
    public WheelCollider LeftMotor;
    public WheelCollider RightMotor;
    public WheelCollider BalancerMotor;
    public Camera Cam;
    [Header("UI")]
    public TextMeshProUGUI ConnectionStatus;
    public TextMeshProUGUI LeftMotorIndicator;
    public TextMeshProUGUI RightMotorIndicator;

    [Header("Tuning")]
    public float BreakDuration = 0.5f;


    #region Comms
    public void OnMessage(JSONObject msg) {
        Debug.Log("Received Command:\n" + msg.Print(true));

        switch (msg["type"].str) {
            case "stop":
                Debug.Log("Stopping");
                StartCoroutine(BrakeCheck());
                break;
            case "motor":
                if (msg["target"].str.Equals("left")) {
                    Debug.Log("Setting Left");
                    LeftMotor.motorTorque = maxTorque * msg["power"].f;
                    LeftMotorIndicator.text = msg["power"].f.ToString();
                }
                else {
                    Debug.Log("Setting Right");
                    RightMotor.motorTorque = maxTorque * msg["power"].f;
                    RightMotorIndicator.text = msg["power"].f.ToString();
                }
                break;
            case "frame request":
                // TODO: implement
                break;
            default:
                break;
        }
    }

    public void OnConnect(ReliableCommunication comm) {
        ConnectionStatus.text = "Connected";
    }

    public void OnDisconnect(ReliableCommunication comm) {
        ConnectionStatus.text = "Not Connected";
    }
    #endregion

    private void Update()
	{
		m_Wheels[0].ConfigureVehicleSubsteps(criticalSpeed, stepsBelow, stepsAbove);

		foreach (WheelCollider wheel in m_Wheels)
		{
			// Update visual wheels if any.
			if (wheelShape) 
			{
				Quaternion q;
				Vector3 p;
				wheel.GetWorldPose (out p, out q);

				// Assume that the only child of the wheelcollider is the wheel shape.
				Transform shapeTransform = wheel.transform.GetChild (0);
                shapeTransform.position = p;
                shapeTransform.rotation = q;
                
			}
		}
	}

    public void Home() {
        StartCoroutine(BrakeCheck());
        this.transform.rotation = new Quaternion();
        this.transform.position = new Vector3(0, 0.15f, 0);
    }

    private IEnumerator BrakeCheck() {
        // Stop Torque
        LeftMotor.motorTorque = 0;
        RightMotor.motorTorque = 0;
        BalancerMotor.motorTorque = 0;
        LeftMotorIndicator.text = "0";
        RightMotorIndicator.text = "0";

        // Tap breaks
        LeftMotor.brakeTorque = brakeTorque;
        RightMotor.brakeTorque = brakeTorque;
        BalancerMotor.brakeTorque = brakeTorque;

        yield return new WaitForSeconds(BreakDuration);
        GetComponent<Rigidbody>().velocity = Vector3.zero;

        LeftMotor.brakeTorque = 0;
        RightMotor.brakeTorque = 0;
        BalancerMotor.brakeTorque = 0;
    }
}
