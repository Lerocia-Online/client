namespace Items {
  using UnityEngine;
  using Lerocia.Items;

  public class ItemFactory : MonoBehaviour {
    public GameObject ItemPrefab;

    public void Spawn(string[] data) {
      // Spawn all items
      for (int i = 1; i < data.Length; i++) {
        string[] d = data[i].Split('%');
        Spawn(int.Parse(d[0]), int.Parse(d[1]), float.Parse(d[2]), float.Parse(d[3]), float.Parse(d[4]));
      }
    }

    public void Spawn(int worldId, int itemId, float x, float y, float z) {
      GameObject item = Instantiate(ItemPrefab);
      item.transform.position = new Vector3(x, y, z);
      item.GetComponent<ItemReference>().ItemId = itemId;
      item.GetComponent<ItemReference>().WorldId = worldId;
      item.name = ItemList.Items[itemId].GetName();
      ItemList.WorldItems.Add(worldId, item);
    }
  }
}