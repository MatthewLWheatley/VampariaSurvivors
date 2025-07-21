using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VampariaSurvivors.Content.Items
{
    public abstract class VSWeapon : ModItem
    {
        public virtual int Level { get; set; } = 1;
        public virtual int MaxLevel => 8;

        public virtual int BaseDamage { get; set; } = 20;
        public virtual float BaseArea { get; set; } = 1.0f;
        public virtual float BaseSpeed { get; set; } = 1.0f;
        public virtual int BaseAmount { get; set; } = 1;
        public virtual int BaseDuration { get; set; } = 60;
        public virtual int BasePierce { get; set; } = 1;
        public virtual int BaseCooldown { get; set; } = 60;
        public virtual int BaseProjectileInterval { get; set; } = 6;
        public virtual float BaseKnockback { get; set; } = 1.5f;
        public virtual int BasePoolLimit { get; set; } = 50;
        public virtual float BaseChance { get; set; } = 0.0f;
        public virtual float BaseCritMulti { get; set; } = 1.0f;
        public virtual bool BaseBlockedByWalls { get; set; } = true;

        public virtual int CalculatedDamage => (int)(BaseDamage * GetDamageScale());
        public virtual float CalculatedArea => BaseArea * GetAreaScale();
        public virtual float CalculatedSpeed => BaseSpeed * GetSpeedScale();
        public virtual int CalculatedAmount => BaseAmount + GetAmountBonus();
        public virtual int CalculatedDuration => BaseDuration + GetDurationBonus();
        public virtual int CalculatedPierce => BasePierce + GetPierceBonus();
        public virtual int CalculatedCooldown => Math.Max(1, BaseCooldown - GetCooldownReduction());
        public virtual int CalculatedProjectileInterval => Math.Max(1, BaseProjectileInterval - GetIntervalReduction());
        public virtual float CalculatedKnockback => BaseKnockback * GetKnockbackScale();
        public virtual int CalculatedPoolLimit => BasePoolLimit + GetPoolLimitBonus();
        public virtual float CalculatedChance => Math.Min(1.0f, BaseChance + GetChanceBonus());
        public virtual float CalculatedCritMulti => BaseCritMulti + GetCritMultiBonus();
        public virtual bool CalculatedBlockedByWalls => BaseBlockedByWalls;

        protected virtual float GetDamageScale() => 1.0f + (0.25f * (Level - 1));
        protected virtual float GetAreaScale() => 1.0f;
        protected virtual float GetSpeedScale() => 1.0f;
        protected virtual int GetAmountBonus() => 0;
        protected virtual int GetDurationBonus() => 0;
        protected virtual int GetPierceBonus() => 0;
        protected virtual int GetCooldownReduction() => 0;
        protected virtual int GetIntervalReduction() => 0;
        protected virtual float GetKnockbackScale() => 1.0f;
        protected virtual int GetPoolLimitBonus() => 0;
        protected virtual float GetChanceBonus() => 0.0f;
        protected virtual float GetCritMultiBonus() => 0.0f;

        public abstract string WeaponName { get; }
        public abstract string WeaponDescription { get; }
        public abstract int ControllerProjectileType { get; }
        public abstract string WeaponIdentifier { get; }

        public override void SetDefaults()
        {
            Item.SetNameOverride(WeaponName);
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.DamageType = DamageClass.Magic;
            Item.damage = 0;
            Item.knockBack = CalculatedKnockback;
            Item.mana = 20;
            Item.noMelee = true;
            Item.autoReuse = false;
            Item.shoot = ControllerProjectileType;
            Item.shootSpeed = 0f;
        }

        public override bool CanUseItem(Player player)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI &&
                    Main.projectile[i].type == ControllerProjectileType)
                {
                    Main.projectile[i].Kill();
                    return false;
                }
            }
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Clear();
            tooltips.Add(new TooltipLine(Mod, "Name", WeaponName));
            tooltips.Add(new TooltipLine(Mod, "Level", "Level " + Level));
            tooltips.Add(new TooltipLine(Mod, "ManaCost", "Mana Cost: " + Main.LocalPlayer.GetManaCost(Item)));
            tooltips.Add(new TooltipLine(Mod, "Manamaintenance", "Mana Maintenance: " + Main.LocalPlayer.GetManaCost(Item) / 2));
            tooltips.Add(new TooltipLine(Mod, "Damage", "Damage: " + CalculatedDamage));

            if (BaseAmount > 0)
                tooltips.Add(new TooltipLine(Mod, "Amount", "Amount: " + CalculatedAmount));

            if (BasePierce > 0)
                tooltips.Add(new TooltipLine(Mod, "Pierce", "Pierce: " + CalculatedPierce));

            if (BaseDuration > 0)
                tooltips.Add(new TooltipLine(Mod, "Duration", "Duration: " + (CalculatedDuration / 60.0f).ToString("F1") + "s"));

            if (BaseArea != 1.0f)
                tooltips.Add(new TooltipLine(Mod, "Area", "Area: " + (CalculatedArea * 100).ToString("F0") + "%"));

            if (BaseSpeed != 1.0f)
                tooltips.Add(new TooltipLine(Mod, "Speed", "Speed: " + (CalculatedSpeed * 100).ToString("F0") + "%"));

            tooltips.Add(new TooltipLine(Mod, "Cooldown", "Cooldown: " + (CalculatedCooldown / 60.0f).ToString("F1") + "s"));
            tooltips.Add(new TooltipLine(Mod, "Description", WeaponDescription));

            base.ModifyTooltips(tooltips);
        }

        public override bool OnPickup(Player player)
        {
            int combinableLevel = this.Level;
            List<int> combinableSlots = new List<int>();
            List<int> maxLevelSlots = new List<int>();

            for (int i = 0; i < player.inventory.Length; i++)
            {
                Item inventoryItem = player.inventory[i];
                if (inventoryItem.IsAir) continue;

                if (inventoryItem.ModItem is VSWeapon existingWeapon &&
                    existingWeapon.WeaponIdentifier == this.WeaponIdentifier)
                {
                    if (existingWeapon.Level >= MaxLevel)
                    {
                        maxLevelSlots.Add(i);
                    }
                    else
                    {
                        combinableLevel += existingWeapon.Level;
                        combinableSlots.Add(i);
                    }
                }
            }

            if (combinableSlots.Count > 0)
            {
                foreach (int slot in combinableSlots)
                {
                    player.inventory[slot].TurnToAir();
                }

                int finalLevel = Math.Min(MaxLevel, combinableLevel);

                int newWeaponType = GetWeaponTypeAtLevel(finalLevel);
                if (newWeaponType != -1)
                {
                    player.QuickSpawnItem(player.GetSource_ItemUse(Item), newWeaponType);

                    if (maxLevelSlots.Count > 0)
                    {
                        Main.NewText($"{WeaponName} combined to Level {finalLevel} (Max level weapons preserved)", 0, 200, 255);
                    }
                    else if (combinableLevel != finalLevel)
                    {
                        Main.NewText($"{WeaponName} combined to Level {finalLevel} (Max Level reached)", 255, 255, 0);
                    }
                    else
                    {
                        Main.NewText($"{WeaponName} combined to Level {finalLevel}!", 0, 255, 0);
                    }
                }

                return false;
            }

            return true;
        }

        protected virtual int GetWeaponTypeAtLevel(int level)
        {
            return -1;
        }

        public virtual WeaponStats GetWeaponStats(Player player = null)
        {
            var baseStats = new WeaponStats
            {
                Damage = CalculatedDamage,
                Area = CalculatedArea,
                Speed = CalculatedSpeed,
                Amount = CalculatedAmount,
                Duration = CalculatedDuration,
                Pierce = CalculatedPierce,
                Cooldown = CalculatedCooldown,
                ProjectileInterval = CalculatedProjectileInterval,
                Knockback = CalculatedKnockback,
                PoolLimit = CalculatedPoolLimit,
                Chance = CalculatedChance,
                CritMulti = CalculatedCritMulti,
                BlockedByWalls = CalculatedBlockedByWalls
            };

            Player targetPlayer = player ?? Main.LocalPlayer;

            var vsPlayer = targetPlayer.GetModPlayer<Content.Accessories.VSPlayer>();
            if (vsPlayer != null)
            {
                baseStats = vsPlayer.ModifyWeaponStats(baseStats);
            }

            return baseStats;
        }
    }

    public struct WeaponStats
    {
        public int Damage;
        public float Area;
        public float Speed;
        public int Amount;
        public int Duration;
        public int Pierce;
        public int Cooldown;
        public int ProjectileInterval;
        public float Knockback;
        public int PoolLimit;
        public float Chance;
        public float CritMulti;
        public bool BlockedByWalls;
    }
}