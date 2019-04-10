namespace Characters.NPCs {
  using UnityEngine;
  using System.Collections;
  using Networking.Constants;
  using Lerocia.Characters.Players;

  public class NPCDestinationCreator : MonoBehaviour {
    public int character_id;
    public float duration;
    [HideInInspector]
    public string ErrorText;

    private bool _isCreating;

    public void Create() {
      if (!_isCreating) {
        _isCreating = true;
        StartCoroutine("RequestCreateNPCDestination");
      }
    }
    
    private IEnumerator RequestCreateNPCDestination() {
      ErrorText = "Creating NPC...";
      WWWForm form = new WWWForm();
      form.AddField("character_id", character_id);
      form.AddField("position_x", transform.position.x.ToString());
      form.AddField("position_y", transform.position.y.ToString());
      form.AddField("position_z", transform.position.z.ToString());
      form.AddField("duration", duration.ToString());

      WWW w = new WWW(NetworkConstants.Api + EndpointConstants.CreateNPCDestination, form);
      yield return w;

      if (string.IsNullOrEmpty(w.error)) {
        DatabasePlayer databasePlayer = JsonUtility.FromJson<DatabasePlayer>(w.text);
        if (databasePlayer.success) {
          if (databasePlayer.error != "") {
            ErrorText = databasePlayer.error;
          } else {
            ErrorText = "Destination created successfully";
          }
        } else {
          ErrorText = databasePlayer.error;
        }
      } else {
        ErrorText = w.error;
      }

      _isCreating = false;
    }
  }
}