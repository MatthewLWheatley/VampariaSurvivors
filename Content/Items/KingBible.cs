using System;
using Terraria.ID;
using Terraria.ModLoader;
using VampariaSurvivors.Content.Projectile;

namespace VampariaSurvivors.Content.Items
{
    public class KingBibleLvl1 : VSWeapon
    {
        public override string Texture => "VampariaSurvivors/Content/Items/KingBible";
        public override string WeaponName => "King Bible";
        public override string WeaponDescription => "Ring of orbiting bibles that rotate around the player";
        public override int ControllerProjectileType => ModContent.ProjectileType<KingBibleControllerProjectile>();
        public override string WeaponIdentifier => "KingBible";

        public override int BaseDamage { get; set; } = 10;
        public override int BaseAmount { get; set; } = 1;
        public override float BaseArea { get; set; } = 1.0f;
        public override float BaseSpeed { get; set; } = 1.0f;
        public override int BaseDuration { get; set; } = 180;
        public override int BaseCooldown { get; set; } = 180;
        public override int BaseProjectileInterval { get; set; } = 0;
        public override bool BaseBlockedByWalls { get; set; } = false;

        protected override float GetDamageScale()
        {
            float baseScale = 1.0f;

            int flatBonus = 0;
            if (Level >= 4) flatBonus += 20;
            if (Level >= 7) flatBonus += 20;

            if (flatBonus > 0)
            {
                baseScale += (float)flatBonus / BaseDamage;
            }

            return baseScale;
        }

        protected override float GetAreaScale()
        {
            float scale = 1.0f;
            if (Level >= 3) scale *= 1.25f;
            if (Level >= 6) scale *= 1.25f;
            return scale;
        }

        protected override float GetSpeedScale()
        {
            float scale = 1.0f;
            if (Level >= 3) scale *= 1.30f;
            if (Level >= 6) scale *= 1.30f;
            return scale;
        }

        protected override int GetAmountBonus()
        {
            int bonus = 0;
            if (Level >= 2) bonus += 1;
            if (Level >= 5) bonus += 1;
            if (Level >= 8) bonus += 1;
            return bonus;
        }

        protected override int GetDurationBonus()
        {
            int bonus = 0;
            if (Level >= 4) bonus += 30;
            if (Level >= 7) bonus += 30;
            return bonus;
        }

        public override int CalculatedCooldown => BaseCooldown + CalculatedDuration;

        protected override int GetWeaponTypeAtLevel(int level)
        {
            return level switch
            {
                1 => ModContent.ItemType<KingBibleLvl1>(),
                2 => ModContent.ItemType<KingBibleLvl2>(),
                3 => ModContent.ItemType<KingBibleLvl3>(),
                4 => ModContent.ItemType<KingBibleLvl4>(),
                5 => ModContent.ItemType<KingBibleLvl5>(),
                6 => ModContent.ItemType<KingBibleLvl6>(),
                7 => ModContent.ItemType<KingBibleLvl7>(),
                8 => ModContent.ItemType<KingBibleLvl8>(),
                _ => level > 8 ? ModContent.ItemType<KingBibleLvl8>() : -1
            };
        }
    }

    public class KingBibleLvl2 : KingBibleLvl1
    {
        public override int Level { get; set; } = 2;
    }

    public class KingBibleLvl3 : KingBibleLvl1
    {
        public override int Level { get; set; } = 3;
    }

    public class KingBibleLvl4 : KingBibleLvl1
    {
        public override int Level { get; set; } = 4;
    }

    public class KingBibleLvl5 : KingBibleLvl1
    {
        public override int Level { get; set; } = 5;
    }

    public class KingBibleLvl6 : KingBibleLvl1
    {
        public override int Level { get; set; } = 6;
    }

    public class KingBibleLvl7 : KingBibleLvl1
    {
        public override int Level { get; set; } = 7;
    }

    public class KingBibleLvl8 : KingBibleLvl1
    {
        public override int Level { get; set; } = 8;
    }
}