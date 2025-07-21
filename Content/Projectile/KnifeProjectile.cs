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
    public class KnifeControllerProjectile : ModProjectile
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
                        Damage = 13,
                        Amount = 1,
                        Pierce = 1,
                        Cooldown = 60,
                        ProjectileInterval = 6
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
                ShootKnife(player);
                burstCooldown = 0;
                burstShotCount++;
            }
        }

        private void ShootKnife(Player player)
        {
            Vector2 shootDirection = Vector2.Zero;

            if (player.velocity != Vector2.Zero)
            {
                shootDirection = Vector2.Normalize(player.velocity);
            }
            else
            {
                shootDirection = new Vector2(player.direction, 0);
            }

            Vector2 perpendicular = new Vector2(-shootDirection.Y, shootDirection.X);

            Vector2 startPosition = player.Center;
            float offsetDistance = 20f;

            int positionIndex = burstShotCount % 3;

            switch (positionIndex)
            {
                case 0:
                    startPosition = player.Center;
                    break;
                case 1:
                    startPosition = player.Center + perpendicular * offsetDistance;
                    break;
                case 2:
                    startPosition = player.Center - perpendicular * offsetDistance;
                    break;
            }

            Vector2 velocity = shootDirection * weaponStats.Speed;

            int projectileType = ModContent.ProjectileType<KnifeProjectile>();

            Terraria.Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                startPosition,
                velocity,
                projectileType,
                weaponStats.Damage,
                weaponStats.Knockback,
                player.whoAmI,
                ai0: weaponStats.Pierce,
                ai1: weaponStats.Area
            );
        }
    }

    public class KnifeProjectile : ModProjectile
    {
        private int penetrationsLeft;
        private List<Vector2> trailPositions = new List<Vector2>();
        private int maxTrailLength = 0;
        private float homingStrength = 0.00f;

        private float areaScale = 1.0f;
        private int baseWidth = 16;
        private int baseHeight = 16;
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.light = 0.5f;
            Projectile.penetrate = 1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            penetrationsLeft = (int)Projectile.ai[0];
            if (penetrationsLeft <= 0) penetrationsLeft = 1;


            areaScale = Projectile.ai[1];
            if (areaScale <= 0) areaScale = 1.0f;

            Projectile.width = (int)(baseWidth * areaScale);
            Projectile.height = (int)(baseHeight * areaScale);
        }

        public override void AI()
        {
            NPC target = FindNearestEnemy(Projectile.Center, 200f);
            if (target != null && homingStrength > 0)
            {
                Vector2 directionToTarget = Vector2.Normalize(target.Center - Projectile.Center);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, directionToTarget * Projectile.velocity.Length(), homingStrength);
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (maxTrailLength > 0)
            {
                trailPositions.Insert(0, Projectile.Center);
                if (trailPositions.Count > maxTrailLength)
                {
                    trailPositions.RemoveAt(trailPositions.Count - 1);
                }
            }

            if (Projectile.timeLeft < 30)
            {
                Projectile.alpha += 8;
            }
        }

        private NPC FindNearestEnemy(Vector2 position, float maxRange)
        {
            NPC closest = null;
            float closestDistance = maxRange;
            Vector2 currentDirection = Vector2.Normalize(Projectile.velocity);

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && !npc.dontTakeDamage && npc.lifeMax > 5)
                {
                    float distance = Vector2.Distance(npc.Center, position);
                    if (distance < closestDistance)
                    {
                        Vector2 directionToEnemy = Vector2.Normalize(npc.Center - position);
                        float dotProduct = Vector2.Dot(currentDirection, directionToEnemy);

                        if (dotProduct > 0.9f)
                        {
                            closest = npc;
                            closestDistance = distance;
                        }
                    }
                }
            }

            return closest;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            penetrationsLeft--;
            if (penetrationsLeft <= 0)
            {
                Projectile.Kill();
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            return true;
            
        }
        
        public override bool PreDraw(ref Color lightColor)
        {
            if (trailPositions.Count > 1 && maxTrailLength > 0)
            {
                for (int i = 1; i < trailPositions.Count; i++)
                {
                    Vector2 drawPosition = trailPositions[i] - Main.screenPosition;

                    float trailAlpha = (float)(maxTrailLength - i) / maxTrailLength;
                    trailAlpha *= 0.6f;

                    float trailScale = trailAlpha * 0.8f;

                    Color trailColor = Color.LightBlue * trailAlpha * (1f - Projectile.alpha / 255f);

                    Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
                    Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);

                    Main.EntitySpriteDraw(
                        texture,
                        drawPosition,
                        null,
                        trailColor,
                        Projectile.rotation,
                        origin,
                        trailScale,
                        SpriteEffects.None,
                        0
                    );
                }
            }

            Texture2D mainTexture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 mainDrawPosition = Projectile.Center - Main.screenPosition;
            Vector2 mainOrigin = new Vector2(mainTexture.Width / 2f, mainTexture.Height / 2f);

            Main.EntitySpriteDraw(
                mainTexture,
                mainDrawPosition,
                null,
                Color.LightBlue * (1f - Projectile.alpha / 255f),
                Projectile.velocity.ToRotation(),
                mainOrigin,
                areaScale,
                SpriteEffects.None,
                0
            );

            return false;
        }
    }
}