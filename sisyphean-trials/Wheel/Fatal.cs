using Godot;
using System;

public partial class Fatal : Node
{
	public void HandleFatalEvent(Player player)
	{
		GD.Print("Player encountered a fatal event");
		// Add more detailed logic here
	}
}
