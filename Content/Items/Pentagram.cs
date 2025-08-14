using System;
using Terraria.ID;
using Terraria.ModLoader;
using VampariaSurvivors.Content.Projectile;

namespace VampariaSurvivors.Content.Items
{
    public class PentagramLvl1 : VSWeapon
    {
        public override string Texture => "VampariaSurvivors/Content/Items/Pentagram";
        public override string WeaponName => "Pentagram";
        public override string WeaponDescription => GetLevelDescription();
        public override int ControllerProjectileType => ModContent.ProjectileType<PentagramControllerProjectile>();
        public override string WeaponIdentifier => "Pentagram";

        public override int BaseDamage { get; set; } = 660;
        public override int BaseCooldown { get; set; } = 3600; // 60 seconds base (60 ticks * 60 seconds)
        public override float BaseChance { get; set; } = 0.0f; // Chance not to erase items

        private string GetLevelDescription()
        {
            return Level switch
            {
                1 => "Erases everything in sight.",
                2 => "Cooldown reduced by 10 seconds.",
                3 => "25% chance not to erase items.",
                4 => "Cooldown reduced by 10 seconds.",
                5 => "45% chance not to erase items.",
                6 => "Cooldown reduced by 5 seconds.",
                7 => "65% chance not to erase items.",
                8 => "Cooldown reduced by 5 seconds.",
                _ => "Erases everything in sight."
            };
        }

        protected override float GetDamageScale()
        {
            // Damage increases by 100% each level: 660, 1320, 1980, 2640, etc.
            return (float)Level;
        }

        protected override int GetCooldownReduction()
        {
            // Convert seconds to ticks (60 ticks = 1 second)
            return Level switch
            {
                8 => 30 * 60, // 30 seconds = 1800 ticks
                7 => 25 * 60, // 25 seconds = 1500 ticks  
                6 => 25 * 60, // 25 seconds = 1500 ticks
                5 => 15 * 60, // 15 seconds = 900 ticks
                4 => 15 * 60, // 15 seconds = 900 ticks
                3 => 5 * 60,  // 5 seconds = 300 ticks
                2 => 5 * 60,  // 5 seconds = 300 ticks
                _ => 0        // 0 reduction
            };
        }

        protected override float GetChanceBonus()
        {
            return Level switch
            {
                >= 8 => 0.65f, // 65% chance not to erase
                >= 7 => 0.65f, // 65% chance not to erase
                >= 5 => 0.45f, // 45% chance not to erase
                >= 3 => 0.25f, // 25% chance not to erase
                _ => 0.0f
            };
        }

        protected override int GetWeaponTypeAtLevel(int level)
        {
            return level switch
            {
                1 => ModContent.ItemType<PentagramLvl1>(),
                2 => ModContent.ItemType<PentagramLvl2>(),
                3 => ModContent.ItemType<PentagramLvl3>(),
                4 => ModContent.ItemType<PentagramLvl4>(),
                5 => ModContent.ItemType<PentagramLvl5>(),
                6 => ModContent.ItemType<PentagramLvl6>(),
                7 => ModContent.ItemType<PentagramLvl7>(),
                8 => ModContent.ItemType<PentagramLvl8>(),
                _ => level > 8 ? ModContent.ItemType<PentagramLvl8>() : -1
            };
        }
    }

    public class PentagramLvl2 : PentagramLvl1
    {
        public override int Level { get; set; } = 2;
    }

    public class PentagramLvl3 : PentagramLvl1
    {
        public override int Level { get; set; } = 3;
    }

    public class PentagramLvl4 : PentagramLvl1
    {
        public override int Level { get; set; } = 4;
    }

    public class PentagramLvl5 : PentagramLvl1
    {
        public override int Level { get; set; } = 5;
    }

    public class PentagramLvl6 : PentagramLvl1
    {
        public override int Level { get; set; } = 6;
    }

    public class PentagramLvl7 : PentagramLvl1
    {
        public override int Level { get; set; } = 7;
    }

    public class PentagramLvl8 : PentagramLvl1
    {
        public override int Level { get; set; } = 8;
    }
}