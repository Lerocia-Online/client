namespace Characters.Players {
	using UnityEngine;
	using Menus;
	using Menus.Controllers;
	using Networking;
	using Characters.Controllers;
	using Lerocia.Characters;

	public class PlayerFactory : MonoBehaviour {
		public GameObject MyPlayerObject;
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
			int dialogueId,
			float ox, float oy, float oz,
			bool isDead
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
					dialogueId,
					ox, oy, oz
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
					dialogueId,
					ox, oy, oz,
					isDead
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
			int dialogueId,
			float ox, float oy, float oz
		) {
			// Create my player object
			GameObject playerObject = MyPlayerObject;
			playerObject.name = characterName;
			playerObject.transform.position = new Vector3(px, py, pz);
			// Add universal player components
			playerObject.GetComponent<CharacterReference>().CharacterId = characterId;
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
				dialogueId,
				new Vector3(ox, oy, oz)
			);
			playerObject.GetComponent<CharacterAvatarController>().UpdateWeapon(ConnectedCharacters.MyPlayer);
			playerObject.GetComponent<CharacterAvatarController>().UpdateApparel(ConnectedCharacters.MyPlayer);
			// Add my player to players dictionary
			ConnectedCharacters.Characters.Add(characterId, ConnectedCharacters.MyPlayer);
			ConnectedCharacters.Players.Add(characterId, ConnectedCharacters.MyPlayer);
			// Activate player HUD
			CanvasSettings.PlayerHud.GetComponent<PlayerHUDController>().Player = ConnectedCharacters.MyPlayer;
			CanvasSettings.PlayerHud.SetActive(true);
			CanvasSettings.MyCharacter = playerObject.transform.Find("Character").gameObject;
			// We are now safe to start
			NetworkSettings.IsStarted = true;
			playerObject.transform.rotation = Quaternion.Euler(new Vector3(rx, ry, rz));
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
			int dialogueId,
			float ox, float oy, float oz,
			bool isDead
		) {
			// Create player object
			GameObject playerObject = Instantiate(PlayerPrefab);
			playerObject.name = characterName;
			playerObject.transform.position = new Vector3(px, py, pz);
			playerObject.transform.rotation = Quaternion.Euler(new Vector3(rx, ry, rz));
			// Add universal player components
			playerObject.GetComponent<CharacterReference>().CharacterId = characterId;
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
				dialogueId,
				new Vector3(ox, oy, oz)
			);
			playerObject.GetComponent<CharacterAvatarController>().UpdateWeapon(player);
			playerObject.GetComponent<CharacterAvatarController>().UpdateApparel(player);
			if (isDead) {
				player.Death();
			}
			// Add player to players dictionary
			ConnectedCharacters.Characters.Add(characterId, player);
			ConnectedCharacters.Players.Add(characterId, player);
			// Set player references
			ConnectedCharacters.Players[characterId].Avatar.GetComponent<CharacterLerpController>().Character = player;
		}
	}
}