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

    }

    public class FireWandLvl2 : FireWandLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 2;
        }
    }
    public class FireWandLvl3 : FireWandLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 3;
        }
    }
    public class FireWandLvl4 : FireWandLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 4;
        }
    }
    public class FireWandLvl5 : FireWandLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 5;
        }
    }
    public class FireWandLvl6 : FireWandLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 6;
        }
    }
    public class FireWandLvl7 : FireWandLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 7;
        }
    }
    public class FireWandLvl8 : FireWandLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 8;
        }
    }
}
