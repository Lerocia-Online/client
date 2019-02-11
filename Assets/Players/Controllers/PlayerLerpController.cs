namespace Players.Controllers {
  using UnityEngine;

  public class PlayerLerpController : MonoBehaviour {
    public Player Player;

    private void FixedUpdate() {
      NetworkLerp();
    }

    private void NetworkLerp() {
      if (Player.IsLerpingPosition) {
        float lerpPercentage = (Time.time - Player.TimeStartedLerping) / Player.TimeToLerp;

        Player.Avatar.transform.position =
          Vector3.Lerp(Player.LastRealPosition, Player.RealPosition, lerpPercentage);
      }

      if (Player.IsLerpingRotation) {
        float lerpPercentage = (Time.time - Player.TimeStartedLerping) / Player.TimeToLerp;

        Player.Avatar.transform.rotation =
          Quaternion.Lerp(Player.LastRealRotation, Player.RealRotation, lerpPercentage);
      }
    }
  }
}