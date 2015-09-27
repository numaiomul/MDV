using UnityEngine;
using System.Collections;

public class free_rotation_character_controller : MonoBehaviour {

	private float itsVelocity = 18.0f;
	public float gravity = 40.0F;
	private Vector3 moveDirection = Vector3.zero;
	private Vector3 targetDirection = Vector3.zero;
	// Update is called once per frame
	void Update ()
	{		

		CharacterController controller = GetComponent<CharacterController> ();
		targetDirection = Vector3.zero;

		if (Input.GetKey (KeyCode.LeftArrow) || Input.GetKey (KeyCode.A))
		{			
			transform.Rotate (new Vector3 (0.0f, -Time.deltaTime * itsVelocity * 4.0f, 0.0f));
		}
		else if (Input.GetKey (KeyCode.RightArrow) || Input.GetKey (KeyCode.D))
		{
			transform.Rotate (new Vector3 (0.0f, Time.deltaTime * itsVelocity * 4.0f, 0.0f));
		}

		if (Input.GetKey (KeyCode.Keypad2))
		{			
			transform.Rotate (new Vector3 (-Time.deltaTime * itsVelocity * 4.0f, 0.0f, 0.0f));
		}
		else if (Input.GetKey (KeyCode.Keypad8))
		{
			transform.Rotate (new Vector3 (Time.deltaTime * itsVelocity * 4.0f, 0.0f, 0.0f));
		}

		if (Input.GetKey (KeyCode.Keypad4))
		{			
			transform.Rotate (new Vector3 (0.0f, 0.0f, -Time.deltaTime * itsVelocity * 4));
		}
		else if (Input.GetKey (KeyCode.Keypad6))
		{
			transform.Rotate (new Vector3 (0.0f, 0.0f, Time.deltaTime * itsVelocity * 4));
		}

		if (controller.isGrounded)
		{
			if (Input.GetKey (KeyCode.DownArrow) || Input.GetKey (KeyCode.S))
			{
				//transform.position -= transform.forward*Time.deltaTime*itsVelocity;
				targetDirection = -transform.forward;
				targetDirection *= 6f;

			}
			if (Input.GetKey (KeyCode.UpArrow) || Input.GetKey (KeyCode.W))
			{
				//transform.position += transform.forward*Time.deltaTime*itsVelocity;
				targetDirection = transform.forward;
				targetDirection *= 6f;


			}		
		}

		moveDirection.x = Mathf.Lerp(moveDirection.x, targetDirection.x, Time.deltaTime);
		moveDirection.z = Mathf.Lerp(moveDirection.z, targetDirection.z, Time.deltaTime);
		moveDirection.y -= gravity * Time.deltaTime;
		controller.Move (moveDirection * Time.deltaTime);
	}




}
