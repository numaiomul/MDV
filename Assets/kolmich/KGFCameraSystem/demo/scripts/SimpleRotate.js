
#pragma strict

public var speed : Vector3;

function OnBecameVisible () {
	enabled = true;	
}

function OnBecameInvisible () {
	enabled = false;	
}

function Update () {
	transform.Rotate(Time.deltaTime * speed.x, Time.deltaTime * speed.y, Time.deltaTime * speed.z);
}