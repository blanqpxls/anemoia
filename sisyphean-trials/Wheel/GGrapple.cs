using Godot;
using System;

public partial class GGrapple : Node
{
	private bool isGrappling;

	public void StartGrapple(Player player)
	{
		isGrappling = true;
		GD.Print("Player started grappling");
		// Add more detailed logic here
	}

	public void StopGrapple(Player player)
	{
		isGrappling = false;
		GD.Print("Player stopped grappling");
		// Add more detailed logic here
	}

	public bool IsGrappling()
	{
		return isGrappling;
	}
}
