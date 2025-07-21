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
    public class KingBibleControllerProjectile : ModProjectile
    {
        private int manaTimer = 0;
        private float ManaCost = 10f;
        private int cycleTimer = 0;
        private bool biblesActive = false;
        private List<int> bibleProjectileIds = new List<int>();

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
                        Duration = 180,
                        Cooldown = 360,
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
                    KillAllBibles();
                    Projectile.Kill();
                    return;
                }
            }

            cycleTimer++;

            if (!biblesActive)
            {
                if (cycleTimer >= weaponStats.Cooldown)
                {
                    SummonBibles(player);
                    biblesActive = true;
                    cycleTimer = 0;
                }
            }
            else
            {
                if (cycleTimer >= weaponStats.Duration)
                {
                    KillAllBibles();
                    biblesActive = false;
                    cycleTimer = 0;
                }
            }

            bibleProjectileIds.RemoveAll(id =>
                id >= Main.maxProjectiles ||
                !Main.projectile[id].active ||
                Main.projectile[id].type != ModContent.ProjectileType<KingBibleProjectile>()
            );
        }

        private void SummonBibles(Player player)
        {
            float orbitRadius = Math.Min(80f + (weaponStats.Area - 1f) * 40f, 200f);

            for (int i = 0; i < weaponStats.Amount; i++)
            {
                float angle = (float)(i * 2 * Math.PI / weaponStats.Amount);
                Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * orbitRadius;
                Vector2 spawnPosition = player.Center + offset;

                int projectileId = Terraria.Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    spawnPosition,
                    Vector2.Zero,
                    ModContent.ProjectileType<KingBibleProjectile>(),
                    weaponStats.Damage,
                    weaponStats.Knockback,
                    player.whoAmI,
                    ai0: angle,
                    ai1: orbitRadius,
                    ai2: weaponStats.Speed
                );

                if (projectileId < Main.maxProjectiles)
                {
                    bibleProjectileIds.Add(projectileId);

                    var bibleProj = Main.projectile[projectileId];
                    bibleProj.localAI[0] = weaponStats.Area;

                    int newWidth = (int)(24 * weaponStats.Area);
                    int newHeight = (int)(28 * weaponStats.Area);
                    bibleProj.width = newWidth;
                    bibleProj.height = newHeight;
                }
            }

            // Play summon sound
            SoundEngine.PlaySound(SoundID.Item4, player.position);
        }

        private void KillAllBibles()
        {
            foreach (int id in bibleProjectileIds)
            {
                if (id < Main.maxProjectiles && Main.projectile[id].active)
                {
                    Main.projectile[id].Kill();
                }
            }
            bibleProjectileIds.Clear();
        }

        public override void Kill(int timeLeft)
        {
            KillAllBibles();
        }
    }

    public class KingBibleProjectile : ModProjectile
    {
        private float currentAngle;
        private float orbitRadius;
        private float rotationSpeed;
        private float areaScale = 1.0f;
        private int pageTimer = 0;
        private int baseWidth = 24;
        private int baseHeight = 28;

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 360000;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.light = 0.2f;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void OnSpawn(IEntitySource source)
        {
            currentAngle = Projectile.ai[0];
            orbitRadius = Projectile.ai[1];
            rotationSpeed = Projectile.ai[2];
            areaScale = Projectile.localAI[0];

            if (areaScale <= 0) areaScale = 1.0f;

            Projectile.width = (int)(baseWidth * areaScale);
            Projectile.height = (int)(baseHeight * areaScale);

            if (Projectile.width < 12) Projectile.width = 12;
            if (Projectile.height < 14) Projectile.height = 14;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (!owner.active)
            {
                Projectile.Kill();
                return;
            }

            float baseRotationSpeed = 0.02f;
            currentAngle += baseRotationSpeed * rotationSpeed;

            Vector2 offset = new Vector2(
                (float)Math.Cos(currentAngle),
                (float)Math.Sin(currentAngle)
            ) * orbitRadius;

            Projectile.Center = owner.Center + offset;

            pageTimer++;
            if (pageTimer >= 90 + Main.rand.Next(60))
            {
                SpawnPageParticle();
                pageTimer = 0;
            }
        }

        private void SpawnPageParticle()
        {
            Vector2 pageVelocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(1f, 3f) * areaScale;
            pageVelocity.Y -= 1f;

            int dustType = DustID.Enchanted_Gold;
            Dust page = Dust.NewDustDirect(
                Projectile.position,
                Projectile.width,
                Projectile.height,
                dustType,
                pageVelocity.X,
                pageVelocity.Y,
                100,
                Color.White,
                Main.rand.NextFloat(0.8f, 1.2f) * areaScale
            );

            page.noGravity = false;
            page.fadeIn = 1f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.Item4, Projectile.position);

            int particleCount = (int)(3 * areaScale);
            for (int i = 0; i < particleCount; i++)
            {
                Dust dust = Dust.NewDustDirect(
                    target.position,
                    target.width,
                    target.height,
                    DustID.Electric,
                    0f, 0f, 100,
                    Color.Gold,
                    0.8f * areaScale
                );
                dust.velocity *= 0.5f;
                dust.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);

            Color drawColor = Color.Lerp(lightColor, Color.White, 0.3f);

            float visualScale = Math.Max(areaScale, 0.5f);

            Main.EntitySpriteDraw(
                texture,
                drawPosition,
                null,
                drawColor,
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