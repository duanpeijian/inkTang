using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

public class Data
{
	public delegate void PropertyChangedEventHandler (object sender, string e);

	public event PropertyChangedEventHandler PropertyChanged;

	protected virtual void OnPropertyChanged(string propertyName){
		if (PropertyChanged != null) {
			PropertyChanged (this, propertyName);
		}
	}

	protected bool SetField<T>(ref T field, T value, string propertyName){
		if (EqualityComparer<T>.Default.Equals (field, value))
			return false;

		field = value;

		OnPropertyChanged (propertyName);
		return true;
	}
}

public class AppSate : Data {
	private bool mShowModel;

	public bool ShowModel {
		get {
			return mShowModel;
		}
		set {
			SetField<bool>(ref mShowModel, value, "ShowModel");
		}
	}
}

public class UIControls : MonoBehaviour {

	static public AppSate state;

	private Transform mCamera;

	private Canvas mCanvas;
	private Button resetBtn;

	// Use this for initialization
	void Start () {
		mCanvas = GameObject.Find ("Canvas").GetComponent<Canvas> ();
		Button exitBtn = mCanvas.transform.FindChild ("ExitBtn").GetComponent<Button> ();
		exitBtn.GetComponentInChildren<Text> ().text = "退出";
		exitBtn.onClick.AddListener (() => {
			Application.Quit ();
		});

		Graphic background = mCanvas.GetComponentInChildren<DefaultGraphic> ();
//		Vector2 size = background.rectTransform.rect.size;
//		Debug.Log (string.Format ("size: {0}", size));

		EventTrigger trigger = background.gameObject.AddComponent<EventTrigger> ();

		EventTrigger.Entry entry = new EventTrigger.Entry ();
		trigger.triggers.Add (entry);
		entry.eventID = EventTriggerType.Drag;

		GameObject scene = GameObject.Find ("Scene");

		resetBtn = mCanvas.transform.Find ("ResetBtn").GetComponent<Button>();
		resetBtn.GetComponentInChildren<Text>().text = "重置";
		resetBtn.gameObject.SetActive (false);

		state = new AppSate ();
		state.PropertyChanged += (data, propertyName) => {
			switch(propertyName){
			case "ShowModel":
				scene.SetActive(state.ShowModel);
				resetBtn.gameObject.SetActive(state.ShowModel);

				if(state.ShowModel){
					entry.callback.AddListener (OnDrag);
				}
				else{
					entry.callback.RemoveListener(OnDrag);
				}

				break;
			}
		};

		scene.SetActive (state.ShowModel);
		mCamera = scene.GetComponentInChildren<Camera> ().transform;

		focus = scene.transform.Find("CameraFocus").transform;


		Quaternion origin = focus.rotation;


		resetBtn.onClick.AddListener (() => {
			focus.rotation = origin;
			rotationTarget = origin;
		});

		//mCamera.LookAt (focus);
		//rotationTarget = mCamera.rotation;
		rotationTarget = origin;

		//state.ShowModel = true;
	}

	private Transform focus;
	private Quaternion rotationTarget;

	void OnDrag(BaseEventData data){
		PointerEventData eventData = (PointerEventData)data;

		Vector2 delta = Vector2.Scale (eventData.delta, new Vector2 (0.1f, 0.1f));

		rotationTarget = rotationTarget * Quaternion.Euler (new Vector3 (delta.y, delta.x, 0f));

		focus.rotation = rotationTarget;

//		Transform focus = mCamera.parent;
//		float distance = (mCamera.position - focus.position).magnitude;
//
//		Quaternion reserve = focus.rotation;
//		focus.rotation = rotationTarget;
//		Vector3 pos = focus.position + focus.TransformVector (Vector3.back) * distance;
//		focus.rotation = reserve;
//
//		mCamera.position = pos;
//		mCamera.rotation = rotationTarget;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
