namespace Players {
  using System.Collections.Generic;
  using UnityEngine;
  using Items;
  using Items.Weapons;
  using Items.Apparel;

  public class Player {
    // Identifiers
    public string Name;
    public GameObject Avatar;

    // Movement interpolation
    public bool IsLerpingPosition;
    public bool IsLerpingRotation;
    public Vector3 RealPosition;
    public Quaternion RealRotation;
    public Vector3 LastRealPosition;
    public Quaternion LastRealRotation;
    public float TimeStartedLerping;
    public float TimeToLerp;
    public float TimeBetweenMovementStart;
    public float TimeBetweenMovementEnd;

    // Stats
    public int MaxHealth;
    public int CurrentHealth;
    public int MaxStamina;
    public int CurrentStamina;
    public int Gold;
    public int Weight;
    public int Armor;
    public int Damage;

    // Equipped armor & weapons
    public int Weapon;
    public int Apparel;

    public List<int> Inventory;

    public Player(string name, GameObject avatar) {
      Name = name;
      Avatar = avatar;
      IsLerpingPosition = false;
      IsLerpingRotation = false;
      RealPosition = avatar.transform.position;
      RealRotation = avatar.transform.rotation;
      TimeBetweenMovementStart = Time.time;
      MaxHealth = 100;
      CurrentHealth = MaxHealth;
      MaxStamina = 100;
      CurrentStamina = MaxStamina;
      Gold = 0;
      Weight = 0;
      Armor = 0;
      Damage = 0;
      Weapon = -1;
      Apparel = -1;
      Inventory = new List<int>();
    }

    public void UpdateStats() {
      if (Weapon >= 0) {
        BaseWeapon weapon = ItemList.Items[Weapon] as BaseWeapon;
        Damage = weapon.GetDamage();
      } else {
        Damage = 0;
      }

      if (Apparel >= 0) {
        BaseApparel apparel = ItemList.Items[Apparel] as BaseApparel;
        Armor = apparel.GetArmor();
      } else {
        Armor = 0;
      }
    }
    
    public void TakeDamage(int damage) {
      damage = damage - Armor;
      if (damage <= 0) {
        damage = 0;
      }

      CurrentHealth -= damage;
      if (CurrentHealth <= 0) {
        KillPlayer();
      }
    }

    private void KillPlayer() {
      // Reset players health
      CurrentHealth = MaxHealth;
      // Move them back to "spawn" point
      Avatar.transform.position = new Vector3(0, 1, 0);
    }
  }
}