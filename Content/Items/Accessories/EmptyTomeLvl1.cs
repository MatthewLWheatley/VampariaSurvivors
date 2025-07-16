using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VampariaSurvivors.Content.Accessories;

namespace VampariaSurvivors.Content.Items.Accessories
{
    public class EmptyTomeLvl1 : VSAccessory
    {
        public virtual int Level { get; set; } = 1;
        public virtual int MaxLevel => 5;

        public override string Texture => "VampariaSurvivors/Content/Items/Accessories/EmptyTome";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.SetNameOverride("Empty Tome");
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Green;
        }

        protected override void ApplyEffects(VSPlayer player)
        {
            float cooldownReduction = 0.08f * Level;
            player.CooldownMultiplier *= (1f - cooldownReduction);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            float cooldownReduction = 8f * Level;

            tooltips.Clear();
            tooltips.Add(new TooltipLine(Mod, "Name", "Empty Tome"));
            tooltips.Add(new TooltipLine(Mod, "Level", "Level " + Level));
            tooltips.Add(new TooltipLine(Mod, "Effect", $"-{cooldownReduction:F0}% weapon cooldown"));
            tooltips.Add(new TooltipLine(Mod, "Description", "The pages are blank, waiting to be filled with knowledge"));
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

                if (inventoryItem.ModItem is EmptyTomeLvl1 existingTome)
                {
                    if (existingTome.Level >= MaxLevel)
                    {
                        maxLevelSlots.Add(i);
                    }
                    else
                    {
                        totalLevel += existingTome.Level;
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
                        Main.NewText($"Empty Tome combined to Level {finalLevel} (Max level tomes preserved)", 0, 200, 255);
                    }
                    else if (totalLevel != finalLevel)
                    {
                        Main.NewText($"Empty Tome combined to Level {finalLevel} (Max Level reached)", 255, 255, 0);
                    }
                    else
                    {
                        Main.NewText($"Empty Tome combined to Level {finalLevel}!", 0, 255, 0);
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
                1 => ModContent.ItemType<EmptyTomeLvl1>(),
                2 => ModContent.ItemType<EmptyTomeLvl2>(),
                3 => ModContent.ItemType<EmptyTomeLvl3>(),
                4 => ModContent.ItemType<EmptyTomeLvl4>(),
                5 => ModContent.ItemType<EmptyTomeLvl5>(),
                _ => level > 5 ? ModContent.ItemType<EmptyTomeLvl5>() : -1
            };
        }

        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ItemID.Book, 1)
                .AddIngredient(ItemID.ManaCrystal, 1)
                .AddTile(TileID.Bookcases)
                .Register();
        }
    }

    public class EmptyTomeLvl2 : EmptyTomeLvl1
    {
        public override int Level { get; set; } = 2;
    }

    public class EmptyTomeLvl3 : EmptyTomeLvl1
    {
        public override int Level { get; set; } = 3;
    }

    public class EmptyTomeLvl4 : EmptyTomeLvl1
    {
        public override int Level { get; set; } = 4;
    }

    public class EmptyTomeLvl5 : EmptyTomeLvl1
    {
        public override int Level { get; set; } = 5;
    }
}