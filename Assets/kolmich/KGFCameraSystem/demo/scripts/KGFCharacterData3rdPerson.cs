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
// <date>2010-02-24</date>
// <summary>short summary</summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KGFCharacterData3rdPerson
{
	/// <summary>
	/// the container where the geometry gets parented
	/// </summary>
	[HideInInspector]
	public Transform itsGeometryContainerTransform = null;
	
	/// <summary>
	/// the root for the KGFOrbitCam
	/// </summary>
	[HideInInspector]
	public Transform itsCameraRootTransform = null;
	
	/// <summary>
	/// Character rotator used for procedural rotation
	/// </summary>
	[HideInInspector]
	public GameObject itsCharacterRotator = null;
	
	/// <summary>
	/// Its character object.
	/// </summary>
	[HideInInspector]
	public Transform itsCharacterObject = null;
	
	/// <summary>
	/// The character controller module this character container belongs to
	/// </summary>
	[HideInInspector]
	public KGFCharacterController3rdPerson itsCharacterController = null;
	
	/// <summary>
	/// Rigidbody of the character
	/// </summary>
	[HideInInspector]
	public Rigidbody itsRigidBody = null;
	
	/// <summary>
	/// CapsuleCollider of the character
	/// </summary>
	[HideInInspector]
	public CapsuleCollider itsCollider = null;

	/// <summary>
	/// Ground collision contact points used to calculate ground normal
	/// </summary>
	[HideInInspector]
	public List<ContactPoint> itsContactPoints = new List<ContactPoint>();
	
	/// <summary>
	/// Up direction of the character. Default is y = 1.0f;
	/// </summary>
	[HideInInspector]
	public Vector3 itsUpDirection = Vector3.up;
	
	/// <summary>
	/// The direction of the ground normal beneath the character
	/// </summary>
	[HideInInspector]
	public Vector3 itsGroundNormalDirection = Vector3.up;
	
	/// <summary>
	/// The direction of the ground normal beneath the character adapted by lerp -> changes are smooth
	/// </summary>
	[HideInInspector]
	public Vector3 itsLerpedGroundNormalDirection = Vector3.up;
	
	/// <summary>
	/// Standard gravity direction is down
	/// </summary>
	[HideInInspector]
	public Vector3 itsGravityDirection = Vector3.down;
	
	/// <summary>
	/// this flag is used to remember if the character was reorienented to the camera. (used in follower mode)
	/// </summary>
	[HideInInspector]
	public bool itsReoriented = false;
	
	/// <summary>
	/// used for hybrid mode accurate rotation
	/// </summary>
	[HideInInspector]
	public bool itsAccurateReorientationReached = false;
	
	/// <summary>
	/// Indicates if the character is moving in any direction
	/// </summary>
	[HideInInspector]
	public bool itsIsMoving = false;
	
	/// <summary>
	/// The direction the camera is facing
	/// </summary>
	[HideInInspector]
	public Vector3 itsCameraDirection = Vector3.zero;
	
	/// <summary>
	/// caching user input
	/// </summary>
	[HideInInspector]
	public Vector3 itsResultingControllerDirection = Vector3.zero;
	
	/// <summary>
	/// chaching velocity of the rigidbody
	/// </summary>
	[HideInInspector]
	public Vector3 itsRigidBodyVelocity = Vector3.zero;
	
	/// <summary>
	/// cache for the transform component
	/// </summary>
	[HideInInspector]
	public Transform itsTransform = null;
}