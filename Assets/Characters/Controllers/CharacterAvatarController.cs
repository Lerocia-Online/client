namespace Characters.Controllers {
  using UnityEngine;
  using Lerocia.Characters;
  using Animation;
  using Items;
  using Items.Controllers;

  public class CharacterAvatarController : MonoBehaviour {
    private CharacterAnimator _characterAnimator;
    private GameObject _currentEquippedWeapon;
    public GameObject WeaponParent;
    public GameObject ApparelParent;

    private void Start() {
      _characterAnimator = GetComponent<CharacterAnimator>();
    }

    private void Update() {
      if (_currentEquippedWeapon) {
        if (_characterAnimator.Attacking) {
          _currentEquippedWeapon.GetComponent<CapsuleCollider>().enabled = true;
        } else {
          _currentEquippedWeapon.GetComponent<CapsuleCollider>().enabled = false;
        }
      }
    }

    public void UpdateWeapon(Character character) {
      if (character.WeaponId >= 0) {
        GameObject item = Instantiate(GameObject.Find("Factory").GetComponent<ItemFactory>().Items[character.WeaponId]);
        item.name = "EquippedWeapon";
        item.transform.SetParent(WeaponParent.transform, false);
        item.AddComponent<WeaponController>();
        _currentEquippedWeapon = item;
      } else {
        Destroy(GameObject.Find("EquippedWeapon"));
        _currentEquippedWeapon = null;
      }
    }

    public void UpdateApparel(Character character) {
      if (character.ApparelId >= 0) {
        GameObject item = Instantiate(GameObject.Find("Factory").GetComponent<ItemFactory>().Items[character.ApparelId]);
        item.name = "EquippedApparel";
        item.transform.SetParent(ApparelParent.transform, false);
        item.transform.localPosition = new Vector3(0, 0.026f, 0.054f);
        item.transform.localRotation = Quaternion.Euler(new Vector3(-90, 180, 0));
      } else {
        Destroy(GameObject.Find("EquippedApparel"));
      }
    }
  }
}