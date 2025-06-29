using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParadisisNostalga.Wheel
{
    public class Theatre
    {
        public string Name { get; set; }
        public List<Character> Actors { get; set; } = new List<Character>();

        // Assign a unique ScriptKey to the Character and its States node
        public void AssignActorId(Character character)
        {
            if (string.IsNullOrEmpty(character.iKey))
                character.iKey = Guid.NewGuid().ToString();
            var states = character.GetNodeOrNull<Engine.States.States>("States");
            if (states != null)
                states.ScriptKey = character.iKey;
            if (!Actors.Contains(character))
                Actors.Add(character);
        }

        // Call this each frame to update all actors' input from the global IStateTable
        public void FeedInputsToActors()
        {
            foreach (var actor in Actors)
            {
                var states = actor.GetNodeOrNull<Engine.States.States>("States");
                if (states != null)
                    states.FeedInputFromStateTable();
            }
        }

        public enum ActorBehaviourType
        {
            Player,
            Fugue,
            Mezzopiano
        }

        // Assign a behaviour to an actor
        public void SetActorBehaviour(Character actor, ActorBehaviourType behaviour)
        {
            switch (behaviour)
            {
                case ActorBehaviourType.Player:
                    actor.SetExecutiveOverride(true); // Player is always under executive override
                    break;
                case ActorBehaviourType.Fugue:
                case ActorBehaviourType.Mezzopiano:
                    actor.SetExecutiveOverride(false); // AI
                    break;
            }
        }

        // Get an actor by their iKey (actor key)
        public Character GetActorByKey(string key)
        {
            return Actors.Find(a => a.iKey == key);
        }
    }
}