//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
using UnityEngine;
using System.Collections;

public class Fps : MonoBehaviour {
	public Color textColor = Color.white;
	string label = "";
	float count;
	GUIStyle style = new GUIStyle();
	
	IEnumerator Start ()
	{
		style.normal.textColor = textColor;
		GUI.depth = 2;
		while (true) {
			if (Time.timeScale == 1) {
				yield return new WaitForSeconds (0.1f);
				count = (1 / Time.deltaTime);
				label = "FPS :" + (Mathf.Round (count));
			} else {
				label = "Pause";
			}
			yield return new WaitForSeconds (0.5f);
		}
	}
	
	void OnGUI ()
	{
		GUI.Label (new Rect (5, 10, 100, 25), label, style);
	}
}
