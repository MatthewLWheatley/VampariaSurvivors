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
    public class CherryBombControllerProjectile : ModProjectile
    {
        private int manaTimer = 0;
        private int shootTimer = 0;
        private float ManaCost = 10f;
        private int burstCooldown = 0;
        private int burstShotCount = 0;

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
                        Damage = 35,
                        Amount = 1,
                        Area = 1.0f,
                        Duration = 90,
                        Cooldown = 180,
                        ProjectileInterval = 12,
                        Speed = 1.0f
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
                ThrowBomb(player);
                burstCooldown = 0;
                burstShotCount++;
            }
        }

        private void ThrowBomb(Player player)
        {
            // Find nearest enemy for targeting
            NPC target = FindNearestEnemy(player.Center, 500f);
            Vector2 throwDirection;
            
            if (target != null)
            {
                // Throw in straight line towards enemy
                Vector2 directionToTarget = target.Center - player.Center;
                directionToTarget.Normalize();
                throwDirection = directionToTarget * 12f; // Straight line, no arc
            }
            else
            {
                // Throw in facing direction if no enemies
                float angle = (burstShotCount - (weaponStats.Amount - 1) * 0.5f) * 0.5f;
                throwDirection = new Vector2(
                    player.direction * (float)Math.Cos(angle) * 12f, 
                    (float)Math.Sin(angle) * 12f
                );
            }

            // Apply speed modifier
            throwDirection *= weaponStats.Speed;

            int projectileType = ModContent.ProjectileType<CherryBombProjectile>();

            Terraria.Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                player.Center,
                throwDirection,
                projectileType,
                weaponStats.Damage,
                weaponStats.Knockback,
                player.whoAmI,
                ai0: 0, // No timer-based explosion
                ai1: weaponStats.Area // Explosion area
            );
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

    public class CherryBombProjectile : ModProjectile
    {
        private float explosionArea;
        private bool hasExploded = false;
        private const float MIN_VELOCITY_FOR_EXPLOSION = 0.5f; // Bomb explodes when slower than this
        private const float DECELERATION = 0.98f; // Gradual slowdown multiplier

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true; // Friendly so it can damage enemies
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 600; // Long time limit, explosion is velocity-based
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.light = 0.2f;
            Projectile.penetrate = -1; // Never dies from penetration
        }

        public override void OnSpawn(IEntitySource source)
        {
            explosionArea = Projectile.ai[1];
            if (explosionArea <= 0) explosionArea = 1.0f;
        }

        public override void AI()
        {
            // Apply gravity for better Terraria feel
            Projectile.velocity.Y += 0.25f;
            if (Projectile.velocity.Y > 16f) Projectile.velocity.Y = 16f;

            // Apply deceleration - bombs gradually slow down
            Projectile.velocity.X *= DECELERATION;

            // Rotation based on velocity
            Projectile.rotation += Projectile.velocity.X * 0.02f;

            // Light sparkle effect
            Projectile.light = 0.2f + (float)Math.Sin(Projectile.timeLeft * 0.1f) * 0.1f;

            // Check if bomb has slowed down enough to explode
            float speed = Projectile.velocity.Length();
            if (speed < MIN_VELOCITY_FOR_EXPLOSION && !hasExploded)
            {
                Explode();
                return;
            }

            // Optional: Small trail effect
            if (Main.rand.NextBool(3))
            {
                Dust trail = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Smoke,
                    -Projectile.velocity.X * 0.2f,
                    -Projectile.velocity.Y * 0.2f,
                    100,
                    Color.Gray,
                    0.6f
                );
                trail.noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Bounce off tiles with energy loss
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X * 0.8f;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y * 0.8f;

            SoundEngine.PlaySound(SoundID.Tink, Projectile.position);
            return false; // Don't kill on tile collision
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Bounce off enemies with energy loss - but don't explode!
            Vector2 bounceDirection = (Projectile.Center - target.Center);
            if (bounceDirection.Length() > 0)
            {
                bounceDirection.Normalize();
                // Maintain some horizontal momentum, reduce overall speed
                float currentSpeed = Projectile.velocity.Length();
                Projectile.velocity = bounceDirection * currentSpeed * 0.7f;
            }
            
            // Play hit sound
            SoundEngine.PlaySound(SoundID.NPCHit1, Projectile.position);
            
            // Create small impact effect
            for (int i = 0; i < 3; i++)
            {
                Dust impactDust = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Smoke,
                    bounceDirection.X * Main.rand.NextFloat(1f, 3f),
                    bounceDirection.Y * Main.rand.NextFloat(1f, 3f),
                    100,
                    Color.Orange,
                    0.8f
                );
                impactDust.noGravity = true;
            }
        }

        private void Explode()
        {
            if (hasExploded) return;
            hasExploded = true;

            // Explosion sound
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            // Calculate explosion radius
            float explosionRadius = 80f * explosionArea;

            // Damage all enemies in radius
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && !npc.dontTakeDamage)
                {
                    float distance = Vector2.Distance(npc.Center, Projectile.Center);
                    if (distance <= explosionRadius)
                    {
                        // Damage falloff based on distance
                        float damageMultiplier = 1f - (distance / explosionRadius) * 0.5f;
                        int finalDamage = (int)(Projectile.damage * damageMultiplier);

                        npc.StrikeNPC(npc.CalculateHitInfo(finalDamage, 0, false, Projectile.knockBack));
                    }
                }
            }

            // SHOCKWAVE EFFECT - expanding ring of dust
            for (int ring = 0; ring < 3; ring++)
            {
                float ringRadius = explosionRadius * (0.3f + ring * 0.35f);
                int particleCount = (int)(ringRadius / 4f);
                
                for (int i = 0; i < particleCount; i++)
                {
                    float angle = (float)(2 * Math.PI * i / particleCount);
                    Vector2 shockwavePos = Projectile.Center + new Vector2(
                        (float)Math.Cos(angle) * ringRadius,
                        (float)Math.Sin(angle) * ringRadius
                    );
                    
                    Dust shockwave = Dust.NewDustDirect(
                        shockwavePos,
                        4, 4,
                        DustID.Smoke,
                        (float)Math.Cos(angle) * 2f,
                        (float)Math.Sin(angle) * 2f,
                        100,
                        Color.White,
                        1.2f - ring * 0.3f
                    );
                    shockwave.noGravity = true;
                }
            }

            // FIREWORK EFFECT - colorful sparks shooting outward
            Color[] fireworkColors = { Color.Red, Color.Orange, Color.Yellow, Color.Pink, Color.Magenta };
            for (int i = 0; i < 25; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(6f, 15f);
                Color sparkColor = fireworkColors[Main.rand.Next(fireworkColors.Length)];
                
                Dust firework = Dust.NewDustDirect(
                    Projectile.Center,
                    8, 8,
                    DustID.RainbowMk2,
                    velocity.X,
                    velocity.Y,
                    100,
                    sparkColor,
                    Main.rand.NextFloat(1.2f, 2.0f)
                );
                firework.noGravity = true;
                firework.fadeIn = 1.2f;
            }

            // FLOWER PETAL EFFECT - gentle floating petals
            Color[] petalColors = { Color.Pink, Color.LightPink, Color.HotPink, Color.DeepPink, Color.White };
            for (int i = 0; i < 15; i++)
            {
                Vector2 petalVelocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(1f, 4f);
                petalVelocity.Y -= Main.rand.NextFloat(0.5f, 2f); // Petals tend to float upward
                Color petalColor = petalColors[Main.rand.Next(petalColors.Length)];
                
                Dust petal = Dust.NewDustDirect(
                    Projectile.Center - new Vector2(20, 20),
                    40, 40,
                    DustID.PinkFairy,
                    petalVelocity.X,
                    petalVelocity.Y,
                    100,
                    petalColor,
                    Main.rand.NextFloat(0.8f, 1.5f)
                );
                petal.noGravity = true;
                petal.fadeIn = 0.8f;
            }

            // Traditional explosion fire/smoke for the center
            for (int i = 0; i < 20; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(3f, 8f);
                Dust fire = Dust.NewDustDirect(
                    Projectile.Center - new Vector2(16, 16),
                    32, 32,
                    DustID.Torch,
                    velocity.X,
                    velocity.Y,
                    100,
                    Color.Orange,
                    Main.rand.NextFloat(1.0f, 1.8f)
                );
                fire.noGravity = true;
            }

            Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);

            // Normal cherry bomb appearance with slight glow
            Color drawColor = Color.Lerp(lightColor, Color.Pink, 0.2f);

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
}