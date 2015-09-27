// Copyright (c) 2010 All Right Reserved, http://www.kolmich.at/
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// </copyright>
// <author>Michal Kolasinski</author>
// <email>michal.kolasinski@kolmich.at</email>
// <date>2011-02-19</date>
// <summary>short summary</summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 
/// </summary>
public class KGFCharacter3rdPerson : KGFObject, KGFIValidator
{
	/// <summary>
	/// data class
	/// </summary>
	public KGFCharacterData3rdPerson itsKGFCharacterData = new KGFCharacterData3rdPerson ();
	
	/// <summary>
	/// Its last axis1 horizontal. Is used to check whether the button is currently getting released or pressed
	/// </summary>
	
	/// <summary>
	/// Its jumping. Value used to inform the animation scripts
	/// </summary>
	private bool itsJumping = false;
	
	/// <summary>
	/// Its turning left. Value used to inform the animation scripts
	/// </summary>
	private bool itsTurningLeft = false;
	
	/// <summary>
	/// Its turning right. Value used to inform the animation scripts
	/// </summary>
	private bool itsTurningRight = false;
	
	/// <summary>
	/// Its physics created. Set to true as soon as the rigid body is created.
	/// </summary>
	[HideInInspector]
	public bool itsPhysicsCreated = false;
	
	private float itsAcceleration = 0;
	
	/// <summary>
	/// Start method. Find and initialize gameobjects
	/// </summary>
	protected override void KGFAwake ()
	{
		base.KGFAwake ();
		Transform aRotatorTransform = transform.FindChild ("rotator");
		itsKGFCharacterData.itsCharacterRotator = aRotatorTransform.gameObject;
		itsKGFCharacterData.itsGeometryContainerTransform = transform.parent.FindChild ("geometrycontainer");
		itsKGFCharacterData.itsCameraRootTransform = transform.parent.FindChild ("geometrycontainer/cameraroot");
		itsKGFCharacterData.itsCharacterObject= transform.parent.FindChild ("geometrycontainer/character");
		itsKGFCharacterData.itsTransform = transform;
	}
	
	/// <summary>
	/// Gets the rigid body.
	/// </summary>
	/// <returns>
	/// The rigid body.
	/// </returns>
	public Rigidbody GetRigidBody ()
	{
		return itsKGFCharacterData.itsRigidBody;
	}
	
