using Godot;
using System;
using Current.InstanceStates; // Updated namespace

public partial class GStrikeSpecialCool : Node, IState
{
    private readonly States owner;

    public GStrikeSpecialCool(States owner)
    {
        this.owner = owner;
    }

    public void Enter()
    {
        GD.Print("Entering GStrikeSpecialCool State");
        // Add logic for entering the GStrikeSpecialCool state
    }

    public void Update(float delta)
    {
        // Add logic for updating the GStrikeSpecialCool state
    }

    public void Exit()
    {
        GD.Print("Exiting GStrikeSpecialCool State");
        // Add logic for exiting the GStrikeSpecialCool state
    }
}
