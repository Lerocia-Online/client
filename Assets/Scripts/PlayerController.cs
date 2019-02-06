using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	private Client client;
	public int id;
	
	// Use this for initialization
	void Start () {
		client = GameObject.Find("Client").GetComponent<Client>();
	}

	private void Update() {
		if (client.ourClientId == id) {
			GameObject.Find("MyCanvas").transform.Find("HealthBar").GetComponent<Slider>().value = client.players[id].currentHealth;
		} else {
			transform.Find("PlayerCanvas").GetComponentInChildren<Slider>().value = client.players[id].currentHealth;
		}
	}

	public void TakeDamage(int damage) {
		damage = damage - client.players[id].armor;
		if (damage <= 0) {
			damage = 0;
		}
		client.players[id].currentHealth -= damage;
		if (client.players[id].currentHealth <= 0) {
			KillPlayer();
		}
	}

	private void KillPlayer() {
		// Reset players health
		client.players[id].currentHealth = client.players[id].maxHealth;
		// Move them back to "spawn" point
		client.players[id].avatar.transform.position = new Vector3(0, 1, 0);
	}
}