	/// <summary>
	/// Gets the rigidbody velocity.
	/// </summary>
	/// <returns>
	/// The rigidbody velocity.
	/// </returns>
	public Vector3 GetRigidbodyVelocity ()
	{
		return itsKGFCharacterData.itsRigidBodyVelocity;
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update ()
	{
		if (!itsPhysicsCreated)
		{
			if (itsKGFCharacterData.itsCharacterController != null) //needs character controller before using rigidbody
			{
				AddRigidbodyAndCollider ();
				itsPhysicsCreated = true;
				itsKGFCharacterData.itsCharacterController.itsGlobalSettings.itsGeometryTransform.parent = itsKGFCharacterData.itsCharacterObject.transform;
				itsKGFCharacterData.itsCharacterController.itsGlobalSettings.itsGeometryTransform.localPosition = Vector3.zero;
			}
		}
		else
		{
			CalculateDirections ();
			CalculateGroundNormal ();
			CalculateVerticalState ();
			ApplyRotation ();
			ApplyCameraRoot ();
		}
	
	}
	
	/// <summary>
	/// Fixed update method for this instance.
	/// </summary>
	public void FixedUpdate ()
	{
		if (itsPhysicsCreated) //only do stuff when rigidbody is created
		{
			ApplyForces ();
			Gravitate ();
		}
	}
	
	/// <summary>
	/// check for errors
	/// </summary>
	/// <returns></returns>
	public KGFMessageList Validate ()
	{
		KGFMessageList aMessageList = new KGFMessageList ();
		return aMessageList;
	}
	
	/// <summary>
	/// Adds a force to the character
	/// </summary>
	/// <param name="theForce"></param>
	public void AddForce (Vector3 theForce)
	{
		itsKGFCharacterData.itsRigidBody.AddForce (theForce, ForceMode.Force);
	}
	
	/// <summary>
	/// Sets the module_charactercontroller this container belongs to
	/// </summary>
	/// <param name="theModuleCharacterController"></param>
	public void SetCharacterController (KGFCharacterController3rdPerson theCharacterController)
	{
		itsKGFCharacterData.itsCharacterController = theCharacterController;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="theCollision">
	/// A <see cref="Collision"/>
	/// </param>
	public void OnCollisionStay (Collision theCollision)
	{
		if (itsKGFCharacterData.itsCharacterController == null)
			return;
		if (theCollision.contacts.Length == 0)
		{
			itsKGFCharacterData.itsContactPoints.Clear ();
			return;
		}
		
		List<ContactPoint> aTempList = new List<ContactPoint> ();
		
		for (int i = 0; i<  theCollision.contacts.Length; i++)
		{
			ContactPoint aContactPoint = theCollision.contacts [i];
			if (aContactPoint.thisCollider == itsKGFCharacterData.itsCollider || aContactPoint.otherCollider == itsKGFCharacterData.itsCollider)
			{
				aTempList.Add (aContactPoint);
			}
		}
		if (aTempList.Count != 0) //if 0 this contact was only with help collider so do nothing.
		{
			itsKGFCharacterData.itsContactPoints.Clear ();
			itsKGFCharacterData.itsContactPoints.AddRange (aTempList);
		}
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="theCollision">
	/// A <see cref="Collision"/>
	/// </param>
	public void OnCollisionExit (Collision theCollision)
	{
		itsKGFCharacterData.itsContactPoints.Clear ();
	}
	
	/// <summary>
	/// calculate the groundnormal from collected collision points
	/// </summary>
	private void CalculateVerticalState ()
	{
		KGFCharacterController3rdPerson aCharacterController = itsKGFCharacterData.itsCharacterController;
		if (aCharacterController.GetVerticalState () == KGFCharacterController3rdPerson.GlobalSettings.eVerticalState.eFalling) //also check jumping because character can jump and lands immediately on higher ground without even start falling
		{
			if (itsKGFCharacterData.itsContactPoints.Count != 0)	//contact points so land
			{	
				itsJumping = false;
				Land ();
			}
		}
		else if (aCharacterController.GetVerticalState () == KGFCharacterController3rdPerson.GlobalSettings.eVerticalState.eGrounded)
		{
			if (itsKGFCharacterData.itsContactPoints.Count == 0)	//no contact points so start falling
				Fall ();
		}
		if (aCharacterController.GetVerticalState () == KGFCharacterController3rdPerson.GlobalSettings.eVerticalState.eJumping && itsKGFCharacterData.itsRigidBody.velocity.y < -0.2f)
		{
			Fall ();
		}
		else if (aCharacterController.GetVerticalState () == KGFCharacterController3rdPerson.GlobalSettings.eVerticalState.eJumping && Mathf.Abs(itsKGFCharacterData.itsRigidBody.velocity.y) < 0.1f)
		{
			Fall ();
		}
		
	}
	
	/// <summary>
	/// initialize jump
	/// </summary>
	public void Jump ()
	{
		KGFCharacterController3rdPerson aCharacterController = itsKGFCharacterData.itsCharacterController;
		
		if (aCharacterController.GetVerticalState () != KGFCharacterController3rdPerson.GlobalSettings.eVerticalState.eGrounded)
			return; //no jumping if no ground beneath feet
		
		aCharacterController.SetVerticalState (KGFCharacterController3rdPerson.GlobalSettings.eVerticalState.eJumping);
		
		
		float aJumpHeight = aCharacterController.GetCurrentJumpHeight ();
		
		
		
		Vector3 aJumpVelocity = transform.up * Mathf.Sqrt (2.0f * aJumpHeight * Physics.gravity.magnitude);
		
		itsKGFCharacterData.itsRigidBody.velocity = itsKGFCharacterData.itsRigidBody.velocity + aJumpVelocity;
		
		itsJumping = true; 
	}
	
	/// <summary>
	/// Character is falling
	/// </summary>
	protected void Fall ()
	{
		KGFCharacterController3rdPerson aCharacterController = itsKGFCharacterData.itsCharacterController;
		
		if (aCharacterController.GetVerticalState () == KGFCharacterController3rdPerson.GlobalSettings.eVerticalState.eFalling)
			return; //no falling if already falling
		
		aCharacterController.SetVerticalState (KGFCharacterController3rdPerson.GlobalSettings.eVerticalState.eFalling);
	}
	
	/// <summary>
	/// Rigidbody is landing on ground
	/// </summary>
	protected void Land ()
	{
		KGFCharacterController3rdPerson aCharacterController = itsKGFCharacterData.itsCharacterController;
		
		if (aCharacterController.GetVerticalState () == KGFCharacterController3rdPerson.GlobalSettings.eVerticalState.eGrounded)
			return; //cannot land if already grounded
		aCharacterController.SetVerticalState (KGFCharacterController3rdPerson.GlobalSettings.eVerticalState.eGrounded);
	}
	
	/// <summary>
	/// creates the rigidbody and capsule collider for the character
	/// </summary>
	private void AddRigidbodyAndCollider ()
	{
		Rigidbody aRigidbody = (Rigidbody)gameObject.AddComponent<Rigidbody> ();			//create the rigidbody for the character
		aRigidbody.mass = itsKGFCharacterData.itsCharacterController.itsGlobalSettings.itsMass;
		aRigidbody.useGravity = false;
		aRigidbody.freezeRotation = true;
		aRigidbody.drag = 0f;
		aRigidbody.angularDrag = 0f;
		aRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
		
		itsKGFCharacterData.itsRigidBody = aRigidbody;
		
		gameObject.AddComponent (typeof(CapsuleCollider));	//add the default collider (with friction)
		
		CapsuleCollider aCollider = gameObject.GetComponent<CapsuleCollider> ();
		aCollider.gameObject.layer = LayerMask.NameToLayer ("Ignore Raycast");
		aCollider = gameObject.GetComponent<CapsuleCollider> ();
		aCollider.radius = itsKGFCharacterData.itsCharacterController.itsGlobalSettings.itsThickness;
		aCollider.height = itsKGFCharacterData.itsCharacterController.itsGlobalSettings.itsHeight;
		aCollider.center = new Vector3 (0f, aCollider.height /2.0f, 0f);
		aCollider.direction = 1;
		
		PhysicMaterial aPhysicMaterial = new PhysicMaterial ("character_physicsmaterial");
		aPhysicMaterial.dynamicFriction = 1.0f;
		aPhysicMaterial.staticFriction = 1.0f;
		aPhysicMaterial.bounciness = 0f;
		aPhysicMaterial.frictionCombine = PhysicMaterialCombine.Maximum;
		aPhysicMaterial.bounceCombine = PhysicMaterialCombine.Minimum;
		aCollider.sharedMaterial = aPhysicMaterial;
		
		itsKGFCharacterData.itsCollider = aCollider;
		
		GameObject aHelpCollider = new GameObject ("character_helpcollider");
		aHelpCollider.transform.parent = gameObject.transform;
		aHelpCollider.transform.localPosition = Vector3.zero;
		aHelpCollider.transform.localRotation = Quaternion.identity;
		aHelpCollider.layer = LayerMask.NameToLayer ("Ignore Raycast");
		CapsuleCollider aHelpCapsuleCollider = (CapsuleCollider)aHelpCollider.AddComponent (typeof(CapsuleCollider));
		
		float aHeighOffset = itsKGFCharacterData.itsCharacterController.itsGlobalSettings.itsHeight * 0.1f;
		aHelpCapsuleCollider.height = itsKGFCharacterData.itsCharacterController.itsGlobalSettings.itsHeight;
		aHelpCapsuleCollider.radius = itsKGFCharacterData.itsCharacterController.itsGlobalSettings.itsThickness * 1.2f;
		aHelpCapsuleCollider.center = new Vector3 (0f, (itsKGFCharacterData.itsCharacterController.itsGlobalSettings.itsHeight / 2.0f) + aHeighOffset, 0f);
		
		aPhysicMaterial = new PhysicMaterial ();
		aPhysicMaterial.dynamicFriction = 0f;
		aPhysicMaterial.staticFriction = 0f;
		aPhysicMaterial.bounciness = 0f;
		aPhysicMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
		aPhysicMaterial.bounceCombine = PhysicMaterialCombine.Minimum;
		aHelpCapsuleCollider.sharedMaterial = aPhysicMaterial;
	}
	
	/// <summary>
	/// calculate the groundnormal from collected collision points
	/// </summary>
	private void CalculateDirections ()
	{
		itsKGFCharacterData.itsCameraDirection = transform.position - itsKGFCharacterData.itsCharacterController.GetOrbitCamTransform ().position;
		itsKGFCharacterData.itsCameraDirection.y = 0.0f;
		itsKGFCharacterData.itsCameraDirection.Normalize ();
	}
	
	/// <summary>
	/// calculate the groundnormal from collected collision points
	/// </summary>
	private void CalculateGroundNormal ()
	{
		Vector3 aGroundNormal = Vector3.zero;
		foreach (ContactPoint aContactPoint in itsKGFCharacterData.itsContactPoints)
		{
			aGroundNormal += aContactPoint.normal;
		}
		aGroundNormal.Normalize ();
		
		
		if (aGroundNormal != Vector3.zero)
			itsKGFCharacterData.itsGroundNormalDirection = aGroundNormal;
		else	
			itsKGFCharacterData.itsGroundNormalDirection = Vector3.up;

		itsKGFCharacterData.itsLerpedGroundNormalDirection = Vector3.Lerp (itsKGFCharacterData.itsLerpedGroundNormalDirection, itsKGFCharacterData.itsGroundNormalDirection, Time.deltaTime * 10.0f);
	}
	
	/// <summary>
	/// Applies the gravitation in the correct direction
	/// </summary>
	protected void Gravitate ()
	{
		if (itsKGFCharacterData.itsRigidBody == null)
			return;
		
		itsKGFCharacterData.itsGravityDirection = Vector3.Lerp (itsKGFCharacterData.itsGravityDirection, -itsKGFCharacterData.itsGroundNormalDirection, Time.deltaTime);
		
//		itsKGFCharacterData.itsRigidBody.AddForce (itsKGFCharacterData.itsGravityDirection * Physics.gravity.magnitude * itsKGFCharacterData.itsRigidBody.mass);		
		itsKGFCharacterData.itsRigidBody.AddForce (Physics.gravity,ForceMode.Acceleration);
	}
	
	/// <summary>
	/// Calculates the forces that have to be applierd to the character container, based on ground normal direction and controller input
	/// </summary>
	private void ApplyForces ()
	{
		KGFCharacterController3rdPerson aCharacterController = itsKGFCharacterData.itsCharacterController;

		itsKGFCharacterData.itsResultingControllerDirection = Vector3.zero;
		Vector3 aLocalControllerInput = Vector3.zero;
		Vector3 aTargetVelocity = Vector3.zero;
		
		
		
		
			float anAxis1Vertical = aCharacterController.GetAxis1Vertical ();
			
			
			if(Mathf.Abs(anAxis1Vertical) > 0)
			{
				itsAcceleration = Mathf.Lerp( itsAcceleration, 1, Time.deltaTime * aCharacterController.GetAcceleration() );
			}
			else
			{
				itsAcceleration = 0;
			}
			aCharacterController.SetSpeedNormalized(itsAcceleration);
			
			
			
			Vector3 aFinalDirection = new Vector3 (0.0f, 0.0f, anAxis1Vertical);
			
			itsKGFCharacterData.itsResultingControllerDirection.z = aFinalDirection.z;//anAxis1Vertical;
			aLocalControllerInput = itsKGFCharacterData.itsTransform.TransformDirection (aFinalDirection);
			itsKGFCharacterData.itsResultingControllerDirection.x = aFinalDirection.x;//aCharacterController.GetAxis1Horizontal ();

			aLocalControllerInput.Normalize ();	//by normalizing this value will always be 1 so acelleration caused by control damping is disabled
			
			float aBackWardSpeedMultiplicator = 1.0f;
			if (itsKGFCharacterData.itsResultingControllerDirection.z < 0.0f)
				aBackWardSpeedMultiplicator = aCharacterController.GetBackwardSpeedMultiplicator ();
			
			bool aIsSlopeOK = CheckIfSlopeOK (aLocalControllerInput);
			if (aIsSlopeOK)	//change velocity only if
				aTargetVelocity = aLocalControllerInput * aCharacterController.GetCurrentSpeed () * aBackWardSpeedMultiplicator;	//multiply direction with desired speed
			else
				aTargetVelocity = Vector3.zero;
			
			
		

		Vector3 aVelocityChange = aTargetVelocity - itsKGFCharacterData.itsRigidBody.velocity;
		
		aVelocityChange.y = 0;	//TODO: check this?
		
		
		itsKGFCharacterData.itsRigidBody.AddForce (aVelocityChange, ForceMode.VelocityChange);
		
		
		
		
		
		if (itsKGFCharacterData.itsRigidBody.velocity.magnitude < aCharacterController.GetCurrentSpeed () * 0.1f)	//if not moving
			itsKGFCharacterData.itsIsMoving = false;
		else
			itsKGFCharacterData.itsIsMoving = true;
		
		itsKGFCharacterData.itsRigidBodyVelocity = itsKGFCharacterData.itsRigidBody.velocity;

	}
	
	/// <summary>
	/// Checks if slope too steep.
	/// </summary>
	/// <returns>
	/// The if slope too steep.
	/// </returns>
	/// <param name='theForwardDirection'>
	/// If set to <c>true</c> the forward direction.
	/// </param>
	public bool CheckIfSlopeOK (Vector3 theForwardDirection)
	{
		KGFCharacterController3rdPerson aCharacterController = itsKGFCharacterData.itsCharacterController;
		if (aCharacterController == null)
			return true;
		
		if (aCharacterController.GetVerticalState () != KGFCharacterController3rdPerson.GlobalSettings.eVerticalState.eGrounded)
			return true;
		
		float aValue = Vector3.Dot (theForwardDirection, itsKGFCharacterData.itsLerpedGroundNormalDirection);
		if (aValue < 0) //character walks up a hill
		{
			float anAngleBetweenNormalAndForwardVector = Vector3.Angle (theForwardDirection, itsKGFCharacterData.itsLerpedGroundNormalDirection) - 90.0f;
			if (anAngleBetweenNormalAndForwardVector > aCharacterController.GetMaxSlope ())
				return false;
		}
		return true;
	}
	
	/// <summary>
	/// 
	/// </summary>
	private void ApplyRotation ()
	{
		KGFCharacterController3rdPerson aCharacterController = itsKGFCharacterData.itsCharacterController;
		float anAxis1Horizontal = aCharacterController.GetAxis1Horizontal ();
			
		
		if (aCharacterController.GetIsTurning ())	//accelerate turn speed
		{
			aCharacterController.SetCurrentTurnSpeed (Mathf.Lerp (aCharacterController.GetCurrentTurnSpeed (), aCharacterController.GetMaxTurnSpeed (), Time.deltaTime / aCharacterController.GetTurnAccerlerationTime ()));
		}
		else
		{
			aCharacterController.SetCurrentTurnSpeed (aCharacterController.GetMinTurnSpeed ());
		}
		
		float aTurnSpeed = aCharacterController.GetCurrentTurnSpeed ();
		
		
		
			Vector3 aControllerDirection = Vector3.zero;
			aControllerDirection.z = aCharacterController.GetAxis1Vertical ();
			
			if (aCharacterController.GetAxis1Horizontal () != 0.0f)
				aCharacterController.SetIsTurning (true);
			else
				aCharacterController.SetIsTurning (false);
			
			if (aCharacterController.GetAxis1Horizontal () > 0.0f)
			{
				itsTurningLeft = true;
			}
			else
			{
				itsTurningLeft = false;	
			}

			if (aControllerDirection.z == 0)
				itsKGFCharacterData.itsReoriented = false;
			
			float aTurnSpeedMultiplicator = 1.0f - (aCharacterController.GetSpeedNormalized () * 0.5f);
			float aFinalTurnSpeed = aTurnSpeedMultiplicator * aTurnSpeed;	//speed hack for ludwig
			if (aFinalTurnSpeed < aCharacterController.GetMinTurnSpeed ())
				aFinalTurnSpeed = aCharacterController.GetMinTurnSpeed ();
			
			transform.Rotate (0.0f, aFinalTurnSpeed * anAxis1Horizontal * Time.deltaTime, 0.0f);

			itsKGFCharacterData.itsCharacterRotator.transform.localRotation = Quaternion.identity;
	
		
	}
	
	
	/// <summary>
	/// Applies the camera root. Also lerps the character to targeet rotation
	/// </summary>
	private void ApplyCameraRoot ()
	{
		
	
		KGFCharacterController3rdPerson aCharacterController = itsKGFCharacterData.itsCharacterController;
		
		float aLerpSpeed = aCharacterController.GetLerpSpeed();
		
		itsKGFCharacterData.itsGeometryContainerTransform.rotation = Quaternion.Lerp (itsKGFCharacterData.itsGeometryContainerTransform.rotation, transform.rotation, Time.deltaTime * aLerpSpeed);
		
		
		
			float anRotationOffset = 0;
			
			float aCurrentRotationOffset = itsKGFCharacterData.itsCharacterObject.transform.localEulerAngles.y;
			aCurrentRotationOffset = Mathf.LerpAngle(aCurrentRotationOffset, anRotationOffset, Time.deltaTime * 4);
			Quaternion anOffsetQuaternion = Quaternion.Euler(0, aCurrentRotationOffset, 0);
			itsKGFCharacterData.itsCharacterObject.transform.localRotation = anOffsetQuaternion;
		
		
		itsKGFCharacterData.itsGeometryContainerTransform.position = Vector3.Lerp (itsKGFCharacterData.itsGeometryContainerTransform.position, transform.position, Time.deltaTime * 20.0f);
	
		
		
		
		if (!Input.GetButton ("Fire2") && itsKGFCharacterData.itsIsMoving)
		{
			float anOffsetY = itsKGFCharacterData.itsCameraRootTransform.position.y - itsKGFCharacterData.itsGeometryContainerTransform.position.y;
			Vector3 aStartPosition = itsKGFCharacterData.itsGeometryContainerTransform.position;
			aStartPosition.y += anOffsetY;
			float aDistance = 1f;
			Vector3 anOffsetPosition = itsKGFCharacterData.itsCameraRootTransform.position;//aStartPosition + (transform.rotation * new Vector3(0,0,aDistance));
			anOffsetPosition.x = Mathf.Lerp (anOffsetPosition.x, aStartPosition.x + (transform.rotation * new Vector3 (0, 0, aDistance)).x, 3 * Time.deltaTime);
			anOffsetPosition.z = Mathf.Lerp (anOffsetPosition.z, aStartPosition.z + (transform.rotation * new Vector3 (0, 0, aDistance)).z, 3 * Time.deltaTime);
			itsKGFCharacterData.itsCameraRootTransform.position = anOffsetPosition;
		}
		else
		{
			float anOffsetY = itsKGFCharacterData.itsCameraRootTransform.position.y - itsKGFCharacterData.itsGeometryContainerTransform.position.y;
			Vector3 aStartPosition = itsKGFCharacterData.itsGeometryContainerTransform.position;
			aStartPosition.y += anOffsetY;
			Vector3 anOffsetPosition = itsKGFCharacterData.itsCameraRootTransform.position;
			anOffsetPosition.x = Mathf.Lerp (anOffsetPosition.x, aStartPosition.x, 3 * Time.deltaTime);
			anOffsetPosition.z = Mathf.Lerp (anOffsetPosition.z, aStartPosition.z, 3 * Time.deltaTime);
			itsKGFCharacterData.itsCameraRootTransform.position = anOffsetPosition;
		}
		
	}
	
	
	/// <summary>
	/// Connects the camera container to character.
	/// </summary>
	/// <param name='theConnect'>
	/// The connect.
	/// </param>
	public void ConnectCameraContainerToCharacter (bool theConnect)
	{
		if (itsKGFCharacterData.itsCameraRootTransform != null)
		{
			if (theConnect)
				itsKGFCharacterData.itsCameraRootTransform.localRotation = Quaternion.identity;
			else
				itsKGFCharacterData.itsCameraRootTransform.rotation = Quaternion.identity;
		}
	}
	
	
	
	
	/// <summary>
	/// Gets the jumping. Access method for animation scripts
	/// </summary>
	/// <returns>
	/// The jumping.
	/// </returns>
	public bool GetJumping ()
	{
		return itsJumping;	
	}
	
	/// <summary>
	/// Gets the turning left. Access method for animation scripts
	/// </summary>
	/// <returns>
	/// The turning left.
	/// </returns>
	public bool GetTurningLeft ()
	{
		return itsTurningLeft;	
	}
	
	/// <summary>
	/// Gets the turning right. Access method for animation scripts
	/// </summary>
	/// <returns>
	/// The turning right.
	/// </returns>
	public bool GetTurningRight ()
	{
		return itsTurningRight;	
	}
	
}
