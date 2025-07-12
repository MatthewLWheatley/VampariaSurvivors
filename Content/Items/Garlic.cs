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
            tooltips.Clear();
            tooltips.Add(new TooltipLine(Mod, "Level", "Level 1"));
            tooltips.Add(new TooltipLine(Mod, "ManaCost", "Mana Cost: " + Main.LocalPlayer.GetManaCost(Item)));
            tooltips.Add(new TooltipLine(Mod, "Manamaintenance", "Mana Maintenance: " + Main.LocalPlayer.GetManaCost(Item) / 2));
            tooltips.Add(new TooltipLine(Mod, "Damage", "Damage: 20"));
            tooltips.Add(new TooltipLine(Mod, "Area", "Area: 4 blocks"));
            tooltips.Add(new TooltipLine(Mod, "Cooldown", "Cooldown: 1.3s"));
            tooltips.Add(new TooltipLine(Mod, "Description", "Toggleable protective aura"));
            base.ModifyTooltips(tooltips);
        }

    }
    public class GarlicLvl2 : ModItem
    {
        public int Level = 2;
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

        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<GarlicLvl1>(), 2)
                .AddTile(TileID.WorkBenches)
                .Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Clear();
            tooltips.Add(new TooltipLine(Mod, "Level", "Level 2"));
            tooltips.Add(new TooltipLine(Mod, "ManaCost", "Mana Cost: " + Main.LocalPlayer.GetManaCost(Item)));
            tooltips.Add(new TooltipLine(Mod, "Manamaintenance", "Mana Maintenance: " + Main.LocalPlayer.GetManaCost(Item) / 2));
            tooltips.Add(new TooltipLine(Mod, "Damage", "Damage: "+ (int)(20 * (1 + .2 * (Level - 1))) ));
            tooltips.Add(new TooltipLine(Mod, "Area", "Area: 5.5"));
            tooltips.Add(new TooltipLine(Mod, "Cooldown", "Cooldown: 1.3s"));
            tooltips.Add(new TooltipLine(Mod, "Description", "Toggleable protective aura"));
            base.ModifyTooltips(tooltips);
        }
    }
    public class GarlicLvl3 : ModItem
    {
        public int Level = 3;
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

        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<GarlicLvl1>(), 1)
                .AddIngredient(ModContent.ItemType<GarlicLvl2>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<GarlicLvl2>(), 2)
                .AddTile(TileID.WorkBenches)
                .Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Clear();
            tooltips.Add(new TooltipLine(Mod, "Level", "Level 3"));
            tooltips.Add(new TooltipLine(Mod, "ManaCost", "Mana Cost: " + Main.LocalPlayer.GetManaCost(Item)));
            tooltips.Add(new TooltipLine(Mod, "Manamaintenance", "Mana Maintenance: " + Main.LocalPlayer.GetManaCost(Item) / 2));
            tooltips.Add(new TooltipLine(Mod, "Damage", "Damage: " + (int)(20 * (1 + .2 * (Level - 1)))));
            tooltips.Add(new TooltipLine(Mod, "Area", "Area: 5.5"));
            tooltips.Add(new TooltipLine(Mod, "Cooldown", "Cooldown: 1.2s"));
            tooltips.Add(new TooltipLine(Mod, "Description", "Toggleable protective aura"));
            base.ModifyTooltips(tooltips);
        }
    }
    public class GarlicLvl4 : ModItem
    {
        public int Level = 4;
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
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<GarlicLvl1>(), 1)
                .AddIngredient(ModContent.ItemType<GarlicLvl3>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<GarlicLvl2>(), 1)
                .AddIngredient(ModContent.ItemType<GarlicLvl3>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<GarlicLvl3>(), 2)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Clear();
            tooltips.Add(new TooltipLine(Mod, "Level", "Level 4"));
            tooltips.Add(new TooltipLine(Mod, "ManaCost", "Mana Cost: " + Main.LocalPlayer.GetManaCost(Item)));
            tooltips.Add(new TooltipLine(Mod, "Manamaintenance", "Mana Maintenance: " + Main.LocalPlayer.GetManaCost(Item) / 2));
            tooltips.Add(new TooltipLine(Mod, "Damage", "Damage: " + (int)(20 * (1 + .2 * (Level - 1)))));
            tooltips.Add(new TooltipLine(Mod, "Area", "Area: 5.6"));
            tooltips.Add(new TooltipLine(Mod, "Cooldown", "Cooldown: 1.3s"));
            tooltips.Add(new TooltipLine(Mod, "Description", "Toggleable protective aura"));
            base.ModifyTooltips(tooltips);
        }
    }
    public class GarlicLvl5 : ModItem
    {
        public int Level = 5;
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
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<GarlicLvl1>(), 1)
                .AddIngredient(ModContent.ItemType<GarlicLvl4>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<GarlicLvl2>(), 1)
                .AddIngredient(ModContent.ItemType<GarlicLvl4>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<GarlicLvl3>(), 1)
                .AddIngredient(ModContent.ItemType<GarlicLvl4>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<GarlicLvl4>(), 2)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Clear();
            tooltips.Add(new TooltipLine(Mod, "Level", "Level 5"));
            tooltips.Add(new TooltipLine(Mod, "ManaCost", "Mana Cost: " + Main.LocalPlayer.GetManaCost(Item)));
            tooltips.Add(new TooltipLine(Mod, "Manamaintenance", "Mana Maintenance: " + Main.LocalPlayer.GetManaCost(Item) / 2));
            tooltips.Add(new TooltipLine(Mod, "Damage", "Damage: " + (int)(20 * (1 + .2 * (Level - 1)))));
            tooltips.Add(new TooltipLine(Mod, "Area", "Area: 5.6"));
            tooltips.Add(new TooltipLine(Mod, "Cooldown", "Cooldown: 1.1s"));
            tooltips.Add(new TooltipLine(Mod, "Description", "Toggleable protective aura"));
            base.ModifyTooltips(tooltips);
        }
    }
    public class GarlicLvl6 : ModItem
    {
        public int Level = 6;
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
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<GarlicLvl1>(), 1)
                .AddIngredient(ModContent.ItemType<GarlicLvl5>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<GarlicLvl2>(), 1)
                .AddIngredient(ModContent.ItemType<GarlicLvl5>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<GarlicLvl3>(), 1)
                .AddIngredient(ModContent.ItemType<GarlicLvl5>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<GarlicLvl4>(), 1)
                .AddIngredient(ModContent.ItemType<GarlicLvl5>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<GarlicLvl5>(), 2)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Clear();
            tooltips.Add(new TooltipLine(Mod, "Level", "Level 6"));
            tooltips.Add(new TooltipLine(Mod, "ManaCost", "Mana Cost: " + Main.LocalPlayer.GetManaCost(Item)));
            tooltips.Add(new TooltipLine(Mod, "Manamaintenance", "Mana Maintenance: " + Main.LocalPlayer.GetManaCost(Item) / 2));
            tooltips.Add(new TooltipLine(Mod, "Damage", "Damage: " + (int)(20 * (1 + .2 * (Level - 1)))));
            tooltips.Add(new TooltipLine(Mod, "Area", "Area: 6.3"));
            tooltips.Add(new TooltipLine(Mod, "Cooldown", "Cooldown: 1.3s"));
            tooltips.Add(new TooltipLine(Mod, "Description", "Toggleable protective aura"));
            base.ModifyTooltips(tooltips);
        }
    }
    public class GarlicLvl7 : ModItem
    {
        public int Level = 7;
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
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<GarlicLvl1>(), 1)
                .AddIngredient(ModContent.ItemType<GarlicLvl6>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<GarlicLvl2>(), 1)
                .AddIngredient(ModContent.ItemType<GarlicLvl6>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<GarlicLvl3>(), 1)
                .AddIngredient(ModContent.ItemType<GarlicLvl6>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<GarlicLvl4>(), 1)
                .AddIngredient(ModContent.ItemType<GarlicLvl6>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<GarlicLvl5>(), 1)
                .AddIngredient(ModContent.ItemType<GarlicLvl6>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<GarlicLvl6>(), 2)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Clear();
            tooltips.Add(new TooltipLine(Mod, "Level", "Level 7"));
            tooltips.Add(new TooltipLine(Mod, "ManaCost", "Mana Cost: " + Main.LocalPlayer.GetManaCost(Item)));
            tooltips.Add(new TooltipLine(Mod, "Manamaintenance", "Mana Maintenance: " + Main.LocalPlayer.GetManaCost(Item) / 2));
            tooltips.Add(new TooltipLine(Mod, "Damage", "Damage: " + (int)(20 * (1 + .2 * (Level - 1)))));
            tooltips.Add(new TooltipLine(Mod, "Area", "Area: 6.3"));
            tooltips.Add(new TooltipLine(Mod, "Cooldown", "Cooldown: 1.3s"));
            tooltips.Add(new TooltipLine(Mod, "Description", "Toggleable protective aura"));
            base.ModifyTooltips(tooltips);
        }
    }
    public class GarlicLvl8 : ModItem
    {
        public int Level = 8;
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
        
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<GarlicLvl1>(), 1)
                .AddIngredient(ModContent.ItemType<GarlicLvl7>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<GarlicLvl2>(), 1)
                .AddIngredient(ModContent.ItemType<GarlicLvl7>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<GarlicLvl3>(), 1)
                .AddIngredient(ModContent.ItemType<GarlicLvl7>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<GarlicLvl4>(), 1)
                .AddIngredient(ModContent.ItemType<GarlicLvl7>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<GarlicLvl5>(), 1)
                .AddIngredient(ModContent.ItemType<GarlicLvl7>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<GarlicLvl6>(), 1)
                .AddIngredient(ModContent.ItemType<GarlicLvl7>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<GarlicLvl7>(), 2)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Clear();
            tooltips.Add(new TooltipLine(Mod, "Level", "Level 8"));
            tooltips.Add(new TooltipLine(Mod, "ManaCost", "Mana Cost: " + Main.LocalPlayer.GetManaCost(Item)));
            tooltips.Add(new TooltipLine(Mod, "Manamaintenance", "Mana Maintenance: " + Main.LocalPlayer.GetManaCost(Item) / 2));
            tooltips.Add(new TooltipLine(Mod, "Damage", "Damage: " + (int)(20 * (1 + .2 * (Level - 1)))));
            tooltips.Add(new TooltipLine(Mod, "Area", "Area: 7"));
            tooltips.Add(new TooltipLine(Mod, "Cooldown", "Cooldown: 1s"));
            tooltips.Add(new TooltipLine(Mod, "Description", "Toggleable protective aura"));
            base.ModifyTooltips(tooltips);
        }
    }
}