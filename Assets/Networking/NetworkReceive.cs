using System.Diagnostics;

namespace Networking {
  using System.Text;
  using UnityEngine;
  using UnityEngine.Networking;
  using Lerocia.Characters;
  using Characters.Players;
  using Characters.NPCs;
  using Characters.Animation;
  using Items;
  using Lerocia.Items;
  using Menus;

  public class NetworkReceive : MonoBehaviour {
    private GameObject _factory;
    private PlayerFactory _playerFactory;
    private ItemFactory _itemFactory;
    private NPCFactory _npcFactory;

    private void Awake() {
      _factory = GameObject.Find("Factory");
      _playerFactory = _factory.GetComponent<PlayerFactory>();
      _npcFactory = _factory.GetComponent<NPCFactory>();
      _itemFactory = _factory.GetComponent<ItemFactory>();
    }

    private void Update() {
      if (!NetworkSettings.IsConnected) {
        return;
      }

      int recHostId;
      int connectionId;
      int channelId;
      byte[] recBuffer = new byte[1024];
      int bufferSize = 1024;
      int dataSize;
      byte error;
      NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer,
        bufferSize, out dataSize, out error);
      switch (recData) {
        case NetworkEventType.DataEvent:
          string message = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
          Debug.Log("Receiving: " + message);
          string[] splitData = message.Split('|');
          switch (splitData[0]) {
            case "ASKNAME":
              OnAskName(splitData);
              break;
            case "NPCS":
              OnNPCs(splitData);
              break;
            case "ITEMS":
              OnItems(splitData);
              break;
            case "INVENTORY":
              OnInventory(splitData);
              break;
            case "CNN":
              OnConnect(splitData);
              break;
            case "DC":
              OnDisconnect(int.Parse(splitData[1]));
              break;
            case "ASKPOSITION":
              OnAskPosition(splitData);
              break;
            case "ATK":
              OnAttack(int.Parse(splitData[1]));
              break;
            case "HIT":
              OnHit(int.Parse(splitData[1]), int.Parse(splitData[2]), int.Parse(splitData[3]));
              break;
            case "HITNPC":
              OnHitNPC(int.Parse(splitData[1]), int.Parse(splitData[2]), int.Parse(splitData[3]));
              break;
            case "USE":
              OnUse(int.Parse(splitData[1]), int.Parse(splitData[2]));
              break;
            case "DROP":
              OnDrop(int.Parse(splitData[1]), int.Parse(splitData[2]), int.Parse(splitData[3]),
                float.Parse(splitData[4]), float.Parse(splitData[5]), float.Parse(splitData[6]));
              break;
            case "PICKUP":
              OnPickup(int.Parse(splitData[1]), int.Parse(splitData[2]));
              break;
            case "NPCITEMS":
              OnNPCItems(splitData);
              break;
            default:
              Debug.Log("Invalid message : " + message);
              break;
          }

          break;
      }
    }

    private void OnAskName(string[] data) {
      // Set this client's ID

      // Send our name to the server
      NetworkSend.Reliable("NAMEIS|" + ConnectedCharacters.MyDatabasePlayer.character_name + "|" + ConnectedCharacters.MyDatabasePlayer.character_id);
      
      // Create all the other players
      for (int i = 2; i < data.Length; i++) {
        string[] d = data[i].Split('%');
        _playerFactory.Spawn(
          int.Parse(d[0]), 
          d[1], 
          d[2],
          float.Parse(d[3]), float.Parse(d[4]), float.Parse(d[5]), 
          float.Parse(d[6]), float.Parse(d[7]), float.Parse(d[8]), 
          int.Parse(d[9]),  
          int.Parse(d[10]), 
          int.Parse(d[11]), 
          int.Parse(d[12]), 
          int.Parse(d[13]), 
          int.Parse(d[14]), 
          int.Parse(d[15]), 
          int.Parse(d[16]), 
          int.Parse(d[17]),
          int.Parse(d[18]),
          int.Parse(d[19])
        );
      }
    }

    private void OnItems(string[] data) {
      _itemFactory.Spawn(data);
    }

    private void OnNPCs(string[] data) {
      _npcFactory.Spawn(data);
    }

    private void OnInventory(string[] data) {
      for (int i = 1; i < data.Length; i++) {
        ConnectedCharacters.MyPlayer.Inventory.Add(int.Parse(data[i]));
      }
    }

    private void OnConnect(string[] data) {
      _playerFactory.Spawn(
        int.Parse(data[1]), 
        data[2], 
        data[3],
        float.Parse(data[4]), float.Parse(data[5]), float.Parse(data[6]), 
        float.Parse(data[7]), float.Parse(data[8]), float.Parse(data[9]), 
        int.Parse(data[10]), 
        int.Parse(data[11]), 
        int.Parse(data[12]), 
        int.Parse(data[13]), 
        int.Parse(data[14]), 
        int.Parse(data[15]), 
        int.Parse(data[16]), 
        int.Parse(data[17]),
        int.Parse(data[18]),
        int.Parse(data[19]),
        int.Parse(data[20])
      );
    }

    private void OnDisconnect(int characterId) {
      Destroy(ConnectedCharacters.Players[characterId].Avatar);
      ConnectedCharacters.Players.Remove(characterId);
    }

