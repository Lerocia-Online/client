using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwing : MonoBehaviour {
	public bool attacking;
	public bool charging;
	private Animator anim;

	void Start() {
		attacking = false;
		charging = false;
		anim = GetComponent<Animator>();
	}

	void Update() {
		if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !anim.IsInTransition(0)) {
			attacking = false;
		}
	}

	public void Attack() {
		charging = false;
		attacking = true;
		anim.SetTrigger("Attack");
	}

	public void Charge() {
		charging = true;
		anim.Play("Charge");
	}
}