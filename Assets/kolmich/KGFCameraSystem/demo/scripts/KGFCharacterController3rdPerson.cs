// <copyright>
// Copyright (c) 2010 All Right Reserved, http://www.kolmich.at/
//
// THIS CODE AND INORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
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
/// This class handles all possible camera/input control modes that can be used in different games to control a character.
/// Further this class allows a smooth switching between all those modes.
/// </summary>
public class KGFCharacterController3rdPerson : KGFObject, KGFIValidator
{
	#region internal classes
	/// <summary>
	/// data used for all possible camera settings
	/// </summary>
	public class CameraSettings
	{
		/// <summary>
		/// The orbit cam orbiting the character
		/// </summary>
		[HideInInspector]
		public KGFOrbitCam itsOrbitCam = null;
		
		/// <summary>
		/// the root for the KGFOrbitCam
		/// </summary>
		[HideInInspector]
		public Transform itsCameraLookatTransform = null;
		
		/// <summary>
		/// The initial position of the CameraLookatTransform
		/// </summary>
		[HideInInspector]
		public Vector3 itsCameraLookatTransformStartPosition = Vector3.zero;
		
		/// <summary>
		/// Vector used to smoothly change the lokkattransform position
		/// </summary>
		[HideInInspector]
		public Vector3 itsCameraLookatTransformTargetPosition = Vector3.zero;
		
		/// <summary>
		/// The camera attached to the orbitcam
		/// </summary>
		[HideInInspector]
		public Camera itsOrbitCamCamera = null;
		
		/// <summary>
		/// Transform of the orbit cam
		/// </summary>
		[HideInInspector]
		public Transform itsOrbitCamTransform = null;
		
		/// <summary>
		/// different orbiter settings
		/// </summary>
		public KGFOrbitCamSettings itsOrbitCamSettings3rdPerson = null;
	}
	
	/// <summary>
	/// camera settings
	/// </summary>
	private CameraSettings itsCameraSettings = new CameraSettings ();
	
	/// <summary>
	/// data used for all possible camera settings
	/// </summary>
	[System.Serializable]
	public class GlobalSettings
	{
		/// <summary>
		/// E vertical state.
		/// </summary>
		[HideInInspector]
		public enum eVerticalState
		{
			eGrounded,
			eJumping,
			eFalling
		}
		
		
		/// <summary>
		/// Use Gamepad axis for camera control
		/// </summary>
		public bool itsUseGamepad = false;
		
		
		/// <summary>
		/// indicates if the character is grounded, jumping or falling.
		/// </summary>
		[HideInInspector]
		public eVerticalState itsVerticalState = eVerticalState.eGrounded;
		
		/// <summary>
		/// Indicates if the character moves forward or backward. (value range: -1.0f,1.0f)
		/// </summary>
		[HideInInspector]
		public float itsAxis1Vertical = 0.0f;
		
		/// <summary>
		/// Indicates if the character rotates left or right (value range: -1.0f,1.0f)
		/// </summary>
		[HideInInspector]
		public float itsAxis1Horizontal = 0.0f;
		
		[HideInInspector]
		public Transform itsGeometryTransform = null;
		
		[HideInInspector]
		public Vector3 itsGeometryTransformVelocity;
		
		public GameObject itsGeometry = null;
		
		/// <summary>
		/// The mass of the character
		/// </summary>
		public float itsMass = 75.0f;
		
		/// <summary>
		/// The thickness of the character. Default is 0.5 meter (Will be used for the creation of the capsule collider (radius))
		/// </summary>
		public float itsThickness = 0.5f;
		
		/// <summary>
		/// The height of the character. Default is 1.8 meter (Will be used for the creation of the capsule collider (height))
		/// </summary>
		public float itsHeight = 1.8f;
		
		/// <summary>
		/// Its running. Adds runspeed multiplicator when enabled
		/// </summary>
		[HideInInspector]
		public bool itsRunning = false;
	}

	/// <summary>
	/// camera settings
	/// </summary>
	public GlobalSettings itsGlobalSettings = new GlobalSettings ();
	
	/// <summary>
	/// Data used for mouse point and click navigation
	/// </summary>
	[System.Serializable]
	public class PointNavigationSettings
	{
		/// <summary>
		/// if true click target controls will be enabled
		/// </summary>
		public bool itsEnableClickTarget = false;
		
		/// <summary>
		/// the raycast for the clicktarget will work with this layers.
		/// </summary>
		public LayerMask itsClickTargetRaycastLayers = -1;
		
		/// <summary>
		/// this layers will block the raycast for the clicktarget. (e.g. so if you don't want to click throgh your character your character layer should be added here)
		/// IMPORTANT: you have to add this layer to itsClickTargetRaycastLayers else thew will be ignored by the test.
		/// </summary>
		public LayerMask itsClickTargetBlockLayers = -1;
		
