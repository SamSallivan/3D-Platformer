using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlide : MonoBehaviour
{
	private PlayerController pc;

	public CapsuleCollider standcollider;

	public CapsuleCollider slideCollider;

	public Vector3 slideDir { get; private set; }

	public int slideState { get; private set; }

	public float slideTimer { get; private set; }

	public GameObject windVFX; 

	private void Awake()
	{
		pc = GetComponent<PlayerController>();
		Grounder grounder = pc.grounder;
	}

	public void Slide()
	{
		//exits if is already sliding, or not grounded
		if (slideState != 0 || !pc.grounder.grounded)
		{
			return;
		}

		//if speed is fast enough, starts sliding.
		//rotates camera and plays effect.
		if (pc.vel.sqrMagnitude > 16)
		{
			slideTimer = 0.6f;
			slideState = 3;
			slideDir = pc.gDir.normalized;
			pc.bob.Sway(new Vector4(2f, 0f, 0f, Mathf.Sign(pc.h)*3f));
			windVFX.GetComponent<ParticleSystem>().Play();
		}
		//if speed is not fast enough, bounces camera.
		else if (pc.inputDir.sqrMagnitude > 0.25f)
		{
			pc.headPosition.Bounce(-0.5f);
			pc.bob.Sway(new Vector4(0f, 0f, 5f, Mathf.Sign(pc.h)*3f));
		}
		//if speed is 0, bounces camera harder.
        else if (pc.grounder.grounded){
            pc.headPosition.Bounce(-2f);
        }
	}

	public void SlidingUpdate()
	{
		switch (slideState)
		{
            case 3:
				//applies force, lowers camera, switches collider.
                pc.rb.AddForce(pc.rb.velocity.normalized * 5f, ForceMode.Impulse);
                pc.headPosition.Slide(-0.25f);
                standcollider.enabled = false;
                slideCollider.enabled = true;
                slideState--;
                break;
            case 2:
				//continues to slide before the force runs out, or the timer reaches 0;
				//tilts camera based on horizontal input;
                if (pc.vel.sqrMagnitude >= 64f && slideTimer > 0f)
                {
                    slideTimer -= Time.deltaTime;
                    pc.bob.Angle(Mathf.Sign(pc.h)*7.5f + pc.h * 2.5f);
                }
                else if (pc.gVel.y >= -1f)
                {
                    slideState--;
                }
                break;
            case 1:
				//back to sliding if player has collider above its head.
                if (Physics.Raycast(pc.t.TransformPoint(pc.playerCollider.center), Vector3.up, 1.5f, 1))
                {
                    slideTimer = 0.2f;
                    slideState = 2;
                    break;
                }
				//or raises camera back, switches collider back.
				//ends sliding.
                pc.headPosition.Slide(0.75f);
                slideCollider.enabled = false;
                standcollider.enabled = true;
                slideState--;
                break;
            }
	}
}
