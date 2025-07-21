using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VampariaSurvivors.Content.Projectile;

namespace VampariaSurvivors.Content.Items
{
    public class BoneLvl1 : VSWeapon
    {
        public override string Texture => "VampariaSurvivors/Content/Items/Bone";
        public override string WeaponName => "Bone";
        public override string WeaponDescription => "Toggleable Personal Sentry";
        public override int ControllerProjectileType => ModContent.ProjectileType<BoneControllerProjectile>();
        public override string WeaponIdentifier => "Bone";

        // Base stats for Bone
        public override int BaseDamage { get; set; } = 20;
        public override int BaseAmount { get; set; } = 1;
        public override int BaseDuration { get; set; } = 120;
        public override float BaseSpeed { get; set; } = 8f;
        public override int BaseCooldown { get; set; } = 180;

        protected override int GetAmountBonus()
        {
            int bonus = 0;
            if (Level >= 3) bonus += 1;
            if (Level >= 5) bonus += 1;
            return bonus;
        }

        protected override int GetDurationBonus()
        {
            int bonus = 0;
            if (Level >= 2) bonus += 16;
            if (Level >= 6) bonus += 16;
            if (Level >= 8) bonus += 16;
            return bonus;
        }

        protected override float GetSpeedScale()
        {
            float scale = 1.0f;
            if (Level >= 4) scale *= 1.5f;
            if (Level >= 8) scale *= 1.5f;
            return scale;
        }

        protected override float GetDamageScale()
        {
            float baseScale = base.GetDamageScale();

            int flatBonus = 0;
            if (Level >= 3) flatBonus += 20;
            if (Level >= 5) flatBonus += 20;
            if (Level >= 7) flatBonus += 20;

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
                1 => ModContent.ItemType<BoneLvl1>(),
                2 => ModContent.ItemType<BoneLvl2>(),
                3 => ModContent.ItemType<BoneLvl3>(),
                4 => ModContent.ItemType<BoneLvl4>(),
                5 => ModContent.ItemType<BoneLvl5>(),
                6 => ModContent.ItemType<BoneLvl6>(),
                7 => ModContent.ItemType<BoneLvl7>(),
                8 => ModContent.ItemType<BoneLvl8>(),
                _ => level > 8 ? ModContent.ItemType<BoneLvl8>() : -1
            };
        }
    }

    public class BoneLvl2 : BoneLvl1
    {
        public override int Level { get; set; } = 2;
    }

    public class BoneLvl3 : BoneLvl1
    {
        public override int Level { get; set; } = 3;
    }

    public class BoneLvl4 : BoneLvl1
    {
        public override int Level { get; set; } = 4;
    }

    public class BoneLvl5 : BoneLvl1
    {
        public override int Level { get; set; } = 5;
    }

    public class BoneLvl6 : BoneLvl1
    {
        public override int Level { get; set; } = 6;
    }

    public class BoneLvl7 : BoneLvl1
    {
        public override int Level { get; set; } = 7;
    }

    public class BoneLvl8 : BoneLvl1
    {
        public override int Level { get; set; } = 8;
    }
}