		/// <summary>
		/// If not null the Character will try to reach this target on the shortest possible way.
		/// When the target is reached the character turns towards the forward direction of the target
		/// </summary>
		[HideInInspector]
		public GameObject itsClickTarget = null;
		
		/// <summary>
		/// Assign here arrows or particles that will instantiated when clicked on terrain in the correct radius
		/// This spatial will be instantiated, so make sure it will destroy itselve after a second or so.
		/// </summary>
		public GameObject itsClickTargetRepesentationOK = null;
		
		/// <summary>
		/// Assign here arrows or particles that will instantiated when clicked on terrain outside the correct radius
		/// This spatial will be instantiated, so make sure it will destroy itselve after a second or so.
		/// </summary>
		public GameObject itsClickTargetRepesentationNotOK = null;
		
		/// <summary>
		/// Assign here arrows or particles that will instantiated when clicked on terrain in the correct radius
		/// This spatial will be instantiated, so make sure it will destroy itselve after a second or so.
		/// </summary>
		public GameObject itsClickTargetRepesentationOKDouble = null;
		
		/// <summary>
		/// Assign here arrows or particles that will instantiated when clicked on terrain outside the correct radius
		/// This spatial will be instantiated, so make sure it will destroy itselve after a second or so.
		/// </summary>
		public GameObject itsClickTargetRepesentationNotOKDouble = null;
		
		/// <summary>
		/// if the player clicks somewhere on the ground this will be set true
		/// if player activates some axis this will be set to false.
		/// while true the robot will try to reach the itsClickTarget.
		/// </summary>
		[HideInInspector]
		public bool itsClickTargetFollow = false;
		
		/// <summary>
		/// if the distance between the character and itsClickTarget is smaller than itsClickTargetRotationDistance
		/// the character will just rotate in clicktarget direction.
		/// else he will start moving in the direction of the click target.
		/// the itsClickTargetRepresentationOK will be instantiated at the clicktarget position
		/// </summary>
		public float itsClickTargetRotationDistance = 3.0f;
		
		/// <summary>
		/// if the distance between the character and itsClickTarget is greather than itsClickTargetMaxDistance
		/// the character will just rotate in  clicktrget direction.
		/// the itsClickTargetRepresentationNotOK will be instantiated at the clicktarget position
		/// </summary>
		public float itsClickTargetMaxDistance = 10.0f;
		[HideInInspector]
		public float itsClickTargetTime = 0.0f;
		
		/// <summary>
		/// Vector pointing in the direction of the click target
		/// </summary>
		[HideInInspector]
		public Vector3 itsDirectionToClickTarget = Vector3.zero;
		[HideInInspector]
		public Projector itsClickTargetProjector = null;
		[HideInInspector]
		public Projector itsClickTargetInvalidProjector = null;
		public float itsProjectorOffset = 2;
		public float itsProjectorShrinkSpeed = 0.5f;
		[HideInInspector]
		public GameObject itsValidParticles = null;
		[HideInInspector]
		public GameObject itsInvalidParticles = null;
	}
	
	/// <summary>
	/// point navigation settings
	/// </summary>
	public PointNavigationSettings itsPointNavigationSettings = new PointNavigationSettings ();
	
	
	[System.Serializable]
	public class Data3rdPersonSettings
	{
		/// <summary>
		/// The speed current speed will be multiplied by this value if running.
		/// </summary>
		public float itsRunSpeedMultiplicator = 2f;
		
		/// <summary>
		/// Its running jump multiplicator.
		/// </summary>
		public float itsRunningJumpMultiplicator = 1.5f;
		
		/// <summary>
		/// if the ground slope gets higher than this value the character will not be able to walk over it.
		/// </summary>
		public float itsMaxSlope = 20.0f;
		
		/// <summary>
		/// The speed the character turns towards the camera root rotation
		/// </summary>
		//public float itsLerpSpeed = 4.0f;
		
		// <summary>
		/// Its maximal speed in unity units per second
		/// </summary>
		public float itsMaxSpeed = 6f;
		
		/// <summary>
		/// Its minimal speed in unity units per second
		/// </summary>
		public float itsMinSpeed = 3f;
		
		
		/// <summary>
		/// Its acceleration.
		/// </summary>
		public float itsAcceleration = 1f;
		
		/// <summary>
		/// Its current speed in unity units per second
		/// </summary>
		[HideInInspector]
		public float itsCurrentSpeed = 3f;
		
		/// <summary>
		/// if true always min speed will be used for current speed
		/// </summary>
		public bool itsSpeedLockToMin = false;
		
		/// <summary>
		/// Its maximal jump height in unity units
		/// </summary>
		public float itsMaxJumpHeight = 5.0f;
		
		/// <summary>
		/// Its minimal jump height in unity units
		/// </summary>
		public float itsMinJumpHeight = 2.0f;
		
		/// <summary>
		/// Its current jump height in unity units
		/// </summary>
		[HideInInspector]
		public float itsCurrentJumpHeight = 2.0f;
		
