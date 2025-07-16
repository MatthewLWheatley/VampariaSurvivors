using System;
using Terraria.ID;
using Terraria.ModLoader;
using VampariaSurvivors.Content.Projectile;

namespace VampariaSurvivors.Content.Items
{
    public class AxeLvl1 : VSWeapon
    {
        public override string Texture => "VampariaSurvivors/Content/Items/Axe";
        public override string WeaponName => "Axe";
        public override string WeaponDescription => "Throws axes above that arc downward";
        public override int ControllerProjectileType => ModContent.ProjectileType<AxeControllerProjectile>();
        public override string WeaponIdentifier => "Axe";

        // Base stats for Axe
        public override int BaseDamage { get; set; } = 20;
        public override int BaseAmount { get; set; } = 2; // projectileCount
        public override int BasePierce { get; set; } = 3; // projectilePenetration
        public override int BaseCooldown { get; set; } = 60;

        // Level progression overrides
        protected override int GetAmountBonus()
        {
            return Level switch
            {
                >= 5 => 2, // Level 5: 4 projectiles (2 base + 2 bonus)
                >= 2 => 1, // Level 2: 3 projectiles (2 base + 1 bonus)
                _ => 0
            };
        }

        protected override int GetPierceBonus()
        {
            int bonus = 0;
            if (Level >= 4) bonus += 2; // Level 4: +2 pierce (5 total)
            if (Level >= 7) bonus += 2; // Level 7: +2 more pierce (7 total)
            return bonus;
        }

        protected override float GetDamageScale()
        {
            float baseScale = base.GetDamageScale(); // 1.0 + (0.25 * (Level - 1))

            // Additional flat damage bonuses at specific levels
            int flatBonus = 0;
            if (Level >= 3) flatBonus += 20; // Level 3: +20 damage
            if (Level >= 6) flatBonus += 20; // Level 6: +20 more damage
            if (Level >= 8) flatBonus += 20; // Level 8: +20 more damage

            // Convert flat bonus to scale multiplier
            if (flatBonus > 0)
            {
                baseScale += (float)flatBonus / BaseDamage;
            }

            return baseScale;
        }

        protected override int GetWeaponTypeAtLevel(int level)
        {
            return level switch
            {
                1 => ModContent.ItemType<AxeLvl1>(),
                2 => ModContent.ItemType<AxeLvl2>(),
                3 => ModContent.ItemType<AxeLvl3>(),
                4 => ModContent.ItemType<AxeLvl4>(),
                5 => ModContent.ItemType<AxeLvl5>(),
                6 => ModContent.ItemType<AxeLvl6>(),
                7 => ModContent.ItemType<AxeLvl7>(),
                8 => ModContent.ItemType<AxeLvl8>(),
                _ => level > 8 ? ModContent.ItemType<AxeLvl8>() : -1
            };
        }
    }

    // Level variants - now much simpler!
    public class AxeLvl2 : AxeLvl1
    {
        public override int Level { get; set; } = 2;
    }

    public class AxeLvl3 : AxeLvl1
    {
        public override int Level { get; set; } = 3;
    }

    public class AxeLvl4 : AxeLvl1
    {
        public override int Level { get; set; } = 4;
    }

    public class AxeLvl5 : AxeLvl1
    {
        public override int Level { get; set; } = 5;
    }

    public class AxeLvl6 : AxeLvl1
    {
        public override int Level { get; set; } = 6;
    }

    public class AxeLvl7 : AxeLvl1
    {
        public override int Level { get; set; } = 7;
    }

    public class AxeLvl8 : AxeLvl1
    {
        public override int Level { get; set; } = 8;
    }
}