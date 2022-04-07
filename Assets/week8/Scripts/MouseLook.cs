using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MouseLook : MonoBehaviour
{
	private float sensitivity;
	public Vector2 rotation;

	public float x { get; private set; }

	public float y { get; private set; }



    void Awake()
    {   
        Cursor.lockState = CursorLockMode.Locked;
		rotation = base.transform.eulerAngles;
        sensitivity = 1.5f;
    }

	private void LateUpdate()
	{
		if (Time.timeScale != 0f)
		{
            y = Input.GetAxis("Mouse X");
            x = Input.GetAxis("Mouse Y");
        
			if (Mathf.Abs(y) > 0.01f)
			{
				rotation.y += y * sensitivity;
			}
			if (Mathf.Abs(x) > 0.01f)
			{
				rotation.x -= x * sensitivity;
			}
			rotation.x = Mathf.Clamp(rotation.x, -85f, 85f);
			transform.rotation = Quaternion.Euler(rotation);
		}
	}
}
