namespace Characters.Players {
	using UnityEngine;
	using Menus;
	using Controllers;
	using Menus.Controllers;
	using Networking;
	using Animation;
	using Characters.Controllers;

	public class PlayerFactory : MonoBehaviour {
		public GameObject MyPlayerPrefab;
		public GameObject PlayerPrefab;

		public void Spawn(string playerName, int connectionId, float x, float y, float z) {
			if (ConnectedCharacters.MyUser.connection_id == connectionId) {
				SpawnMyPlayer(playerName, connectionId, x, y, z);
			} else {
				SpawnPlayer(playerName, connectionId, x, y, z);
			}
		}

		private void SpawnMyPlayer(string playerName, int connectionId, float x, float y, float z) {
			// Create my player object
			GameObject playerObject = Instantiate(MyPlayerPrefab);
			playerObject.name = playerName;
			playerObject.transform.position = new Vector3(x, y, z);
			// Add MyPlayer specific components
			playerObject.AddComponent<PlayerController>();
			playerObject.transform.Find("FirstPersonCharacter").gameObject.AddComponent<PlayerCameraController>();
			// Add universal player components
			playerObject.AddComponent<PlayerReference>();
			playerObject.GetComponent<PlayerReference>().ConnectionId = connectionId;
			playerObject.AddComponent<CharacterAnimator>();
			// Create new player
			ConnectedCharacters.MyPlayer = new Player(playerName, playerObject, "friendly", 100, 100, 5, 0);
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

		private void SpawnPlayer(string playerName, int connectionId, float x, float y, float z) {
			// Create player object
			GameObject playerObject = Instantiate(PlayerPrefab);
			playerObject.name = playerName;
			playerObject.transform.position = new Vector3(x, y, z);
			// Add non-MyPlayer specific components
			playerObject.AddComponent<CharacterLerpController>();
			// Add universal player components
			playerObject.AddComponent<PlayerReference>();
			playerObject.GetComponent<PlayerReference>().ConnectionId = connectionId;
			playerObject.AddComponent<CharacterAnimator>();
			// Create new player
			Player player = new Player(playerName, playerObject, "friendly", 100, 100, 5, 0);
			// Add player to players dictionary
			ConnectedCharacters.Characters.Add(player);
			ConnectedCharacters.Players.Add(connectionId, player);
			// Set player references
			ConnectedCharacters.Players[connectionId].Avatar.GetComponent<CharacterLerpController>().Character = player;
		}
	}
}