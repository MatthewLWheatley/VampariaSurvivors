using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VampariaSurvivors.Content.Accessories;

namespace VampariaSurvivors.Content.Items.Accessories
{
    public class BracerLvl1 : VSAccessory
    {
        public virtual int Level { get; set; } = 1;
        public virtual int MaxLevel => 5;

        public override string Texture => "VampariaSurvivors/Content/Items/Accessories/Bracer";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.SetNameOverride("Bracer");
            Item.value = Item.buyPrice(0, 3, 0, 0);
            Item.rare = ItemRarityID.Orange;
        }

        protected override void ApplyEffects(VSPlayer player)
        {
            float speedIncrease = 0.10f * Level;
            player.SpeedMultiplier *= (1f + speedIncrease);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            float speedPercent = 10f * Level;

            tooltips.Clear();
            tooltips.Add(new TooltipLine(Mod, "Name", "Bracer"));
            tooltips.Add(new TooltipLine(Mod, "Level", "Level " + Level));

            if (Level == 1)
            {
                tooltips.Add(new TooltipLine(Mod, "Effect", "Increases projectiles speed by 10%."));
            }
            else
            {
                tooltips.Add(new TooltipLine(Mod, "Effect", $"Base speed up by {speedPercent:F0}%"));
            }

            tooltips.Add(new TooltipLine(Mod, "Description", "Enhances arm strength and projectile velocity."));
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

                if (inventoryItem.ModItem is BracerLvl1 existingBracer)
                {
                    if (existingBracer.Level >= MaxLevel)
                    {
                        maxLevelSlots.Add(i);
                    }
                    else
                    {
                        totalLevel += existingBracer.Level;
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
                        Main.NewText($"Bracer combined to Level {finalLevel} (Max level bracers preserved)", 0, 200, 255);
                    }
                    else if (totalLevel != finalLevel)
                    {
                        Main.NewText($"Bracer combined to Level {finalLevel} (Max Level reached)", 255, 255, 0);
                    }
                    else
                    {
                        Main.NewText($"Bracer combined to Level {finalLevel}!", 0, 255, 0);
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
                1 => ModContent.ItemType<BracerLvl1>(),
                2 => ModContent.ItemType<BracerLvl2>(),
                3 => ModContent.ItemType<BracerLvl3>(),
                4 => ModContent.ItemType<BracerLvl4>(),
                5 => ModContent.ItemType<BracerLvl5>(),
                _ => level > 5 ? ModContent.ItemType<BracerLvl5>() : -1
            };
        }

        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ItemID.Leather, 3)
                .AddIngredient(ItemID.IronBar, 2)
                .AddIngredient(ItemID.Shackle, 1)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class BracerLvl2 : BracerLvl1
    {
        public override int Level { get; set; } = 2;
    }

    public class BracerLvl3 : BracerLvl1
    {
        public override int Level { get; set; } = 3;
    }

    public class BracerLvl4 : BracerLvl1
    {
        public override int Level { get; set; } = 4;
    }

    public class BracerLvl5 : BracerLvl1
    {
        public override int Level { get; set; } = 5;
    }
}