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
    public class MagicWand : ModItem
    {
        public int Level = 1;
        public  int fireTimer = 0;
        public int FIRE_COOLDOWN = 72;
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
            Item.shoot = ModContent.ProjectileType<MagicWandProjectile>();
            Item.shootSpeed = 0f;
        }

        public override bool CanUseItem(Player player)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI &&
                    Main.projectile[i].type == ModContent.ProjectileType<MagicWandProjectile>())
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
            tooltips.Add(new TooltipLine(Mod, "ProjectileCount", "Projecttile Count: 1"));
            tooltips.Add(new TooltipLine(Mod, "Cooldown", "Cooldown: 1.2s"));
            tooltips.Add(new TooltipLine(Mod, "Description", "Toggleable Personal Sentry"));
            base.ModifyTooltips(tooltips);
        }

    }
}
