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

		public void Spawn(
			int characterId, 
			string characterName, 
			string characterPersonality,
			float px, float py, float pz, 
			float rx, float ry, float rz,  
			int maxHealth, 
			int currentHealth, 
			int maxStamina, 
			int currentStamina, 
			int gold,
			int baseWeight,
			int baseDamage,
			int baseArmor,
			int weaponId, 
			int apparelId,
			int dialogueId
		) {
			if (ConnectedCharacters.MyDatabasePlayer.character_id == characterId) {
				SpawnMyPlayer(
					characterId, 
					characterName, 
					characterPersonality,
					px, py, pz, 
					rx, ry, rz,  
					maxHealth, 
					currentHealth, 
					maxStamina, 
					currentStamina, 
					gold,
					baseWeight,
					baseDamage,
					baseArmor,
					weaponId, 
					apparelId,
					dialogueId
				);
			} else {
				SpawnPlayer(
					characterId, 
					characterName, 
					characterPersonality,
					px, py, pz, 
					rx, ry, rz,  
					maxHealth, 
					currentHealth, 
					maxStamina, 
					currentStamina, 
					gold,
					baseWeight,
					baseDamage,
					baseArmor,
					weaponId, 
					apparelId,
					dialogueId
				);
			}
		}

		private void SpawnMyPlayer(
			int characterId, 
			string characterName, 
			string characterPersonality,
			float px, float py, float pz, 
			float rx, float ry, float rz,  
			int maxHealth, 
			int currentHealth, 
			int maxStamina, 
			int currentStamina, 
			int gold,
			int baseWeight,
			int baseDamage,
			int baseArmor,
			int weaponId, 
			int apparelId,
			int dialogueId
		) {
			// Create my player object
			GameObject playerObject = Instantiate(MyPlayerPrefab);
			playerObject.name = characterName;
			playerObject.transform.position = new Vector3(px, py, pz);
			playerObject.transform.rotation = Quaternion.Euler(new Vector3(rx, ry, rz));
			// Add MyPlayer specific components
			playerObject.AddComponent<PlayerController>();
			playerObject.transform.Find("FirstPersonCharacter").gameObject.AddComponent<PlayerCameraController>();
			// Add universal player components
			playerObject.AddComponent<CharacterReference>();
			playerObject.GetComponent<CharacterReference>().CharacterId = characterId;
			playerObject.AddComponent<CharacterAnimator>();
			// Create new player
			ConnectedCharacters.MyPlayer = new ClientPlayer(
				characterId, 
				characterName, 
				characterPersonality, 
				playerObject, 
				maxHealth, 
				currentHealth, 
				maxStamina, 
				currentStamina, 
				gold, 
				baseWeight,
				baseDamage,
				baseArmor,
				weaponId, 
				apparelId,
				dialogueId
			);
			// Add my player to players dictionary
			ConnectedCharacters.Characters.Add(characterId, ConnectedCharacters.MyPlayer);
			ConnectedCharacters.Players.Add(characterId, ConnectedCharacters.MyPlayer);
			//Disable login menu
			CanvasSettings.LoginMenu.SetActive(false);
			// Activate player HUD
			CanvasSettings.PlayerHud.GetComponent<PlayerHUDController>().Player = ConnectedCharacters.MyPlayer;
			CanvasSettings.PlayerHud.SetActive(true);
			// We are now safe to start
			NetworkSettings.IsStarted = true;
		}

		private void SpawnPlayer(
			int characterId, 
			string characterName, 
			string characterPersonality,
			float px, float py, float pz, 
			float rx, float ry, float rz,  
			int maxHealth, 
			int currentHealth, 
			int maxStamina, 
			int currentStamina, 
			int gold,
			int baseWeight,
			int baseDamage,
			int baseArmor,
			int weaponId, 
			int apparelId,
			int dialogueId
		) {
			// Create player object
			GameObject playerObject = Instantiate(PlayerPrefab);
			playerObject.name = characterName;
			playerObject.transform.position = new Vector3(px, py, pz);
			playerObject.transform.rotation = Quaternion.Euler(new Vector3(rx, ry, rz));
			// Add non-MyPlayer specific components
			playerObject.AddComponent<CharacterLerpController>();
			// Add universal player components
			playerObject.AddComponent<CharacterReference>();
			playerObject.GetComponent<CharacterReference>().CharacterId = characterId;
			playerObject.AddComponent<CharacterAnimator>();
			// Create new player
			ClientPlayer player = new ClientPlayer(
				characterId, 
				characterName, 
				characterPersonality, 
				playerObject, 
				maxHealth, 
				currentHealth, 
				maxStamina, 
				currentStamina, 
				gold, 
				baseWeight,
				baseDamage,
				baseArmor,
				weaponId, 
				apparelId,
				dialogueId
			);
			// Add player to players dictionary
			ConnectedCharacters.Characters.Add(characterId, player);
			ConnectedCharacters.Players.Add(characterId, player);
			// Set player references
			ConnectedCharacters.Players[characterId].Avatar.GetComponent<CharacterLerpController>().Character = player;
		}
	}
}