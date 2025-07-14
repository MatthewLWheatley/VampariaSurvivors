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
    public class boneLvl1 : ModItem
    {
        public int Level = 1;
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
            Item.shoot = ModContent.ProjectileType<boneControllerProjectile>();
            Item.shootSpeed = 0f;
        }

        public override bool CanUseItem(Player player)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI &&
                    Main.projectile[i].type == ModContent.ProjectileType<boneControllerProjectile>())
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

    }

    public class boneLvl2 : boneLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 2;
        }
    }
    public class boneLvl3 : boneLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 3;
        }
    }
    public class boneLvl4 : boneLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 4;
        }
    }
    public class boneLvl5 : boneLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 5;
        }
    }
    public class boneLvl6 : boneLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 6;
        }
    }
    public class boneLvl7 : boneLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 7;
        }
    }
    public class boneLvl8 : boneLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 8;
        }
    }
}
