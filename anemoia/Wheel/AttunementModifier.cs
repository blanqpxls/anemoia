namespace Overture.Leveling
{
    public class AttunementModifier
    {
        public int Level { get; private set; }
        public float DamageMultiplier { get; private set; }
        public float CooldownReduction { get; private set; }
        public float HealthBonus { get; private set; }

        public AttunementModifier(int initialLevel = 1)
        {
            Level = initialLevel;
            UpdateModifiers();
        }

        private void UpdateModifiers()
        {
            DamageMultiplier = 1.0f + Level * 0.05f;       // 5% bonus damage per level.
            CooldownReduction = Level * 0.01f;             // 1% cooldown reduction per level.
            HealthBonus = Level * 10f;                     // +10 health per level.
        }

        public void LevelUp()
        {
            Level++;
            UpdateModifiers();
        }
    }
}
