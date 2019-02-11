namespace Items.Apparel {
  using Players;
  public class BaseApparel : BaseItem {
    private int armor;

    public BaseApparel(int id, string name, int weight, int value, int armor) : base(id, name, weight, value, "Apparel") {
      this.armor = armor;
      AddStat("Armor", armor.ToString());
    }

    public int GetArmor() {
      return armor;
    }
  
    public override void Use(Player player) {
      if (player.Apparel != GetId()) {
        player.Apparel = GetId();
      } else {
        player.Apparel = -1;
      }
      player.UpdateStats();
    }
  }
}