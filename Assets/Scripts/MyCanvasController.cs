using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyCanvasController : MonoBehaviour {
	public GameObject itemStatPrefab;

	private Client client;
	// Use this for initialization
	void Start () {
		client = GameObject.Find("Client").GetComponent<Client>();
	}

	public void UpdateItemView(Item item) {
		DestroyItemView();
		CreateItemView(item);
	}

	private void CreateItemView(Item item) {
		transform.Find("Item View").transform.Find("Item Name").GetComponent<Text>().text =
			client.items[item.getId()].getName();
		List<GameObject> statList = new List<GameObject>();
		// Create stat object in item view for each stat on this item
		foreach (KeyValuePair<string, string> stat in item.getStats()) {
			GameObject itemStat = Instantiate(itemStatPrefab);
			itemStat.transform.SetParent(transform.Find("Item View").transform.Find("Item Stats").transform, false);
			itemStat.transform.Find("Title").GetComponent<Text>().text = stat.Key;
			itemStat.transform.Find("Value").GetComponent<Text>().text = stat.Value;
			statList.Add(itemStat);
		}

		// Set x position of each stat in the item view based on the number of stats to display
		int counter = 1;
		foreach (GameObject stat in statList) {
			float width = stat.GetComponent<RectTransform>().rect.width;
			float offset = counter - (float)(statList.Count + 1) / 2;
			stat.transform.localPosition = new Vector3(width * offset, 0, 0);
			counter++;
		}
	}
	
	private void DestroyItemView() {
		Transform panel = transform.Find("Item View").transform.Find("Item Stats");
		foreach (Transform child in panel) {
			Destroy(child.gameObject);
		}
	}
}
