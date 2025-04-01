using Godot;
using System;
using Current.InstanceStates; // Updated namespace

public partial class GSymphonCool : Node, IState
{
    private readonly States owner;

    public GSymphonCool(States owner)
    {
        this.owner = owner;
    }

    public void Enter()
    {
        GD.Print("Entering GSymphonCool State");
        // Add logic for entering the GSymphonCool state
    }

    public void Update(float delta)
    {
        // Add logic for updating the GSymphonCool state
    }

    public void Exit()
    {
        GD.Print("Exiting GSymphonCool State");
        // Add logic for exiting the GSymphonCool state
    }
}
