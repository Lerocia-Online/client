namespace Items {
  using UnityEngine;
  using System.Collections;
  using Networking.Constants;
  using Lerocia.Items;
  using Random = System.Random;

  public class ItemCreator : MonoBehaviour {
    [HideInInspector] public string ErrorText;

    private bool _isCreating;

    public void Create() {
      if (!_isCreating) {
        _isCreating = true;
        StartCoroutine("RequestCreateItem");
      }
    }

    private IEnumerator RequestCreateItem() {
      ErrorText = "Creating item...";
      Random random = new Random();
      int worldId = random.Next();
      WWWForm form = new WWWForm();
      form.AddField("world_id", worldId);
      form.AddField("item_id", GetComponent<ItemReference>().ItemId);
      form.AddField("position_x", transform.position.x.ToString());
      form.AddField("position_y", transform.position.y.ToString());
      form.AddField("position_z", transform.position.z.ToString());

      WWW w = new WWW(NetworkConstants.Api + EndpointConstants.CreateItem, form);
      yield return w;

      if (string.IsNullOrEmpty(w.error)) {
        ErrorText = "Item created successfully";
      } else {
        ErrorText = w.error;
      }

      _isCreating = false;
    }
  }
}