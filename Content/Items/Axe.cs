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
    public class AxeLvl1 : ModItem
    {
        public int Level = 1;
        public override string Texture => "VampariaSurvivors/Content/Items/Axe";

        public override void SetDefaults()
        {
            Item.SetNameOverride("Axe");
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
            Item.shoot = ModContent.ProjectileType<AxeControllerProjectile>();
            Item.shootSpeed = 0f;
        }

        public override bool CanUseItem(Player player)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI &&
                    Main.projectile[i].type == ModContent.ProjectileType<AxeControllerProjectile>())
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
            int projectileCount = 2;
            int projectilePenetration = 3;
            int shootCooldown = 60;

            if (Level >= 2) projectileCount = 3;
            if (Level >= 3) damage += 20;
            if (Level >= 4) projectilePenetration += 2;
            if (Level >= 5) projectileCount = 4;
            if (Level >= 6) damage += 20;
            if (Level >= 7) projectilePenetration += 2;
            if (Level >= 8) damage += 20;

            tooltips.Clear();
            tooltips.Add(new TooltipLine(Mod, "Name", "Axe"));
            tooltips.Add(new TooltipLine(Mod, "Level", "Level " + Level));
            tooltips.Add(new TooltipLine(Mod, "ManaCost", "Mana Cost: " + Main.LocalPlayer.GetManaCost(Item)));
            tooltips.Add(new TooltipLine(Mod, "Manamaintenance", "Mana Maintenance: " + Main.LocalPlayer.GetManaCost(Item) / 2));
            tooltips.Add(new TooltipLine(Mod, "Damage", "Damage: " + damage));
            tooltips.Add(new TooltipLine(Mod, "ProjectileCount", "Projectile Count: " + projectileCount));
            tooltips.Add(new TooltipLine(Mod, "ProjectilePenetration", "Projectile Penetration: " + projectilePenetration));
            tooltips.Add(new TooltipLine(Mod, "Cooldown", "Cooldown: " + shootCooldown / 60 + "s"));
            tooltips.Add(new TooltipLine(Mod, "Description", "Throws axes above that arc downward"));
            base.ModifyTooltips(tooltips);
        }

        public override bool OnPickup(Player player)
        {
            for (int i = 0; i < player.inventory.Length; i++)
            {
                Item inventoryItem = player.inventory[i];

                if (inventoryItem.IsAir)
                    continue;

                if (inventoryItem.ModItem is AxeLvl1 invenAxe)
                {
                    if (invenAxe.Level < 8 && this.Level < 8)
                    {
                        int combinedLevel = invenAxe.Level + this.Level;

                        inventoryItem.TurnToAir();

                        int newAxeType = GetLevel(combinedLevel);

                        if (newAxeType != -1)
                        {
                            player.QuickSpawnItem(player.GetSource_ItemUse(Item), newAxeType);
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
                1 => ModContent.ItemType<AxeLvl1>(),
                2 => ModContent.ItemType<AxeLvl2>(),
                3 => ModContent.ItemType<AxeLvl3>(),
                4 => ModContent.ItemType<AxeLvl4>(),
                5 => ModContent.ItemType<AxeLvl5>(),
                6 => ModContent.ItemType<AxeLvl6>(),
                7 => ModContent.ItemType<AxeLvl7>(),
                8 => ModContent.ItemType<AxeLvl8>(),
                _ => level > 8 ? ModContent.ItemType<AxeLvl8>() : -1
            };
        }
    }

    public class AxeLvl2 : AxeLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 2;
        }
    }

    public class AxeLvl3 : AxeLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 3;
        }
    }

    public class AxeLvl4 : AxeLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 4;
        }
    }

    public class AxeLvl5 : AxeLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 5;
        }
    }

    public class AxeLvl6 : AxeLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 6;
        }
    }

    public class AxeLvl7 : AxeLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 7;
        }
    }

    public class AxeLvl8 : AxeLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 8;
        }
    }
}