		/// <summary>
		/// if true always min jumpheight will be used for current jumpheight
		/// </summary>
		public bool itsJumpHeightLockToMin = false;
		
		/// <summary>
		/// Will return true if the character is turning
		/// </summary>
		[HideInInspector]
		public bool itsIsTurning = false;
		
		/// <summary>
		/// The speed the character turns on place
		/// </summary>
		[HideInInspector]
		public float itsCurrentTurnSpeed = 180.0f;
		
		/// <summary>
		/// The speed the character turns towards the camera root rotation
		/// </summary>
		public float itsLerpSpeed = 4.0f;
		
		/// <summary>
		/// This turn speed will be reached in itsTurnAccerlerationTime seconds
		/// </summary>
		public float itsMaxTurnSpeed = 180.0f;
		
		/// <summary>
		/// The turn speed the character starts turning in degrees / second
		/// </summary>
		public float itsMinTurnSpeed = 180.0f;
		
		/// <summary>
		/// When the character starts turning it will start at turnspeed itsMinTurnspeed and will reach the itsMaxTurnSpeed in intsTurnAccelerationTime seconds.
		/// </summary>
		public float itsTurnAccelerationTime = 1.0f;
		
		/// <summary>
		/// The speed current speed will be multiplied by this value if moving backward.
		/// </summary>
		public float itsBackwardSpeedMultiplicator = 0.95f;
		
	}
	
	
	public Data3rdPersonSettings its3rdPersonSettings = new Data3rdPersonSettings ();
	
	#endregion
	
	/// <summary>
	/// Its character.
	/// </summary>
	private KGFCharacter3rdPerson itsCharacter = null;
	
	/// <summary>
	/// Its original link target rotation. Stores the orbitcameras target rotation during switching cameras and restores it after switch.
	/// </summary>
	private float itsOriginalLinkTargetRotation;
	
	/// <summary>
	/// Its original X axis sensitivity. Stores the orbitcameras x axis sensitivity during switching cameras and restores it after switch.
	/// </summary>
	private float itsOriginalXAxisSensitivity;
	
	/// <summary>
	/// Its original Y axis sensitivity. Stores the orbitcameras y axis sensitivity during switching cameras and restores it after switch.
	/// </summary>
	private float itsOriginalYAxisSensitivity;
	
	/// <summary>
	/// Its last mouse click time.
	/// </summary>
	private float itsLastMouseClickTime = 0;
	
	/// <summary>
	/// Its max double click time.
	/// </summary>
	private float itsMaxDoubleClickTime = 0.2f;
	
