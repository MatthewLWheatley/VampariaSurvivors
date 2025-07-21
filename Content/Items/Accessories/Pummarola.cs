using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VampariaSurvivors.Content.Accessories;

namespace VampariaSurvivors.Content.Items.Accessories
{
    public class PummarolaLvl1 : VSAccessory
    {
        public virtual int Level { get; set; } = 1;
        public virtual int MaxLevel => 5;

        public override string Texture => "VampariaSurvivors/Content/Items/Accessories/Pummarola";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.SetNameOverride("Pummarola");
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Green;
        }

        protected override void ApplyEffects(VSPlayer player)
        {
            player.RecoveryBonus += Level;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int recoveryBonus = Level;

            tooltips.Clear();
            tooltips.Add(new TooltipLine(Mod, "Name", "Pummarola"));
            tooltips.Add(new TooltipLine(Mod, "Level", "Level " + Level));
            tooltips.Add(new TooltipLine(Mod, "Effect", $"+{recoveryBonus} Recovery"));
            tooltips.Add(new TooltipLine(Mod, "Description", "Healing power of the golden fruit"));
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

                if (inventoryItem.ModItem is PummarolaLvl1 existingPummarola)
                {
                    if (existingPummarola.Level >= MaxLevel)
                    {
                        maxLevelSlots.Add(i);
                    }
                    else
                    {
                        totalLevel += existingPummarola.Level;
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
                        Main.NewText($"Pummarola combined to Level {finalLevel} (Max level pummarolas preserved)", 0, 200, 255);
                    }
                    else if (totalLevel != finalLevel)
                    {
                        Main.NewText($"Pummarola combined to Level {finalLevel} (Max Level reached)", 255, 255, 0);
                    }
                    else
                    {
                        Main.NewText($"Pummarola combined to Level {finalLevel}!", 0, 255, 0);
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
                1 => ModContent.ItemType<PummarolaLvl1>(),
                2 => ModContent.ItemType<PummarolaLvl2>(),
                3 => ModContent.ItemType<PummarolaLvl3>(),
                4 => ModContent.ItemType<PummarolaLvl4>(),
                5 => ModContent.ItemType<PummarolaLvl5>(),
                _ => level > 5 ? ModContent.ItemType<PummarolaLvl5>() : -1
            };
        }

        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ItemID.LifeFruit, 1)
                .AddIngredient(ItemID.HealingPotion, 5)
                .AddTile(TileID.Bottles)
                .Register();
        }
    }

    public class PummarolaLvl2 : PummarolaLvl1
    {
        public override int Level { get; set; } = 2;
    }

    public class PummarolaLvl3 : PummarolaLvl1
    {
        public override int Level { get; set; } = 3;
    }

    public class PummarolaLvl4 : PummarolaLvl1
    {
        public override int Level { get; set; } = 4;
    }

    public class PummarolaLvl5 : PummarolaLvl1
    {
        public override int Level { get; set; } = 5;
    }
}