using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

	public GameObject helpPanel;

	public void OnPressPlay () {
		SceneManager.LoadScene ("Level1-1");
	}

	public void OnPressHelp () {
		helpPanel.SetActive (true);
	}

}