	/// <summary>
	/// Start method. Initializes gameobjects
	/// </summary>
	protected override void KGFAwake ()
	{
		base.KGFAwake ();
		
		GameObject aSettingsGameObject = GameObject.Find("KGFOrbitCamSettings_3rdPerson").gameObject;
		itsCameraSettings.itsOrbitCamSettings3rdPerson = aSettingsGameObject.GetComponent<KGFOrbitCamSettings> ();
		
		itsPointNavigationSettings.itsClickTarget = transform.FindChild ("clicktarget").gameObject;
		itsPointNavigationSettings.itsClickTargetProjector = itsPointNavigationSettings.itsClickTarget.transform.FindChild ("Projector").GetComponent<Projector> ();
		itsPointNavigationSettings.itsClickTargetInvalidProjector = itsPointNavigationSettings.itsClickTarget.transform.FindChild ("InvalidProjector").GetComponent<Projector> ();
		itsPointNavigationSettings.itsValidParticles = itsPointNavigationSettings.itsClickTarget.transform.FindChild ("ValidParticles").gameObject;
		itsPointNavigationSettings.itsInvalidParticles = itsPointNavigationSettings.itsClickTarget.transform.FindChild ("InvalidParticles").gameObject;
		
		itsCharacter = transform.GetComponentInChildren<KGFCharacter3rdPerson> ();
		
		itsCharacter.SetCharacterController (this);
		
		GameObject anOrbitCamera = GameObject.Find("KGFOrbiterCam");
		
		itsCameraSettings.itsOrbitCam = anOrbitCamera.GetComponent<KGFOrbitCam> ();
		itsCameraSettings.itsOrbitCamTransform = itsCameraSettings.itsOrbitCam.transform;
		itsCameraSettings.itsOrbitCamCamera = itsCameraSettings.itsOrbitCam.GetComponent<Camera> ();
		
		SetCurrentSpeed ( GetMinSpeed ());
		
		itsCameraSettings.itsCameraLookatTransform = transform.FindChild ("restricted/geometrycontainer/cameralookat");
		itsCameraSettings.itsCameraLookatTransformStartPosition = itsCameraSettings.itsCameraLookatTransform.localPosition;
		
		if (itsGlobalSettings.itsGeometry != null)
		{
			itsGlobalSettings.itsGeometryTransform = itsGlobalSettings.itsGeometry.transform;
			
		}
		initOrbitCam();
	}
	
	
	public void initOrbitCam()
	{
		KGFOrbitCamSettings anOrbitCamSettings = itsCameraSettings.itsOrbitCamSettings3rdPerson;
		KGFOrbitCam anOrbitCam = GetOrbitCam();
		anOrbitCam.SetRotationHorizontalCurrent(anOrbitCamSettings.itsRotation.itsHorizontal.itsStartValue);
		anOrbitCam.SetRotationVerticalCurrent(anOrbitCamSettings.itsRotation.itsVertical.itsStartValue);
		anOrbitCam.SetZoomCurrent(anOrbitCamSettings.itsZoom.itsStartValue);
		anOrbitCamSettings.Apply();
	}
	
	
	/// <summary>
	/// Instatiates the click target representation.
	/// </summary>
	/// <param name='theOK'>
	/// The O.
	/// </param>
	/// <param name='theDouble'>
	/// The double.
	/// </param>
	private void InstatiateClickTargetRepresentation (bool theOK, bool theDouble)
	{
		Vector3 aPosition = itsPointNavigationSettings.itsClickTarget.transform.position;
		GameObject anObject = null;
		GameObject anParticleObject = null;
		if (theDouble)
		{
			if (theOK && itsPointNavigationSettings.itsClickTargetRepesentationOKDouble != null)
			{
				anObject = GameObject.Instantiate (itsPointNavigationSettings.itsClickTargetRepesentationOKDouble, aPosition, Quaternion.identity) as GameObject;
				anObject.AddComponent<KGFTempProjector3rdPerson> ();
				anObject.GetComponent<KGFTempProjector3rdPerson> ().Init (itsPointNavigationSettings.itsProjectorShrinkSpeed, itsPointNavigationSettings.itsProjectorOffset);
				anObject.transform.eulerAngles = new Vector3 (90, 180, 0);
				
				anParticleObject = GameObject.Instantiate (itsPointNavigationSettings.itsValidParticles, aPosition, Quaternion.identity) as GameObject;
				anParticleObject.AddComponent<KGFTempProjector3rdPerson> ();
				anParticleObject.GetComponent<KGFTempProjector3rdPerson> ().Init (itsPointNavigationSettings.itsProjectorShrinkSpeed * 2, 0);
				anParticleObject.transform.eulerAngles = new Vector3 (270, 0, 0);
			}
			else if (!theOK && itsPointNavigationSettings.itsClickTargetRepesentationNotOKDouble != null)
			{
				anObject = GameObject.Instantiate (itsPointNavigationSettings.itsClickTargetRepesentationNotOKDouble, aPosition, Quaternion.identity) as GameObject;
				anObject.AddComponent<KGFTempProjector3rdPerson> ();
				anObject.GetComponent<KGFTempProjector3rdPerson> ().Init (itsPointNavigationSettings.itsProjectorShrinkSpeed, itsPointNavigationSettings.itsProjectorOffset);
				anObject.transform.eulerAngles = new Vector3 (90, 180, 0);
				
				anParticleObject = GameObject.Instantiate (itsPointNavigationSettings.itsInvalidParticles, aPosition, Quaternion.identity) as GameObject;
				anParticleObject.AddComponent<KGFTempProjector3rdPerson> ();
				anParticleObject.GetComponent<KGFTempProjector3rdPerson> ().Init (itsPointNavigationSettings.itsProjectorShrinkSpeed * 2, 0);
				anParticleObject.transform.eulerAngles = new Vector3 (270, 0, 0);
			}
		}
		else
		{
			if (theOK && itsPointNavigationSettings.itsClickTargetRepesentationOK != null)
			{
				anObject = GameObject.Instantiate (itsPointNavigationSettings.itsClickTargetRepesentationOK, aPosition, Quaternion.identity) as GameObject;
				anObject.AddComponent<KGFTempProjector3rdPerson> ();
				anObject.GetComponent<KGFTempProjector3rdPerson> ().Init (itsPointNavigationSettings.itsProjectorShrinkSpeed, itsPointNavigationSettings.itsProjectorOffset);
				anObject.transform.eulerAngles = new Vector3 (90, 180, 0);
				
				anParticleObject = GameObject.Instantiate (itsPointNavigationSettings.itsValidParticles, aPosition, Quaternion.identity) as GameObject;
				anParticleObject.AddComponent<KGFTempProjector3rdPerson> ();
				anParticleObject.GetComponent<KGFTempProjector3rdPerson> ().Init (itsPointNavigationSettings.itsProjectorShrinkSpeed * 2, 0);
				anParticleObject.transform.eulerAngles = new Vector3 (270, 0, 0);
			}
			else if (!theOK && itsPointNavigationSettings.itsClickTargetRepesentationNotOK != null)
			{
				anObject = GameObject.Instantiate (itsPointNavigationSettings.itsClickTargetRepesentationNotOK, aPosition, Quaternion.identity) as GameObject;
				anObject.AddComponent<KGFTempProjector3rdPerson> ();
				anObject.GetComponent<KGFTempProjector3rdPerson> ().Init (itsPointNavigationSettings.itsProjectorShrinkSpeed, itsPointNavigationSettings.itsProjectorOffset);
				anObject.transform.eulerAngles = new Vector3 (90, 180, 0);
				
				anParticleObject = GameObject.Instantiate (itsPointNavigationSettings.itsInvalidParticles, aPosition, Quaternion.identity) as GameObject;
				anParticleObject.AddComponent<KGFTempProjector3rdPerson> ();
				anParticleObject.GetComponent<KGFTempProjector3rdPerson> ().Init (itsPointNavigationSettings.itsProjectorShrinkSpeed * 2, 0);
				anParticleObject.transform.eulerAngles = new Vector3 (270, 0, 0);
			}
		}
		anObject.SetActive (true);
		anParticleObject.SetActive (true);
	}
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start ()
	{
		itsCharacter.ConnectCameraContainerToCharacter (true);	//connect camera container to character
		itsCameraSettings.itsOrbitCamSettings3rdPerson.Apply ();
	}

