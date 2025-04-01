using Godot;
using System;
using Current.InstanceStates; // Updated namespace

public partial class gDefending : Node, IState
{
    private readonly States owner;

    public gDefending(States owner)
    {
        this.owner = owner;
    }

    public void Enter()
    {
        GD.Print("Entering gDefending State");
        // Add logic for entering the gDefending state
    }

    public void Update(float delta)
    {
        // Add logic for updating the gDefending state
    }

    public void Exit()
    {
        GD.Print("Exiting gDefending State");
        // Add logic for exiting the gDefending state
    }
}
