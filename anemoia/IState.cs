namespace Current.InstanceStates
{
    public interface IState
    {
        void Enter();
        void Update(float delta);
        void Exit();

        void InAirState();
    }
}
