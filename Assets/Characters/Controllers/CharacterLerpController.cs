namespace Characters.Controllers {
  using UnityEngine;
  using Lerocia.Characters;
  using Animation;

  public class CharacterLerpController : MonoBehaviour {
    public Character Character;

    private void FixedUpdate() {
      NetworkLerp();
    }

    private void NetworkLerp() {
      if (Character.IsLerpingPosition) {
        float lerpPercentage = (Time.time - Character.TimeStartedLerping) / Character.TimeToLerp;

        Character.Avatar.transform.position =
          Vector3.Lerp(Character.LastRealPosition, Character.RealPosition, lerpPercentage);
        Character.Avatar.GetComponent<CharacterAnimator>().Running(true);
      } else {
        Character.Avatar.GetComponent<CharacterAnimator>().Running(false);
      }

      if (Character.IsLerpingRotation) {
        float lerpPercentage = (Time.time - Character.TimeStartedLerping) / Character.TimeToLerp;

        Character.Avatar.transform.rotation =
          Quaternion.Lerp(Character.LastRealRotation, Character.RealRotation, lerpPercentage);
      }
    }
  }
}