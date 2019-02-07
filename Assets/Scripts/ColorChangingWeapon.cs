using UnityEngine;

public class ColorChangingWeapon : Weapon {
	private Color color;

	public ColorChangingWeapon(int id, string name, int weight, int value, int damage, Color color) : base(id, name, weight, value,
		damage) {
		this.color = color;
	}

	public override void Use(Player player) {
		player.currentColor = color;
		Equip(player);
	}
}