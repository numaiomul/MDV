using System;
using System.Collections;
using UnityEngine;

public class KGFCameraSystemDemo : KGFModule
{
	private KGFOrbitCam itsOrbitCam = null;
	public Texture2D itsKOLMICHTexture = null;
	public KGFOrbitCamSettings itsEventSwitchToCapsule;
	public KGFOrbitCamSettings itsEventSwitchToPanning;
	public KGFOrbitCamSettings itsEventSwitchToPanningCameraSpace;
	public KGFOrbitCamSettings itsEventSwitchToFollow;
	public KGFOrbitCamSettings itsEventSwitchToFollow1;
	public KGFOrbitCamSettings itsEventSwitchToInnerCity;
	public KGFOrbitCamSettings itsEventSwitchToUpSideDown;
	public KGFOrbitCamSettings itsEventSwitchToFishEye;
	public KGFOrbitCamSettings itsEventSwitchToCollisions;
	public KGFOrbitCamSettings itsEventSwitchToObserve;
	public KGFOrbitCamSettings itsEventIsometric;
	
	public KGFCutscene itsCutScene;
	
	
	private Rect itsRect;
	private Rect itsRectButtons;
	
	/// <summary>
	/// The possible camera roots
	/// </summary>
	public enum eCameraRoot
	{
		eCharacter,
		eLeftTower,
		eLake,
		ePanning,
		eFollow,
		eFollowPosition,
		eUpSideDown,
		eFishEye,
		eCollision,
		eObserve,
		eIsometric,
		ePanningCamera
	}
	
	/// <summary>
	/// The current
	/// </summary>
	public eCameraRoot itsCurrentCameraRoot = eCameraRoot.eCharacter;
	
	
	public KGFCameraSystemDemo() : base(new Version(1,0,0,0), new Version(1,1,0,0))
	{}
	
	protected override void KGFAwake()
	{
		base.KGFAwake();
		itsRect = new Rect(0.0f,0.0f,350.0f,Screen.height);
		itsRectButtons = new Rect(0.0f,0.0f,Screen.width,150.0f);
	}
	
	void Start()
	{
		itsOrbitCam = KGFAccessor.GetObject<KGFOrbitCam>();
		itsEventSwitchToObserve.Apply();
		itsCurrentCameraRoot = eCameraRoot.eObserve;
		itsCutScene.StopCutscene();
	}
	
	void Update()
	{
		if(itsCurrentCameraRoot == eCameraRoot.eCharacter && !Input.GetMouseButton(1))
			itsOrbitCam.SetRotationToStartValues();
	}
	
