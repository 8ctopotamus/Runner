﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {
	public GameObject model;
	public GameObject effectPrefab;
	public float rotatingSpeed = 200f;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		model.transform.RotateAround(model.transform.position, Vector3.up, rotatingSpeed * Time.deltaTime);
		model.transform.RotateAround(model.transform.position, Vector3.right, rotatingSpeed * Time.deltaTime);
		model.transform.RotateAround(model.transform.position, Vector3.forward, rotatingSpeed * Time.deltaTime);
	}

	public void Collect () {
		GameObject effectObject = Instantiate (effectPrefab);
		effectObject.transform.SetParent (transform.parent);
		effectObject.transform.position = transform.position;
		Destroy (gameObject);
	}
}
