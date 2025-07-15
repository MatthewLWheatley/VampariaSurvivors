using log4net.Core;
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
    public class RuneTracerLvl1 : ModItem
    {
        public int Level = 1;
        public override void SetDefaults()
        {
            Item.SetNameOverride("Rune Tracer");
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
            Item.shoot = ModContent.ProjectileType<RuneTracerControllerProjectile>();
            Item.shootSpeed = 0f;
        }

        public override bool CanUseItem(Player player)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI &&
                    Main.projectile[i].type == ModContent.ProjectileType<RuneTracerControllerProjectile>())
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
            int shootCooldown = 180;
            int projectileCount = 2;
            damage = (int)(damage * (1 + 0.25f * (Level - 1)));
            float projectileSpeed = 10f;
            int duration = 135;

            if (Level >= 2) damage += 10;
            if (Level >= 2) projectileSpeed *= 1.2f;
            if (Level >= 3) duration += 18;
            if (Level >= 3) damage += 10;
            if (Level >= 4) projectileCount = 2;
            if (Level >= 5) damage += 10;
            if (Level >= 5) projectileSpeed *= 1.2f;
            if (Level >= 6) duration += 18;
            if (Level >= 6) damage += 10;
            if (Level >= 7) projectileCount = 3;
            if (Level >= 8) duration += 30;

            tooltips.Clear();
            tooltips.Add(new TooltipLine(Mod, "Name", "RuneTracer"));
            tooltips.Add(new TooltipLine(Mod, "Level", "Level " + Level));
            tooltips.Add(new TooltipLine(Mod, "ManaCost", "Mana Cost: " + Main.LocalPlayer.GetManaCost(Item)));
            tooltips.Add(new TooltipLine(Mod, "Manamaintenance", "Mana Maintenance: " + Main.LocalPlayer.GetManaCost(Item) / 2));
            tooltips.Add(new TooltipLine(Mod, "Damage", "Damage: " + damage));
            tooltips.Add(new TooltipLine(Mod, "ProjectileCount", "Projecttile Count: " + projectileCount));
            tooltips.Add(new TooltipLine(Mod, "ProjectileDuration", "Projecttile Duration: " + duration / 60 + "s"));
            tooltips.Add(new TooltipLine(Mod, "ProjectileSpeed", "Projectile Speed: " + projectileSpeed + " tiles/s"));
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

                if (inventoryItem.ModItem is RuneTracerLvl1 inven)
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
                1 => ModContent.ItemType<RuneTracerLvl1>(),
                2 => ModContent.ItemType<RuneTracerLvl2>(),
                3 => ModContent.ItemType<RuneTracerLvl3>(),
                4 => ModContent.ItemType<RuneTracerLvl4>(),
                5 => ModContent.ItemType<RuneTracerLvl5>(),
                6 => ModContent.ItemType<RuneTracerLvl6>(),
                7 => ModContent.ItemType<RuneTracerLvl7>(),
                8 => ModContent.ItemType<RuneTracerLvl8>(),
                _ => level > 8 ? ModContent.ItemType<RuneTracerLvl8>() : -1
            };
        }
    }

    public class RuneTracerLvl2 : RuneTracerLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 2;
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<RuneTracerLvl1>(), 1)
                .AddIngredient(ModContent.ItemType<RuneTracerLvl1>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
    public class RuneTracerLvl3 : RuneTracerLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 3;
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<RuneTracerLvl2>(), 1)
                .AddIngredient(ModContent.ItemType<RuneTracerLvl1>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
    public class RuneTracerLvl4 : RuneTracerLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 4;
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<RuneTracerLvl3>(), 1)
                .AddIngredient(ModContent.ItemType<RuneTracerLvl1>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
    public class RuneTracerLvl5 : RuneTracerLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 5;
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<RuneTracerLvl4>(), 1)
                .AddIngredient(ModContent.ItemType<RuneTracerLvl1>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
    public class RuneTracerLvl6 : RuneTracerLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 6;
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<RuneTracerLvl5>(), 1)
                .AddIngredient(ModContent.ItemType<RuneTracerLvl1>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
    public class RuneTracerLvl7 : RuneTracerLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 7;
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<RuneTracerLvl6>(), 1)
                .AddIngredient(ModContent.ItemType<RuneTracerLvl1>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
    public class RuneTracerLvl8 : RuneTracerLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 8;
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<RuneTracerLvl7>(), 1)
                .AddIngredient(ModContent.ItemType<RuneTracerLvl1>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
