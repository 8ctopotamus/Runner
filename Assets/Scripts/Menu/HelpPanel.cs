using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpPanel : MonoBehaviour {

	void Start () {
		gameObject.SetActive(false);
	}

	public void OnConfirm () {
		gameObject.SetActive (false);
	}

}
