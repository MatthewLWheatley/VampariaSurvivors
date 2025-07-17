using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VampariaSurvivors.Content.Accessories;

namespace VampariaSurvivors.Content.Items.Accessories
{
    public class CandelabradorLvl1 : VSAccessory
    {
        public virtual int Level { get; set; } = 1;
        public virtual int MaxLevel => 5;

        public override string Texture => "VampariaSurvivors/Content/Items/Accessories/Candelabrador";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.SetNameOverride("Candelabrador");
            Item.value = Item.buyPrice(0, 3, 0, 0);
            Item.rare = ItemRarityID.Orange;
        }

        protected override void ApplyEffects(VSPlayer player)
        {
            float areaIncrease = 0.10f * Level;
            player.AreaMultiplier *= (1f + areaIncrease);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            float areaPercent = 10f * Level;

            tooltips.Clear();
            tooltips.Add(new TooltipLine(Mod, "Name", "Candelabrador"));
            tooltips.Add(new TooltipLine(Mod, "Level", "Level " + Level));

            if (Level == 1)
            {
                tooltips.Add(new TooltipLine(Mod, "Effect", "Augments area of attacks by 10%."));
            }
            else
            {
                tooltips.Add(new TooltipLine(Mod, "Effect", $"Base area up by {areaPercent:F0}%"));
            }

            tooltips.Add(new TooltipLine(Mod, "Description", "Illuminates a wider path to destruction."));
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

                if (inventoryItem.ModItem is CandelabradorLvl1 existingCandelabrador)
                {
                    if (existingCandelabrador.Level >= MaxLevel)
                    {
                        maxLevelSlots.Add(i);
                    }
                    else
                    {
                        totalLevel += existingCandelabrador.Level;
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
                        Main.NewText($"Candelabrador combined to Level {finalLevel} (Max level candelabradors preserved)", 0, 200, 255);
                    }
                    else if (totalLevel != finalLevel)
                    {
                        Main.NewText($"Candelabrador combined to Level {finalLevel} (Max Level reached)", 255, 255, 0);
                    }
                    else
                    {
                        Main.NewText($"Candelabrador combined to Level {finalLevel}!", 0, 255, 0);
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
                1 => ModContent.ItemType<CandelabradorLvl1>(),
                2 => ModContent.ItemType<CandelabradorLvl2>(),
                3 => ModContent.ItemType<CandelabradorLvl3>(),
                4 => ModContent.ItemType<CandelabradorLvl4>(),
                5 => ModContent.ItemType<CandelabradorLvl5>(),
                _ => level > 5 ? ModContent.ItemType<CandelabradorLvl5>() : -1
            };
        }

        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ItemID.Candle, 5)
                .AddIngredient(ItemID.Torch, 10)
                .AddIngredient(ItemID.Chain, 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class CandelabradorLvl2 : CandelabradorLvl1
    {
        public override int Level { get; set; } = 2;
    }

    public class CandelabradorLvl3 : CandelabradorLvl1
    {
        public override int Level { get; set; } = 3;
    }

    public class CandelabradorLvl4 : CandelabradorLvl1
    {
        public override int Level { get; set; } = 4;
    }

    public class CandelabradorLvl5 : CandelabradorLvl1
    {
        public override int Level { get; set; } = 5;
    }
}