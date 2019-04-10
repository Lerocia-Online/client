namespace Characters.NPCs {
  using UnityEngine;
  using System.Collections;
  using Networking.Constants;
  using Lerocia.Characters.Players;

  public enum CharacterPersonality {
    friendly,
    passive,
    enemy
  }

  public class NPCCreator : MonoBehaviour {
    public string character_name;
    public CharacterPersonality character_personality;
    public int max_health;
    public int current_health;
    public int max_stamina;
    public int current_stamina;
    public int gold;
    public int weapon_id;
    public int apparel_id;
    public int dialogue_id;
    public float respawn_time;
    public float look_radius;
    [HideInInspector]
    public string ErrorText;

    private bool _isCreating;

    public void Create() {
      if (!_isCreating) {
        _isCreating = true;
        StartCoroutine("RequestCreateNPC");
      }
    }
    
    private IEnumerator RequestCreateNPC() {
      ErrorText = "Creating NPC...";
      WWWForm form = new WWWForm();
      form.AddField("character_name", character_name);
      form.AddField("character_personality", character_personality.ToString());
      form.AddField("position_x", transform.position.x.ToString());
      form.AddField("position_y", transform.position.y.ToString());
      form.AddField("position_z", transform.position.z.ToString());
      form.AddField("rotation_x", transform.rotation.x.ToString());
      form.AddField("rotation_y", transform.rotation.y.ToString());
      form.AddField("rotation_z", transform.rotation.z.ToString());
      form.AddField("max_health", max_health);
      form.AddField("current_health", current_health);
      form.AddField("max_stamina", max_stamina);
      form.AddField("current_stamina", current_stamina);
      form.AddField("gold", gold);
      form.AddField("weapon_id", weapon_id);
      form.AddField("apparel_id", apparel_id);
      form.AddField("dialogue_id", dialogue_id);
      form.AddField("respawn_time", respawn_time.ToString());
      form.AddField("look_radius", look_radius.ToString());

      WWW w = new WWW(NetworkConstants.Api + EndpointConstants.CreateNPC, form);
      yield return w;

      if (string.IsNullOrEmpty(w.error)) {
        DatabasePlayer databasePlayer = JsonUtility.FromJson<DatabasePlayer>(w.text);
        if (databasePlayer.success) {
          if (databasePlayer.error != "") {
            ErrorText = databasePlayer.error;
          } else {
            ErrorText = "NPC created successfully";
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