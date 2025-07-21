using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using VampariaSurvivors.Content.Items;

namespace VampariaSurvivors.Content.Projectile
{
    public class CrossControllerProjectile : ModProjectile
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
                        Damage = 10,
                        Amount = 1,
                        Area = 1.0f,
                        Speed = 1.0f,
                        Pierce = -1,
                        Cooldown = 120,
                        ProjectileInterval = 6,
                        BlockedByWalls = false
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
                NPC target = FindNearestEnemy(Projectile.Center, 800f);
                if (target != null)
                {
                    ShootCross(player, target);
                }
                burstCooldown = 0;
                burstShotCount++;
            }
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

        private void ShootCross(Player player, NPC target)
        {
            Vector2 shootDirection = Vector2.Normalize(target.Center - player.Center);
            float baseSpeed = 8f * weaponStats.Speed;
            Vector2 velocity = shootDirection * baseSpeed;

            int projectileType = ModContent.ProjectileType<CrossProjectile>();

            Terraria.Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                player.Center,
                velocity,
                projectileType,
                weaponStats.Damage,
                weaponStats.Knockback,
                player.whoAmI,
                ai0: weaponStats.Speed,
                ai1: weaponStats.Area,
                ai2: weaponStats.Pierce
            );
        }
    }

    public class CrossProjectile : ModProjectile
    {
        private bool isReturning = false;
        private bool isTurning = false;
        private int maxTravelTime;
        private int timeAlive = 0;
        private int turnStartTime = 0;
        private int turnDuration = 60;
        private float rotationSpeed = 0.2f;
        private float areaScale = 1.0f;
        private int baseWidth = 20;
        private int baseHeight = 20;
        private int pierceCount = -1;
        private Vector2 originalVelocity;
        private Vector2 targetVelocity;

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.light = 0.3f;
            Projectile.penetrate = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            float speedMultiplier = Projectile.ai[0];
            areaScale = Projectile.ai[1];
            pierceCount = (int)Projectile.ai[2];

            if (areaScale <= 0) areaScale = 1.0f;
            if (speedMultiplier <= 0) speedMultiplier = 1.0f;

            maxTravelTime = (int)(10 * speedMultiplier);

            Projectile.width = (int)(baseWidth * areaScale);
            Projectile.height = (int)(baseHeight * areaScale);

            originalVelocity = Projectile.velocity;
        }

        public override void AI()
        {
            timeAlive++;

            float currentRotationSpeed = isTurning ? rotationSpeed * 2f : rotationSpeed;
            Projectile.rotation += currentRotationSpeed;

            if (!isReturning && !isTurning && timeAlive >= maxTravelTime)
            {
                isTurning = true;
                turnStartTime = timeAlive;
                originalVelocity = Projectile.velocity;
                targetVelocity = -originalVelocity;

                SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
            }

            if (isTurning && !isReturning)
            {
                int turnProgress = timeAlive - turnStartTime;
                float turnRatio = (float)turnProgress / turnDuration;

                if (turnRatio >= 1.0f)
                {
                    isTurning = false;
                    isReturning = true;
                    Projectile.velocity = targetVelocity;
                }
                else
                {
                    float easedRatio = SmoothStep(turnRatio);

                    Projectile.velocity = Vector2.Lerp(originalVelocity, targetVelocity, easedRatio);
                }
            }

            if (isReturning)
            {
                Player owner = Main.player[Projectile.owner];
                float distanceFromPlayer = Vector2.Distance(Projectile.Center, owner.Center);

                if (distanceFromPlayer > 1200f)
                {
                    Projectile.Kill();
                }
            }

            if (Projectile.timeLeft < 60)
            {
                Projectile.alpha += 4;
            }
        }

        private float SmoothStep(float t)
        {
            return t * t * (3f - 2f * t);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.NPCHit1, Projectile.position);

            for (int i = 0; i < 3; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height,
                    DustID.Electric, 0f, 0f, 100, default, 0.5f);
                dust.velocity *= 0.3f;
                dust.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);

            Color drawColor;
            if (isTurning)
            {
                float pulse = (float)Math.Sin(timeAlive * 0.5f) * 0.3f + 0.7f;
                drawColor = Color.Lerp(Color.White, Color.Gold, pulse) * (1f - Projectile.alpha / 255f);
            }
            else if (isReturning)
            {
                drawColor = Color.LightGoldenrodYellow * (1f - Projectile.alpha / 255f);
            }
            else
            {
                drawColor = Color.White * (1f - Projectile.alpha / 255f);
            }

            float drawScale = isTurning ? areaScale * (1f + 0.1f * (float)Math.Sin(timeAlive * 0.8f)) : areaScale;

            Main.EntitySpriteDraw(
                texture,
                drawPosition,
                null,
                drawColor,
                Projectile.rotation,
                origin,
                drawScale,
                SpriteEffects.None,
                0
            );

            return false;
        }
    }
}