namespace Items.Potions {
  using Players;
  public class HealthPotion : BasePotion {
    private int health;

    public HealthPotion(int id, string name, int weight, int value, int health) : base(id, name, weight, value) {
      this.health = health;
      SetDescription("Heals by " + health + " points.");
    }

    public override void Use(Player player) {
      player.CurrentHealth += health;
      if (player.CurrentHealth >= player.MaxHealth) {
        player.CurrentHealth = player.MaxHealth;
      }
      player.Inventory.Remove(GetId());
    }
  }
}