using Godot;
using System;

public partial class GStam : Node
{
	private int stamina;
	private int maxStamina = 100;

	public void IncreaseStamina(int amount)
	{
		stamina = Math.Min(stamina + amount, maxStamina);
		GD.Print("Stamina increased: " + stamina);
	}

	public void DecreaseStamina(int amount)
	{
		stamina = Math.Max(stamina - amount, 0);
		GD.Print("Stamina decreased: " + stamina);
	}

	public int GetStamina()
	{
		return stamina;
	}
}
