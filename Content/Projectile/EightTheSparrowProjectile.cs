using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using VampariaSurvivors.Content.Items;

namespace VampariaSurvivors.Content.Projectile
{
    public class EightTheSparrowControllerProjectile : ModProjectile
    {
        private int manaTimer = 0;
        private int shootTimer = 0;
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
                        Damage = 10,
                        Amount = 1,
                        Pierce = 1,
                        Cooldown = 45,
                        Speed = 1.0f
                    };
                    originalWeaponStats = weaponStats;
                }
            }
        }

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 360000;
            Projectile.alpha = 255;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        private bool IsWeaponEquipped(Player player)
        {
            // Check if any EightTheSparrow weapon is in the player's inventory
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].ModItem is VSWeapon weapon && weapon.WeaponIdentifier == "EightTheSparrow")
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
                if (player.inventory[i].ModItem is VSWeapon weapon && weapon.WeaponIdentifier == "EightTheSparrow")
                {
                    WeaponStats currentStats = weapon.GetWeaponStats(player);
                    
                    // Compare key stats that affect weapon performance
                    return currentStats.Damage != weaponStats.Damage ||
                           currentStats.Amount != weaponStats.Amount ||
                           currentStats.Pierce != weaponStats.Pierce ||
                           currentStats.Cooldown != weaponStats.Cooldown ||
                           Math.Abs(currentStats.Speed - weaponStats.Speed) > 0.001f;
                }
            }
            return false; // Weapon not found, let the IsWeaponEquipped check handle it
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.Center;

            // Check if the EightTheSparrow weapon is still equipped/in inventory
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

            shootTimer++;
            if (shootTimer >= weaponStats.Cooldown)
            {
                ShootBullets(player);
                shootTimer = 0;
            }
        }

        private void ShootBullets(Player player)
        {
            // Four diagonal directions toward screen corners
            Vector2[] directions = new Vector2[]
            {
                Vector2.Normalize(new Vector2(-1, -1)), // Top-left corner
                Vector2.Normalize(new Vector2(1, -1)),  // Top-right corner
                Vector2.Normalize(new Vector2(-1, 1)),  // Bottom-left corner
                Vector2.Normalize(new Vector2(1, 1))    // Bottom-right corner
            };

            float shootSpeed = 8f * weaponStats.Speed;

            // Fire Amount bullets in each direction
            for (int dir = 0; dir < directions.Length; dir++)
            {
                for (int bullet = 0; bullet < weaponStats.Amount; bullet++)
                {
                    Vector2 velocity = directions[dir] * shootSpeed;
                    
                    // Slight spacing for multiple bullets in same direction
                    Vector2 spawnOffset = directions[dir] * (bullet * 20f);

                    int projectileType = ModContent.ProjectileType<EightTheSparrowBulletProjectile>();

                    Terraria.Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        player.Center + spawnOffset,
                        velocity,
                        projectileType,
                        weaponStats.Damage,
                        weaponStats.Knockback,
                        player.whoAmI,
                        ai0: weaponStats.Pierce
                    );
                }
            }
        }
    }

    public class EightTheSparrowBulletProjectile : ModProjectile
    {
        private int penetrationsLeft;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.light = 0.4f;
            Projectile.penetrate = 100;
        }

        public override void OnSpawn(IEntitySource source)
        {
            penetrationsLeft = (int)Projectile.ai[0];
            if (penetrationsLeft <= 0) penetrationsLeft = 1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            // Create blue energy trail
            if (Main.rand.NextBool(3))
            {
                Dust trail = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width, Projectile.height,
                    Terraria.ID.DustID.BlueTorch,
                    0, 0, 100,
                    Color.Blue,
                    1.2f
                );
                trail.noGravity = true;
                trail.velocity *= 0.5f;
            }

            if (Projectile.timeLeft < 30)
            {
                Projectile.alpha += 8;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            penetrationsLeft--;
            if (penetrationsLeft <= 0)
            {
                Projectile.Kill();
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return true; // Bullets cannot pass through walls
        }
    }
}