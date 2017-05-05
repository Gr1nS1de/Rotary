using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTorqueComponent : MonoBehaviour 
{
	public float TorqueSpeed = 0f;
	public float MaxAngularSpeed = 0f;
	void Start()
	{
		//GetComponent<Rigidbody2D> ().AddTorque (1f);
		//GetComponent<Rigidbody2D> ().angularVelocity = 5f;

	}
	void FixedUpdate()
	{
		//GetComponent<Rigidbody2D> ().AddTorque (TorqueSpeed * Time.fixedDeltaTime);
		GetComponent<Rigidbody2D> ().angularVelocity = TorqueSpeed;
	}

}
