using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour {
	private Client client;
	private PlayerSwing playerSwing;
	private PlayerAttack playerAttack;

	void Start() {
		client = GameObject.Find("Client").GetComponent<Client>();
		playerSwing = gameObject.GetComponent<PlayerSwing>();
		playerAttack = gameObject.GetComponent<PlayerAttack>();
	}

	void Update() {
		if (!playerSwing.attacking) {
			if (Input.GetButtonDown("Fire1")) {
				client.SendReliable("ATK|");
				playerSwing.Attack();
				playerAttack.Attack();
			}
		}
	}
} 