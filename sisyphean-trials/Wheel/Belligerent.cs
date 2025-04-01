using Godot;

public partial class Belligerent : Node
{
    public static Belligerent Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
        // Initialize belligerent-specific logic
    }

    public void OnStateChanged(Engine.eStates newState)
    {
        GD.Print("Belligerent notified of state change: " + newState);
        // Add logic to handle state changes, e.g., AI behavior or reactions
    }
}
