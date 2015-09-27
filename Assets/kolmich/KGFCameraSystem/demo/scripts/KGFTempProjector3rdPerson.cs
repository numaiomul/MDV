using UnityEngine;
using System.Collections;

public class KGFTempProjector3rdPerson : MonoBehaviour {

	public float itsTime = 0.5f;
	public float itsOffsetY = 3;
	private float itsCurrentTime;
	
	// Use this for initialization
	void Start () {
		Vector3 aPosition = transform.position;
		aPosition.y += itsOffsetY;
		transform.position = aPosition;
		itsCurrentTime = 0;
	}
	
	public void Init(float theTime, float theOffsetY)
	{
		itsTime = theTime;
		itsOffsetY = theOffsetY;
	}
	
	// Update is called once per frame
	void Update () {
		if(itsCurrentTime < itsTime)
		{
			itsCurrentTime += Time.deltaTime;
			Vector3 aPosition = transform.position;
			aPosition.y -= itsOffsetY / itsTime * Time.deltaTime;
			transform.position = aPosition;
		}
		else
		{
			Destroy(gameObject);
		}
	
	}
}
