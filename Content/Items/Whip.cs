using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VampariaSurvivors.Content.Projectile;

namespace VampariaSurvivors.Content.Items
{
    public class WhipLvl1 : ModItem
    {
        public int Level = 1;
        public override string Texture => "VampariaSurvivors/Content/Items/Whip";
        public override void SetDefaults()
        {
            Item.SetNameOverride("Whip");
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.DamageType = DamageClass.Magic;
            Item.damage = 0;
            Item.knockBack = 1.5f;
            Item.mana = 20;
            Item.noMelee = true;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<WhipControllerProjectile>();
            Item.shootSpeed = 0f;
        }

        public override bool CanUseItem(Player player)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI &&
                    Main.projectile[i].type == ModContent.ProjectileType<WhipControllerProjectile>())
                {
                    Main.projectile[i].Kill();
                    return false;
                }
            }
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int damage = 20;
            int area = 100; // Base area percentage

            if (Level >= 3) damage += 5;
            if (Level >= 4) { area += 10; damage += 5; }
            if (Level >= 5) damage += 5;
            if (Level >= 6) { area += 10; damage += 5; }
            if (Level >= 7) damage += 5;
            if (Level >= 8) damage += 5;

            tooltips.Clear();
            tooltips.Add(new TooltipLine(Mod, "Name", "Whip"));
            tooltips.Add(new TooltipLine(Mod, "Level", "Level " + Level));
            tooltips.Add(new TooltipLine(Mod, "ManaCost", "Mana Cost: " + Main.LocalPlayer.GetManaCost(Item)));
            tooltips.Add(new TooltipLine(Mod, "Manamaintenance", "Mana Maintenance: " + Main.LocalPlayer.GetManaCost(Item) / 2));
            tooltips.Add(new TooltipLine(Mod, "Damage", "Damage: " + damage));
            tooltips.Add(new TooltipLine(Mod, "Area", "Area: " + area + "%"));
            tooltips.Add(new TooltipLine(Mod, "Description", "Horizontal slashing attack"));
            base.ModifyTooltips(tooltips);
        }

        public override bool OnPickup(Player player)
        {
            for (int i = 0; i < player.inventory.Length; i++)
            {
                Item inventoryItem = player.inventory[i];

                if (inventoryItem.IsAir)
                    continue;

                if (inventoryItem.ModItem is WhipLvl1 invenWand)
                {
                    if (invenWand.Level < 8 && this.Level < 8)
                    {
                        int combinedLevel = invenWand.Level + this.Level;

                        inventoryItem.TurnToAir();

                        int newWandType = GetLevel(combinedLevel);

                        if (newWandType != -1)
                        {
                            player.QuickSpawnItem(player.GetSource_ItemUse(Item), newWandType);
                        }

                        return false;
                    }
                }
            }

            return true;
        }

        private int GetLevel(int level)
        {
            return level switch
            {
                1 => ModContent.ItemType<WhipLvl1>(),
                2 => ModContent.ItemType<WhipLvl2>(),
                3 => ModContent.ItemType<WhipLvl3>(),
                4 => ModContent.ItemType<WhipLvl4>(),
                5 => ModContent.ItemType<WhipLvl5>(),
                6 => ModContent.ItemType<WhipLvl6>(),
                7 => ModContent.ItemType<WhipLvl7>(),
                8 => ModContent.ItemType<WhipLvl8>(),
                _ => level > 8 ? ModContent.ItemType<WhipLvl8>() : -1
            };
        }
    }

    public class WhipLvl2 : WhipLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 2;
        }
    }
    public class WhipLvl3 : WhipLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 3;
        }
    }
    public class WhipLvl4 : WhipLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 4;
        }
    }
    public class WhipLvl5 : WhipLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 5;
        }
    }
    public class WhipLvl6 : WhipLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 6;
        }
    }
    public class WhipLvl7 : WhipLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 7;
        }
    }
    public class WhipLvl8 : WhipLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 8;
        }
    }
}
