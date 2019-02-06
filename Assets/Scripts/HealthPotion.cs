public class HealthPotion : Potion {
	private int health;

	public HealthPotion(int id, string name, int weight, int value, int health) : base(id, name, weight, value) {
		this.health = health;
		setDescription("Heals by " + health + " points.");
	}

	public override void Use(Player player) {
		player.currentHealth += health;
		if (player.currentHealth >= player.maxHealth) {
			player.currentHealth = player.maxHealth;
		}
		player.inventory.Remove(getId());
	}
}