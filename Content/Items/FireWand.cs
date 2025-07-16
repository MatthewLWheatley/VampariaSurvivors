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
    public class FireWandLvl1 : ModItem
    {
        public int Level = 1;
        public override string Texture => "VampariaSurvivors/Content/Items/FireWand";
        public override void SetDefaults()
        {
            Item.SetNameOverride("Fire Wand");
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
            Item.shoot = ModContent.ProjectileType<FireWandControllerProjectile>();
            Item.shootSpeed = 0f;
        }

        public override bool CanUseItem(Player player)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI &&
                    Main.projectile[i].type == ModContent.ProjectileType<FireWandControllerProjectile>())
                {
                    Main.projectile[i].Kill();
                    return false;
                }
            }
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int damage = 40;
            int shootCooldown = 180;
            int projectileCount = 3;

            float projectileSpeed = 3f;


            damage = (int)(damage * (1 + 0.25f * (Level - 1)));

            if (Level >= 2) damage += 20;
            if (Level >= 3) damage += 20;
            if (Level >= 3) projectileSpeed *= 1.2f;
            if (Level >= 4) damage += 20;
            if (Level >= 5) damage += 20;
            if (Level >= 5) projectileSpeed *= 1.2f;
            if (Level >= 6) damage += 20;
            if (Level >= 7) damage += 20;
            if (Level >= 7) projectileSpeed *= 1.2f;
            if (Level >= 8) damage += 20;

            tooltips.Clear();
            tooltips.Add(new TooltipLine(Mod, "Name", "Fire Wand"));
            tooltips.Add(new TooltipLine(Mod, "Level", "Level " + Level));
            tooltips.Add(new TooltipLine(Mod, "ManaCost", "Mana Cost: " + Main.LocalPlayer.GetManaCost(Item)));
            tooltips.Add(new TooltipLine(Mod, "Manamaintenance", "Mana Maintenance: " + Main.LocalPlayer.GetManaCost(Item) / 2));
            tooltips.Add(new TooltipLine(Mod, "Damage", "Damage: " + damage));
            tooltips.Add(new TooltipLine(Mod, "ProjectileCount", "Projecttile Count: " + projectileCount));
            tooltips.Add(new TooltipLine(Mod, "ProjectileSpeed", "Projecttile Speed: " + Math.Round(projectileSpeed, 1)));
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

                if (inventoryItem.ModItem is FireWandLvl1 inven)
                {
                    if (inven.Level < 8 && this.Level < 8)
                    {
                        int combinedLevel = inven.Level + this.Level;

                        inventoryItem.TurnToAir();

                        int newType = GetLevel(combinedLevel);

                        if (newType != -1)
                        {
                            player.QuickSpawnItem(player.GetSource_ItemUse(Item), newType);
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
                1 => ModContent.ItemType<FireWandLvl1>(),
                2 => ModContent.ItemType<FireWandLvl2>(),
                3 => ModContent.ItemType<FireWandLvl3>(),
                4 => ModContent.ItemType<FireWandLvl4>(),
                5 => ModContent.ItemType<FireWandLvl5>(),
                6 => ModContent.ItemType<FireWandLvl6>(),
                7 => ModContent.ItemType<FireWandLvl7>(),
                8 => ModContent.ItemType<FireWandLvl8>(),
                _ => level > 8 ? ModContent.ItemType<FireWandLvl8>() : -1
            };
        }
    }

    public class FireWandLvl2 : FireWandLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 2;
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<FireWandLvl1>(), 1)
                .AddIngredient(ModContent.ItemType<FireWandLvl1>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
    public class FireWandLvl3 : FireWandLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 3;
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<FireWandLvl1>(), 1)
                .AddIngredient(ModContent.ItemType<FireWandLvl2>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
    public class FireWandLvl4 : FireWandLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 4;
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<FireWandLvl1>(), 1)
                .AddIngredient(ModContent.ItemType<FireWandLvl3>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
    public class FireWandLvl5 : FireWandLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 5;
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<FireWandLvl1>(), 1)
                .AddIngredient(ModContent.ItemType<FireWandLvl4>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
    public class FireWandLvl6 : FireWandLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 6;
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<FireWandLvl1>(), 1)
                .AddIngredient(ModContent.ItemType<FireWandLvl5>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
    public class FireWandLvl7 : FireWandLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 7;
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<FireWandLvl1>(), 1)
                .AddIngredient(ModContent.ItemType<FireWandLvl6>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
    public class FireWandLvl8 : FireWandLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 8;
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<FireWandLvl1>(), 1)
                .AddIngredient(ModContent.ItemType<FireWandLvl7>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
