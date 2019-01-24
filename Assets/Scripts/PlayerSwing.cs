using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwing : MonoBehaviour {
	public bool attacking;
	private Animator anim;

	void Start() {
		attacking = false;
		anim = GetComponent<Animator>();
	}

	void Update() {
		if (!anim.GetCurrentAnimatorStateInfo(0).IsName("New Animation")) {
			attacking = false;
		}
	}

	public void Attack() {
		attacking = true;
		anim.Play("New Animation");
	}
}