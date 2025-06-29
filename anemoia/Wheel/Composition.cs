using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParadisisNostalga.Wheel
{
    public class Composition
    {
        public enum CompositionType
        {


            Emilia,


            Luca,

            KarlvanderWeiss,

            Lullaby,

            Cassidy,

            Bella,

            Willow,

            Demon,

            Memory,

            Morgan,


            Maia,

            Sean,

            Sienna,


            Gideon,
         

        
        }

        // Inventory data structure
        public class InventoryData
        {
            public List<string> Items { get; set; } = new(); // Item IDs or names
            public Dictionary<string, int> Quantities { get; set; } = new(); // ItemID -> count
            // Add more as needed (equipment, currency, etc.)
        }

        // Composition data structure
        public class CompositionData
        {
            public string Name { get; set; }
            public float BaseDamage { get; set; }
            public float BaseHealth { get; set; }
            public float BaseSpeed { get; set; }
            public string AppearanceAsset { get; set; }
            public string LastGameState { get; set; } // e.g. serialized state or state id
            public int SaveSlot { get; set; } // which save file
            public InventoryData Inventory { get; set; } = new InventoryData();
        }

        // Static dictionary of base compositions
        public static readonly Dictionary<CompositionType, CompositionData> BaseCompositions = new()
        {
            { CompositionType.Emilia, new CompositionData {
                Name = "Emilia",
                BaseDamage = 10f,
                BaseHealth = 500f,
                BaseSpeed = 300f,
                AppearanceAsset = "res://assets/characters/emilia.png",
                LastGameState = null,
                SaveSlot = 0,
                Inventory = new InventoryData() // This will be loaded/updated from save data
            }},
            // Add other compositions here as needed
        };

        // Get base stats for a composition
        public static CompositionData GetBaseComposition(CompositionType type)
        {
            if (BaseCompositions.TryGetValue(type, out var data))
                return data;
            return null;
        }

        // Animation data for each composition (e.g. frame counts, animation names, etc.)
        public class AnimationProfile
        {
            public string IdleAnimation { get; set; }
            public string RunAnimation { get; set; }
            public string AttackAnimation { get; set; }
            public int IdleFrames { get; set; }
            public int RunFrames { get; set; }
            public int AttackFrames { get; set; }
            // Add more as needed (jump, death, etc.)
        }

        // Map each composition to its animation profile
        public static readonly Dictionary<CompositionType, AnimationProfile> AnimationProfiles = new()
        {
            { CompositionType.Emilia, new AnimationProfile {
                IdleAnimation = "EmiliaIdle",
                RunAnimation = "EmiliaRun",
                AttackAnimation = "EmiliaAttack",
                IdleFrames = 6,
                RunFrames = 8,
                AttackFrames = 5
            }}
            // Add more as needed
        };

        // Get animation profile for a composition
        public static AnimationProfile GetAnimationProfile(CompositionType type)
        {
            if (AnimationProfiles.TryGetValue(type, out var profile))
                return profile;
            return null;
        }
    }
}