    private void OnAskPosition(string[] data) {
      if (!NetworkSettings.IsStarted) {
        return;
      }

      // Update everyone else
      for (int i = 1; i < data.Length; i++) {
        string[] d = data[i].Split('%');

        // Prevent the server from updating us
        if (ConnectedCharacters.MyPlayer.CharacterId != int.Parse(d[0])) {
          Vector3 position = Vector3.zero;
          position.x = float.Parse(d[1]);
          position.y = float.Parse(d[2]);
          position.z = float.Parse(d[3]);

          Quaternion rotation = Quaternion.identity;
          rotation.w = float.Parse(d[4]);
          rotation.x = float.Parse(d[5]);
          rotation.y = float.Parse(d[6]);
          rotation.z = float.Parse(d[7]);

          ConnectedCharacters.Players[int.Parse(d[0])].LastRealPosition =
            ConnectedCharacters.Players[int.Parse(d[0])].RealPosition;
          ConnectedCharacters.Players[int.Parse(d[0])].LastRealRotation =
            ConnectedCharacters.Players[int.Parse(d[0])].RealRotation;

          ConnectedCharacters.Players[int.Parse(d[0])].RealPosition = position;
          ConnectedCharacters.Players[int.Parse(d[0])].RealRotation = rotation;

          ConnectedCharacters.Players[int.Parse(d[0])].TimeToLerp = float.Parse(d[8]);
          if (ConnectedCharacters.Players[int.Parse(d[0])].RealPosition !=
              ConnectedCharacters.Players[int.Parse(d[0])].Avatar.transform.position) {
            ConnectedCharacters.Players[int.Parse(d[0])].IsLerpingPosition = true;
          }

          if (ConnectedCharacters.Players[int.Parse(d[0])].RealRotation !=
              ConnectedCharacters.Players[int.Parse(d[0])].Avatar.transform.rotation) {
            ConnectedCharacters.Players[int.Parse(d[0])].IsLerpingRotation = true;
          }

          ConnectedCharacters.Players[int.Parse(d[0])].TimeStartedLerping = Time.time;
        }
      }

      // Send our own position
      Vector3 myPosition = ConnectedCharacters.MyPlayer.Avatar.transform.position;
      Quaternion myRotation = ConnectedCharacters.MyPlayer.Avatar.transform.rotation;
      ConnectedCharacters.MyPlayer.TimeBetweenMovementEnd = Time.time;
      string message = "MYPOSITION|" + myPosition.x + '|' + myPosition.y + '|' + myPosition.z + '|' + myRotation.w +
                       '|' + +myRotation.x + '|' + +myRotation.y + '|' + +myRotation.z + '|' +
                       (ConnectedCharacters.MyPlayer.TimeBetweenMovementEnd -
                        ConnectedCharacters.MyPlayer.TimeBetweenMovementStart);
      NetworkSend.Unreliable(message);
      ConnectedCharacters.MyPlayer.TimeBetweenMovementStart = Time.time;
    }

    private void OnAttack(int characterId) {
      if (characterId != ConnectedCharacters.MyPlayer.CharacterId) {
        ConnectedCharacters.Players[characterId].Avatar.GetComponent<CharacterAnimator>().Attack();
      }
    }

    private void OnHit(int characterId, int hitId, int damage) {
      if (hitId == ConnectedCharacters.MyPlayer.CharacterId) {
        CanvasSettings.PlayerHudController.ActivateHealthView();
      }

      if (characterId != ConnectedCharacters.MyPlayer.CharacterId) {
        ConnectedCharacters.Players[hitId].TakeDamage(damage);
      }
    }

    private void OnHitNPC(int characterId, int hitId, int damage) {
      if (characterId != ConnectedCharacters.MyPlayer.CharacterId) {
        ConnectedCharacters.NPCs[hitId].TakeDamage(damage);
      }
    }

    private void OnUse(int characterId, int itemId) {
      if (characterId != ConnectedCharacters.MyPlayer.CharacterId) {
        ItemList.Items[itemId].Use(ConnectedCharacters.Players[characterId]);
      }
    }

    private void OnDrop(int characterId, int worldId, int itemId, float x, float y, float z) {
      _itemFactory.Spawn(worldId, itemId, x, y, z);
      if (characterId == ConnectedCharacters.MyPlayer.CharacterId && CanvasSettings.InventoryMenu.activeSelf) {
        CanvasSettings.InventoryMenuController.RefreshMenu();
      }
    }

    private void OnPickup(int characterId, int worldId) {
      ConnectedCharacters.Players[characterId].Inventory
        .Add(ItemList.WorldItems[worldId].GetComponent<ItemReference>().ItemId);
      Destroy(ItemList.WorldItems[worldId]);
      ItemList.WorldItems.Remove(worldId);
    }

    private void OnNPCItems(string[] data) {
      // Populate NPC inventory
      ConnectedCharacters.NPCs[int.Parse(data[1])].Inventory.Clear();
      for (int i = 2; i < data.Length; i++) {
        ConnectedCharacters.NPCs[int.Parse(data[1])].Inventory.Add(int.Parse(data[i]));
      }
    }
  }
}