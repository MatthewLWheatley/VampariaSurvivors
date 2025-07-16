using System;
using Terraria.ID;
using Terraria.ModLoader;
using VampariaSurvivors.Content.Projectile;

namespace VampariaSurvivors.Content.Items
{
    public class KnifeLvl1 : VSWeapon
    {
        public override string Texture => "VampariaSurvivors/Content/Items/Knife";
        public override string WeaponName => "Knife";
        public override string WeaponDescription => "Toggleable Personal Sentry";
        public override int ControllerProjectileType => ModContent.ProjectileType<KnifeControllerProjectile>();
        public override string WeaponIdentifier => "Knife";

        public override int BaseDamage { get; set; } = 13;
        public override int BaseAmount { get; set; } = 1;
        public override int BasePierce { get; set; } = 1;
        public override int BaseCooldown { get; set; } = 60;
        public override int BaseProjectileInterval { get; set; } = 6;

        protected override int GetAmountBonus()
        {
            return Level switch
            {
                >= 7 => 5,
                >= 6 => 4,
                >= 4 => 3,
                >= 3 => 2,
                >= 2 => 1,
                _ => 0
            };
        }

        protected override int GetPierceBonus()
        {
            return Level switch
            {
                >= 8 => 2,
                >= 5 => 1,
                _ => 0
            };
        }

        protected override int GetIntervalReduction()
        {
            int reduction = 0;
            if (Level >= 4) reduction += 2;
            if (Level >= 6) reduction += 1;
            if (Level >= 8) reduction += 1;
            return reduction;
        }

        protected override float GetDamageScale()
        {
            float baseScale = base.GetDamageScale();

            int flatBonus = 0;
            if (Level >= 3) flatBonus += 10;
            if (Level >= 7) flatBonus += 10;

            if (flatBonus > 0)
            {
                baseScale += (float)flatBonus / BaseDamage;
            }

            return baseScale;
        }

        protected override int GetCooldownReduction()
        {
            return Level >= 3 ? 12 : 0;
        }

        protected override int GetWeaponTypeAtLevel(int level)
        {
            return level switch
            {
                1 => ModContent.ItemType<KnifeLvl1>(),
                2 => ModContent.ItemType<KnifeLvl2>(),
                3 => ModContent.ItemType<KnifeLvl3>(),
                4 => ModContent.ItemType<KnifeLvl4>(),
                5 => ModContent.ItemType<KnifeLvl5>(),
                6 => ModContent.ItemType<KnifeLvl6>(),
                7 => ModContent.ItemType<KnifeLvl7>(),
                8 => ModContent.ItemType<KnifeLvl8>(),
                _ => level > 8 ? ModContent.ItemType<KnifeLvl8>() : -1
            };
        }
    }

    public class KnifeLvl2 : KnifeLvl1
    {
        public override int Level { get; set; } = 2;
    }

    public class KnifeLvl3 : KnifeLvl1
    {
        public override int Level { get; set; } = 3;
    }

    public class KnifeLvl4 : KnifeLvl1
    {
        public override int Level { get; set; } = 4;
    }

    public class KnifeLvl5 : KnifeLvl1
    {
        public override int Level { get; set; } = 5;
    }

    public class KnifeLvl6 : KnifeLvl1
    {
        public override int Level { get; set; } = 6;
    }

    public class KnifeLvl7 : KnifeLvl1
    {
        public override int Level { get; set; } = 7;
    }

    public class KnifeLvl8 : KnifeLvl1
    {
        public override int Level { get; set; } = 8;
    }
}