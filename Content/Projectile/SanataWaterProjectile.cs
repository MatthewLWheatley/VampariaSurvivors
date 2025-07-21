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
    public class SantaWaterControllerProjectile : ModProjectile
    {
        private int manaTimer = 0;
        private int shootTimer = 0;
        private float ManaCost = 10f;
        private int burstCooldown = 0;
        private int burstShotCount = 0;
        private float circularAngle = 0f;

        private WeaponStats weaponStats;

        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse itemUse)
            {
                ManaCost = Main.player[Projectile.owner].GetManaCost(itemUse.Item) / 2;

                if (itemUse.Item.ModItem is VSWeapon weapon)
                {
                    weaponStats = weapon.GetWeaponStats();
                }
                else
                {
                    weaponStats = new WeaponStats
                    {
                        Damage = 20,
                        Amount = 1,
                        Area = 1.0f,
                        Duration = 120,
                        Cooldown = 270,
                        ProjectileInterval = 18
                    };
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

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.Center;

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
                burstCooldown = 0;
                burstShotCount = 0;
                shootTimer = 0;
            }

            burstCooldown++;
            if (burstCooldown >= weaponStats.ProjectileInterval && burstShotCount < weaponStats.Amount)
            {
                DropBottle(player);
                burstCooldown = 0;
                burstShotCount++;
            }
        }

        private void DropBottle(Player player)
        {
            Vector2 dropPosition;

            if (weaponStats.Amount < 4 && burstShotCount == 0)
            {
                NPC target = FindNearestEnemy(player.Center, 600f);
                if (target != null)
                {
                    dropPosition = target.Center + new Vector2(Main.rand.Next(-50, 51), -400f);
                }
                else
                {
                    dropPosition = GetCircularDropPosition(player);
                }
            }
            else
            {
                dropPosition = GetCircularDropPosition(player);
            }

            int projectileType = ModContent.ProjectileType<SantaWaterBottleProjectile>();

            Terraria.Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                dropPosition,
                new Vector2(0, 8f),
                projectileType,
                weaponStats.Damage,
                weaponStats.Knockback,
                player.whoAmI,
                ai0: weaponStats.Duration,
                ai1: weaponStats.Area
            );
        }

        private Vector2 GetCircularDropPosition(Player player)
        {
            float radius = 150f + (weaponStats.Area * 50f);

            Vector2 offset = new Vector2(
                (float)Math.Cos(circularAngle) * radius,
                (float)Math.Sin(circularAngle) * radius
            );

            circularAngle += (float)(2 * Math.PI / Math.Max(weaponStats.Amount, 4));

            return player.Center + offset + new Vector2(0, -400f);
        }

        private NPC FindNearestEnemy(Vector2 position, float maxRange)
        {
            NPC closest = null;
            float closestDistance = maxRange;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && !npc.dontTakeDamage && npc.lifeMax > 5)
                {
                    float distance = Vector2.Distance(npc.Center, position);
                    if (distance < closestDistance)
                    {
                        closest = npc;
                        closestDistance = distance;
                    }
                }
            }

            return closest;
        }
    }

    public class SantaWaterBottleProjectile : ModProjectile
    {
        private int puddleDuration;
        private float puddleArea;
        private Vector2 impactPosition; // Store where we actually hit

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 24;
            Projectile.friendly = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.light = 0.3f;
            Projectile.penetrate = 1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            puddleDuration = (int)Projectile.ai[0];
            puddleArea = Projectile.ai[1];

            if (puddleDuration <= 0) puddleDuration = 120;
            if (puddleArea <= 0) puddleArea = 1.0f;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.3f;
            if (Projectile.velocity.Y > 16f) Projectile.velocity.Y = 16f;

            Projectile.rotation += 0.1f;

            if (Projectile.timeLeft < 60)
            {
                Projectile.alpha += 4;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            impactPosition = Projectile.Center; // Store exact impact point
            BreakBottle();
            return true;
        }

        public override void Kill(int timeLeft)
        {
            if (timeLeft <= 0)
            {
                impactPosition = Projectile.Center;
                BreakBottle();
            }
        }

        private void BreakBottle()
        {
            SoundEngine.PlaySound(SoundID.Shatter, impactPosition);

            // Glass shards
            for (int i = 0; i < 8; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2f, 6f);
                Dust glass = Dust.NewDustDirect(
                    impactPosition - new Vector2(8, 8),
                    16, 16,
                    DustID.Glass,
                    velocity.X,
                    velocity.Y,
                    100,
                    Color.LightBlue,
                    Main.rand.NextFloat(0.8f, 1.4f)
                );
                glass.noGravity = false;
            }

            // Water splash
            for (int i = 0; i < 5; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(1f, 4f);
                Dust water = Dust.NewDustDirect(
                    impactPosition - new Vector2(8, 8),
                    16, 16,
                    DustID.Water,
                    velocity.X,
                    velocity.Y,
                    100,
                    Color.LightBlue,
                    Main.rand.NextFloat(1.0f, 1.5f)
                );
                water.noGravity = true;
            }

            // Spawn puddle at exact impact point
            int puddleType = ModContent.ProjectileType<SantaWaterPuddleProjectile>();
            Terraria.Projectile.NewProjectile(
                Projectile.GetSource_Death(),
                impactPosition, // Use stored impact position
                Vector2.Zero,
                puddleType,
                Projectile.damage,
                Projectile.knockBack,
                Projectile.owner,
                ai0: puddleDuration,
                ai1: puddleArea
            );
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);

            Color drawColor = Color.Lerp(lightColor, Color.LightBlue, 0.3f) * (1f - Projectile.alpha / 255f);

            Main.EntitySpriteDraw(
                texture,
                drawPosition,
                null,
                drawColor,
                Projectile.rotation,
                origin,
                1.0f,
                SpriteEffects.None,
                0
            );

            return false;
        }
    }

    public class SantaWaterPuddleProjectile : ModProjectile
    {
        private int maxDuration;
        private float areaScale;
        private int damageTimer = 0;
        private Dictionary<int, int> enemyCooldowns = new Dictionary<int, int>();
        private Vector2 puddleCenter; // Store our actual center

        public override void SetDefaults()
        {
            Projectile.width = 64; // Start smaller
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.light = 0.4f;
            Projectile.penetrate = -1;
            Projectile.alpha = 100;
        }

        public override void OnSpawn(IEntitySource source)
        {
            maxDuration = (int)Projectile.ai[0];
            areaScale = Projectile.ai[1];

            if (maxDuration <= 0) maxDuration = 120;
            if (areaScale <= 0) areaScale = 1.0f;

            Projectile.timeLeft = maxDuration;

            // Calculate final size
            int finalSize = (int)(64 * areaScale);
            if (finalSize < 32) finalSize = 32;

            // Store the spawn position as our center point
            puddleCenter = new Vector2(Projectile.position.X, Projectile.position.Y);

            // Set the projectile size and reposition so it's centered on spawn point
            Projectile.width = finalSize;
            Projectile.height = finalSize;
            Projectile.position = puddleCenter - new Vector2(finalSize / 2f, finalSize / 2f);
        }

        public override void AI()
        {
            damageTimer++;

            if (damageTimer >= 30)
            {
                DamageNearbyEnemies();
                damageTimer = 0;
            }

            // Clean up expired enemy cooldowns
            List<int> expiredKeys = new List<int>();
            foreach (var kvp in enemyCooldowns)
            {
                enemyCooldowns[kvp.Key]--;
                if (enemyCooldowns[kvp.Key] <= 0)
                    expiredKeys.Add(kvp.Key);
            }
            foreach (int key in expiredKeys)
                enemyCooldowns.Remove(key);

            // Particle effects
            if (Main.rand.NextBool(3))
            {
                Vector2 particlePos = Projectile.position + new Vector2(
                    Main.rand.Next(Projectile.width),
                    Main.rand.Next(Projectile.height)
                );

                Dust flame = Dust.NewDustDirect(
                    particlePos,
                    4, 4,
                    DustID.BlueTorch,
                    0f, -Main.rand.NextFloat(1f, 3f),
                    100,
                    Color.LightBlue,
                    Main.rand.NextFloat(0.8f, 1.2f)
                );
                flame.noGravity = true;
            }

            // Fade out
            if (Projectile.timeLeft < 60)
            {
                Projectile.alpha += 3;
            }
        }

        private void DamageNearbyEnemies()
        {
            float damageRadius = (Projectile.width + Projectile.height) * 0.25f; // Quarter of combined dimensions

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && !npc.dontTakeDamage)
                {
                    float distance = Vector2.Distance(npc.Center, Projectile.Center);

                    if (distance < damageRadius)
                    {
                        int uniqueKey = npc.whoAmI;

                        if (!enemyCooldowns.ContainsKey(uniqueKey))
                        {
                            npc.StrikeNPC(npc.CalculateHitInfo(Projectile.damage, 0, false, 0));
                            enemyCooldowns[uniqueKey] = 30;
                        }
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);

            Color drawColor = Color.Lerp(Color.LightBlue, Color.White, 0.7f) * (1f - Projectile.alpha / 255f);

            Main.EntitySpriteDraw(
                texture,
                drawPosition,
                null,
                drawColor,
                0f,
                origin,
                areaScale,
                SpriteEffects.None,
                0
            );

            return false;
        }
    }
}