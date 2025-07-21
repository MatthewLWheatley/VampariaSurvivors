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
        public float DurationMultiplier = 1.0f;
        public int PierceBonus = 0;
        public float CooldownMultiplier = 1.0f;
        public float KnockbackMultiplier = 1.0f;
        public float ChanceBonus = 0.0f;
        public float CritMultiBonus = 0.0f;
        public int RecoveryBonus = 0;
        public int MaxHealthBonus = 0;

        private int recoveryTimer = 0;
        private const int RecoveryInterval = 60;

        public override void ResetEffects()
        {
            DamageMultiplier = 1.0f;
            AreaMultiplier = 1.0f;
            SpeedMultiplier = 1.0f;
            AmountBonus = 0;
            DurationBonus = 0;
            DurationMultiplier = 1.0f;
            PierceBonus = 0;
            CooldownMultiplier = 1.0f;
            KnockbackMultiplier = 1.0f;
            ChanceBonus = 0.0f;
            CritMultiBonus = 0.0f;
            RecoveryBonus = 0;
            MaxHealthBonus = 0;
        }

        public override void PostUpdateEquips()
        {
            if (MaxHealthBonus > 0)
            {
                Player.statLifeMax2 += MaxHealthBonus;
            }

            if (RecoveryBonus > 0)
            {
                recoveryTimer++;
                if (recoveryTimer >= RecoveryInterval)
                {
                    int healAmount = RecoveryBonus;

                    if (Player.statLife < Player.statLifeMax)
                    {
                        Player.statLife += healAmount;
                        if (Player.statLife > Player.statLifeMax)
                        {
                            Player.statLife = Player.statLifeMax;
                        }

                        if (Main.netMode != NetmodeID.Server)
                        {
                            CombatText.NewText(Player.getRect(), CombatText.HealLife, healAmount);
                        }
                    }

                    recoveryTimer = 0;
                }
            }
            else
            {
                recoveryTimer = 0;
            }
        }

        public WeaponStats ModifyWeaponStats(WeaponStats baseStats)
        {
            var modifiedStats = new WeaponStats
            {
                Damage = (int)(baseStats.Damage * DamageMultiplier),
                Area = baseStats.Area * AreaMultiplier,
                Speed = baseStats.Speed * SpeedMultiplier,
                Amount = baseStats.Amount + AmountBonus,
                Duration = (int)((baseStats.Duration + DurationBonus) * DurationMultiplier),
                Pierce = baseStats.Pierce + PierceBonus,
                Cooldown = (int)(baseStats.Cooldown * CooldownMultiplier),
                ProjectileInterval = baseStats.ProjectileInterval,
                Knockback = baseStats.Knockback * KnockbackMultiplier,
                PoolLimit = baseStats.PoolLimit,
                Chance = System.Math.Min(1.0f, baseStats.Chance + ChanceBonus),
                CritMulti = baseStats.CritMulti + CritMultiBonus,
                BlockedByWalls = baseStats.BlockedByWalls
            };

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