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
    public class AxeControllerProjectile : ModProjectile
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
                        Damage = 20,
                        Amount = 2,
                        Pierce = 3,
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
                ShootAxe(player, burstShotCount);
                burstCooldown = 0;
                burstShotCount++;
            }
        }

        private void ShootAxe(Player player, int axeIndex)
        {
            Vector2 shootPosition = player.Center;
            Vector2 velocity;

            if (axeIndex == 0)
            {
                velocity = new Vector2(0, -8f);
            }
            else
            {
                float facingDirection = player.direction;
                float arcAngle = axeIndex * 0.5f;

                velocity = new Vector2(
                    facingDirection * arcAngle * 3f,
                    -8f + (arcAngle * 0.5f)
                );
            }

            // Apply speed modifier from weapon stats
            velocity *= weaponStats.Speed;

            int projectileType = ModContent.ProjectileType<AxeProjectile>();

            Terraria.Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                shootPosition,
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

    public class AxeProjectile : ModProjectile
    {
        private int penetrationsLeft;
        private float areaScale = 1.0f;
        private int baseWidth = 16;
        private int baseHeight = 16;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 150;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.light = 0.3f;
            Projectile.penetrate = 100;
        }

        public override void OnSpawn(IEntitySource source)
        {
            penetrationsLeft = (int)Projectile.ai[0];
            if (penetrationsLeft <= 0) penetrationsLeft = 3;
            areaScale = Projectile.ai[1];
            if (areaScale <= 0) areaScale = 1.0f;

            Projectile.width = (int)(baseWidth * areaScale);
            Projectile.height = (int)(baseHeight * areaScale);
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.2f;

            Projectile.rotation += 0.3f;

            if (Projectile.timeLeft < 30)
            {
                Projectile.alpha += 8;
            }
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

            Texture2D mainTexture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 mainDrawPosition = Projectile.Center - Main.screenPosition;
            Vector2 mainOrigin = new Vector2(mainTexture.Width / 2f, mainTexture.Height / 2f);

            Main.EntitySpriteDraw(mainTexture, mainDrawPosition, null,
                                Color.White * (1f - Projectile.alpha / 255f),
                                Projectile.rotation, mainOrigin, areaScale,
                                SpriteEffects.None, 0);

            return false;
        }
    }
}