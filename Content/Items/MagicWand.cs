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
    public class MagicWandLvl1 : ModItem
    {
        public int Level = 1;
        public override void SetDefaults()
        {
            Item.SetNameOverride("Magic Wand");
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
            Item.shoot = ModContent.ProjectileType<MagicWandControllerProjectile>();
            Item.shootSpeed = 0f;
        }

        public override bool CanUseItem(Player player)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI &&
                    Main.projectile[i].type == ModContent.ProjectileType<MagicWandControllerProjectile>())
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
            damage = (int)(damage * (1 + 0.25f * (Level - 1)));
            int shootCooldown = 60;
            int projectileCount = 2;
            int projectilePenetration = 1;

            if (Level >= 2) projectileCount = 2;
            if (Level >= 3) shootCooldown = 48;
            if (Level >= 4) projectileCount = 3;
            if (Level >= 5) damage += 20;
            if (Level >= 6) projectileCount = 4;
            if (Level >= 7) projectilePenetration = 2;
            if (Level >= 8) damage += 20;

            tooltips.Clear();
            tooltips.Add(new TooltipLine(Mod, "Name", "Magic Wand"));
            tooltips.Add(new TooltipLine(Mod, "Level", "Level " + Level));
            tooltips.Add(new TooltipLine(Mod, "ManaCost", "Mana Cost: " + Main.LocalPlayer.GetManaCost(Item)));
            tooltips.Add(new TooltipLine(Mod, "Manamaintenance", "Mana Maintenance: " + Main.LocalPlayer.GetManaCost(Item) / 2));
            tooltips.Add(new TooltipLine(Mod, "Damage", "Damage: " + damage));
            tooltips.Add(new TooltipLine(Mod, "ProjectileCount", "Projecttile Count: " + projectileCount));
            tooltips.Add(new TooltipLine(Mod, "ProjectilePenetration", "Projecttile Penetration: " + projectilePenetration));
            tooltips.Add(new TooltipLine(Mod, "Cooldown", "Cooldown: " + shootCooldown / 60 + "s"));
            tooltips.Add(new TooltipLine(Mod, "Description", "Toggleable Personal Sentry"));
            base.ModifyTooltips(tooltips);
        }

        public override bool OnPickup(Player player)
        {
            for (int i = 0; i < player.inventory.Length; i++)
            {
                Item inventoryItem = player.inventory[i];

                if (inventoryItem.IsAir)
                    continue;

                if (inventoryItem.ModItem is MagicWandLvl1 invenWand)
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
                1 => ModContent.ItemType<MagicWandLvl1>(),
                2 => ModContent.ItemType<MagicWandLvl2>(),
                3 => ModContent.ItemType<MagicWandLvl3>(),
                4 => ModContent.ItemType<MagicWandLvl4>(),
                5 => ModContent.ItemType<MagicWandLvl5>(),
                6 => ModContent.ItemType<MagicWandLvl6>(),
                7 => ModContent.ItemType<MagicWandLvl7>(),
                8 => ModContent.ItemType<MagicWandLvl8>(),
                _ => level > 8 ? ModContent.ItemType<MagicWandLvl8>() : -1
            };
        }
    }

    public class MagicWandLvl2 : MagicWandLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 2;
        }
    }
    public class MagicWandLvl3 : MagicWandLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 3;
        }
    }
    public class MagicWandLvl4 : MagicWandLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 4;
        }
    }
    public class MagicWandLvl5 : MagicWandLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 5;
        }
    }
    public class MagicWandLvl6 : MagicWandLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 6;
        }
    }
    public class MagicWandLvl7 : MagicWandLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 7;
        }
    }
    public class MagicWandLvl8 : MagicWandLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 8;
        }
    }
}
