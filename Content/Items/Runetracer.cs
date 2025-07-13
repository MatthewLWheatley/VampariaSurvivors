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
            tooltips.Add(new TooltipLine(Mod, "Name", "RuneTracer"));
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

    }

    public class RuneTracerLvl2 : RuneTracerLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 2;
        }
    }
    public class RuneTracerLvl3 : RuneTracerLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 3;
        }
    }
    public class RuneTracerLvl4 : RuneTracerLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 4;
        }
    }
    public class RuneTracerLvl5 : RuneTracerLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 5;
        }
    }
    public class RuneTracerLvl6 : RuneTracerLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 6;
        }
    }
    public class RuneTracerLvl7 : RuneTracerLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 7;
        }
    }
    public class RuneTracerLvl8 : RuneTracerLvl1
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Level = 8;
        }
    }
}
