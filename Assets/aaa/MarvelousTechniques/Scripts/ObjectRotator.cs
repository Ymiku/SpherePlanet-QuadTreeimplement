//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DragDirection{
	private static int MAX_ITEM_COUNT = 2;
	private List<float> items = new List<float> ();
	private float currentDirection=0;

	public void addItem(float item){
		if (items.Count == MAX_ITEM_COUNT) {
			if(item!=items[1]){
				items.RemoveAt(0);
				items.Insert (MAX_ITEM_COUNT-1,item);
			}
		} 
		else {
			items.Add (item);
		}
		// Check if direction has changed
		float dir = getDirection ();
		if (currentDirection != 0 && dir != currentDirection) {
			//items.RemoveAt(0);
		}
		currentDirection = dir;
	}

	public float getDirection(){
		if (items.Count <2) {
			return 0;
		}
		float start = items[0];
		float end = items [items.Count - 1];
		return end - start;
	}

	public void clear(){
		items.Clear ();
	}
}

public class DragForce{
	private static int MAX_ITEM_COUNT = 5;
	private static int DELAY = 1;
	private List<float> items = new List<float> ();
	private int delayIndex = 0;

	public void addItem(float item){
		if (items.Count == MAX_ITEM_COUNT) {
			items.RemoveAt(0);
			items.Insert (MAX_ITEM_COUNT-1,item);
		} 
		else {
			if(delayIndex>DELAY){
				items.Add (item);
			}
			else{
				delayIndex++;
			}
		}
	}

	public float getForce(){
		if (items.Count <2) {
			return 0;
		}

		float start = items[0];
		float end = items [items.Count - 1];

		return start-end;
	}

	public void clear(){
		delayIndex = 0;
		items.Clear ();
	}
}

[RequireComponent (typeof (Collider))]
public class ObjectRotator : MonoBehaviour {
	public Transform transformToRotate;
	public bool insideTransformToRotate = false;
	public float speed = 0.3f;
	private bool horizontal = true;
	private Quaternion fromRotation;
	private Quaternion toRotation;

	private float clickPosition;
	private IEnumerator slowDownCoroutine = null;

	private DragForce dragForce = new DragForce();
	private DragDirection dragDirection=new DragDirection();
	private float lastRotation;
	private static float MAX_DRAG_FORCE = 6f;
	private static float SNAP_DISTANCE = 5f;

	private float animationTime =0;
	private static float ANIMATION_DURATION = 1.0f;

	void Start () {
		if (transformToRotate == null) {
			transformToRotate = transform;
		}
	}
	
	void OnMouseDown() {
		if (slowDownCoroutine!=null) {
			StopCoroutine (slowDownCoroutine);
		}
		clickPosition = getCurrentMousePosition();

		dragForce.addItem (clickPosition);
		dragDirection.addItem (clickPosition);
	}

	void OnMouseUp() {
		dragForce.clear ();

		float toRotation = getClosestSnapRotation();
		float fromRotation = (horizontal ? transformToRotate.eulerAngles.y : transformToRotate.eulerAngles.x);
		slowDownCoroutine = slowRotationDown (fromRotation, toRotation);
		StartCoroutine (slowDownCoroutine);
	}

	void OnMouseDrag() {		    
		float curPos = getCurrentMousePosition ();
		dragForce.addItem (curPos);
		dragDirection.addItem (curPos);

		float force =  dragForce.getForce();
		rotate (getRotationForce(force,MAX_DRAG_FORCE,0));

		snapToRotation ();
		clickPosition = getCurrentMousePosition();
	}

	private void rotate(float force){
		lastRotation = transformToRotate.eulerAngles.y;
		if (horizontal) {
			if(transformToRotate != transform && !insideTransformToRotate){
				transform.Rotate (Vector3.up * force * speed);
			}
			transformToRotate.Rotate (Vector3.up * force * speed);
		} 
		else {
			if(transformToRotate != transform && !insideTransformToRotate){
				transform.Rotate (-Vector3.forward * force * speed);
			}
			transformToRotate.Rotate (-Vector3.forward * force * speed);
		}
		clickPosition = getCurrentMousePosition();
	}

