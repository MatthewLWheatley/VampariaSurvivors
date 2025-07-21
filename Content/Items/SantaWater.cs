using System;
using Terraria.ID;
using Terraria.ModLoader;
using VampariaSurvivors.Content.Projectile;

namespace VampariaSurvivors.Content.Items
{
    public class SantaWaterLvl1 : VSWeapon
    {
        public override string Texture => "VampariaSurvivors/Content/Items/SantaWater";
        public override string WeaponName => "Santa Water";
        public override string WeaponDescription => "Rains holy water bottles that create damaging puddles";
        public override int ControllerProjectileType => ModContent.ProjectileType<SantaWaterControllerProjectile>();
        public override string WeaponIdentifier => "SantaWater";

        public override int BaseDamage { get; set; } = 20;
        public override int BaseAmount { get; set; } = 1;
        public override float BaseArea { get; set; } = 1.0f;
        public override float BaseSpeed { get; set; } = 1.0f;
        public override int BaseDuration { get; set; } = 120; // 2.0 seconds
        public override int BaseCooldown { get; set; } = 270; // 4.5 seconds
        public override int BaseProjectileInterval { get; set; } = 18; // 0.3 seconds

        protected override float GetDamageScale()
        {
            float baseScale = 1.0f; // No base scaling

            // Flat damage bonuses at specific levels
            int flatBonus = 0;
            if (Level >= 3) flatBonus += 20; // Level 3: +20 damage
            if (Level >= 5) flatBonus += 20; // Level 5: +20 damage (total +40)
            if (Level >= 7) flatBonus += 10; // Level 7: +10 damage (total +50)
            if (Level >= 8) flatBonus += 5;  // Level 8: +5 damage (total +55)

            if (flatBonus > 0)
            {
                baseScale += (float)flatBonus / BaseDamage;
            }

            return baseScale;
        }

        protected override float GetAreaScale()
        {
            float scale = 1.0f;
            if (Level >= 2) scale *= 1.20f; // Level 2: +20% area
            if (Level >= 4) scale *= 1.20f; // Level 4: +20% area
            if (Level >= 6) scale *= 1.20f; // Level 6: +20% area
            if (Level >= 8) scale *= 1.20f; // Level 8: +20% area
            return scale;
        }

        protected override int GetAmountBonus()
        {
            int bonus = 0;
            if (Level >= 2) bonus += 1; // Level 2: +1 bottle
            if (Level >= 4) bonus += 1; // Level 4: +1 bottle (total 3)
            if (Level >= 6) bonus += 1; // Level 6: +1 bottle (total 4)
            return bonus;
        }

        protected override int GetDurationBonus()
        {
            int bonus = 0;
            if (Level >= 3) bonus += 30; // Level 3: +0.5 seconds
            if (Level >= 5) bonus += 18; // Level 5: +0.3 seconds (total +0.8s)
            if (Level >= 7) bonus += 18; // Level 7: +0.3 seconds (total +1.1s)
            return bonus;
        }

        protected override int GetWeaponTypeAtLevel(int level)
        {
            return level switch
            {
                1 => ModContent.ItemType<SantaWaterLvl1>(),
                2 => ModContent.ItemType<SantaWaterLvl2>(),
                3 => ModContent.ItemType<SantaWaterLvl3>(),
                4 => ModContent.ItemType<SantaWaterLvl4>(),
                5 => ModContent.ItemType<SantaWaterLvl5>(),
                6 => ModContent.ItemType<SantaWaterLvl6>(),
                7 => ModContent.ItemType<SantaWaterLvl7>(),
                8 => ModContent.ItemType<SantaWaterLvl8>(),
                _ => level > 8 ? ModContent.ItemType<SantaWaterLvl8>() : -1
            };
        }
    }

    public class SantaWaterLvl2 : SantaWaterLvl1
    {
        public override int Level { get; set; } = 2;
    }

    public class SantaWaterLvl3 : SantaWaterLvl1
    {
        public override int Level { get; set; } = 3;
    }

    public class SantaWaterLvl4 : SantaWaterLvl1
    {
        public override int Level { get; set; } = 4;
    }

    public class SantaWaterLvl5 : SantaWaterLvl1
    {
        public override int Level { get; set; } = 5;
    }

    public class SantaWaterLvl6 : SantaWaterLvl1
    {
        public override int Level { get; set; } = 6;
    }

    public class SantaWaterLvl7 : SantaWaterLvl1
    {
        public override int Level { get; set; } = 7;
    }

    public class SantaWaterLvl8 : SantaWaterLvl1
    {
        public override int Level { get; set; } = 8;
    }
}