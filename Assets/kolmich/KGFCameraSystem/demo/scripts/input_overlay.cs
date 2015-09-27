using UnityEngine;
using System.Collections;

public class input_overlay : MonoBehaviour
{
	
	public Texture2D itsMouseDrag;
	public Texture2D itsMouseMove;
	public Texture2D itsMouseRotate;
	public Texture2D itsMouseZoom;
	public Texture2D itsKeys;
	
	public KGFOrbitCam itsOrbitCam;
	public GameObject itsCharacter;
	public GameObject itsPanning;
	
	public KGFCameraSystemDemo itsDemo;
	
	private Rect itsRect;
	
	// Use this for initialization
	void Start ()
	{
		itsRect = new Rect(0,0,Screen.width, Screen.height);
	}
	
	public void OnGUI()
	{
		GUILayout.BeginArea(itsRect);
		{
			KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxInvisible);
			{
				GUILayout.FlexibleSpace();
				
				GUILayout.BeginHorizontal();
				{
					GUILayout.FlexibleSpace();
					
					if(itsDemo.itsCurrentCameraRoot == KGFCameraSystemDemo.eCameraRoot.eCharacter)
					{
						KGFGUIUtility.Box(itsKeys,KGFGUIUtility.eStyleBox.eBoxInvisible, GUILayout.Height(Screen.height/10f), GUILayout.Width(Screen.width/10f));
//						KGFGUIUtility.Box(itsMouseMove,KGFGUIUtility.eStyleBox.eBoxInvisible, GUILayout.Height(Screen.height/10f), GUILayout.Width(Screen.width/10f));
						KGFGUIUtility.Box(itsMouseRotate,KGFGUIUtility.eStyleBox.eBoxInvisible, GUILayout.Height(Screen.height/10f), GUILayout.Width(Screen.width/10f));
						KGFGUIUtility.Box(itsMouseZoom,KGFGUIUtility.eStyleBox.eBoxInvisible, GUILayout.Height(Screen.height/10f), GUILayout.Width(Screen.width/10f));
						//KGFGUIUtility.Box(itsMouseZoom,KGFGUIUtility.eStyleBox.eBoxInvisible, GUILayout.Height(Screen.height/10f), GUILayout.Width(Screen.width/10f));
					}
					else if(itsDemo.itsCurrentCameraRoot == KGFCameraSystemDemo.eCameraRoot.eObserve)
					{
						KGFGUIUtility.Box(itsKeys,KGFGUIUtility.eStyleBox.eBoxInvisible, GUILayout.Height(Screen.height/10f), GUILayout.Width(Screen.width/10f));
					}
					else if(itsDemo.itsCurrentCameraRoot == KGFCameraSystemDemo.eCameraRoot.ePanning || itsDemo.itsCurrentCameraRoot == KGFCameraSystemDemo.eCameraRoot.ePanningCamera)
					{
						KGFGUIUtility.Box(itsKeys,KGFGUIUtility.eStyleBox.eBoxInvisible, GUILayout.Height(Screen.height/10f), GUILayout.Width(Screen.width/10f));
						KGFGUIUtility.Box(itsMouseRotate,KGFGUIUtility.eStyleBox.eBoxInvisible, GUILayout.Height(Screen.height/10f), GUILayout.Width(Screen.width/10f));
					}
					else
					{
						KGFGUIUtility.Box(itsMouseRotate,KGFGUIUtility.eStyleBox.eBoxInvisible, GUILayout.Height(Screen.height/10f), GUILayout.Width(Screen.width/10f));
						KGFGUIUtility.Box(itsMouseZoom,KGFGUIUtility.eStyleBox.eBoxInvisible, GUILayout.Height(Screen.height/10f), GUILayout.Width(Screen.width/10f));
					}
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(32);
			}
			GUILayout.EndVertical();
		}
		GUILayout.EndArea();
	}
}
