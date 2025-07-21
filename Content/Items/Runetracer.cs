using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VampariaSurvivors.Content.Projectile;

namespace VampariaSurvivors.Content.Items
{
    public class RuneTracerLvl1 : VSWeapon
    {
        public override string Texture => "VampariaSurvivors/Content/Items/RuneTracer";
        public override string WeaponName => "Rune Tracer";
        public override string WeaponDescription => "Toggleable Personal Sentry";
        public override int ControllerProjectileType => ModContent.ProjectileType<RuneTracerControllerProjectile>();
        public override string WeaponIdentifier => "RuneTracer";

        public override int BaseDamage { get; set; } = 20;
        public override int BaseAmount { get; set; } = 1;
        public override float BaseSpeed { get; set; } = 10f;
        public override int BaseDuration { get; set; } = 135;
        public override int BaseCooldown { get; set; } = 180;

        protected override int GetAmountBonus()
        {
            return Level switch
            {
                >= 7 => 2,
                >= 4 => 1,
                _ => 0
            };
        }

        protected override int GetDurationBonus()
        {
            int bonus = 0;
            if (Level >= 3) bonus += 18;
            if (Level >= 6) bonus += 18;
            if (Level >= 8) bonus += 30;
            return bonus;
        }

        protected override float GetSpeedScale()
        {
            float scale = 1.0f;
            if (Level >= 2) scale *= 1.2f;
            if (Level >= 5) scale *= 1.2f;
            return scale;
        }

        protected override float GetDamageScale()
        {
            float baseScale = base.GetDamageScale();

            int flatBonus = 0;
            if (Level >= 2) flatBonus += 10;
            if (Level >= 3) flatBonus += 10;
            if (Level >= 5) flatBonus += 10;
            if (Level >= 6) flatBonus += 10;

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
                1 => ModContent.ItemType<RuneTracerLvl1>(),
                2 => ModContent.ItemType<RuneTracerLvl2>(),
                3 => ModContent.ItemType<RuneTracerLvl3>(),
                4 => ModContent.ItemType<RuneTracerLvl4>(),
                5 => ModContent.ItemType<RuneTracerLvl5>(),
                6 => ModContent.ItemType<RuneTracerLvl6>(),
                7 => ModContent.ItemType<RuneTracerLvl7>(),
                8 => ModContent.ItemType<RuneTracerLvl8>(),
                _ => level > 8 ? ModContent.ItemType<RuneTracerLvl8>() : -1
            };
        }
    }

    public class RuneTracerLvl2 : RuneTracerLvl1
    {
        public override int Level { get; set; } = 2;
    }

    public class RuneTracerLvl3 : RuneTracerLvl1
    {
        public override int Level { get; set; } = 3;
    }

    public class RuneTracerLvl4 : RuneTracerLvl1
    {
        public override int Level { get; set; } = 4;
    }

    public class RuneTracerLvl5 : RuneTracerLvl1
    {
        public override int Level { get; set; } = 5;
    }

    public class RuneTracerLvl6 : RuneTracerLvl1
    {
        public override int Level { get; set; } = 6;
    }

    public class RuneTracerLvl7 : RuneTracerLvl1
    {
        public override int Level { get; set; } = 7;
    }

    public class RuneTracerLvl8 : RuneTracerLvl1
    {
        public override int Level { get; set; } = 8;
    }
}