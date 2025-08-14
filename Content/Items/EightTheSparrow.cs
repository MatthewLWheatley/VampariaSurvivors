using System;
using Terraria.ID;
using Terraria.ModLoader;
using VampariaSurvivors.Content.Projectile;

namespace VampariaSurvivors.Content.Items
{
    public class EightTheSparrowLvl1 : VSWeapon
    {
        public override string Texture => "VampariaSurvivors/Content/Items/EightTheSparrow";
        public override string WeaponName => "Eight The Sparrow";
        public override string WeaponDescription => GetLevelDescription();
        public override int ControllerProjectileType => ModContent.ProjectileType<EightTheSparrowControllerProjectile>();
        public override string WeaponIdentifier => "EightTheSparrow";

        public override int BaseDamage { get; set; } = 10;
        public override int BaseAmount { get; set; } = 1;
        public override int BasePierce { get; set; } = 1;
        public override int BaseCooldown { get; set; } = 45; // Fast firing
        public override float BaseSpeed { get; set; } = 1.0f;

        private string GetLevelDescription()
        {
            return Level switch
            {
                1 => "Fires quickly toward four screen corners.",
                2 => "Fires 1 more projectile.",
                3 => "Passes through 2 more enemies.",
                4 => "Fires 1 more projectile.",
                5 => "Base Damage up by 5.",
                6 => "Passes through 2 more enemies.",
                7 => "Base Damage up by 5. Base Speed up by 50%.",
                8 => "Passes through 2 more enemies.",
                _ => "Fires quickly toward four screen corners."
            };
        }

        protected override int GetAmountBonus()
        {
            return Level switch
            {
                >= 4 => 2, // Level 4 and 8 each add 1
                >= 2 => 1, // Level 2 adds 1
                _ => 0
            };
        }

        protected override int GetPierceBonus()
        {
            return Level switch
            {
                >= 8 => 6, // Level 3, 6, 8 each add 2 pierce
                >= 6 => 4, // Level 3, 6 each add 2 pierce  
                >= 3 => 2, // Level 3 adds 2 pierce
                _ => 0
            };
        }

        protected override float GetDamageScale()
        {
            float baseScale = base.GetDamageScale();
            
            // Level 5 and 7 each add +5 base damage
            if (Level >= 7) baseScale += 5.0f / BaseDamage; // +5 from level 7
            if (Level >= 5) baseScale += 5.0f / BaseDamage; // +5 from level 5

            return baseScale;
        }

        protected override float GetSpeedScale()
        {
            // Level 7 adds +50% speed (1.0 base + 0.5 bonus = 1.5x speed)
            return Level >= 7 ? 1.5f : 1.0f;
        }

        protected override int GetWeaponTypeAtLevel(int level)
        {
            return level switch
            {
                1 => ModContent.ItemType<EightTheSparrowLvl1>(),
                2 => ModContent.ItemType<EightTheSparrowLvl2>(),
                3 => ModContent.ItemType<EightTheSparrowLvl3>(),
                4 => ModContent.ItemType<EightTheSparrowLvl4>(),
                5 => ModContent.ItemType<EightTheSparrowLvl5>(),
                6 => ModContent.ItemType<EightTheSparrowLvl6>(),
                7 => ModContent.ItemType<EightTheSparrowLvl7>(),
                8 => ModContent.ItemType<EightTheSparrowLvl8>(),
                _ => level > 8 ? ModContent.ItemType<EightTheSparrowLvl8>() : -1
            };
        }
    }

    public class EightTheSparrowLvl2 : EightTheSparrowLvl1
    {
        public override int Level { get; set; } = 2;
    }

    public class EightTheSparrowLvl3 : EightTheSparrowLvl1
    {
        public override int Level { get; set; } = 3;
    }

    public class EightTheSparrowLvl4 : EightTheSparrowLvl1
    {
        public override int Level { get; set; } = 4;
    }

    public class EightTheSparrowLvl5 : EightTheSparrowLvl1
    {
        public override int Level { get; set; } = 5;
    }

    public class EightTheSparrowLvl6 : EightTheSparrowLvl1
    {
        public override int Level { get; set; } = 6;
    }

    public class EightTheSparrowLvl7 : EightTheSparrowLvl1
    {
        public override int Level { get; set; } = 7;
    }

    public class EightTheSparrowLvl8 : EightTheSparrowLvl1
    {
        public override int Level { get; set; } = 8;
    }
}