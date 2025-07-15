using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using VampariaSurvivors.Content.Projectile;

namespace VampariaSurvivors.Content.Items
{

    public class GarlicLvl1 : ModItem
    {

        public int Level = 1;
        public override void SetDefaults()
        {
            Item.SetNameOverride("Garlic");
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.DamageType = DamageClass.Magic;
            Item.damage = 0;
            Item.knockBack = 0f;
            Item.mana = 20;
            Item.noMelee = true;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<GarlicAuraProjectile>();
            Item.shootSpeed = 0f;
        }

        public override bool CanUseItem(Player player)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI &&
                    Main.projectile[i].type == ModContent.ProjectileType<GarlicAuraProjectile>())
                {
                    Main.projectile[i].Kill();
                    return false;
                }
            }
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            float Damage = 20;
            float scale = 3.5f;
            int DAMAGE_COOLDOWN = 78;

            Damage = (int)(Damage*(1+.2*(Level-1)));
            if (Level > 1) scale = 3.5f * 1.4f;
            if (Level > 2) DAMAGE_COOLDOWN = 72;
            if (Level > 3) scale = 3.5f * 1.6f;
            if (Level > 4) DAMAGE_COOLDOWN = 66;
            if (Level > 5) scale = 3.5f * 1.8f;
            if (Level > 6) DAMAGE_COOLDOWN = 60;
            if (Level > 7) scale = 3.5f * 2.0f;
            tooltips.Clear();
            tooltips.Add(new TooltipLine(Mod, "Name", "Garlic"));
            tooltips.Add(new TooltipLine(Mod, "Level", "Level " + Level));
            tooltips.Add(new TooltipLine(Mod, "ManaCost", "Mana Cost: " + Main.LocalPlayer.GetManaCost(Item)));
            tooltips.Add(new TooltipLine(Mod, "Manamaintenance", "Mana Maintenance: " + Main.LocalPlayer.GetManaCost(Item) / 2));
            tooltips.Add(new TooltipLine(Mod, "Damage", "Damage: " + Damage));
            tooltips.Add(new TooltipLine(Mod, "Area", "Area: " + (scale + 0.5f) + " blocks"));
            tooltips.Add(new TooltipLine(Mod, "Cooldown", "Cooldown: " + DAMAGE_COOLDOWN / 60 + "s"));
            tooltips.Add(new TooltipLine(Mod, "Description", "Toggleable protective aura"));
            base.ModifyTooltips(tooltips);
        }
        
        public override bool OnPickup(Player player)
        {
            for (int i = 0; i < player.inventory.Length; i++)
            {
                Item inventoryItem = player.inventory[i];

                if (inventoryItem.IsAir)
                    continue;

                if (inventoryItem.ModItem is GarlicLvl1 inven)
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
                1 => ModContent.ItemType<GarlicLvl1>(),
                2 => ModContent.ItemType<GarlicLvl2>(),
                3 => ModContent.ItemType<GarlicLvl3>(),
                4 => ModContent.ItemType<GarlicLvl4>(),
                5 => ModContent.ItemType<GarlicLvl5>(),
                6 => ModContent.ItemType<GarlicLvl6>(),
                7 => ModContent.ItemType<GarlicLvl7>(),
                8 => ModContent.ItemType<GarlicLvl8>(),
                _ => level > 8 ? ModContent.ItemType<GarlicLvl8>() : -1
            };
        }
    }
    public class GarlicLvl2 : GarlicLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 2;
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<GarlicLvl1>(), 1)
                .AddIngredient(ModContent.ItemType<GarlicLvl1>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
    public class GarlicLvl3 : GarlicLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 3;
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<GarlicLvl1>(), 1)
                .AddIngredient(ModContent.ItemType<GarlicLvl2>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
    public class GarlicLvl4 : GarlicLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 4;
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<GarlicLvl1>(), 1)
                .AddIngredient(ModContent.ItemType<GarlicLvl3>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
    public class GarlicLvl5 : GarlicLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 5;
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<GarlicLvl1>(), 1)
                .AddIngredient(ModContent.ItemType<GarlicLvl4>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
    public class GarlicLvl6 : GarlicLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 6;
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<GarlicLvl1>(), 1)
                .AddIngredient(ModContent.ItemType<GarlicLvl5>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
    public class GarlicLvl7 : GarlicLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 7;
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<GarlicLvl1>(), 1)
                .AddIngredient(ModContent.ItemType<GarlicLvl6>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
    public class GarlicLvl8 : GarlicLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 8;
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<GarlicLvl1>(), 1)
                .AddIngredient(ModContent.ItemType<GarlicLvl7>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}