	private void setRotation(float rotation){
		lastRotation = transformToRotate.eulerAngles.y;
		Vector3 r;
		if (horizontal) {
			if(transformToRotate != transform && !insideTransformToRotate){
				r = transform.eulerAngles;
				r.y = rotation;
				transform.eulerAngles = r;
			}
			r = transformToRotate.eulerAngles;
			r.y = rotation;
			transformToRotate.eulerAngles = r;
		} 
		else {
			if(transformToRotate != transform && !insideTransformToRotate){
				r = transform.eulerAngles;
				r.x = rotation;
				transform.eulerAngles = r;
			}
			r = transformToRotate.eulerAngles;
			r.x = rotation;
			transformToRotate.eulerAngles = r;
		}
	}

	private float getRotationForce(float force,float maxForce,float minForce){
		if (Mathf.Abs (force) > maxForce) {
			force = maxForce * Mathf.Sign (force);
		} else if (Mathf.Abs (force) < minForce) {
			force = minForce * Mathf.Sign (force);
		}

		return force;
	}

	private float getCurrentMousePosition(){
		return (horizontal ? Input.mousePosition.x : Input.mousePosition.y);
	}

	private bool snapToRotation(){
		float dir = dragDirection.getDirection ();
		if (dir == 0) {
			return false;
		}
		dir = Mathf.Sign(dir)*-1;
		return snapToRotationWithDir(dir);
	}

	private bool snapToRotationWithDir(float dir){
		float previousToAngle =getSnapRotationFromDirection (dir,lastRotation);
		float toAngle =getSnapRotationFromDirection (dir,transformToRotate.eulerAngles.y);

		bool over = (toAngle!=previousToAngle);

		if(Mathf.Abs(transformToRotate.eulerAngles.y-toAngle)<SNAP_DISTANCE||over){
			Vector3 rot= transformToRotate.eulerAngles;
			rot.y=over?previousToAngle:toAngle;
			if(transformToRotate != transform && !insideTransformToRotate ){
				transform.eulerAngles=rot;;
			}
			transformToRotate.eulerAngles=rot;
			dragForce.clear ();
			return true;
		}
		return false;
	}



	private float getSnapRotationFromDirection(float dir,float rotation){
		float toAngle = Mathf.Floor ((rotation-0.1f)/ 90f);
		if (dir > 0) {
			toAngle=Mathf.Ceil ((rotation+0.1f)/ 90f);
		}
		if (toAngle < 0) {
			toAngle = 3;
		} else if (toAngle > 4) {
			toAngle=0;
		}
		toAngle *= 90f;

		return toAngle;
	}

	private float getClosestSnapDirection(){
		float previous = Mathf.Abs (transformToRotate.rotation.eulerAngles.y-getSnapRotationFromDirection (-1,transformToRotate.eulerAngles.y));
		float next = Mathf.Abs (transformToRotate.rotation.eulerAngles.y-getSnapRotationFromDirection (1,transformToRotate.eulerAngles.y));
		return previous<next?-1:1;
	}

	private float getClosestSnapRotation(){
		float rotPrev = getSnapRotationFromDirection (-1,transformToRotate.eulerAngles.y);
		float rotNext = getSnapRotationFromDirection (1, transformToRotate.eulerAngles.y);
		float previous = Mathf.Abs (transformToRotate.rotation.eulerAngles.y-rotPrev);
		float next = Mathf.Abs (transformToRotate.rotation.eulerAngles.y-rotNext);
		return previous<next?rotPrev:rotNext;
	}

	public static float elasticEaseInOut( float t, float b, float c, float d )
	{
		t /= d;
		float ts=t*t;
		float tc=ts*t;
		return b+c*(22.645f*tc*ts + -50.09f*ts*ts + 36.495f*tc + -11.8f*ts + 3.75f*t);
	}

	public IEnumerator slowRotationDown(float fromAngle,float toAngle) {
		animationTime = 0;
		float snap = (transformToRotate.rotation.eulerAngles.y) / 90f;
		
		if (snap - Mathf.Floor (snap) > 0.001f) {
			float range = toAngle-fromAngle;

			while (true) {
				animationTime += Time.deltaTime;

				setRotation (elasticEaseInOut(animationTime,0,range, ANIMATION_DURATION )+fromAngle);

				if(animationTime >= ANIMATION_DURATION){
					setRotation(toAngle);
					break;
				}
				else {
					yield return 0;
				}
			}
		} else {
			// Do nothing. Already snapped.
		}
	}
}
