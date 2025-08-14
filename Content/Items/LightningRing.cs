using System;
using Terraria.ID;
using Terraria.ModLoader;
using VampariaSurvivors.Content.Projectile;

namespace VampariaSurvivors.Content.Items
{
    public class LightningRingLvl1 : VSWeapon
    {
        public override string Texture => "VampariaSurvivors/Content/Items/LightningRing";
        public override string WeaponName => "Lightning Ring";
        public override string WeaponDescription => "Strikes at random enemies";
        public override int ControllerProjectileType => ModContent.ProjectileType<LightningRingControllerProjectile>();
        public override string WeaponIdentifier => "LightningRing";

        // Base stats for Lightning Ring
        public override int BaseDamage { get; set; } = 25;
        public override int BaseAmount { get; set; } = 1; // Number of lightning strikes per cast
        public override float BaseArea { get; set; } = 1.0f; // Ground hitbox size
        public override int BaseCooldown { get; set; } = 120; // 2 seconds
        public override int BaseProjectileInterval { get; set; } = 10; // 0.17s between strikes

        protected override float GetDamageScale()
        {
            float baseScale = 1.0f;

            // Specific damage increases per level
            int flatBonus = 0;
            if (Level >= 3) flatBonus += 20; // Level 3: +20 damage
            if (Level >= 5) flatBonus += 50; // Level 5: +50 damage  
            if (Level >= 7) flatBonus += 50; // Level 7: +50 damage

            if (flatBonus > 0)
            {
                baseScale += (float)flatBonus / BaseDamage;
            }

            return baseScale;
        }

        protected override float GetAreaScale()
        {
            float scale = 1.0f;
            if (Level >= 3) scale *= 2.0f; // Level 3: Area up by 100%
            if (Level >= 5) scale *= 2.0f; // Level 5: Area up by 100%
            if (Level >= 7) scale *= 2.0f; // Level 7: Area up by 100%
            return scale;
        }

        protected override int GetAmountBonus()
        {
            int bonus = 0;
            if (Level >= 2) bonus += 1; // Level 2: +1 projectile
            if (Level >= 4) bonus += 1; // Level 4: +1 projectile  
            if (Level >= 6) bonus += 1; // Level 6: +1 projectile
            if (Level >= 8) bonus += 1; // Level 8: +1 projectile
            return bonus;
        }

        protected override int GetWeaponTypeAtLevel(int level)
        {
            return level switch
            {
                1 => ModContent.ItemType<LightningRingLvl1>(),
                2 => ModContent.ItemType<LightningRingLvl2>(),
                3 => ModContent.ItemType<LightningRingLvl3>(),
                4 => ModContent.ItemType<LightningRingLvl4>(),
                5 => ModContent.ItemType<LightningRingLvl5>(),
                6 => ModContent.ItemType<LightningRingLvl6>(),
                7 => ModContent.ItemType<LightningRingLvl7>(),
                8 => ModContent.ItemType<LightningRingLvl8>(),
                _ => level > 8 ? ModContent.ItemType<LightningRingLvl8>() : -1
            };
        }
    }

    // Level variants
    public class LightningRingLvl2 : LightningRingLvl1
    {
        public override int Level { get; set; } = 2;
    }

    public class LightningRingLvl3 : LightningRingLvl1
    {
        public override int Level { get; set; } = 3;
    }

    public class LightningRingLvl4 : LightningRingLvl1
    {
        public override int Level { get; set; } = 4;
    }

    public class LightningRingLvl5 : LightningRingLvl1
    {
        public override int Level { get; set; } = 5;
    }

    public class LightningRingLvl6 : LightningRingLvl1
    {
        public override int Level { get; set; } = 6;
    }

    public class LightningRingLvl7 : LightningRingLvl1
    {
        public override int Level { get; set; } = 7;
    }

    public class LightningRingLvl8 : LightningRingLvl1
    {
        public override int Level { get; set; } = 8;
    }
}