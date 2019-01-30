using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {
  private Client client;
  private int damage = 10;
  private float range = 10f;
  private GameObject arms;

  private void Start() {
    client = GameObject.Find("Client").GetComponent<Client>();
  }

  public void Attack() {
    RaycastHit hit;
    if (Physics.Raycast(gameObject.transform.position, transform.forward, out hit, range)) {
      if (hit.transform.tag == "Player") {
        client.SendReliable("HIT|" + hit.transform.gameObject.GetComponent<PlayerController>().id + "|" + damage);
      }
    }
  }
}
