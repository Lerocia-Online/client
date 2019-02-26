namespace Characters.Players {
	using UnityEngine;
	using Menus;
	using Controllers;
	using Menus.Controllers;
	using Networking;
	using Animation;
	using Characters.Controllers;
	using Lerocia.Characters;

	public class PlayerFactory : MonoBehaviour {
		public GameObject MyPlayerPrefab;
		public GameObject PlayerPrefab;

		public void Spawn(string playerName, int connectionId, float px, float py, float pz, float rx, float ry, float rz, string type, int equippedWeapon, int equippedApparel, int maxHealth, int currentHealth, int maxStamina, int currentStamina, int gold) {
			if (ConnectedCharacters.MyUser.connection_id == connectionId) {
				SpawnMyPlayer(playerName, connectionId, px, py, pz, rx, ry, rz, type, equippedWeapon, equippedApparel, maxHealth, currentHealth, maxStamina, currentStamina, gold);
			} else {
				SpawnPlayer(playerName, connectionId, px, py, pz, rx, ry, rz, type, equippedWeapon, equippedApparel, maxHealth, currentHealth, maxStamina, currentStamina, gold);
			}
		}

		private void SpawnMyPlayer(string playerName, int connectionId, float px, float py, float pz, float rx, float ry, float rz, string type, int equippedWeapon, int equippedApparel, int maxHealth, int currentHealth, int maxStamina, int currentStamina, int gold) {
			// Create my player object
			GameObject playerObject = Instantiate(MyPlayerPrefab);
			playerObject.name = playerName;
			playerObject.transform.position = new Vector3(px, py, pz);
			playerObject.transform.rotation = Quaternion.Euler(new Vector3(rx, ry, rz));
			// Add MyPlayer specific components
			playerObject.AddComponent<PlayerController>();
			playerObject.transform.Find("FirstPersonCharacter").gameObject.AddComponent<PlayerCameraController>();
			// Add universal player components
			playerObject.AddComponent<PlayerReference>();
			playerObject.GetComponent<PlayerReference>().ConnectionId = connectionId;
			playerObject.AddComponent<CharacterAnimator>();
			// Create new player
			ConnectedCharacters.MyPlayer = new ClientPlayer(playerName, playerObject, type, maxHealth, currentHealth, maxStamina, currentStamina, gold, 5, 0, equippedWeapon, equippedApparel);
			// Add my player to players dictionary
			ConnectedCharacters.Characters.Add(ConnectedCharacters.MyPlayer);
			ConnectedCharacters.Players.Add(connectionId, ConnectedCharacters.MyPlayer);
			//Disable login menu
			CanvasSettings.LoginMenu.SetActive(false);
			// Activate player HUD
			CanvasSettings.PlayerHud.GetComponent<PlayerHUDController>().Player = ConnectedCharacters.MyPlayer;
			CanvasSettings.PlayerHud.SetActive(true);
			// We are now safe to start
			NetworkSettings.IsStarted = true;
		}

		private void SpawnPlayer(string playerName, int connectionId, float px, float py, float pz, float rx, float ry, float rz, string type, int equippedWeapon, int equippedApparel, int maxHealth, int currentHealth, int maxStamina, int currentStamina, int gold) {
			// Create player object
			GameObject playerObject = Instantiate(PlayerPrefab);
			playerObject.name = playerName;
			playerObject.transform.position = new Vector3(px, py, pz);
			playerObject.transform.rotation = Quaternion.Euler(new Vector3(rx, ry, rz));
			// Add non-MyPlayer specific components
			playerObject.AddComponent<CharacterLerpController>();
			// Add universal player components
			playerObject.AddComponent<PlayerReference>();
			playerObject.GetComponent<PlayerReference>().ConnectionId = connectionId;
			playerObject.AddComponent<CharacterAnimator>();
			// Create new player
			ClientPlayer player = new ClientPlayer(playerName, playerObject, type, maxHealth, currentHealth, maxStamina, currentStamina, gold, 5, 0, equippedWeapon, equippedApparel);
			// Add player to players dictionary
			ConnectedCharacters.Characters.Add(player);
			ConnectedCharacters.Players.Add(connectionId, player);
			// Set player references
			ConnectedCharacters.Players[connectionId].Avatar.GetComponent<CharacterLerpController>().Character = player;
		}
	}
}