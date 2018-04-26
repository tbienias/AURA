using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Motor : MonoBehaviour {
	
	// Maximal range for the motor bounds
	public const float BOUNDS_MAX_RANGE = 1.0f;

	// Motor ID
	public string MotorId;
	[Range(1.0f, 5.0f)]
	public float MotorSpeed;

	// Bounds the effector can move in relation to mounting point of the motor
	[Range(0.0f, BOUNDS_MAX_RANGE)]
	public float boundsPosX;
	[Range(0.0f, BOUNDS_MAX_RANGE)]
	public float boundsPosY;
	[Range(0.0f, BOUNDS_MAX_RANGE)]
	public float boundsPosZ;
	[Range(-BOUNDS_MAX_RANGE, 0.0f)]
	public float boundsNegX;
	[Range(-BOUNDS_MAX_RANGE, 0.0f)]
	public float boundsNegY;
	[Range(-BOUNDS_MAX_RANGE, 0.0f)]
	public float boundsNegZ;

	// The gameobjects created from boundings and effector prefab
	private GameObject boundings;
	private GameObject effector;

	// only for animation to make effector movement smooth
	private int AnimateStepCount = 0;
	private int AnimateDirection = 0;

	// Use this for initialization
	void Start () {

		// Create boundings and effector
		boundings = Instantiate(Resources.Load("Motor/Prefab_Boundings", typeof(GameObject)) as GameObject);
		effector = Instantiate(Resources.Load("Motor/Prefab_Effector", typeof(GameObject)) as GameObject);

		// Set gameobject this script is attached to as parent
		boundings.transform.SetParent(gameObject.transform, false);
		effector.transform.SetParent(gameObject.transform, false);

		// Set scale and offset
		boundings.transform.localScale = new Vector3(boundsPosX - boundsNegX, boundsPosY - boundsNegY, boundsPosZ - boundsNegZ);
		boundings.transform.localPosition = new Vector3(boundsPosX, boundsPosY, boundsPosZ) - (boundings.transform.localScale / 2.0f);

	}

	// Update is called once per frame
	void Update () {
		// HandleInput ();
		Animate();
	}

	// Move the effector in motor boundings
	void MoveEffector (Vector3 diff) {
		float clampedX = Mathf.Clamp (effector.transform.localPosition.x + diff.x * MotorSpeed, boundsNegX, boundsPosX);
		float clampedY = Mathf.Clamp (effector.transform.localPosition.y + diff.y * MotorSpeed, boundsNegY, boundsPosY);
		float clampedZ = Mathf.Clamp (effector.transform.localPosition.z + diff.z * MotorSpeed, boundsNegZ, boundsPosZ);
		effector.transform.localPosition = new Vector3 (clampedX, clampedY, clampedZ);
	}

	// Handle key input to move the motor
	void HandleInput() {
		if (Input.GetKey(KeyCode.L)) MoveEffector(new Vector3(0.01f, 0.0f, 0.0f));
		if (Input.GetKey(KeyCode.J)) MoveEffector(new Vector3(-0.01f, 0.0f, 0.0f));
		if (Input.GetKey(KeyCode.I)) MoveEffector(new Vector3(0.0f, 0.01f, 0.0f));
		if (Input.GetKey(KeyCode.K)) MoveEffector(new Vector3(0.0f, -0.01f, 0.0f));
		if (Input.GetKey(KeyCode.U)) MoveEffector(new Vector3(0.0f, 0.0f, 0.01f));
		if (Input.GetKey(KeyCode.O)) MoveEffector(new Vector3(0.0f, 0.0f, -0.01f));
	}

	// Animate motor to move random
	void Animate() {

		// Get new stepcount and direction
		if (AnimateStepCount == 0) {
			AnimateStepCount = Random.Range (5, 20) * (int)BOUNDS_MAX_RANGE;
			AnimateDirection = Random.Range(0, 6);
		}

		if (AnimateDirection == 0) MoveEffector(new Vector3(0.01f, 0.0f, 0.0f));
		if (AnimateDirection == 1) MoveEffector(new Vector3(-0.01f, 0.0f, 0.0f));
		if (AnimateDirection == 2) MoveEffector(new Vector3(0.0f, 0.01f, 0.0f));
		if (AnimateDirection == 3) MoveEffector(new Vector3(0.0f, -0.01f, 0.0f));
		if (AnimateDirection == 4) MoveEffector(new Vector3(0.0f, 0.0f, 0.01f));
		if (AnimateDirection == 5) MoveEffector(new Vector3(0.0f, 0.0f, -0.01f));

		AnimateStepCount -= 1;
	}
}
