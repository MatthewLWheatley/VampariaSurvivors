using System;
using Terraria.ID;
using Terraria.ModLoader;
using VampariaSurvivors.Content.Projectile;

namespace VampariaSurvivors.Content.Items
{
    public class CherryBombLvl1 : VSWeapon
    {
        public override string Texture => "VampariaSurvivors/Content/Items/CherryBomb";
        public override string WeaponName => "Cherry Bomb";
        public override string WeaponDescription => "Throws bombs that bounce and explode when they stop moving";
        public override int ControllerProjectileType => ModContent.ProjectileType<CherryBombControllerProjectile>();
        public override string WeaponIdentifier => "CherryBomb";

        // Base stats for Cherry Bomb
        public override int BaseDamage { get; set; } = 35;
        public override int BaseAmount { get; set; } = 1;
        public override float BaseArea { get; set; } = 1.0f;
        public override float BaseSpeed { get; set; } = 1.0f;
        public override int BaseCooldown { get; set; } = 180; // 3 seconds
        public override int BaseProjectileInterval { get; set; } = 12; // 0.2 seconds between bombs

        protected override float GetDamageScale()
        {
            float baseScale = 1.0f;

            // Flat damage bonuses at specific levels
            int flatBonus = 0;
            if (Level >= 2) flatBonus += 15; // Level 2: +15 damage
            if (Level >= 4) flatBonus += 20; // Level 4: +20 damage
            if (Level >= 6) flatBonus += 25; // Level 6: +25 damage
            if (Level >= 8) flatBonus += 30; // Level 8: +30 damage

            if (flatBonus > 0)
            {
                baseScale += (float)flatBonus / BaseDamage;
            }

            return baseScale;
        }

        protected override float GetAreaScale()
        {
            float scale = 1.0f;
            if (Level >= 3) scale *= 1.2f; // Level 3: +20% area
            if (Level >= 5) scale *= 1.2f; // Level 5: +20% area
            if (Level >= 7) scale *= 1.15f; // Level 7: +15% area
            return scale;
        }

        protected override int GetAmountBonus()
        {
            int bonus = 0;
            if (Level >= 3) bonus += 1; // Level 3: +1 bomb
            if (Level >= 6) bonus += 1; // Level 6: +1 bomb (total 3)
            return bonus;
        }


        protected override int GetCooldownReduction()
        {
            int reduction = 0;
            if (Level >= 2) reduction += 30; // Level 2: -0.5s cooldown
            if (Level >= 5) reduction += 30; // Level 5: -0.5s cooldown
            if (Level >= 8) reduction += 30; // Level 8: -0.5s cooldown
            return reduction;
        }

        protected override float GetSpeedScale()
        {
            float scale = 1.0f;
            if (Level >= 4) scale *= 1.25f; // Level 4: +25% projectile speed
            if (Level >= 8) scale *= 1.25f; // Level 8: +25% projectile speed
            return scale;
        }

        protected override int GetWeaponTypeAtLevel(int level)
        {
            return level switch
            {
                1 => ModContent.ItemType<CherryBombLvl1>(),
                2 => ModContent.ItemType<CherryBombLvl2>(),
                3 => ModContent.ItemType<CherryBombLvl3>(),
                4 => ModContent.ItemType<CherryBombLvl4>(),
                5 => ModContent.ItemType<CherryBombLvl5>(),
                6 => ModContent.ItemType<CherryBombLvl6>(),
                7 => ModContent.ItemType<CherryBombLvl7>(),
                8 => ModContent.ItemType<CherryBombLvl8>(),
                _ => level > 8 ? ModContent.ItemType<CherryBombLvl8>() : -1
            };
        }
    }

    // Level variants
    public class CherryBombLvl2 : CherryBombLvl1
    {
        public override int Level { get; set; } = 2;
    }

    public class CherryBombLvl3 : CherryBombLvl1
    {
        public override int Level { get; set; } = 3;
    }

    public class CherryBombLvl4 : CherryBombLvl1
    {
        public override int Level { get; set; } = 4;
    }

    public class CherryBombLvl5 : CherryBombLvl1
    {
        public override int Level { get; set; } = 5;
    }

    public class CherryBombLvl6 : CherryBombLvl1
    {
        public override int Level { get; set; } = 6;
    }

    public class CherryBombLvl7 : CherryBombLvl1
    {
        public override int Level { get; set; } = 7;
    }

    public class CherryBombLvl8 : CherryBombLvl1
    {
        public override int Level { get; set; } = 8;
    }
}