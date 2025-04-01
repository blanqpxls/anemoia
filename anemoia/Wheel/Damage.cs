using Godot;
using System;

public partial class Damage : Node
{
	public void ApplyDamage(Player player, int damage)
	{
		player.Health -= damage;
		GD.Print("Player took damage: " + damage);
		// Add more detailed logic here
	}
}
