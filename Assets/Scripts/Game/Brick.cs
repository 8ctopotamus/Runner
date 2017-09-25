using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour {

	public GameObject coinPrefab;
	public bool hasCoin;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	void OnKill () {
		Player player = GameObject.Find ("Player").GetComponent<Player> ();

		if (hasCoin) {
			GameObject coinObject = GameObject.Instantiate (coinPrefab);
			coinObject.transform.position = transform.position + new Vector3 (0, 0.8f, 0);

			Coin coin = coinObject.GetComponent<Coin> ();
			coin.Vanish();

			player.onCollectCoin ();
		}

		player.OnDestroyBrick ();
	}
}
