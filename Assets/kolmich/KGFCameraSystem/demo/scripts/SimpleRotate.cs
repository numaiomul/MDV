using UnityEngine;
using System.Collections;

public class SimpleRotate : MonoBehaviour
{
	public Vector3 itsRotate;
	
	private Vector3 itsCurrentRotation;
	void Update ()
	{
		itsCurrentRotation.x += itsRotate.x/360.0f * Time.deltaTime;
		itsCurrentRotation.y += itsRotate.y/360.0f * Time.deltaTime;
		itsCurrentRotation.z += itsRotate.z/360.0f * Time.deltaTime;
		gameObject.transform.localRotation = Quaternion.Euler(itsCurrentRotation);
	}
}
