using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour {
	[Header("Visuals")]
	public GameObject model;
	public GameObject normalModel;
	public GameObject powerUpModel;

	[Header("Acceleration")]
	public float acceleration = 2.5f;
	public float deacceleration = 5.0f;

	[Header("Movement Variables")]
	[Range (4f, 6f)]
	public float movementSpeed = 4f;
	public float movementSpeedRight = 8f;
	public float movementSpeedLeft = 2f;

	[Header("Jumping Fields")]
	public float normalJumpingSpeed = 6f;
	public float longJumpingSpeed = 10f;
	public float destroyEnemyJumpingSpeed = 9f;
	public float jumpDuration = 0.75f;
	public float verticalWallJumpingSpeed = 5f;
	public float horizontalWallJumpingSpeed = 3.5f;

	[Header("Power ups")]
	public float InvincibilityDuration = 5f;

	public Action onCollectCoin;

	public float jumpingTimer = 0f;
	private float speed = 0f;
	private float jumpingSpeed = 0f;

	private bool dead = false;
	private bool paused = false;
	private bool canJump = false;
	private bool jumping = false;
	private bool canWallJump = false;
	private bool WallJumpLeft = false;
	private bool onSpeedAreaLeft = false;
	private bool onSpeedAreaRight = false;
	private bool onLongJumpBlock = false;
	private bool finished = false;

	private bool hasPowerUp = false;
  private bool hasInvincibility = false;

	public bool Dead {
		get {
			return dead;
		}
	}

	public bool Finished {
		get {
			return finished;
		}
	}

	void Start () {
		jumpingSpeed = normalJumpingSpeed;

		normalModel.SetActive (true);
		powerUpModel.SetActive (false);
	}

	void Update () {
		if (dead) {
			return;
		}

		// Accelerate the player
		speed += acceleration * Time.deltaTime;
		float targetMovementSpeed = movementSpeed;

		if (onSpeedAreaLeft) {
			targetMovementSpeed = movementSpeedLeft;
		} else if (onSpeedAreaRight) {
			targetMovementSpeed = movementSpeedRight;
		}

		if (speed > targetMovementSpeed) {
			speed -= deacceleration * Time.deltaTime;
		}

		// move horizontally
		GetComponent<Rigidbody>().velocity = new Vector3(
			paused ? 0 : speed,
			GetComponent<Rigidbody>().velocity.y,
			GetComponent<Rigidbody>().velocity.z
		);

		// Check for input
		bool pressingJumpButton = Input.GetMouseButton(0) || Input.GetKey("space");
		if (pressingJumpButton) {
			if (canJump) {
				Jump ();
			}
		}

		// check for unpause
		if (paused && pressingJumpButton) {
			paused = false;
		}

		// make the player jump
		if (jumping) {
			jumpingTimer += Time.deltaTime;

			if (pressingJumpButton && jumpingTimer < jumpDuration) {
				if (onLongJumpBlock) {
					jumpingSpeed = longJumpingSpeed;
				}

				GetComponent<Rigidbody>().velocity = new Vector3(
					GetComponent<Rigidbody>().velocity.x,
					jumpingSpeed,
					GetComponent<Rigidbody>().velocity.z
				);
			}
		}

		// make the player wall jump
		if (canWallJump) {
			speed = 0;

			if (pressingJumpButton) {
				canWallJump = false;

				speed = WallJumpLeft ? -horizontalWallJumpingSpeed : horizontalWallJumpingSpeed;

				GetComponent<Rigidbody>().velocity = new Vector3(
					GetComponent<Rigidbody>().velocity.x,
					verticalWallJumpingSpeed,
					GetComponent<Rigidbody>().velocity.z
				);
			}
		}
	}

	public void Pause () {
		paused = true;
	}

	void OnTriggerEnter(Collider otherCollider) {
		// Collect coins
		if (otherCollider.transform.GetComponent<Coin>() != null) {
			Destroy(otherCollider.gameObject);
			onCollectCoin ();
		}

		// speed up or down
		if (otherCollider.GetComponent<SpeedArea> () != null) {
			SpeedArea speedArea = otherCollider.GetComponent<SpeedArea> ();
			if (speedArea.direction == Direction.Left) {
				onSpeedAreaLeft = true;
			} else if (speedArea.direction == Direction.Right) {
				onSpeedAreaRight = true;
			}
		}

		// long jump
		if (otherCollider.GetComponent<LongJumpBlock> () != null) {
			onLongJumpBlock = true;
		}

		// kill player when touching enemy
		if (otherCollider.GetComponent<Enemy> () != null) {
			Enemy enemy = otherCollider.GetComponent<Enemy> ();

			if (hasInvincibility == false && enemy.Dead == false) {
				if (hasPowerUp == false) {
					Kill();
				} else {
					hasPowerUp = false;
					normalModel.SetActive (true);
					powerUpModel.SetActive (false);
					ApplyInvincibility ();
				}
			}
		}

		// collect the powerup
		if (otherCollider.GetComponent<PowerUp> () != null) {
			PowerUp powerup = otherCollider.GetComponent<PowerUp> ();
			powerup.Collect ();
			ApplyPowerUp ();
		}

		// reach the finish line
		if (otherCollider.GetComponent<FinishLine> () != null) {
			hasInvincibility = true;
			finished = true;
		}
	}

	void OnTriggerStay(Collider otherCollider) {
		if (otherCollider.tag == "JumpingArea" ) {
			canJump = true;
			jumping = false;
			jumpingSpeed = normalJumpingSpeed;
			jumpingTimer = 0f;
		} else if (otherCollider.tag == "WallJumpingArea") {
			canWallJump = true;
			WallJumpLeft = transform.position.x < otherCollider.transform.position.x;
		}
	}

	void OnTriggerExit(Collider otherCollider) {
		if (otherCollider.tag =="WallJumpingArea") {
			canWallJump = false;
		}

		if (otherCollider.GetComponent<SpeedArea> () != null) {
			SpeedArea speedArea = otherCollider.GetComponent<SpeedArea> ();
			if (speedArea.direction == Direction.Left) {
				onSpeedAreaLeft = false;
			} else if (speedArea.direction == Direction.Right) {
				onSpeedAreaRight = false;
			}
		}

		if (otherCollider.GetComponent<LongJumpBlock> () != null) {
			onLongJumpBlock = false;
		}
	}

	void Kill () {
		dead = true;
		GetComponent<Collider> ().enabled = false;
		GetComponent<Rigidbody> ().velocity = Vector3.zero;
		GetComponent<Rigidbody> ().AddForce (new Vector3(0f, 500f, -400f));
	}

	public void Jump (bool hitEnemy = false) {
		jumping = true;

		if (hitEnemy) {
			GetComponent<Rigidbody>().velocity = new Vector3(
				GetComponent<Rigidbody>().velocity.x,
				destroyEnemyJumpingSpeed,
				GetComponent<Rigidbody>().velocity.z
			);
		}
	}

	public void ApplyPowerUp () {
		hasPowerUp = true;
		normalModel.SetActive (false);
		powerUpModel.SetActive (true);
	}

	public void ApplyInvincibility () {
		hasInvincibility = true;
		StartCoroutine (InvincibilityRoutine());
	}

	private IEnumerator InvincibilityRoutine () {
		// slow blinks
		float initialWaitingTime = InvincibilityDuration * 0.75f;
		int initialBlinks = 20;

		for ( int i = 0; i < initialBlinks; i++) {
			model.SetActive (!model.activeSelf);
			yield return new WaitForSeconds (initialWaitingTime / initialBlinks);
		}

		// fast blinks
		float finalWaitingTime = InvincibilityDuration * 0.25f;
		int finalBlinks = 30;

		for (int i = 0; i < finalBlinks; i++) {
			model.SetActive (!model.activeSelf);
			yield return new WaitForSeconds (finalWaitingTime / finalBlinks);
		}

		model.SetActive (true);
		hasInvincibility = false;
	}

	public void OnDestroyBrick () {
		GetComponent<Rigidbody> ().velocity = new Vector3 (
			GetComponent<Rigidbody> ().velocity.x,
			0,
			GetComponent<Rigidbody> ().velocity.z
		);
		canJump = false;
		jumping = false;
	}
}
