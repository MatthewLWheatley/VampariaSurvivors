using System;
using Terraria.ID;
using Terraria.ModLoader;
using VampariaSurvivors.Content.Projectile;

namespace VampariaSurvivors.Content.Items
{
    public class WhipLvl1 : VSWeapon
    {
        public override string Texture => "VampariaSurvivors/Content/Items/Whip";
        public override string WeaponName => "Whip";
        public override string WeaponDescription => "Horizontal slashing attack";
        public override int ControllerProjectileType => ModContent.ProjectileType<WhipControllerProjectile>();
        public override string WeaponIdentifier => "Whip";

        public override int BaseDamage { get; set; } = 20;
        public override int BaseAmount { get; set; } = 1;
        public override float BaseArea { get; set; } = 1.0f;
        public override int BaseCooldown { get; set; } = 60;
        public override int BaseProjectileInterval { get; set; } = 6;

        protected override int GetAmountBonus()
        {
            return Level switch
            {
                >= 8 => 2,
                >= 2 => 1,
                _ => 0
            };
        }

        protected override float GetAreaScale()
        {
            float scale = 1.0f;
            if (Level >= 4) scale += 0.1f;
            if (Level >= 6) scale += 0.1f;
            return scale;
        }

        protected override float GetDamageScale()
        {
            float baseScale = 1.0f;

            int flatBonus = 0;
            if (Level >= 3) flatBonus += 10;
            if (Level >= 4) flatBonus += 10;
            if (Level >= 5) flatBonus += 10;
            if (Level >= 6) flatBonus += 10;
            if (Level >= 7) flatBonus += 10;
            if (Level >= 8) flatBonus += 10;

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
                1 => ModContent.ItemType<WhipLvl1>(),
                2 => ModContent.ItemType<WhipLvl2>(),
                3 => ModContent.ItemType<WhipLvl3>(),
                4 => ModContent.ItemType<WhipLvl4>(),
                5 => ModContent.ItemType<WhipLvl5>(),
                6 => ModContent.ItemType<WhipLvl6>(),
                7 => ModContent.ItemType<WhipLvl7>(),
                8 => ModContent.ItemType<WhipLvl8>(),
                _ => level > 8 ? ModContent.ItemType<WhipLvl8>() : -1
            };
        }
    }

    public class WhipLvl2 : WhipLvl1
    {
        public override int Level { get; set; } = 2;
    }

    public class WhipLvl3 : WhipLvl1
    {
        public override int Level { get; set; } = 3;
    }

    public class WhipLvl4 : WhipLvl1
    {
        public override int Level { get; set; } = 4;
    }

    public class WhipLvl5 : WhipLvl1
    {
        public override int Level { get; set; } = 5;
    }

    public class WhipLvl6 : WhipLvl1
    {
        public override int Level { get; set; } = 6;
    }

    public class WhipLvl7 : WhipLvl1
    {
        public override int Level { get; set; } = 7;
    }

    public class WhipLvl8 : WhipLvl1
    {
        public override int Level { get; set; } = 8;
    }
}