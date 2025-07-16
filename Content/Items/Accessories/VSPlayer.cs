using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VampariaSurvivors.Content.Items;

namespace VampariaSurvivors.Content.Accessories
{
    public class VSPlayer : ModPlayer
    {
        public float DamageMultiplier = 1.0f;
        public float AreaMultiplier = 1.0f;
        public float SpeedMultiplier = 1.0f;
        public int AmountBonus = 0;
        public int DurationBonus = 0;
        public int PierceBonus = 0;
        public float CooldownMultiplier = 1.0f;
        public float KnockbackMultiplier = 1.0f;
        public float ChanceBonus = 0.0f;
        public float CritMultiBonus = 0.0f;

        public override void ResetEffects()
        {
            DamageMultiplier = 1.0f;
            AreaMultiplier = 1.0f;
            SpeedMultiplier = 1.0f;
            AmountBonus = 0;
            DurationBonus = 0;
            PierceBonus = 0;
            CooldownMultiplier = 1.0f;
            KnockbackMultiplier = 1.0f;
            ChanceBonus = 0.0f;
            CritMultiBonus = 0.0f;
        }

        public WeaponStats ModifyWeaponStats(WeaponStats baseStats)
        {
            var modifiedStats = new WeaponStats
            {
                Damage = (int)(baseStats.Damage * DamageMultiplier),
                Area = baseStats.Area * AreaMultiplier,
                Speed = baseStats.Speed * SpeedMultiplier,
                Amount = baseStats.Amount + AmountBonus,
                Duration = baseStats.Duration + DurationBonus,
                Pierce = baseStats.Pierce + PierceBonus,
                Cooldown = (int)(baseStats.Cooldown * CooldownMultiplier),
                ProjectileInterval = baseStats.ProjectileInterval,
                Knockback = baseStats.Knockback * KnockbackMultiplier,
                PoolLimit = baseStats.PoolLimit,
                Chance = System.Math.Min(1.0f, baseStats.Chance + ChanceBonus),
                CritMulti = baseStats.CritMulti + CritMultiBonus,
                BlockedByWalls = baseStats.BlockedByWalls
            };

            if (Main.netMode != NetmodeID.Server && CooldownMultiplier != 1.0f)
            {
                Main.NewText($"Cooldown modified: {baseStats.Cooldown} -> {modifiedStats.Cooldown} (multiplier: {CooldownMultiplier:F2})");
            }

            return modifiedStats;
        }
    }

    public abstract class VSAccessory : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var vsPlayer = player.GetModPlayer<VSPlayer>();
            ApplyEffects(vsPlayer);
        }

        protected abstract void ApplyEffects(VSPlayer player);
    }
}