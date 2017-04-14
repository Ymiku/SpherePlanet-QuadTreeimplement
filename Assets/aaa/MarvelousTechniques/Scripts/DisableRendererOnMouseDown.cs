using UnityEngine;
using System.Collections;

public class DisableRendererOnMouseDown : MonoBehaviour {

	void OnMouseDown() {
		GetComponent<Renderer> ().enabled = !GetComponent<Renderer> ().enabled;
	}
}
