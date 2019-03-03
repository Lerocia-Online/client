namespace Menus.Controllers {
  using System.Collections.Generic;
  using System.Linq;
  using UnityEngine;
  using UnityEngine.UI;
  using Lerocia.Characters;
  using Lerocia.Items;
  using Items;
  using Networking;

  public class InventoryMenuController : MonoBehaviour {
    public GameObject CategoryTextPrefab;
    public GameObject ItemTextPrefab;
    public GameObject ItemImagePrefab;
    public GameObject ItemNamePrefab;
    public GameObject ItemStatPrefab;
    public GameObject ItemDescriptionPrefab;
    private GameObject _playerHealthBar;
    private GameObject _playerStaminaBar;
    private Dictionary<GameObject, List<GameObject>> _itemDictionary;
    private GameObject _currentCategory;
    private const float ScrollDelay = 0.25f;
    private float _lastScrollTime;
    private int _currentCategoryIndex;
    private List<int> _currentItemIndexes;
    private bool _isItemView;

    // Use this for initialization
    void Start() {
      _lastScrollTime = 0;
      _currentCategoryIndex = 0;
      _isItemView = false;
    }

    // Update is called once per frame
    void Update() {
      if (Time.time - _lastScrollTime > ScrollDelay && ConnectedCharacters.MyPlayer.Inventory.Count > 0) {
        if (Input.GetAxis("Vertical") > 0) {
          MoveUp();
        } else if (Input.GetAxis("Vertical") < 0) {
          MoveDown();
        }

        if (Input.GetAxis("Horizontal") > 0) {
          MoveRight();
        } else if (Input.GetAxis("Horizontal") < 0) {
          MoveLeft();
        }

        if (_isItemView) {
          if (Input.GetKeyDown(KeyCode.E)) {
            UseItem();
          } else if (Input.GetKeyDown(KeyCode.R)) {
            DropItem();
          }

          GameObject playerPanel = transform.Find("Player Panel").gameObject;
          playerPanel.transform.Find("Health Bar").GetComponent<Slider>().maxValue =
            ConnectedCharacters.MyPlayer.MaxHealth;
          playerPanel.transform.Find("Health Bar").GetComponent<Slider>().value =
            ConnectedCharacters.MyPlayer.CurrentHealth;
          playerPanel.transform.Find("Stamina Bar").GetComponent<Slider>().maxValue =
            ConnectedCharacters.MyPlayer.MaxStamina;
          playerPanel.transform.Find("Stamina Bar").GetComponent<Slider>().value =
            ConnectedCharacters.MyPlayer.CurrentStamina;
          playerPanel.transform.Find("Gold").transform.Find("Value").GetComponent<Text>().text =
            ConnectedCharacters.MyPlayer.Gold.ToString();
          playerPanel.transform.Find("Weight").transform.Find("Value").GetComponent<Text>().text =
            ConnectedCharacters.MyPlayer.Weight.ToString();
          playerPanel.transform.Find("Armor").transform.Find("Value").GetComponent<Text>().text =
            ConnectedCharacters.MyPlayer.Armor.ToString();
          playerPanel.transform.Find("Damage").transform.Find("Value").GetComponent<Text>().text =
            ConnectedCharacters.MyPlayer.Damage.ToString();
        }
      }
    }

    public void OpenMenu() {
      // Initialize dictionary of all items with key->value being category->items
      _itemDictionary = new Dictionary<GameObject, List<GameObject>>();

      // This list keeps track of the current index of each item list from category to category so location is persistent
      _currentItemIndexes = new List<int>();

      // Initialize categories for each item in inventory
      List<string> distinctCategories = new List<string>();
      foreach (int itemId in ConnectedCharacters.MyPlayer.Inventory) {
        distinctCategories.Add(ItemList.Items[itemId].GetCategory());
      }

      // Remove all duplicate categories from list
      distinctCategories = distinctCategories.Distinct().ToList();

      // Initialize list of category GameObject's, create Text object for each category, and place them in the menu
      List<GameObject> categoryList = new List<GameObject>();
      Vector3 nextPosition = Vector3.zero;
      foreach (string category in distinctCategories) {
        GameObject categoryText = Instantiate(CategoryTextPrefab);
        categoryText.name = category;
        categoryText.GetComponent<Text>().text = category;
        categoryText.transform.SetParent(transform.Find("Categories Selector Panel"), false);
        categoryText.transform.localPosition = nextPosition;
        nextPosition = new Vector3(0, nextPosition.y - categoryText.GetComponent<RectTransform>().rect.height, 0);
        categoryList.Add(categoryText);
        _currentItemIndexes.Add(0);
      }

      // Initialize list of item GameObject's, create Text objects for each item, and separate them into lists by category
      foreach (GameObject category in categoryList) {
        nextPosition = Vector3.zero;
        _itemDictionary[category] = new List<GameObject>();
        Dictionary<int, int> uniqueItemDictionary = new Dictionary<int, int>();
        foreach (int item_id in ConnectedCharacters.MyPlayer.Inventory) {
          if (uniqueItemDictionary.ContainsKey(item_id)) {
            uniqueItemDictionary[item_id]++;
          } else {
            uniqueItemDictionary.Add(item_id, 1);
          }
        }

        foreach (KeyValuePair<int, int> itemId in uniqueItemDictionary) {
          if (ItemList.Items[itemId.Key].GetCategory() == category.GetComponent<Text>().text) {
            GameObject itemText = Instantiate(ItemTextPrefab);
            itemText.name = ItemList.Items[itemId.Key].GetName();
            itemText.GetComponent<Text>().text = ItemList.Items[itemId.Key].GetName();
            itemText.transform.SetParent(transform.Find("Items Selector Panel"), false);
            itemText.transform.localPosition = nextPosition;
            nextPosition = new Vector3(0, nextPosition.y - itemText.GetComponent<RectTransform>().rect.height, 0);
            itemText.GetComponent<ItemReference>().ItemId = itemId.Key;
            if (itemId.Value > 1) {
              itemText.GetComponent<Text>().text += " (" + itemId.Value + ")";
            }

            if (ConnectedCharacters.MyPlayer.WeaponId == itemId.Key || ConnectedCharacters.MyPlayer.ApparelId == itemId.Key) {
              itemText.transform.Find("Equipped").gameObject.SetActive(true);
            } else {
              itemText.transform.Find("Equipped").gameObject.SetActive(false);
            }
            _itemDictionary[category].Add(itemText);
          }
        }
      }

      SetCurrentCategory();
      DisableAllItems();
      UpdateItemList();
      ToggleItemView(false);
    }

    public void CloseMenu() {
      foreach (KeyValuePair<GameObject, List<GameObject>> entry in _itemDictionary) {
        Destroy(entry.Key);
        foreach (GameObject go in entry.Value) {
          Destroy(go);
        }
      }

      _itemDictionary.Clear();
      _currentItemIndexes.Clear();
      _currentCategoryIndex = 0;
    }

    private void MoveUp() {
      if (!_isItemView && _currentCategoryIndex > 0) {
        MoveVertical(-1);
      } else if (_isItemView && _currentItemIndexes[_currentCategoryIndex] > 0) {
        MoveVertical(-1);
      }
    }

    private void MoveDown() {
      if (!_isItemView && _currentCategoryIndex < _itemDictionary.Count - 1) {
        MoveVertical(1);
      } else if (_isItemView && _currentItemIndexes[_currentCategoryIndex] < _itemDictionary[_currentCategory].Count - 1) {
        MoveVertical(1);
      }
    }

    private void MoveVertical(int directionMultiplier) {
      _lastScrollTime = Time.time;

      if (_isItemView) {
        _currentItemIndexes[_currentCategoryIndex] += directionMultiplier;
        foreach (GameObject item in _itemDictionary[_currentCategory]) {
          item.transform.localPosition = new Vector3(0,
            item.transform.localPosition.y + item.GetComponent<RectTransform>().rect.height * directionMultiplier,
            0);
        }

        UpdateItemView();
      } else {
        _currentCategoryIndex += directionMultiplier;
        // Move all text upwards
        foreach (GameObject category in _itemDictionary.Keys) {
          category.transform.localPosition = new Vector3(0,
            category.transform.localPosition.y +
            category.GetComponent<RectTransform>().rect.height * directionMultiplier,
            0);
        }

        // Update which item list based on new category
        UpdateItemList();
      }
    }

    private void MoveRight() {
      if (!_isItemView) {
        ToggleItemView(true);
        UpdateItemView();
      }
    }

    private void MoveLeft() {
      if (_isItemView) {
        ToggleItemView(false);
      }
    }

    private void UseItem() {
      NetworkSend.Reliable("USE|" + GetCurrentSelectedItem().GetId());
      GetCurrentSelectedItem().Use(ConnectedCharacters.MyPlayer);
      RefreshMenu();
    }

    private void DropItem() {
      GameObject item = new GameObject();
      item.transform.position = ConnectedCharacters.MyPlayer.Avatar.transform.position;
      item.transform.rotation = ConnectedCharacters.MyPlayer.Avatar.transform.rotation;
      item.transform.position += item.transform.TransformDirection(Vector3.forward) * 2;
      NetworkSend.Reliable("DROP|" + GetCurrentSelectedItem().GetId() + "|" + item.transform.position.x + "|" +
                          item.transform.position.y + "|" + item.transform.position.z);
      Destroy(item);
      GetCurrentSelectedItem().Drop(ConnectedCharacters.MyPlayer);
      RefreshMenu();
    }

    private void ToggleItemView(bool visible) {
      _isItemView = visible;
      transform.Find("Items Selector Panel").gameObject.SetActive(visible);
      transform.Find("Item Panel").gameObject.SetActive(visible);
      transform.Find("Player Panel").gameObject.SetActive(visible);
    }

    private void UpdateItemList() {
      // Check if the users inventory is empty
      if (_itemDictionary.Count == 0) {
        return;
      }

      // hide items for old category
      foreach (GameObject item in _itemDictionary[_currentCategory]) {
        item.SetActive(false);
      }

      // switch category to current selected category
      SetCurrentCategory();

      // show items for new category
      foreach (GameObject item in _itemDictionary[_currentCategory]) {
        item.SetActive(true);
      }
    }

    private void UpdateItemView() {
      DestroyItemView();
      CreateItemView();
    }

    private void CreateItemView() {
      List<GameObject> statList = new List<GameObject>();
      BaseItem item = GetCurrentSelectedItem();
      GameObject panel = transform.Find("Item Panel").gameObject;

      GameObject itemImage = Instantiate(ItemImagePrefab);
      itemImage.transform.SetParent(panel.transform, false);
      //TODO set item image

      GameObject itemName = Instantiate(ItemNamePrefab);
      itemName.transform.SetParent(panel.transform, false);
      itemName.GetComponent<Text>().text = item.GetName();

      // Create stat object in item view for each stat on this item
      foreach (KeyValuePair<string, string> stat in item.GetStats()) {
        GameObject itemStat = Instantiate(ItemStatPrefab);
        itemStat.transform.SetParent(panel.transform, false);
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

      // Only create a description text box if the item has a description
      if (item.GetDescription() != null) {
        GameObject itemDescription = Instantiate(ItemDescriptionPrefab);
        itemDescription.transform.SetParent(panel.transform, false);
        itemDescription.GetComponent<Text>().text = item.GetDescription();
      }
    }

    private void DestroyItemView() {
      Transform panel = transform.Find("Item Panel");
      foreach (Transform child in panel) {
        Destroy(child.gameObject);
      }
    }

    private BaseItem GetCurrentSelectedItem() {
      foreach (GameObject item in _itemDictionary[_currentCategory]) {
        if (item.transform.localPosition.y == 0) {
          return ItemList.Items[item.GetComponent<ItemReference>().ItemId];
        }
      }

      return null;
    }

    private void SetCurrentCategory() {
      // Set category based on height in list (in line with selector)
      foreach (GameObject category in _itemDictionary.Keys) {
        if (category.transform.localPosition.y == 0) {
          _currentCategory = category;
        }
      }
    }

    private void DisableAllItems() {
      foreach (List<GameObject> items in _itemDictionary.Values) {
        foreach (GameObject item in items) {
          item.SetActive(false);
        }
      }
    }

    public void RefreshMenu() {
      int categoryIndex = _currentCategoryIndex;
      List<int> itemIndexes = new List<int>(_currentItemIndexes);
      CloseMenu();
      OpenMenu();
      if (_itemDictionary.Count > 0) {
        for (int i = 0; i < categoryIndex; i++) {
          MoveDown();
        }

        MoveRight();
        for (int i = 0; i < itemIndexes[categoryIndex]; i++) {
          MoveDown();
        }
      }
    }
  }
}