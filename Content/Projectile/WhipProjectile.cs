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
    public class WhipControllerProjectile : ModProjectile
    {
        private int manaTimer = 0;
        private int shootTimer = 0;
        private float ManaCost = 10f;
        private int burstCooldown = 0;
        private int burstShotCount = 0;

        // Weapon stats - populated from the weapon item
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
                CreateWhipSlash(player);
                burstCooldown = 0;
                burstShotCount++;
            }
        }

        private void CreateWhipSlash(Player player)
        {
            int direction = (burstShotCount % 2 == 0) ? player.direction : -player.direction;

            float verticalOffset = -burstShotCount * 15f;
            Vector2 slashStart = player.Center + new Vector2(0, verticalOffset);

            int projectileType = ModContent.ProjectileType<WhipProjectile>();

            Terraria.Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                slashStart,
                Vector2.Zero,
                projectileType,
                weaponStats.Damage,
                weaponStats.Knockback,
                player.whoAmI,
                ai0: weaponStats.Area,
                ai1: direction
            );
        }
    }

    public class WhipProjectile : ModProjectile
    {
        private float areaMultiplier = 1f;
        private float maxWidth = 128f;
        private float maxHeight = 32f;
        private int growthTime = 6;
        private float finalVerticalSpeed = 0.25f;

        private Vector2 originalSpawnPos;
        private bool hasSetSpawnPos = false;

        private float areaScale = 1.0f;
        private int baseWidth = 16;
        private int baseHeight = 16;

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 30;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.alpha = 100;
        }

        public override void OnSpawn(IEntitySource source)
        {
            areaMultiplier = Projectile.ai[0];

            maxWidth *= areaMultiplier;
            maxHeight *= areaMultiplier;

            Projectile.velocity = Vector2.Zero;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            int slashDirection = (int)Projectile.ai[1];

            if (!hasSetSpawnPos)
            {
                originalSpawnPos = Projectile.Center;
                hasSetSpawnPos = true;
            }

            if (Projectile.timeLeft > 30 - growthTime)
            {
                float growthProgress = (30 - Projectile.timeLeft) / (float)growthTime;

                Projectile.width = (int)(maxWidth * growthProgress);
                Projectile.height = (int)(maxHeight * growthProgress);

                Vector2 horizontalOffset = new Vector2(slashDirection * Projectile.width * 0.5f, 0);
                Projectile.Center = originalSpawnPos + horizontalOffset;
            }
            else
            {
                originalSpawnPos.Y -= finalVerticalSpeed;

                Projectile.width = (int)maxWidth;
                Projectile.height = (int)maxHeight;

                Vector2 horizontalOffset = new Vector2(slashDirection * Projectile.width * 0.5f, 0);
                Projectile.Center = originalSpawnPos + horizontalOffset;
            }

            if (Projectile.timeLeft < 20)
            {
                Projectile.alpha += 10;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int slashDirection = (int)Projectile.ai[1];

            Rectangle drawRect = new Rectangle(
                (int)(Projectile.Center.X - Main.screenPosition.X - Projectile.width / 2),
                (int)(Projectile.Center.Y - Main.screenPosition.Y - Projectile.height / 2),
                Projectile.width,
                Projectile.height
            );

            Color drawColor = Color.White * (1f - Projectile.alpha / 255f);

            SpriteEffects effects = slashDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.spriteBatch.Draw(texture, drawRect, null, drawColor, 0f, Vector2.Zero, effects, 0f);
            return false;
        }
    }
}