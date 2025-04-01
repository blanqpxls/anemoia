using Godot;
using System;
using HerNostalgia.States;

public partial class GStaggered : Node, IState
{
    private readonly States owner;

    public GStaggered(States owner)
    {
        this.owner = owner;
    }

    public void Enter()
    {
        GD.Print("Entering GStaggered State");
    }

    public void Update(float delta)
    {
        // Add logic for updating the GStaggered state
    }

    public void Exit()
    {
        GD.Print("Exiting GStaggered State");
    }
}

namespace HerNostalgia.States
{
    public class StaggeredState : IState
    {
        private readonly States owner;

        public StaggeredState(States owner)
        {
            this.owner = owner;
        }

        public void Enter()
        {
            GD.Print("Entering Staggered State");
            // Add logic for entering the staggered state
        }

        public void Update(float delta)
        {
            // Add logic for updating the staggered state
        }

        public void Exit()
        {
            GD.Print("Exiting Staggered State");
            // Add logic for exiting the staggered state
        }
    }
}
