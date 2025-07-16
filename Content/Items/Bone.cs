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
    public class BoneLvl1 : ModItem
    {
        public int Level = 1;
        public override string Texture => "VampariaSurvivors/Content/Items/Bone";
        public override void SetDefaults()
        {
            Item.SetNameOverride("Bone");
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
            Item.shoot = ModContent.ProjectileType<BoneControllerProjectile>();
            Item.shootSpeed = 0f;
        }

        public override bool CanUseItem(Player player)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI &&
                    Main.projectile[i].type == ModContent.ProjectileType<BoneControllerProjectile>())
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
            int duration = 120;
            float speed = 12f;

            if (Level >= 2) duration += 16;
            if (Level >= 3) projectileCount += 1;
            if (Level >= 3) damage += 20;
            if (Level >= 4) speed *= 1.5f;
            if (Level >= 5) projectileCount += 1;
            if (Level >= 5) damage += 20;
            if (Level >= 6) duration += 16;
            if (Level >= 7) damage += 20;
            if (Level >= 8) duration += 16;
            if (Level >= 8) speed *= 1.5f;

            tooltips.Clear();
            tooltips.Add(new TooltipLine(Mod, "Name", "Bone"));
            tooltips.Add(new TooltipLine(Mod, "Level", "Level " + Level));
            tooltips.Add(new TooltipLine(Mod, "ManaCost", "Mana Cost: " + Main.LocalPlayer.GetManaCost(Item)));
            tooltips.Add(new TooltipLine(Mod, "Manamaintenance", "Mana Maintenance: " + Main.LocalPlayer.GetManaCost(Item) / 2));
            tooltips.Add(new TooltipLine(Mod, "Damage", "Damage: " + damage));
            tooltips.Add(new TooltipLine(Mod, "ProjectileCount", "Projecttile Count: " + projectileCount));
            tooltips.Add(new TooltipLine(Mod, "ProjectileDuration", "Projecttile Duration: " + duration / 60 + "s"));
            tooltips.Add(new TooltipLine(Mod, "ProjectileSpeed", "Projectile Speed: " + speed));
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

                if (inventoryItem.ModItem is BoneLvl1 inven)
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
                1 => ModContent.ItemType<BoneLvl1>(),
                2 => ModContent.ItemType<BoneLvl2>(),
                3 => ModContent.ItemType<BoneLvl3>(),
                4 => ModContent.ItemType<BoneLvl4>(),
                5 => ModContent.ItemType<BoneLvl5>(),
                6 => ModContent.ItemType<BoneLvl6>(),
                7 => ModContent.ItemType<BoneLvl7>(),
                8 => ModContent.ItemType<BoneLvl8>(),
                _ => level > 8 ? ModContent.ItemType<BoneLvl8>() : -1
            };
        }
    }

    public class BoneLvl2 : BoneLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 2;
        }

        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<BoneLvl1>(), 2)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
    public class BoneLvl3 : BoneLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 3;
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<BoneLvl1>(), 1)
                .AddIngredient(ModContent.ItemType<BoneLvl2>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
    public class BoneLvl4 : BoneLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 4;
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<BoneLvl1>(), 1)
                .AddIngredient(ModContent.ItemType<BoneLvl3>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
    public class BoneLvl5 : BoneLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 5;
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<BoneLvl1>(), 1)
                .AddIngredient(ModContent.ItemType<BoneLvl4>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
    public class BoneLvl6 : BoneLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 6;
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<BoneLvl1>(), 1)
                .AddIngredient(ModContent.ItemType<BoneLvl5>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
    public class BoneLvl7 : BoneLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 7;
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<BoneLvl1>(), 1)
                .AddIngredient(ModContent.ItemType<BoneLvl6>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
    public class BoneLvl8 : BoneLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 8;
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<BoneLvl1>(), 1)
                .AddIngredient(ModContent.ItemType<BoneLvl7>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
