using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VampariaSurvivors.Content.Accessories;

namespace VampariaSurvivors.Content.Items.Accessories
{
    public class SpellbinderLvl1 : VSAccessory
    {
        public virtual int Level { get; set; } = 1;
        public virtual int MaxLevel => 5;

        public override string Texture => "VampariaSurvivors/Content/Items/Accessories/Spellbinder";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.SetNameOverride("Spellbinder");
            Item.value = Item.buyPrice(0, 2, 50, 0);
            Item.rare = ItemRarityID.Blue;
        }

        protected override void ApplyEffects(VSPlayer player)
        {
            float durationIncrease = Level * 0.10f; // 10% per level
            player.DurationMultiplier += durationIncrease;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int durationPercent = Level * 10;

            tooltips.Clear();
            tooltips.Add(new TooltipLine(Mod, "Name", "Spellbinder"));
            tooltips.Add(new TooltipLine(Mod, "Level", "Level " + Level));
            tooltips.Add(new TooltipLine(Mod, "Effect", $"+{durationPercent}% Duration"));
            tooltips.Add(new TooltipLine(Mod, "Description", "Extends the life of magical effects"));
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

                if (inventoryItem.ModItem is SpellbinderLvl1 existingSpellbinder)
                {
                    if (existingSpellbinder.Level >= MaxLevel)
                    {
                        maxLevelSlots.Add(i);
                    }
                    else
                    {
                        totalLevel += existingSpellbinder.Level;
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
                        Main.NewText($"Spellbinder combined to Level {finalLevel} (Max level spellbinders preserved)", 0, 200, 255);
                    }
                    else if (totalLevel != finalLevel)
                    {
                        Main.NewText($"Spellbinder combined to Level {finalLevel} (Max Level reached)", 255, 255, 0);
                    }
                    else
                    {
                        Main.NewText($"Spellbinder combined to Level {finalLevel}!", 0, 255, 0);
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
                1 => ModContent.ItemType<SpellbinderLvl1>(),
                2 => ModContent.ItemType<SpellbinderLvl2>(),
                3 => ModContent.ItemType<SpellbinderLvl3>(),
                4 => ModContent.ItemType<SpellbinderLvl4>(),
                5 => ModContent.ItemType<SpellbinderLvl5>(),
                _ => level > 5 ? ModContent.ItemType<SpellbinderLvl5>() : -1
            };
        }

        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ItemID.MagicMirror, 1)
                .AddIngredient(ItemID.FallenStar, 3)
                .AddIngredient(ItemID.Silk, 5)
                .AddTile(TileID.Bookcases)
                .Register();
        }
    }

    public class SpellbinderLvl2 : SpellbinderLvl1
    {
        public override int Level { get; set; } = 2;
    }

    public class SpellbinderLvl3 : SpellbinderLvl1
    {
        public override int Level { get; set; } = 3;
    }

    public class SpellbinderLvl4 : SpellbinderLvl1
    {
        public override int Level { get; set; } = 4;
    }

    public class SpellbinderLvl5 : SpellbinderLvl1
    {
        public override int Level { get; set; } = 5;
    }
}