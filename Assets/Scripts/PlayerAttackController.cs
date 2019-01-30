using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour {
	private Client client;
	private PlayerSwing playerSwing;
	private PlayerAttack playerAttack;
	private float chargeStartTime;
	private float chargeEndTime;

	void Start() {
		client = GameObject.Find("Client").GetComponent<Client>();
		playerSwing = gameObject.GetComponent<PlayerSwing>();
		playerAttack = gameObject.GetComponent<PlayerAttack>();
	}

	void Update() {
		if (!playerSwing.attacking) {
			if (Input.GetButtonDown("Fire1")) {
				client.SendReliable("CHARGE|");
				chargeStartTime = Time.time;
				playerSwing.Charge();
			}
		}

		if (playerSwing.charging) {
			if (Input.GetButtonUp("Fire1")) {
				client.SendReliable("ATK|");
				chargeEndTime = Time.time;
				playerSwing.Attack();
				playerAttack.Attack(chargeEndTime - chargeStartTime);
			}
		}
	}
} 