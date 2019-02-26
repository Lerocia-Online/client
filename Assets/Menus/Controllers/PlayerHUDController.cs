namespace Menus.Controllers {
  using System.Collections.Generic;
  using System;
  using System.Reflection;
  using UnityEngine;
  using UnityEngine.UI;
  using Lerocia.Characters;
  using Lerocia.Characters.Players;
  using Characters.NPCs;
  using Lerocia.Items;

  public class PlayerHUDController : MonoBehaviour {
    [SerializeField] private GameObject _itemStatPrefab;
    [SerializeField] private GameObject _dialoguePrefab;

    private GameObject _enemyView;
    private Slider _enemyHealthBar;
    private Text _enemyName;
    private GameObject _healthBar;
    private Slider _healthBarSlider;
    private GameObject _staminaBar;
    private Slider _staminaBarSlider;
    private GameObject _interactableView;
    private Text _helpText;
    private Text _name;
    private GameObject _statsContainer;
    private GameObject _dialogueView;
    private List<GameObject> _dialogueList;
    private int _currentDialogueIndex;
    private GameObject _currentDialogue;
    private bool _isDialogueView;
    private Character _currentInteractingCharacter;
    private const float ScrollDelay = 0.25f;
    private float _lastScrollTime;
    private GameObject _caption;
    private Text _captionText;
    private Character _enemyCharacter;
    private float _enemyViewUpdateTime;
    private const float EnemyViewTimer = 30.0f;
    private float _healthViewUpdateTime;
    private const float HealthViewTimer = 30.0f;
    private float _staminaViewUpdateTime;
    private const float StaminaViewTimer = 30.0f;
    private float _captionViewUpdateTime;
    private const float CaptionViewTimer = 10.0f;
    public Player Player;

    // Use this for initialization
    private void Start() {
      _enemyView = transform.Find("Enemy View").gameObject;
      _enemyHealthBar = _enemyView.transform.Find("HealthBar").GetComponent<Slider>();
      _enemyName = _enemyView.transform.Find("Name").GetComponent<Text>();
      DeactivateEnemyView();
      _healthBar = transform.Find("HealthBar").gameObject;
      _healthBarSlider = _healthBar.GetComponent<Slider>();
      DeactivateHealthView();
      _staminaBar = transform.Find("StaminaBar").gameObject;
      _staminaBarSlider = _staminaBar.GetComponent<Slider>();
      DeactivateStaminaView();
      _interactableView = transform.Find("Interactable View").gameObject;
      _helpText = _interactableView.transform.Find("Help Text").GetComponent<Text>();
      _name = _interactableView.transform.Find("Name").GetComponent<Text>();
      _statsContainer = _interactableView.transform.Find("Stats").gameObject;
      DeactivateInteractableView();
      _dialogueView = transform.Find("Dialogue View").gameObject;
      _dialogueList = new List<GameObject>();
      _currentDialogueIndex = 0;
      _lastScrollTime = 0;
      _isDialogueView = false;
      DeactivateDialogueView();
      _caption = transform.Find("Caption").gameObject;
      _captionText = _caption.GetComponent<Text>();
      DeactivateCaptionView();
    }

    private void Update() {
      _healthBarSlider.value = Player.CurrentHealth;
      _staminaBarSlider.value = Player.CurrentStamina;
      if (_enemyCharacter != null) {
        _enemyHealthBar.value = _enemyCharacter.CurrentHealth;
        if (Time.time - _enemyViewUpdateTime > EnemyViewTimer) {
          DeactivateEnemyView();
        }
      }

      if (Time.time - _healthViewUpdateTime > HealthViewTimer) {
        DeactivateHealthView();
      }

      if (Time.time - _staminaViewUpdateTime > StaminaViewTimer) {
        DeactivateHealthView();
      }

      if (Time.time - _captionViewUpdateTime > CaptionViewTimer) {
        DeactivateCaptionView();
      }

      if (_isDialogueView && Time.time - _lastScrollTime > ScrollDelay) {
        if (Input.GetAxis("Vertical") > 0) {
          MoveUp();
        } else if (Input.GetAxis("Vertical") < 0) {
          MoveDown();
        }
      }

      if (_isDialogueView && Time.time - _lastScrollTime > ScrollDelay) {
        if (Input.GetKeyDown(KeyCode.E)) {
          Interact(_currentDialogue.GetComponent<Text>().text);
        }
      }
    }

    public void ActivateInteractableView() {
      _interactableView.SetActive(true);
    }

    public void DeactivateInteractableView() {
      _interactableView.SetActive(false);
      foreach (Transform child in _statsContainer.transform) {
        Destroy(child.gameObject);
      }
    }

    public void SetCurrentInteractingCharacter(Character character) {
      _currentInteractingCharacter = character;
    }

    public void Interact(string text) {
      string[] options = ConnectedCharacters
        .NPCs[_currentInteractingCharacter.Avatar.GetComponent<NPCReference>().NPCId]
        .Interact(text);
      if (options != null) {
        if (options.Length > 1) {
          UpdateDialogueView(options);
        } else {
          DeactivateDialogueView();
          Type thisType = _currentInteractingCharacter.GetType();
          MethodInfo theMethod = thisType.GetMethod(options[0]);
          theMethod.Invoke(_currentInteractingCharacter, null);
        }
      } else {
        DeactivateDialogueView();
      }
    }

    public void SetNPCView(string npcName) {
      _helpText.text = "(E) Talk";
      _name.text = npcName;
    }

    public void SetLootView(string name) {
      _helpText.text = "(E) Loot";
      _name.text = name;
    }

    public void SetItemView(BaseItem item) {
      _helpText.text = "(E) Take";
      _name.text = ItemList.Items[item.GetId()].GetName();
      List<GameObject> statList = new List<GameObject>();
      // Create stat object in item view for each stat on this item
      foreach (KeyValuePair<string, string> stat in item.GetStats()) {
        GameObject itemStat = Instantiate(_itemStatPrefab);
        itemStat.transform.SetParent(_statsContainer.transform, false);
        itemStat.transform.Find("Title").GetComponent<Text>().text = stat.Key;
        itemStat.transform.Find("Value").GetComponent<Text>().text = stat.Value;
        statList.Add(itemStat);
      }

      // Set x position of each stat in the item view based on the number of stats to display
      int counter = 1;
      foreach (GameObject stat in statList) {
        float width = stat.GetComponent<RectTransform>().rect.width;
        float offset = counter - (float) (statList.Count + 1) / 2;
        stat.transform.localPosition = new Vector3(width * offset, 0, 0);
        counter++;
      }
    }

    public void ActivateEnemyView(Character character) {
      _enemyView.SetActive(true);
      UpdateEnemyView(character);
    }

    public void DeactivateEnemyView() {
      _enemyView.SetActive(false);
    }

    public void UpdateEnemyView(Character character) {
      _enemyViewUpdateTime = Time.time;
      _enemyCharacter = character;
      _enemyName.text = character.Name;
    }

    public void ActivateHealthView() {
      _healthViewUpdateTime = Time.time;
      _healthBar.SetActive(true);
    }

    public void DeactivateHealthView() {
      _healthBar.SetActive(false);
    }

    public void ActivateStaminaView() {
      _staminaViewUpdateTime = Time.time;
      _staminaBar.SetActive(true);
    }

    public void DeactivateStaminaView() {
      _staminaBar.SetActive(false);
    }

    public void ActivateCaptionView(string caption) {
      _captionViewUpdateTime = Time.time;
      _captionText.text = caption;
      _caption.SetActive(true);
    }

    public void DeactivateCaptionView() {
      _caption.SetActive(false);
    }

    public void ActivateDialogueView(Character character, string[] options) {
      _dialogueView.SetActive(true);
      _isDialogueView = true;
      CanvasSettings.ToggleControl(false);
      _currentInteractingCharacter = character;
      _helpText.text = character.Name;
      _name.text = "";
      _lastScrollTime = Time.time;
      _currentDialogueIndex = 0;
      _dialogueList.Clear();
      Vector3 nextPosition = Vector3.zero;
      foreach (string dialogue in options) {
        GameObject dialogueObject = Instantiate(_dialoguePrefab);
        dialogueObject.transform.SetParent(_dialogueView.transform, false);
        dialogueObject.GetComponent<Text>().text = dialogue;
        dialogueObject.transform.localPosition = nextPosition;
        nextPosition = new Vector3(0, nextPosition.y - dialogueObject.GetComponent<RectTransform>().rect.height, 0);
        _dialogueList.Add(dialogueObject);
      }

      SetCurrentDialogue();
    }

    public void DeactivateDialogueView() {
      _dialogueView.SetActive(false);
      CanvasSettings.ToggleControl(true);
      foreach (GameObject dialogue in _dialogueList) {
        Destroy(dialogue);
      }

      _dialogueList.Clear();
      _currentDialogueIndex = 0;
      _isDialogueView = false;
    }

    private void UpdateDialogueView(string[] options) {
      DeactivateDialogueView();
      ActivateDialogueView(_currentInteractingCharacter, options);
    }

    private void MoveUp() {
      if (_currentDialogueIndex > 0) {
        MoveVertical(-1);
      }
    }

    private void MoveDown() {
      if (_currentDialogueIndex < _dialogueList.Count - 1) {
        MoveVertical(1);
      }
    }

    private void MoveVertical(int directionMultiplier) {
      _lastScrollTime = Time.time;
      _currentDialogueIndex += directionMultiplier;
      // Move all text upwards
      foreach (GameObject dialogue in _dialogueList) {
        dialogue.transform.localPosition = new Vector3(0,
          dialogue.transform.localPosition.y + dialogue.GetComponent<RectTransform>().rect.height * directionMultiplier,
          0);
      }

      // Update which item list based on new category
      SetCurrentDialogue();
    }

    private void SetCurrentDialogue() {
      // Set category based on height in list (in line with selector)
      foreach (GameObject dialogue in _dialogueList) {
        if (dialogue.transform.localPosition.y == 0) {
          _currentDialogue = dialogue;
        }
      }
    }
  }
}