using Godot;
using System;
using HerNostalgia.States;

public partial class GParry : Node
{
}

namespace HerNostalgia.States
{
    public class ParryState : IState
    {
        private readonly States owner;

        public ParryState(States owner)
        {
            this.owner = owner;
        }

        public void Enter()
        {
            GD.Print("Entering Parry State");
            Engine.Instance.NotifyParryStart();
        }

        public void Update(float delta)
        {
            if (owner.Input.IsActionJustReleased("parry"))
            {
                owner.ChangeState(Engine.eStates.Idle);
            }
        }

        public void Exit()
        {
            GD.Print("Exiting Parry State");
            Engine.Instance.NotifyParryEnd();
        }
    }
}
