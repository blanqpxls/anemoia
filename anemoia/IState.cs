using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;   
using Godot;
using Target;
using Motifs;




namespace IState
{
    public interface IState
    {
        void Enter();
        void Update(float delta);
        void Exit();
        void getTarget();
        void getTarget(int id);

        void InAirState();
        void IAirState()
        {
            Target.Belligerent belligerentInstance = new Target.Belligerent();
            if (belligerentInstance.IsGrounded == false)
            if (Target.rState.isGrounded == false)
            {
                // Logic for when the belligerent is in the air
                // e.g., apply gravity, check for collisions, etc.
            }
            else
            {
                // Logic for when the belligerent is grounded
                // e.g., reset jump state, apply ground effects, etc.
            }
        }
        void GetMotifs();
        void GetMotif(int id);  
    }
}
