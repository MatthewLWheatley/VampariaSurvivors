using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VampariaSurvivors.Content.Projectile;

namespace VampariaSurvivors.Content.Items
{
    public class GarlicLvl1 : VSWeapon
    {
        public override string Texture => "VampariaSurvivors/Content/Items/Garlic";
        public override string WeaponName => "Garlic";
        public override string WeaponDescription => "Toggleable protective aura";
        public override int ControllerProjectileType => ModContent.ProjectileType<GarlicAuraProjectile>();
        public override string WeaponIdentifier => "Garlic";

        public override int BaseDamage { get; set; } = 20;
        public override float BaseArea { get; set; } = 3.5f;
        public override float BaseKnockback { get; set; } = 0.2f;
        public override int BaseProjectileInterval { get; set; } = 78;

        protected override float GetAreaScale()
        {
            return Level switch
            {
                >= 8 => 2.0f,
                >= 6 => 1.8f,
                >= 4 => 1.6f,
                >= 2 => 1.4f,
                _ => 1.0f
            };
        }

        protected override int GetIntervalReduction()
        {
            int reduction = 0;
            if (Level >= 3) reduction += 6;
            if (Level >= 5) reduction += 6;
            if (Level >= 7) reduction += 6;
            return reduction;
        }

        protected override int GetWeaponTypeAtLevel(int level)
        {
            return level switch
            {
                1 => ModContent.ItemType<GarlicLvl1>(),
                2 => ModContent.ItemType<GarlicLvl2>(),
                3 => ModContent.ItemType<GarlicLvl3>(),
                4 => ModContent.ItemType<GarlicLvl4>(),
                5 => ModContent.ItemType<GarlicLvl5>(),
                6 => ModContent.ItemType<GarlicLvl6>(),
                7 => ModContent.ItemType<GarlicLvl7>(),
                8 => ModContent.ItemType<GarlicLvl8>(),
                _ => level > 8 ? ModContent.ItemType<GarlicLvl8>() : -1
            };
        }
    }

    public class GarlicLvl2 : GarlicLvl1
    {
        public override int Level { get; set; } = 2;
    }

    public class GarlicLvl3 : GarlicLvl1
    {
        public override int Level { get; set; } = 3;
    }

    public class GarlicLvl4 : GarlicLvl1
    {
        public override int Level { get; set; } = 4;
    }

    public class GarlicLvl5 : GarlicLvl1
    {
        public override int Level { get; set; } = 5;
    }

    public class GarlicLvl6 : GarlicLvl1
    {
        public override int Level { get; set; } = 6;
    }

    public class GarlicLvl7 : GarlicLvl1
    {
        public override int Level { get; set; } = 7;
    }

    public class GarlicLvl8 : GarlicLvl1
    {
        public override int Level { get; set; } = 8;
    }
}