	/// <summary>
	/// Draw buttons
	/// </summary>
	void OnGUI()
	{
		float aButtonWidth = 130.0f;
		KGFGUIUtility.SetSkinIndex(1);
		GUILayout.BeginArea(itsRectButtons);
		{
			GUILayout.BeginHorizontal();
			{
				KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxDecorated);
				{
					GUILayout.BeginHorizontal();
					{
						KGFGUIUtility.Label("TEST HERE! ->");
						GUILayout.FlexibleSpace();
						GUI.color = new Color(0.5f,1.0f,0.5f,1.0f);
						if(KGFGUIUtility.Button("follow rot.",KGFGUIUtility.eStyleButton.eButton,GUILayout.Width(aButtonWidth)))
						{
							itsEventSwitchToFollow.Apply();
							itsCurrentCameraRoot = eCameraRoot.eFollow;
							itsCutScene.StopCutscene();
						}
						if(KGFGUIUtility.Button("follow pos.",KGFGUIUtility.eStyleButton.eButton,GUILayout.Width(aButtonWidth)))
						{
							itsEventSwitchToFollow1.Apply();
							itsCurrentCameraRoot = eCameraRoot.eFollowPosition;
							itsCutScene.StopCutscene();
						}
						if(KGFGUIUtility.Button("pan + borders",KGFGUIUtility.eStyleButton.eButton,GUILayout.Width(aButtonWidth)))
						{
							itsEventSwitchToPanning.Apply();
							itsCurrentCameraRoot = eCameraRoot.ePanning;
							itsCutScene.StopCutscene();
						}
						if(KGFGUIUtility.Button("pan world space",KGFGUIUtility.eStyleButton.eButton,GUILayout.Width(aButtonWidth)))
						{
							itsEventSwitchToPanningCameraSpace.Apply();
							itsCurrentCameraRoot = eCameraRoot.ePanningCamera;
							itsCutScene.StopCutscene();
						}
						if(KGFGUIUtility.Button("lookat",KGFGUIUtility.eStyleButton.eButton,GUILayout.Width(aButtonWidth)))
						{
							itsEventSwitchToInnerCity.Apply();
							itsCurrentCameraRoot = eCameraRoot.eLake;
							itsCutScene.StopCutscene();
						}
						if(KGFGUIUtility.Button("up side down",KGFGUIUtility.eStyleButton.eButton,GUILayout.Width(aButtonWidth)))
						{
							itsEventSwitchToUpSideDown.Apply();
							itsCurrentCameraRoot = eCameraRoot.eUpSideDown;
							itsCutScene.StopCutscene();
						}
						GUI.color = new Color(1.0f,1.0f,1.0f,1.0f);
					}
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
					{
						KGFGUIUtility.Label("AND HERE! ->");
						GUILayout.FlexibleSpace();
						GUI.color = new Color(0.5f,1.0f,0.5f,1.0f);
						if(KGFGUIUtility.Button("cutscene",KGFGUIUtility.eStyleButton.eButton,GUILayout.Width(aButtonWidth)))
						{
							itsCutScene.StartCutscene();
						}
						if(KGFGUIUtility.Button("char. 3rd pers.",KGFGUIUtility.eStyleButton.eButton,GUILayout.Width(aButtonWidth)))
						{
							itsEventSwitchToCapsule.Apply();
							itsCurrentCameraRoot = eCameraRoot.eCharacter;
							itsCutScene.StopCutscene();
						}
						if(KGFGUIUtility.Button("char. observe",KGFGUIUtility.eStyleButton.eButton,GUILayout.Width(aButtonWidth)))
						{
							itsEventSwitchToObserve.Apply();
							itsCurrentCameraRoot = eCameraRoot.eObserve;
							itsCutScene.StopCutscene();
						}
						if(KGFGUIUtility.Button("char. isometric",KGFGUIUtility.eStyleButton.eButton,GUILayout.Width(aButtonWidth)))
						{
							itsEventIsometric.Apply();
							itsCurrentCameraRoot = eCameraRoot.eIsometric;
							itsCutScene.StopCutscene();
						}
						if(KGFGUIUtility.Button("collide",KGFGUIUtility.eStyleButton.eButton,GUILayout.Width(aButtonWidth)))
						{
							itsEventSwitchToCollisions.Apply();
							itsCurrentCameraRoot = eCameraRoot.eCollision;
							itsCutScene.StopCutscene();
						}						
						if(KGFGUIUtility.Button("field of view",KGFGUIUtility.eStyleButton.eButton,GUILayout.Width(aButtonWidth)))
						{
							itsEventSwitchToFishEye.Apply();
							itsCurrentCameraRoot = eCameraRoot.eFishEye;
							itsCutScene.StopCutscene();
						}						
						GUI.color = new Color(1.0f,1.0f,1.0f,1.0f);
					}
					GUILayout.EndHorizontal();
				}
				KGFGUIUtility.EndVerticalBox();
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndArea();
		
		KGFGUIUtility.SetSkinIndex(0);
		GUILayout.BeginArea(itsRect);
		{
			GUILayout.BeginVertical();
			{
				GUILayout.FlexibleSpace();
				GUILayout.BeginHorizontal();
				{
					KGFGUIUtility.Space();
					GUILayout.Label(itsKOLMICHTexture);
				}
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				{
					KGFGUIUtility.Space();
					KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxDecorated);
					{
						KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxDarkTop);
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Label("CURRENT CAMERA SETTINGS:");
						GUILayout.FlexibleSpace();
						KGFGUIUtility.EndHorizontalBox();
						
						//target
						KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
						KGFGUIUtility.Label("Target:");
						GUILayout.FlexibleSpace();
						KGFGUIUtility.EndHorizontalBox();
						GUILayout.BeginHorizontal();
						KGFGUIUtility.Label("- gameObject: ");
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Label(itsOrbitCam.GetTarget().gameObject.name);
						GUILayout.EndHorizontal();
						float aFollowPositionSpeed = DrawFloat("- follow pos. speed:",itsOrbitCam.GetTargetFollowPositionSpeed(),true,1.0f,20.0f);
						float aFollowRotationSpeed = DrawFloat("- follow rot. speed:",itsOrbitCam.GetTargetFollowRotationSpeed(),true,1.0f,20.0f);
						itsOrbitCam.SetTargetFollowPositionSpeed(aFollowPositionSpeed);
						itsOrbitCam.SetTargetFollowRotationSpeed(aFollowRotationSpeed);
						
						//zoom
						KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
						KGFGUIUtility.Label("Zoom:");
						GUILayout.FlexibleSpace();
						KGFGUIUtility.EndHorizontalBox();
						bool anEnabled = DrawBoolean("- enabled: ",itsOrbitCam.GetZoomEnable());
						itsOrbitCam.SetZoomEnable(anEnabled);
						float aMinLimit = DrawFloat("- min limit:",itsOrbitCam.GetZoomMinLimit(),itsOrbitCam.GetZoomUseLimits(),0.5f,20.0f);
						float aMaxLimit = DrawFloat("- max limit:",itsOrbitCam.GetZoomMaxLimit(),itsOrbitCam.GetZoomUseLimits(),0.5f,20.0f);
						if(itsOrbitCam.GetZoomUseLimits())
						{
							itsOrbitCam.SetZoomMinLimit(aMinLimit);
							itsOrbitCam.SetZoomMaxLimit(aMaxLimit);
						}
						
						//rotation horizontal
						KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
						KGFGUIUtility.Label("Horizontal rotation:");
						GUILayout.FlexibleSpace();
						KGFGUIUtility.EndHorizontalBox();
						anEnabled = DrawBoolean("- enabled: ",itsOrbitCam.GetRotationHorizontalEnable());
						itsOrbitCam.SetRotationHorizontalEnable(anEnabled);
						float aLeftLimit = DrawFloat("- left limit:",itsOrbitCam.GetRotationHorizontalLeftLimit(),itsOrbitCam.GetRotationHorizontalUseLimits(),0.0f,180.0f);
						float aRightLimit = DrawFloat("- right limit:",itsOrbitCam.GetRotationHorizontalRightLimit(),itsOrbitCam.GetRotationHorizontalUseLimits(),0.0f,180.0f);
						if(itsOrbitCam.GetRotationHorizontalUseLimits())
						{
							itsOrbitCam.SetRotationHorizontalLeftLimit(aLeftLimit);
							itsOrbitCam.SetRotationHorizontalRightLimit(aRightLimit);
						}
						
						//rotation vertical
						KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
						KGFGUIUtility.Label("Vertical rotation:");
						GUILayout.FlexibleSpace();
						KGFGUIUtility.EndHorizontalBox();
						anEnabled = DrawBoolean("- enabled: ",itsOrbitCam.GetRotationVerticalEnable());
						itsOrbitCam.SetRotationVerticalEnable(anEnabled);
						float anUpLimit = DrawFloat("- up limit:",itsOrbitCam.GetRotationVerticalUpLimit(),itsOrbitCam.GetRotationVerticalUseLimits(),0.0f,180.0f);
						float aDownLimit = DrawFloat("- down limit:",itsOrbitCam.GetRotationVerticalDownLimit(),itsOrbitCam.GetRotationVerticalUseLimits(),0.0f,180.0f);
						if(itsOrbitCam.GetRotationVerticalUseLimits())
						{
							itsOrbitCam.SetRotationVerticalUpLimit(anUpLimit);
							itsOrbitCam.SetRotationVerticalDownLimit(aDownLimit);
						}
						
						//panning left right
						KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
						KGFGUIUtility.Label("Panning:");
						GUILayout.FlexibleSpace();
						KGFGUIUtility.EndHorizontalBox();
						
						float aSpeed = DrawFloat("- speed: ",itsOrbitCam.GetPanningSpeed(),true,1.0f,10.0f);
						itsOrbitCam.SetPanningSpeed(aSpeed);
						anEnabled = DrawBoolean("- left right enabled: ",itsOrbitCam.GetPanningLeftRightEnable());
//						itsOrbitCam.SetPanningLeftRightEnable(anEnabled);
						anEnabled = DrawBoolean("- forward backward enabled: ",itsOrbitCam.GetPanningForwardBackwardEnable());
//						itsOrbitCam.SetPanningForwardBackwardEnable(anEnabled);
						anEnabled = DrawBoolean("- up down enabled: ",itsOrbitCam.GetPanningUpDownEnable());
//						itsOrbitCam.SetPanningUpDownEnable(anEnabled);
						
						//camera
						KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
						KGFGUIUtility.Label("Camera:");
						GUILayout.FlexibleSpace();
						KGFGUIUtility.EndHorizontalBox();
						float aFieldOfView = DrawFloat("- field of view: ",itsOrbitCam.GetCameraFieldOfView(),true,45.0f,179.0f);
						itsOrbitCam.SetCameraFieldOfView(aFieldOfView);
						
						//lookat target
						KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
						KGFGUIUtility.Label("Lookat:");
						GUILayout.FlexibleSpace();
						KGFGUIUtility.EndHorizontalBox();
						GUILayout.BeginHorizontal();
						KGFGUIUtility.Label("- target: ");
						GUILayout.FlexibleSpace();
						if(itsOrbitCam.GetLookatTarget() != null)
						{
							KGFGUIUtility.Label(itsOrbitCam.GetLookatTarget().gameObject.name);
						}
						else
						{
							KGFGUIUtility.Label("NO TARGET");
						}
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal();
						KGFGUIUtility.Label("- up vector target: ");
						GUILayout.FlexibleSpace();
						if(itsOrbitCam.GetLookatUpVectorSource() != null)
						{
							KGFGUIUtility.Label(itsOrbitCam.GetLookatUpVectorSource().gameObject.name);
						}
						else
						{
							KGFGUIUtility.Label("NO TARGET");
						}
						GUILayout.EndHorizontal();
					}
					KGFGUIUtility.EndVerticalBox();
				}
				GUILayout.EndHorizontal();
			}
			KGFGUIUtility.Space();
			GUILayout.EndVertical();
		}
		GUILayout.EndArea();
	}
	
	private float DrawFloat(string theLimitTitle, float theLimitValue, bool theUseLimits, float theMinValue, float theMaxValue)
	{
		//rotation left limit
		GUILayout.BeginHorizontal();
		string aCurrentLimitString = "NO LIMIT SET";
		if(theUseLimits)
			aCurrentLimitString = string.Format("{0:0.00}",theLimitValue);
		KGFGUIUtility.Label(theLimitTitle);
		float aValue = 0.0f;
		if(theUseLimits)
		{
			GUILayout.FlexibleSpace();
			aValue = KGFGUIUtility.HorizontalSlider(theLimitValue,theMinValue,theMaxValue,GUILayout.Width(120.0f));
		}
		KGFGUIUtility.Label(aCurrentLimitString);
		GUILayout.EndHorizontal();
		return aValue;
	}
	
	private bool DrawBoolean(string theTitle, bool theValue)
	{
		//rotation left limit
		GUILayout.BeginHorizontal();
		KGFGUIUtility.Label(theTitle);
		GUILayout.FlexibleSpace();
		bool aValue = KGFGUIUtility.Toggle(theValue,"",KGFGUIUtility.eStyleToggl.eTogglCompact);
		GUILayout.EndHorizontal();
		return aValue;
	}
	
	#region KGFModule methods
	public override KGFMessageList Validate()
	{
		return new KGFMessageList();
	}
	
	public override string GetName()
	{
		return name;
	}
	
	public override Texture2D GetIcon()
	{
		return null;
	}
	
	public override string GetForumPath()
	{
		return "";
	}
	
	public override string GetDocumentationPath()
	{
		return "";
	}
	#endregion
}
