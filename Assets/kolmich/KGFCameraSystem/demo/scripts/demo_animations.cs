using UnityEngine;
using System.Collections;

public class demo_animations : MonoBehaviour {
	
	
	private Animator itsAnimator;
	private bool itsWalking = false;
	private bool itsWalkingBackwards = false;
	private bool itsRunning = false;
	private bool itsJumping = false;
	private float itsSpeed = 0;
	public float itsMaxWalkSpeed = 4.1f;
	
	
	public KGFCharacterController3rdPerson itsController;
	
	// Use this for initialization
	void Start () {
		
		itsAnimator = GetComponent<Animator>();
		
	}
	
	// Update is called once per frame
	void Update () {
		
		
		KGFCharacter3rdPerson aCharacter = itsController.GetCharacter();
		
		Vector2 aSpeed = new Vector2(itsController.GetRigidbodyVelocity().x, itsController.GetRigidbodyVelocity().z);
		itsSpeed = aSpeed.magnitude;
		if(itsSpeed > 0.2f && itsSpeed < itsMaxWalkSpeed)
		{
			itsWalking = true;
			itsWalkingBackwards = false;
			itsRunning = false;
			
			
			
		}
		else if(itsSpeed >= itsMaxWalkSpeed)
		{
			itsRunning = true;
		}
		else
		{
			itsWalking = false;
			itsRunning = false;
		}
		
		itsWalkingBackwards = false;
		if(aCharacter != null)
		{
			
				if(aCharacter.itsKGFCharacterData.itsResultingControllerDirection.z < 0)
				{
					itsWalking = false;
					itsRunning = false;
					itsWalkingBackwards = true;
				}
			
		}
		
	
		if(itsWalkingBackwards)
		{
			itsWalking = true;	
		}
		
		
		itsAnimator.SetBool("Walking", itsWalking);
		itsAnimator.SetBool("Running", itsRunning);
		itsJumping = aCharacter.GetJumping();
		itsAnimator.SetBool("Jump", itsJumping);
	}
	
}
