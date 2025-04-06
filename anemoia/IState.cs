namespace Engine.States
{
    public interface IState
    {
        void Enter();
        void Update(float delta);
        void Exit();
        void InAirState();
        // Returns a motif associated with the state (if any)
        Motif GetMotif();
    }
}
