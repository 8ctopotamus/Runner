﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour {

	public float rotatingSpeed = 180f;
	public float movementSpeed = 10f;
	public float radiusTolerance = 0.4f;

	private bool movingRight = true;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		// make the shell spin
		transform.RotateAround(transform.position, Vector3.up, rotatingSpeed * Time.deltaTime);

		// make shell move on X
		transform.position = new Vector3 (
			transform.position.x + movementSpeed * (movingRight ? 1 : -1) * Time.deltaTime,
			transform.position.y,
			transform.position.z
		);
	}

	void OnTriggerEnter (Collider otherCollider) {
		if (otherCollider.GetComponent<Enemy> () != null) {
			Destroy (otherCollider.gameObject);
		}
		else if (otherCollider.GetComponent<Destroyer> () || otherCollider.tag == "JumpingArea") {
			return;
		} else if (transform.position.y < otherCollider.transform.position.y + radiusTolerance) {
			// bounce
			if (transform.position.x < otherCollider.transform.position.x && movingRight == true) {
				movingRight = false;
			} else if (transform.position.x > otherCollider.transform.position.x && movingRight == false) {
				movingRight = true;
			}
		}
	}

	void OnCollisionEnter (Collision collision) {
		if (collision.gameObject.GetComponent<Player> ()) {
			return;
		}

		if (transform.position.y < collision.transform.position.y + radiusTolerance) {
			// bounce
			if (transform.position.x < collision.transform.position.x && movingRight == true) {
				movingRight = false;
			} else if (transform.position.x > collision.transform.position.x && movingRight == false) {
				movingRight = true;
			}
		}
	}

}
