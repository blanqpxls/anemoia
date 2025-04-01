using Godot;
using System;

public partial class Ground : Node
{
    public bool IsOnGround(Player player)
    {
        // Implement ground detection logic here
        // This is a placeholder implementation
        return player.Position.Y >= 0;
    }
}
