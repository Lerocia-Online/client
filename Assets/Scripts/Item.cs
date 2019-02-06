using System.Collections.Generic;

public abstract class Item {
	private int id;
	private string name;
	private int weight;
	private int value;
	private string category;
	private string description;
	private List<KeyValuePair<string, string>> stats;

	protected Item(int id, string name, int weight, int value, string category) {
		this.id = id;
		this.name = name;
		this.weight = weight;
		this.value = value;
		this.category = category;
		description = null;
		stats = new List<KeyValuePair<string, string>>();
		stats.Add(new KeyValuePair<string, string>("Weight", weight.ToString()));
		stats.Add(new KeyValuePair<string, string>("Value", value.ToString()));
	}

	public int getId() {
		return id;
	}

	public string getName() {
		return name;
	}

	public int getWeight() {
		return weight;
	}

	public int getValue() {
		return value;
	}

	public string getCategory() {
		return category;
	}

	public string getDescription() {
		return description;
	}

	protected void setDescription(string description) {
		this.description = description;
	}

	protected void addStat(string title, string value) {
		stats.Add(new KeyValuePair<string, string>(title, value));
	}

	public List<KeyValuePair<string, string>> getStats() {
		return stats;
	}

	public abstract void Use(Player player);

	public void Drop(Player player) {
		player.inventory.Remove(id);
	}
}