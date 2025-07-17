using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VampariaSurvivors.Content.Accessories;

namespace VampariaSurvivors.Content.Items.Accessories
{
    public class SpinachLvl1 : VSAccessory
    {
        public virtual int Level { get; set; } = 1;
        public virtual int MaxLevel => 5;

        public override string Texture => "VampariaSurvivors/Content/Items/Accessories/Spinach";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.SetNameOverride("Spinach");
            Item.value = Item.buyPrice(0, 3, 0, 0);
            Item.rare = ItemRarityID.Orange;
        }

        protected override void ApplyEffects(VSPlayer player)
        {
            float damageIncrease = 0.10f * Level;
            player.DamageMultiplier *= (1f + damageIncrease);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            float damagePercent = 10f * Level;

            tooltips.Clear();
            tooltips.Add(new TooltipLine(Mod, "Name", "Spinach"));
            tooltips.Add(new TooltipLine(Mod, "Level", "Level " + Level));

            if (Level == 1)
            {
                tooltips.Add(new TooltipLine(Mod, "Effect", "Raises inflicted damage by 10%."));
            }
            else
            {
                tooltips.Add(new TooltipLine(Mod, "Effect", $"Base damage up by {damagePercent:F0}%"));
            }

            tooltips.Add(new TooltipLine(Mod, "Description", "Remarkably nutritious! Ew."));
        }

        public override bool OnPickup(Player player)
        {
            int totalLevel = this.Level;
            List<int> accessorySlots = new List<int>();
            List<int> maxLevelSlots = new List<int>();

            for (int i = 0; i < player.inventory.Length; i++)
            {
                Item inventoryItem = player.inventory[i];
                if (inventoryItem.IsAir) continue;

                if (inventoryItem.ModItem is SpinachLvl1 existingSpinach)
                {
                    if (existingSpinach.Level >= MaxLevel)
                    {
                        maxLevelSlots.Add(i);
                    }
                    else
                    {
                        totalLevel += existingSpinach.Level;
                        accessorySlots.Add(i);
                    }
                }
            }

            if (accessorySlots.Count > 0)
            {
                foreach (int slot in accessorySlots)
                {
                    player.inventory[slot].TurnToAir();
                }

                int finalLevel = System.Math.Min(MaxLevel, totalLevel);

                int newAccessoryType = GetAccessoryTypeAtLevel(finalLevel);
                if (newAccessoryType != -1)
                {
                    player.QuickSpawnItem(player.GetSource_ItemUse(Item), newAccessoryType);

                    if (maxLevelSlots.Count > 0)
                    {
                        Main.NewText($"Spinach combined to Level {finalLevel} (Max level spinach preserved)", 0, 200, 255);
                    }
                    else if (totalLevel != finalLevel)
                    {
                        Main.NewText($"Spinach combined to Level {finalLevel} (Max Level reached)", 255, 255, 0);
                    }
                    else
                    {
                        Main.NewText($"Spinach combined to Level {finalLevel}!", 0, 255, 0);
                    }
                }

                return false;
            }

            return true;
        }

        protected virtual int GetAccessoryTypeAtLevel(int level)
        {
            return level switch
            {
                1 => ModContent.ItemType<SpinachLvl1>(),
                2 => ModContent.ItemType<SpinachLvl2>(),
                3 => ModContent.ItemType<SpinachLvl3>(),
                4 => ModContent.ItemType<SpinachLvl4>(),
                5 => ModContent.ItemType<SpinachLvl5>(),
                _ => level > 5 ? ModContent.ItemType<SpinachLvl5>() : -1
            };
        }

        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ItemID.Daybloom, 3)
                .AddIngredient(ItemID.LifeCrystal, 1)
                .AddTile(TileID.Bottles)
                .Register();
        }
    }

    public class SpinachLvl2 : SpinachLvl1
    {
        public override int Level { get; set; } = 2;
    }

    public class SpinachLvl3 : SpinachLvl1
    {
        public override int Level { get; set; } = 3;
    }

    public class SpinachLvl4 : SpinachLvl1
    {
        public override int Level { get; set; } = 4;
    }

    public class SpinachLvl5 : SpinachLvl1
    {
        public override int Level { get; set; } = 5;
    }
}