	public GlobalSettings.eVerticalState GetVerticalState ()
	{
		return itsGlobalSettings.itsVerticalState;
	}
	
	public void SetVerticalState (GlobalSettings.eVerticalState theVerticalState)
	{
		itsGlobalSettings.itsVerticalState = theVerticalState;
	}
	
	public void SetPosition (Vector3 thePosition)
	{
		if (itsCharacter != null)
			itsCharacter.transform.position = thePosition;
	}
	
	public Vector3 GetPosition ()
	{
		if (itsCharacter != null)
			return itsCharacter.transform.position;
		else
			return Vector3.zero;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="theLookDirection">
	/// A <see cref="Vector3"/>
	/// </param>
	public void SetLookDirection (Vector3 theLookDirection)
	{
		if (theLookDirection.magnitude == 0.0f)
			return;
		Quaternion aRotation = new Quaternion ();
		if (itsCharacter != null)
		{
			aRotation = KGFUtility.SetLookRotationSafe (aRotation, itsCharacter.transform.up, theLookDirection, new Vector3 (0.0f, 0.0f, 1.0f));
			itsCharacter.transform.rotation = aRotation;
		}
	}
	
	
	

	
	/// <summary>
	/// unity3d update
	/// </summary>
	void Update ()
	{
		HandleClickTarget ();
		HandleInput ();
		CheckSpeedOverGround ();
	}
	
	/// <summary>
	/// Appliers the user input
	/// </summary>
	public void HandleInput ()
	{
		itsGlobalSettings.itsAxis1Horizontal = Input.GetAxis ("Horizontal");
		itsGlobalSettings.itsAxis1Vertical = Input.GetAxis ("Vertical");
		
		
		if (Input.GetButton("Fire2") || itsGlobalSettings.itsUseGamepad && Mathf.Abs(Input.GetAxis("Mouse X")) > 0 || itsGlobalSettings.itsUseGamepad && Mathf.Abs(Input.GetAxis("Mouse Y")) > 0)
		{
			itsCameraSettings.itsCameraLookatTransformTargetPosition = new Vector3 (0.0f, itsCameraSettings.itsCameraLookatTransformStartPosition.y, 0.0f);
		}
		else
		{
			itsCameraSettings.itsCameraLookatTransformTargetPosition = itsCameraSettings.itsCameraLookatTransformStartPosition;
		}
		
		
//		if (Input.GetAxis ("Mouse ScrollWheel") != 0)
//		{
//			itsCameraSettings.itsOrbitCam.SetZoom (itsCameraSettings.itsOrbitCam.GetZoomCurrent () - Input.GetAxis ("Mouse ScrollWheel") * itsCameraSettings.itsOrbitCam.GetZoomAxisSensitivity () * itsCameraSettings.itsOrbitCam.GetZoomSpeed ());
//		}
		
		
		if (Input.GetButtonDown ("Jump"))
		{
			Jump ();
		}
		
		if (Input.GetButton ("Fire3"))
		{
			itsGlobalSettings.itsRunning = true;
		}
		else
		{
			if(!itsPointNavigationSettings.itsClickTargetFollow)
				itsGlobalSettings.itsRunning = false;
		}
		
		
		if (itsGlobalSettings.itsAxis1Horizontal != 0.0f || itsGlobalSettings.itsAxis1Vertical != 0.0f)
			StopFollowingClickTarget ();
		
		if (Input.GetButton("Fire1"))//.GetMouseButtonDown (0))
		{
			MouseClick ();
			itsLastMouseClickTime = Time.time;
		}
	}
	
	private void MouseClick ()
	{
		if (GetUseClickTarget ())
		{
			if(Time.time - itsLastMouseClickTime > itsMaxDoubleClickTime)
			{
				itsGlobalSettings.itsRunning = false;
				SetClickTarget (false);
				SetSpeedNormalized (0.0f);
			}
			else
			{
				itsGlobalSettings.itsRunning = true;
			}
		}
		
	}
	
	public void SetUseClickTarget (bool theUseClickTarget)
	{
		itsPointNavigationSettings.itsEnableClickTarget = theUseClickTarget;
	}
	
	public bool GetUseClickTarget ()
	{
		return itsPointNavigationSettings.itsEnableClickTarget;
	}
	
	/// <summary>
	/// Checks the speed over ground.
	/// </summary>
	public void CheckSpeedOverGround ()
	{
		if(itsCharacter.itsPhysicsCreated)
		{
			float aSpeed = itsCharacter.GetRigidBody ().velocity.magnitude;
			if (itsPointNavigationSettings.itsClickTargetFollow == true)
			{
				if (Time.realtimeSinceStartup - itsPointNavigationSettings.itsClickTargetTime > 1.0f)
				{
					if (aSpeed < GetMinSpeed () / 2.0f)
						StopFollowingClickTarget ();
				}
			}
		}
	}
	
	/// <summary>
	/// Sets the click target and calculates reachability.
	/// </summary>
	/// <param name='theDouble'>
	/// The double.
	/// </param>
	private void SetClickTarget (bool theDouble)
	{
		Ray aRay = itsCameraSettings.itsOrbitCamCamera.ScreenPointToRay (Input.mousePosition);
		
		RaycastHit aCollisionHit;
		if (Physics.Raycast (aRay, out aCollisionHit, 1000.0f, itsPointNavigationSettings.itsClickTargetRaycastLayers.value))
		{
			LayerMask aLayerMask = 1 << aCollisionHit.collider.gameObject.layer;
			int aCorrectLayers = aLayerMask & itsPointNavigationSettings.itsClickTargetBlockLayers.value;
			if (aCorrectLayers != 0)
			{
				return;
			}
			
			if (Vector3.Angle (aCollisionHit.normal, Vector3.up) >= 80)
			{
				Ray aRayDown = new Ray (aCollisionHit.point + 0.1f * aCollisionHit.normal, Vector3.down);
				Physics.Raycast (aRayDown, out aCollisionHit, 1000.0f, itsPointNavigationSettings.itsClickTargetRaycastLayers.value);
			}
			
			
			Vector3 aTargetDirection = aCollisionHit.point - itsCharacter.transform.position;
			aTargetDirection.y = 0.0f;
			float aTargetDistance = Vector3.Distance (aCollisionHit.point, itsCharacter.transform.position);
			
			bool aUseSlope = true;
			Vector3 aClickPosition = GetRealClickTargetPosition (aCollisionHit, out aUseSlope);
			
			if (!IsSlopeOK (aCollisionHit.normal) && aUseSlope)
			{
				SetLookDirection (aTargetDirection);
				itsPointNavigationSettings.itsClickTarget.transform.position = aClickPosition;
				itsPointNavigationSettings.itsClickTargetFollow = false;
				InstatiateClickTargetRepresentation (false, theDouble);
				return;
			}
			if (aTargetDistance < itsPointNavigationSettings.itsClickTargetRotationDistance)
			{
				SetLookDirection (aTargetDirection);
				StopFollowingClickTarget ();
			}
			else if (aTargetDistance > itsPointNavigationSettings.itsClickTargetRotationDistance && aTargetDistance < itsPointNavigationSettings.itsClickTargetMaxDistance)	//correct distance
			{
				itsPointNavigationSettings.itsClickTarget.transform.position = aClickPosition;
				itsPointNavigationSettings.itsClickTargetFollow = true;
				itsPointNavigationSettings.itsClickTargetTime = Time.realtimeSinceStartup;
				InstatiateClickTargetRepresentation (true, theDouble);
			}
			else //too far! look at the target and instantiate itsSlickTargetRepresentationNotOK
			{
				SetLookDirection (aTargetDirection);
				itsPointNavigationSettings.itsClickTarget.transform.position = aClickPosition;
				itsPointNavigationSettings.itsClickTargetFollow = false;
				InstatiateClickTargetRepresentation (false, theDouble);
			}
		}
	}
	
	private Vector3 GetRealClickTargetPosition (RaycastHit theRayCastHit, out bool theUseSlope)
	{
		theUseSlope = true;
		return theRayCastHit.point;
	}
	
	private bool IsSlopeOK (Vector3 theTerrainNormal)
	{
		Vector3 aGroundProjection = new Vector3 (theTerrainNormal.x, 0.0f, theTerrainNormal.z);
		if (aGroundProjection.magnitude == 0.0f)	//terrain normal points up
			return true;
		
		float anAngle = Vector3.Angle (aGroundProjection, theTerrainNormal);
		float aSlope = 90.0f - anAngle;
		if (aSlope > GetMaxSlope ())
			return false;
		return true;
	}
	
	/// <summary>
	/// Handles the click target.
	/// </summary>
	private void HandleClickTarget ()
	{
		if (itsPointNavigationSettings.itsClickTargetFollow == true)
		{
			itsPointNavigationSettings.itsDirectionToClickTarget = itsPointNavigationSettings.itsClickTarget.transform.position - itsCharacter.transform.position;
			itsPointNavigationSettings.itsDirectionToClickTarget.y = 0;
			float aDistanceToTarget = itsPointNavigationSettings.itsDirectionToClickTarget.magnitude;
			itsPointNavigationSettings.itsDirectionToClickTarget.Normalize ();
			
			if (aDistanceToTarget > 0.5f)
			{
				SetLookDirection (itsPointNavigationSettings.itsDirectionToClickTarget);	//character rotation will rotate camera too
			}
			else
			{
				StopFollowingClickTarget ();
			}
		}
	}
	
	/// <summary>
	/// Stops following click target.
	/// </summary>
	public void StopFollowingClickTarget ()
	{
		if (itsPointNavigationSettings.itsClickTargetFollow == true)	//if was following target before stop speed
			SetSpeedNormalized (0.0f);
		
		itsPointNavigationSettings.itsClickTargetFollow = false;
		itsPointNavigationSettings.itsClickTargetTime = 0.0f;
	}
	
	public KGFOrbitCam GetOrbitCam ()
	{
		return itsCameraSettings.itsOrbitCam;
	}
	
	public Transform GetOrbitCamTransform ()
	{
		return itsCameraSettings.itsOrbitCamTransform;
	}
	
	public bool GetIsFollowClickTarget ()
	{
		return itsPointNavigationSettings.itsClickTargetFollow;
	}
	
	/// <summary>
	/// returns the forward backward
	/// </summary>
	/// <returns></returns>
	public float GetAxis1Vertical ()
	{
		if (itsPointNavigationSettings.itsClickTargetFollow)
		{
			return 1.0f;
		}
		return itsGlobalSettings.itsAxis1Vertical;
	}
	
	/// <summary>
	/// returns the rotate left right
	/// </summary>
	/// <returns></returns>
	public float GetAxis1Horizontal ()
	{
		if (itsPointNavigationSettings.itsClickTargetFollow)
		{
			return 0.0f;
		}
		return itsGlobalSettings.itsAxis1Horizontal;
	}
	
	
	/// <summary>
	/// 
	/// </summary>
	public void Jump ()
	{
		SetJumpHeightNormalized(0);
		if (GetVerticalState () == GlobalSettings.eVerticalState.eGrounded)
			itsCharacter.Jump ();
	}
	
	
	/// <summary>
	/// Adds force to character robie
	/// </summary>
	/// <param name="theForce"></param>
	public void AddForce (Vector3 theForce)
	{
		itsCharacter.AddForce (theForce);
	}
	
	
	
	public float GetMaxSlope ()
	{
		return its3rdPersonSettings.itsMaxSlope;
	}
	
	public bool GetIsTurning ()
	{
		return its3rdPersonSettings.itsIsTurning;
	}
	
	public void SetIsTurning (bool theIsTurning)
	{
		its3rdPersonSettings.itsIsTurning = theIsTurning;
	}
	
	public float GetTurnAccerlerationTime ()
	{
		return its3rdPersonSettings.itsTurnAccelerationTime;
	}
	
	public float GetMinTurnSpeed ()
	{
		return its3rdPersonSettings.itsMinTurnSpeed;
	}
	
	public float GetMaxTurnSpeed ()
	{
		return its3rdPersonSettings.itsMaxTurnSpeed;
	}
	
	public void SetCurrentTurnSpeed (float theCurrentTurnSpeed)
	{
		its3rdPersonSettings.itsCurrentTurnSpeed = theCurrentTurnSpeed;
	}
	
	public float GetCurrentTurnSpeed ()
	{
		return its3rdPersonSettings.itsCurrentTurnSpeed;
	}
	
	public void SetCurrentSpeed (float theSpeed)
	{
		its3rdPersonSettings.itsCurrentSpeed = theSpeed;
	}
	
	public float GetCurrentSpeed ()
	{
		
		float aRunMultiplicator = 1;
		
		aRunMultiplicator = its3rdPersonSettings.itsRunSpeedMultiplicator;
		
		if (GetIsSpeedLockedToMin ())
			return GetMinSpeed () * aRunMultiplicator;
		else
			return its3rdPersonSettings.itsCurrentSpeed * aRunMultiplicator;
	}
	
	
	
	public float GetLerpSpeed ()
	{
		return its3rdPersonSettings.itsLerpSpeed;
	}
	
	
	
	public float GetBackwardSpeedMultiplicator ()
	{
		return its3rdPersonSettings.itsBackwardSpeedMultiplicator;
	}
	
	
	public float GetCurrentJumpHeight ()
	{
		float aRunningJumpMultiplicator = 1.0f;
		if(itsGlobalSettings.itsRunning)
		{
			aRunningJumpMultiplicator = its3rdPersonSettings.itsRunningJumpMultiplicator;
		}
		
		if (its3rdPersonSettings.itsJumpHeightLockToMin)
			return its3rdPersonSettings.itsMinJumpHeight * aRunningJumpMultiplicator;
		else
			return its3rdPersonSettings.itsCurrentJumpHeight * aRunningJumpMultiplicator;
	}
	
	
	
	
	public float GetAcceleration ()
	{
		return its3rdPersonSettings.itsAcceleration;
	}
	
	
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="theForwardSpeed">
	/// A <see cref="System.Single"/>
	/// </param>
	public void SetSpeedNormalized (float theForwardSpeed)
	{
		float aSpeed = Mathf.Clamp (theForwardSpeed, 0f, 1f);
		SetCurrentSpeed (GetMinSpeed () + (GetMaxSpeed () - GetMinSpeed ()) * aSpeed);
	}
	
	public float GetSpeedNormalized ()
	{
		return (GetCurrentSpeed () - GetMinSpeed ()) / (GetMaxSpeed () - GetMinSpeed ());
	}
	
	public void SetMinSpeed (float theSpeed)
	{
		its3rdPersonSettings.itsMinSpeed = theSpeed;
	}
	
	public float GetMinSpeed ()
	{
		return its3rdPersonSettings.itsMinSpeed;
	}
	
	public void SetMaxSpeed (float theSpeed)
	{
		its3rdPersonSettings.itsMaxSpeed = theSpeed;
	}
	
	public float GetMaxSpeed ()
	{
		return its3rdPersonSettings.itsMaxSpeed;
	}

	public void SetIsSpeedLockedToMin (bool theLockToMin)
	{
		its3rdPersonSettings.itsSpeedLockToMin = theLockToMin;
	}
	
	public bool GetIsSpeedLockedToMin ()
	{
		return its3rdPersonSettings.itsSpeedLockToMin;
	}
	
	public Vector3 GetRigidbodyVelocity ()
	{
		if (itsCharacter == null)
			return Vector3.zero;
		return itsCharacter.GetRigidbodyVelocity ();
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="theJumpHeight"></param>
	public void SetJumpHeightNormalized (float theJumpHeight)
	{
		float aJumpHeight = Mathf.Clamp (theJumpHeight, 0f, 1f);
		its3rdPersonSettings.itsCurrentJumpHeight = its3rdPersonSettings.itsMinJumpHeight + (its3rdPersonSettings.itsMaxJumpHeight - its3rdPersonSettings.itsMinJumpHeight) * aJumpHeight;
	}
	
	/// <summary>
	/// Locks the Jump Height to the min value in this case it cant be increased by setJumpHeightNormalized
	/// </summary>
	/// <param name="theLockToMin"></param>
	public void SetIsJumpHeightLockedToMin (bool theLockToMin)
	{
		its3rdPersonSettings.itsJumpHeightLockToMin = theLockToMin;
	}
	
	public void EnableClickTarget (bool theValue)
	{
		itsPointNavigationSettings.itsEnableClickTarget = theValue;
	}
	
	public bool GetClickTargetEnabled ()
	{
		return itsPointNavigationSettings.itsEnableClickTarget;
	}
	
	public KGFCharacter3rdPerson GetCharacter ()
	{
		return itsCharacter;
	}
	
	public void SetUseGampad(bool theValue)
	{
		itsGlobalSettings.itsUseGamepad = theValue;
	}
	
	public bool GetUseGampad()
	{
		return itsGlobalSettings.itsUseGamepad;
	}
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	#region validate

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public KGFMessageList Validate ()
	{
		KGFMessageList aMessageList = new KGFMessageList ();
		
		if (itsGlobalSettings.itsGeometry == null)
		{
			aMessageList.AddError ("its Geometry should nor be empty. Assign a character.");
		}
		if (itsGlobalSettings.itsMass <= 0)
		{
			aMessageList.AddError ("its Mass should be > 0.");
		}
		if (itsGlobalSettings.itsThickness <= 0)
		{
			aMessageList.AddError ("its Thickness should be > 0.");
		}
		if (itsGlobalSettings.itsHeight <= 0)
		{
			aMessageList.AddError ("its Height should be > 0.");
		}
		
		if(itsPointNavigationSettings.itsClickTargetRepesentationOK == null)
		{
			aMessageList.AddError ("itsClickTargetRepesentationOK should not be empty. Assign a projector");
		}
		if(itsPointNavigationSettings.itsClickTargetRepesentationNotOK == null)
		{
			aMessageList.AddError ("itsClickTargetRepesentationNotOK should not be empty. Assign a projector");
		}
		if(itsPointNavigationSettings.itsClickTargetRepesentationOKDouble == null)
		{
			aMessageList.AddError ("itsClickTargetRepesentationOKDouble should not be empty. Assign a projector");
		}
		if(itsPointNavigationSettings.itsClickTargetRepesentationNotOKDouble == null)
		{
			aMessageList.AddError ("itsClickTargetRepesentationNotOKDouble should not be empty. Assign a projector");
		}

		return aMessageList;
	}

	#endregion
}
