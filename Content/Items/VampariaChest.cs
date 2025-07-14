using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VampariaSurvivors.Content.Items
{
    public  class VampariaChest : ModItem
    {
        public override void SetDefaults()
        {
            Item.SetNameOverride("Vamparia Chest");
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = 99;
            Item.consumable = true;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.autoReuse = false;
        }

        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            var lootPool = new List<(int itemType, int weight)>
            {
                (ModContent.ItemType<GarlicLvl1>(), 100),
                (ModContent.ItemType<MagicWandLvl1>(), 80),
                (ModContent.ItemType<RuneTracerLvl1>(), 60),
                (ModContent.ItemType<FireWandLvl1>(), 60),
                (ModContent.ItemType<boneLvl1>(), 80),
                //add other weapons
            };

            int totalWeight = 0;
            foreach (var item in lootPool)
            {
                totalWeight += item.weight;
            }

            int roll = Main.rand.Next(totalWeight);
            int currentWeight = 0;

            foreach (var item in lootPool)
            {
                currentWeight += item.weight;
                if (roll < currentWeight)
                {
                    player.QuickSpawnItem(player.GetSource_OpenItem(Type), item.itemType, 1);
                    break;
                }
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "Hint", "Right click to open"));
            tooltips.Add(new TooltipLine(Mod, "Contains", "Contains vampire hunting equipment"));
        }
    }
}
