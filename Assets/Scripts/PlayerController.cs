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
		if (client.ourClientId == id) {
			GameObject.Find("MyCanvas(Clone)").transform.Find("HealthBar").GetComponent<Slider>().value = client.players[id].currentHealth;
		} else {
			transform.Find("PlayerCanvas").GetComponentInChildren<Slider>().value = client.players[id].currentHealth;
		}
	}
}
