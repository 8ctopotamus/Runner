using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager {

	private static LevelManager instance;
	public static LevelManager Instance {
		get {
			if (instance == null) {
				instance = new LevelManager ();
			}
			return instance;
		}
	}

	// define constants.
	private const int LEVELS_PER_WORLD = 4;
	private const int MAXIMUM_WORLD = 1;
	private const int MAXIMUM_LEVEL = 1;

	// variables to keep track of current level.
	private int world = 1;
	private int level = 1;
	// properties to read world and level information.
	public int World { get { return world; } }
	public int Level { get { return world; } }
	public string LevelName { get { return "Level" + world + "-" + level; } }

	public void LoadLevel (int world, int level) {
		this.world = world;
		this.level = level;

		if (world > MAXIMUM_WORLD || (world == MAXIMUM_WORLD && level > MAXIMUM_LEVEL)) {
			SceneManager.LoadScene("Menu");
		} else {
			SceneManager.LoadScene("Level" + world + "-" + level);
		}
	}

	public void LoadNextLevel () {
		level++;
		if (this.level > LEVELS_PER_WORLD) {
			this.level = 1;
			this.world++;
		}

		LoadLevel(this.world, this.level);
	}

	public void ReloadLevel () {
		SceneManager.LoadScene (SceneManager.GetActiveScene().name);
	}

}
