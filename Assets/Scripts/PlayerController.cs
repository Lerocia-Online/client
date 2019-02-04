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

	public void TakeDamage(int damage) {
		client.players[id].currentHealth -= damage;
		UpdateHealthBar();
		if (client.players[id].currentHealth <= 0) {
			KillPlayer();
		}
	}

	private void KillPlayer() {
		// Reset players health
		client.players[id].currentHealth = client.players[id].maxHealth;
		UpdateHealthBar();
		// Move them back to "spawn" point
		client.players[id].avatar.transform.position = new Vector3(0, 1, 0);
	}

	private void UpdateHealthBar() {
		if (client.ourClientId == id) {
			GameObject.Find("MyCanvas").transform.Find("HealthBar").GetComponent<Slider>().value = client.players[id].currentHealth;
		} else {
			transform.Find("PlayerCanvas").GetComponentInChildren<Slider>().value = client.players[id].currentHealth;
		}
	}
}
