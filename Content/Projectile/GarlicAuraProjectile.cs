using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using VampariaSurvivors.Content.Items;

namespace VampariaSurvivors.Content.Projectile
{
    // i did this one first so i could be the structure no thinking about it, its nothing like the rest of the weapons
    public class GarlicAuraProjectile : ModProjectile
    {
        private int manaTimer = 0;
        private float currentRotation = 0f;
        private float rotationSpeed = 0.02f;

        private Dictionary<int, int> enemyCooldowns = new Dictionary<int, int>();
        private float ManaCost = 10f;

        private WeaponStats weaponStats;
        private WeaponStats originalWeaponStats;

        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse itemUse)
            {
                ManaCost = Main.player[Projectile.owner].GetManaCost(itemUse.Item) / 2;

                if (itemUse.Item.ModItem is VSWeapon weapon)
                {
                    weaponStats = weapon.GetWeaponStats();
                    originalWeaponStats = weapon.GetWeaponStats();
                }
                else
                {
                    weaponStats = new WeaponStats
                    {
                        Damage = 20,
                        Area = 3.5f,
                        ProjectileInterval = 78,
                        Knockback = 0f
                    };
                    originalWeaponStats = weaponStats;
                }
            }
        }

        public override void SetDefaults()
        {
            Projectile.width = 160;
            Projectile.height = 160;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 360000;
            Projectile.alpha = 0;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        private bool IsWeaponEquipped(Player player)
        {
            // Check if any Garlic weapon is in the player's inventory
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].ModItem is VSWeapon weapon && weapon.WeaponIdentifier == "Garlic")
                {
                    return true;
                }
            }
            return false;
        }

        private bool HasAccessoryStatsChanged(Player player)
        {
            // Find the current weapon and check if its current stats differ from when projectile was created
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].ModItem is VSWeapon weapon && weapon.WeaponIdentifier == "Garlic")
                {
                    WeaponStats currentStats = weapon.GetWeaponStats(player);
                    
                    // Compare key stats that affect weapon performance
                    return currentStats.Damage != weaponStats.Damage ||
                           Math.Abs(currentStats.Area - weaponStats.Area) > 0.001f ||
                           currentStats.ProjectileInterval != weaponStats.ProjectileInterval ||
                           Math.Abs(currentStats.Knockback - weaponStats.Knockback) > 0.001f;
                }
            }
            return false; // Weapon not found, let the IsWeaponEquipped check handle it
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.Center = player.Center;

            // Check if the garlic weapon is still equipped/in inventory
            if (!IsWeaponEquipped(player))
            {
                Projectile.Kill();
                return;
            }

            // Check if accessories changed and kill if weapon stats would be different
            if (HasAccessoryStatsChanged(player))
            {
                Projectile.Kill();
                return;
            }

            currentRotation += rotationSpeed;
            Projectile.rotation = currentRotation;

            manaTimer++;
            if (manaTimer >= 60)
            {
                if (player.statMana >= ManaCost)
                {
                    player.statMana -= (int)ManaCost;
                    manaTimer = 0;
                }
                else
                {
                    Projectile.Kill();
                    return;
                }
            }

            float auraRadius = (weaponStats.Area + 0.5f) * 16;

            List<int> expiredKeys = new List<int>();
            foreach (var kvp in enemyCooldowns)
            {
                enemyCooldowns[kvp.Key]--;
                if (enemyCooldowns[kvp.Key] <= 0)
                    expiredKeys.Add(kvp.Key);
            }
            foreach (int key in expiredKeys)
                enemyCooldowns.Remove(key);

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && !npc.dontTakeDamage)
                {
                    float distance = Vector2.Distance(npc.Center, Projectile.Center);
                    if (distance < auraRadius)
                    {
                        int uniqueKey = npc.whoAmI;

                        if (!enemyCooldowns.ContainsKey(uniqueKey))
                        {
                            int damage = weaponStats.Damage;
                            npc.StrikeNPC(npc.CalculateHitInfo(damage, 0, false, 0));

                            enemyCooldowns[uniqueKey] = weaponStats.ProjectileInterval;
                        }
                    }
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.Splash, Projectile.position);
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);

            float visualScale = weaponStats.Area;

            Main.EntitySpriteDraw(
                texture,
                drawPosition,
                null,
                Color.White * 0.8f,
                Projectile.rotation,
                origin,
                visualScale,
                SpriteEffects.None,
                0
            );

            return false;
        }
    }
}