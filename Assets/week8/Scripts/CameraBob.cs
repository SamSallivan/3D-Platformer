using System;
using UnityEngine;

public class CameraBob : MonoBehaviour
{
	public AnimationCurve curve; 

	private Transform headTransform;

	private Vector3 bob;

	private Vector3 headAngles;

	private float bobMagnitude;

	private float bobMagnitudeSpeed = 4f;

	private float bobTime;

	[SerializeField]
	private float xAmp = 0.02f;

	[SerializeField]
	private float yAmp = 0.06f;

	private float rotTimer;

	private float rotSpeed;

	private Quaternion rot;

	private Quaternion startRot = Quaternion.identity;

	private void Awake()
	{
		headTransform = transform.parent;
		headAngles = default(Vector3);
	}

	//enables camera bob;
	public void Bob(float speed = 1f)
	{
		//loops bobTime from 0 to 2pi;
		if (bobTime < (float)Math.PI * 2f)
		{
			bobTime += Time.deltaTime / speed;
		}
		else
		{
			bobTime = 0;
		}

		//lerps the bob magnitude from 0 to 1;
		//calculates bob offset using Sig(bobTime), modified with xy amplitute and bob magnitude.
		//applies bob offset to camera.
		if (bobMagnitude != 1f)
		{
			bobMagnitude = Mathf.Lerp(bobMagnitude, 1f, Time.deltaTime * bobMagnitudeSpeed);
		}
		bob.x = Mathf.Sin(bobTime * 8f) * xAmp * bobMagnitude;
		bob.y = Mathf.Sin(bobTime * 16f) * yAmp * bobMagnitude;
		transform.localPosition = bob;
	}

	//disables camera bob, sets bob magnitude to 0, sets camera position to 0.
	public void Reset()
	{
		if (bobMagnitude != 0f)
		{
			bobMagnitude = Mathf.Lerp(bobMagnitude, 0f, Time.deltaTime * bobMagnitudeSpeed);
		}
		transform.localPosition = bob * bobMagnitude;
	}

	//lerps head rotation.z to given value over time.
	public void Angle(float z)
	{
		headAngles.z = Mathf.LerpAngle(headAngles.z, z, Time.deltaTime * 6f);
		headTransform.localEulerAngles = headAngles;
	}

	//sets rotTimer to 0;
	//sets camera target rotation to given value.
	public void Sway(Vector4 sway)
	{
		rotTimer = 0f;
		rotSpeed = sway.w;
		rot = Quaternion.Euler(sway);
	}
	
	private void Update()
	{
		//moves rotTimer to 1;
		//lerps between camera rotation and target rotation based on rotTimer on curve.
		if (rotTimer != 1f)
		{
			rotTimer = Mathf.MoveTowards(rotTimer, 1f, Time.deltaTime * rotSpeed);
			transform.localRotation = Quaternion.SlerpUnclamped(startRot, rot, curve.Evaluate(rotTimer));
		}
	}
}
