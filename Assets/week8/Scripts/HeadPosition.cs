using UnityEngine;

public class HeadPosition : MonoBehaviour
{
	public AnimationCurve slideCurve;

	public AnimationCurve bounceCurve;

	public float speed = 4f;

	public float bounceSpeed = 2f;

	private Vector3 headPos;

	public float slideStart;

	public float slideTarget;

	public float bounceTarget;

	public float slideTimer;

	public float bounceTimer;

	//sets timer to 0;
	//sets start y position.
	//sets target y position.
	public void Slide(float y)
	{
		slideTimer = 0f;
		slideStart = transform.localPosition.y;
		slideTarget = y;
	}

	//sets timer to 0;
	//sets target y position.
	public void Bounce(float value = -0.25f)
	{
		bounceTimer = 0f;
		bounceTarget = Mathf.Clamp(value, -0.75f, 0f);
	}

	public void PositionUpdate()
	{
		headPos = Vector3.zero;

		//moves timer towards 1;
		//lerps head y position towards target.
		slideTimer = Mathf.MoveTowards(slideTimer, 1f, Time.deltaTime * speed);
		headPos.y += Mathf.LerpUnclamped(slideStart, slideTarget, slideCurve.Evaluate(slideTimer));

		//moves timer towards 1;
		//lerps head y position towards target.
		bounceTimer = Mathf.MoveTowards(bounceTimer, 1f, Time.deltaTime * bounceSpeed);
		headPos.y += Mathf.LerpUnclamped(0f, bounceTarget, bounceCurve.Evaluate(bounceTimer));

		//applies head y position;
		transform.localPosition = headPos;
	}
}
