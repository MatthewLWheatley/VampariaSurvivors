using System;
using Terraria.ID;
using Terraria.ModLoader;
using VampariaSurvivors.Content.Projectile;

namespace VampariaSurvivors.Content.Items
{
    public class MagicWandLvl1 : VSWeapon
    {
        public override string Texture => "VampariaSurvivors/Content/Items/MagicWand";
        public override string WeaponName => "Magic Wand";
        public override string WeaponDescription => "Toggleable Personal Sentry";
        public override int ControllerProjectileType => ModContent.ProjectileType<MagicWandControllerProjectile>();
        public override string WeaponIdentifier => "MagicWand";

        public override int BaseDamage { get; set; } = 20;
        public override int BaseAmount { get; set; } = 2;
        public override int BaseCooldown { get; set; } = 60;
        public override int BasePierce { get; set; } = 1;

        protected override int GetAmountBonus()
        {
            return Level switch
            {
                >= 6 => 2,
                >= 4 => 1,
                >= 2 => 0,
                _ => 0
            };
        }

        protected override int GetCooldownReduction()
        {
            return Level >= 3 ? 12 : 0;
        }

        protected override int GetPierceBonus()
        {
            return Level >= 7 ? 1 : 0;
        }

        protected override float GetDamageScale()
        {
            float baseScale = base.GetDamageScale();

            if (Level >= 8) baseScale += 20.0f / BaseDamage;
            if (Level >= 5) baseScale += 20.0f / BaseDamage;

            return baseScale;
        }

        protected override int GetWeaponTypeAtLevel(int level)
        {
            return level switch
            {
                1 => ModContent.ItemType<MagicWandLvl1>(),
                2 => ModContent.ItemType<MagicWandLvl2>(),
                3 => ModContent.ItemType<MagicWandLvl3>(),
                4 => ModContent.ItemType<MagicWandLvl4>(),
                5 => ModContent.ItemType<MagicWandLvl5>(),
                6 => ModContent.ItemType<MagicWandLvl6>(),
                7 => ModContent.ItemType<MagicWandLvl7>(),
                8 => ModContent.ItemType<MagicWandLvl8>(),
                _ => level > 8 ? ModContent.ItemType<MagicWandLvl8>() : -1
            };
        }
    }

    public class MagicWandLvl2 : MagicWandLvl1
    {
        public override int Level { get; set; } = 2;
    }

    public class MagicWandLvl3 : MagicWandLvl1
    {
        public override int Level { get; set; } = 3;
    }

    public class MagicWandLvl4 : MagicWandLvl1
    {
        public override int Level { get; set; } = 4;
    }

    public class MagicWandLvl5 : MagicWandLvl1
    {
        public override int Level { get; set; } = 5;
    }

    public class MagicWandLvl6 : MagicWandLvl1
    {
        public override int Level { get; set; } = 6;
    }

    public class MagicWandLvl7 : MagicWandLvl1
    {
        public override int Level { get; set; } = 7;
    }

    public class MagicWandLvl8 : MagicWandLvl1
    {
        public override int Level { get; set; } = 8;
    }
}