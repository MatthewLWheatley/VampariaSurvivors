using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VampariaSurvivors.Content.Accessories;

namespace VampariaSurvivors.Content.Items.Accessories
{
    public class DuplicatorLvl1 : VSAccessory
    {
        public virtual int Level { get; set; } = 1;
        public virtual int MaxLevel => 2;
        public override string Texture => "VampariaSurvivors/Content/Items/Accessories/Duplicator";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.SetNameOverride("Duplicator");
            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Orange;
        }

        protected override void ApplyEffects(VSPlayer player)
        {
            player.AmountBonus += Level;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Clear();
            tooltips.Add(new TooltipLine(Mod, "Name", "Duplicator"));
            tooltips.Add(new TooltipLine(Mod, "Level", "Level " + Level));

            if (Level == 1)
            {
                tooltips.Add(new TooltipLine(Mod, "Effect", "Weapons fire more projectiles"));
                tooltips.Add(new TooltipLine(Mod, "Amount", "+1 projectile amount"));
            }
            else if (Level == 2)
            {
                tooltips.Add(new TooltipLine(Mod, "Effect", "Fires 1 more projectile"));
                tooltips.Add(new TooltipLine(Mod, "Amount", "+2 projectile amount"));
            }

            tooltips.Add(new TooltipLine(Mod, "Description", "Probability splits into possibility"));
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

                if (inventoryItem.ModItem is DuplicatorLvl1 existingDuplicator)
                {
                    if (existingDuplicator.Level >= MaxLevel)
                    {
                        maxLevelSlots.Add(i);
                    }
                    else
                    {
                        totalLevel += existingDuplicator.Level;
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
                        Main.NewText($"Duplicator combined to Level {finalLevel} (Max level duplicators preserved)", 0, 200, 255);
                    }
                    else if (totalLevel != finalLevel)
                    {
                        Main.NewText($"Duplicator combined to Level {finalLevel} (Max Level reached)", 255, 255, 0);
                    }
                    else
                    {
                        Main.NewText($"Duplicator combined to Level {finalLevel}!", 0, 255, 0);
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
                1 => ModContent.ItemType<DuplicatorLvl1>(),
                2 => ModContent.ItemType<DuplicatorLvl2>(),
                _ => level > 2 ? ModContent.ItemType<DuplicatorLvl2>() : -1
            };
        }

        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ItemID.CrystalBall, 1)
                .AddIngredient(ItemID.SoulofMight, 5)
                .AddIngredient(ItemID.SoulofSight, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class DuplicatorLvl2 : DuplicatorLvl1
    {
        public override int Level { get; set; } = 2;
    }
}