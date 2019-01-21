using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour {
	private Client client;
	private PlayerSwing playerSwing;

	void Start() {
		client = GameObject.Find("Client").GetComponent<Client>();
		playerSwing = gameObject.GetComponent<PlayerSwing>();
	}

	void Update() {
		if (!playerSwing.attacking) {
			if (Input.GetButton("Fire1")) {
				client.SendReliable("ATK|");
				playerSwing.Attack();
			}
		}
	}
} 