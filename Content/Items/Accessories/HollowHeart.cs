using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VampariaSurvivors.Content.Accessories;

namespace VampariaSurvivors.Content.Items.Accessories
{
    public class HollowHeartLvl1 : VSAccessory
    {
        public virtual int Level { get; set; } = 1;
        public virtual int MaxLevel => 5;

        public override string Texture => "VampariaSurvivors/Content/Items/Accessories/HollowHeart";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.SetNameOverride("Hollow Heart");
            Item.value = Item.buyPrice(0, 3, 0, 0);
            Item.rare = ItemRarityID.Orange;
        }

        protected override void ApplyEffects(VSPlayer player)
        {
            player.MaxHealthBonus += Level * 20;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int healthBonus = Level * 20;

            tooltips.Clear();
            tooltips.Add(new TooltipLine(Mod, "Name", "Hollow Heart"));
            tooltips.Add(new TooltipLine(Mod, "Level", "Level " + Level));
            tooltips.Add(new TooltipLine(Mod, "Effect", $"+{healthBonus} Max Health"));
            tooltips.Add(new TooltipLine(Mod, "Description", "A heart that beats with hollow purpose"));
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

                if (inventoryItem.ModItem is HollowHeartLvl1 existingHeart)
                {
                    if (existingHeart.Level >= MaxLevel)
                    {
                        maxLevelSlots.Add(i);
                    }
                    else
                    {
                        totalLevel += existingHeart.Level;
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
                        Main.NewText($"Hollow Heart combined to Level {finalLevel} (Max level hearts preserved)", 0, 200, 255);
                    }
                    else if (totalLevel != finalLevel)
                    {
                        Main.NewText($"Hollow Heart combined to Level {finalLevel} (Max Level reached)", 255, 255, 0);
                    }
                    else
                    {
                        Main.NewText($"Hollow Heart combined to Level {finalLevel}!", 0, 255, 0);
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
                1 => ModContent.ItemType<HollowHeartLvl1>(),
                2 => ModContent.ItemType<HollowHeartLvl2>(),
                3 => ModContent.ItemType<HollowHeartLvl3>(),
                4 => ModContent.ItemType<HollowHeartLvl4>(),
                5 => ModContent.ItemType<HollowHeartLvl5>(),
                _ => level > 5 ? ModContent.ItemType<HollowHeartLvl5>() : -1
            };
        }

        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ItemID.LifeCrystal, 1)
                .AddIngredient(ItemID.Ruby, 2)
                .AddIngredient(ItemID.SoulofNight, 5)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }

    public class HollowHeartLvl2 : HollowHeartLvl1
    {
        public override int Level { get; set; } = 2;
    }

    public class HollowHeartLvl3 : HollowHeartLvl1
    {
        public override int Level { get; set; } = 3;
    }

    public class HollowHeartLvl4 : HollowHeartLvl1
    {
        public override int Level { get; set; } = 4;
    }

    public class HollowHeartLvl5 : HollowHeartLvl1
    {
        public override int Level { get; set; } = 5;
    }
}