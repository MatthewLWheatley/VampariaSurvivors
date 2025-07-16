using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VampariaSurvivors.Content.Projectile;

namespace VampariaSurvivors.Content.Items
{
    public class FireWandLvl1 : VSWeapon
    {
        public override string Texture => "VampariaSurvivors/Content/Items/FireWand";
        public override string WeaponName => "Fire Wand";
        public override string WeaponDescription => "Toggleable Personal Sentry";
        public override int ControllerProjectileType => ModContent.ProjectileType<FireWandControllerProjectile>();
        public override string WeaponIdentifier => "FireWand";

        public override int BaseDamage { get; set; } = 40;
        public override int BaseAmount { get; set; } = 3;
        public override float BaseSpeed { get; set; } = 3f;
        public override int BaseCooldown { get; set; } = 180;
        public override int BasePierce { get; set; } = 0;

        protected override float GetSpeedScale()
        {
            float scale = 1.0f;
            if (Level >= 3) scale *= 1.2f;
            if (Level >= 5) scale *= 1.2f;
            if (Level >= 7) scale *= 1.2f;
            return scale;
        }

        protected override float GetDamageScale()
        {
            float baseScale = base.GetDamageScale();

            int flatBonus = 0;
            if (Level >= 2) flatBonus += 20;
            if (Level >= 3) flatBonus += 20;
            if (Level >= 4) flatBonus += 20;
            if (Level >= 5) flatBonus += 20;
            if (Level >= 6) flatBonus += 20;
            if (Level >= 7) flatBonus += 20;
            if (Level >= 8) flatBonus += 20;

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
                1 => ModContent.ItemType<FireWandLvl1>(),
                2 => ModContent.ItemType<FireWandLvl2>(),
                3 => ModContent.ItemType<FireWandLvl3>(),
                4 => ModContent.ItemType<FireWandLvl4>(),
                5 => ModContent.ItemType<FireWandLvl5>(),
                6 => ModContent.ItemType<FireWandLvl6>(),
                7 => ModContent.ItemType<FireWandLvl7>(),
                8 => ModContent.ItemType<FireWandLvl8>(),
                _ => level > 8 ? ModContent.ItemType<FireWandLvl8>() : -1
            };
        }
    }

    public class FireWandLvl2 : FireWandLvl1
    {
        public override int Level { get; set; } = 2;
    }

    public class FireWandLvl3 : FireWandLvl1
    {
        public override int Level { get; set; } = 3;
    }

    public class FireWandLvl4 : FireWandLvl1
    {
        public override int Level { get; set; } = 4;
    }

    public class FireWandLvl5 : FireWandLvl1
    {
        public override int Level { get; set; } = 5;
    }

    public class FireWandLvl6 : FireWandLvl1
    {
        public override int Level { get; set; } = 6;
    }

    public class FireWandLvl7 : FireWandLvl1
    {
        public override int Level { get; set; } = 7;
    }

    public class FireWandLvl8 : FireWandLvl1
    {
        public override int Level { get; set; } = 8;
    }
}