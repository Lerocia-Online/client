namespace Items.Weapons {
  using Players;
  public class BaseWeapon : BaseItem {
    private int damage;

    public BaseWeapon(int id, string name, int weight, int value, int damage) : base(id, name, weight, value, "Weapon") {
      this.damage = damage;
      AddStat("Damage", damage.ToString());
    }

    public int GetDamage() {
      return damage;
    }

    public void Equip(Player player) {
      if (player.Weapon != GetId()) {
        player.Weapon = GetId();
      } else {
        player.Weapon = -1;
      }
      player.UpdateStats();
    }

    public override void Use(Player player) {
      Equip(player);
    }
  }
}