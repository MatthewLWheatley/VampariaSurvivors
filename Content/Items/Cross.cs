using System;
using Terraria.ID;
using Terraria.ModLoader;
using VampariaSurvivors.Content.Projectile;

namespace VampariaSurvivors.Content.Items
{
    public class CrossLvl1 : VSWeapon
    {
        public override string Texture => "VampariaSurvivors/Content/Items/Cross";
        public override string WeaponName => "Cross";
        public override string WeaponDescription => "Spinning cross that aims at enemies and boomerangs back";
        public override int ControllerProjectileType => ModContent.ProjectileType<CrossControllerProjectile>();
        public override string WeaponIdentifier => "Cross";

        public override int BaseDamage { get; set; } = 10;
        public override int BaseAmount { get; set; } = 1;
        public override float BaseArea { get; set; } = 1.0f;
        public override float BaseSpeed { get; set; } = 1.0f;
        public override int BaseCooldown { get; set; } = 120;
        public override int BaseProjectileInterval { get; set; } = 6;
        public override int BasePierce { get; set; } = -1;
        public override bool BaseBlockedByWalls { get; set; } = false;

        protected override float GetDamageScale()
        {
            float baseScale = 1.0f;

            int flatBonus = 0;
            if (Level >= 2) flatBonus += 20;
            if (Level >= 5) flatBonus += 20;
            if (Level >= 8) flatBonus += 20;

            if (flatBonus > 0)
            {
                baseScale += (float)flatBonus / BaseDamage;
            }

            return baseScale;
        }

        protected override float GetAreaScale()
        {
            float scale = 1.0f;
            if (Level >= 3) scale *= 1.1f;
            if (Level >= 6) scale *= 1.1f;
            return scale;
        }

        protected override float GetSpeedScale()
        {
            float scale = 1.0f;
            if (Level >= 3) scale *= 1.25f;
            if (Level >= 6) scale *= 1.25f;
            return scale;
        }

        protected override int GetAmountBonus()
        {
            int bonus = 0;
            if (Level >= 4) bonus += 1;
            if (Level >= 7) bonus += 1;
            return bonus;
        }

        protected override int GetWeaponTypeAtLevel(int level)
        {
            return level switch
            {
                1 => ModContent.ItemType<CrossLvl1>(),
                2 => ModContent.ItemType<CrossLvl2>(),
                3 => ModContent.ItemType<CrossLvl3>(),
                4 => ModContent.ItemType<CrossLvl4>(),
                5 => ModContent.ItemType<CrossLvl5>(),
                6 => ModContent.ItemType<CrossLvl6>(),
                7 => ModContent.ItemType<CrossLvl7>(),
                8 => ModContent.ItemType<CrossLvl8>(),
                _ => level > 8 ? ModContent.ItemType<CrossLvl8>() : -1
            };
        }
    }

    public class CrossLvl2 : CrossLvl1
    {
        public override int Level { get; set; } = 2;
    }

    public class CrossLvl3 : CrossLvl1
    {
        public override int Level { get; set; } = 3;
    }

    public class CrossLvl4 : CrossLvl1
    {
        public override int Level { get; set; } = 4;
    }

    public class CrossLvl5 : CrossLvl1
    {
        public override int Level { get; set; } = 5;
    }

    public class CrossLvl6 : CrossLvl1
    {
        public override int Level { get; set; } = 6;
    }

    public class CrossLvl7 : CrossLvl1
    {
        public override int Level { get; set; } = 7;
    }

    public class CrossLvl8 : CrossLvl1
    {
        public override int Level { get; set; } = 8;
    }
}