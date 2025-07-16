using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace VampariaSurvivors.Content.Projectile
{
    public class WhipControllerProjectile : ModProjectile
    {
        public int level = 1;
        private int manaTimer = 0;
        private int shootTimer = 0;
        private float ManaCost = 10f;

        private int shootCooldown = 60;
        private int projectileCount = 2;
        private int projectilePenetration = 1;
        private int damage = 20;

        private int burstDelay = 6;
        private int burstCooldown = 0;

        private int burstShotCount = 0;

        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse itemUse)
            {
                ManaCost = Main.player[Projectile.owner].GetManaCost(itemUse.Item) / 2;

                if (itemUse.Item.type == ModContent.ItemType<Content.Items.WhipLvl1>()) level = 1;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.WhipLvl2>()) level = 2;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.WhipLvl3>()) level = 3;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.WhipLvl4>()) level = 4;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.WhipLvl5>()) level = 5;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.WhipLvl6>()) level = 6;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.WhipLvl7>()) level = 7;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.WhipLvl8>()) level = 8;
            }

            damage = 20;
            projectileCount = 1;
            float areaMultiplier = 1f;

            if (level >= 2) projectileCount = 2;
            if (level >= 3) damage += 10;
            if (level >= 4) { areaMultiplier += 0.1f; damage += 10; }
            if (level >= 5) damage += 10;
            if (level >= 6) { areaMultiplier += 0.1f; damage += 10; }
            if (level >= 7) damage += 10;
            if (level >= 8) { damage += 10; projectileCount = 3; }
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
            if (shootTimer >= shootCooldown)
            {
                burstCooldown = 0;
                burstShotCount = 0;
                shootTimer = 0;
            }

            burstCooldown++;
            if (burstCooldown >= burstDelay && burstShotCount < projectileCount)
            {
                NPC target = FindNearestEnemy(Projectile.Center, 800f);
                if (target != null)
                {
                    ShootAtTarget(player, target);
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


        private void ShootAtTarget(Player player, NPC target = null)
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
                damage,
                2f,
                player.whoAmI,
                ai0: level,
                ai1: direction
            );
        }
    }
    public class WhipProjectile : ModProjectile
    {
        private int level;
        private float areaMultiplier = 1f;
        private float maxWidth = 128f;
        private float maxHeight = 32f;
        private int growthTime = 6; 
        private float finalVerticalSpeed = 0.25f;

        private Vector2 originalSpawnPos;
        private bool hasSetSpawnPos = false;

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
            level = (int)Projectile.ai[0];

            if (level >= 4) areaMultiplier += 0.1f;
            if (level >= 6) areaMultiplier += 0.1f;

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

            if (Projectile.timeLeft > 60 - growthTime)
            {
                float growthProgress = (60 - Projectile.timeLeft) / (float)growthTime;

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