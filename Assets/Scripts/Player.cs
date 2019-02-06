using System.Collections.Generic;
using UnityEngine;

public class Player {
	
	// Identifiers
	public string playerName;
	public GameObject avatar;
	public int connectionId;

	// Movement interpolation
	public bool isLerpingPosition;
	public bool isLerpingRotation;
	public Vector3 realPosition;
	public Quaternion realRotation;
	public Vector3 lastRealPosition;
	public Quaternion lastRealRotation;
	public float timeStartedLerping;
	public float timeToLerp;

	// Stats
	public int maxHealth;
	public int currentHealth;
	public int maxStamina;
	public int currentStamina;
	public int gold;
	public int weight;
	public int armor;
	public int damage;

	// Equipped armor & weapons
	public int weapon;
	public int apparel;

	public List<int> inventory;
	public List<Item> items;

	public Player(string playerName, GameObject avatar, int connectionId, List<Item> items) {
		this.playerName = playerName;
		this.avatar = avatar;
		this.connectionId = connectionId;
		maxHealth = 100;
		currentHealth = maxHealth;
		maxStamina = 100;
		currentStamina = maxStamina;
		gold = 0;
		weight = 0;
		armor = 0;
		damage = 0;
		weapon = -1;
		apparel = -1;
		inventory = new List<int>();
		this.items = items;
	}

	public void UpdateStats() {
		if (this.weapon >= 0) {
			Weapon weapon = items[this.weapon] as Weapon;
			damage = weapon.getDamage();
		} else {
			damage = 0;
		}

		if (this.apparel >= 0) {
			Apparel apparel = items[this.apparel] as Apparel;
			armor = apparel.getArmor();
		} else {
			armor = 0;
		}
	}
}