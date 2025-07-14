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
    public class boneControllerProjectile : ModProjectile
    {
        public int level = 1;
        private int manaTimer = 0;
        private int shootTimer = 0;
        private float ManaCost = 10f;

        private int shootCooldown = 180;
        private int projectileCount = 1;
        private int projectilePenetration = 1;
        private int damage = 10;

        private int burstDelay = 6;
        private int burstCooldown = 0;

        private int burstShotCount = 0;

        private int duration = 120;
        private float speed = 12f;

        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse itemUse)
            {
                ManaCost = Main.player[Projectile.owner].GetManaCost(itemUse.Item) / 2;

                if (itemUse.Item.type == ModContent.ItemType<Content.Items.boneLvl1>()) level = 1;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.boneLvl2>()) level = 2;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.boneLvl3>()) level = 3;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.boneLvl4>()) level = 4;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.boneLvl5>()) level = 5;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.boneLvl6>()) level = 6;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.boneLvl7>()) level = 7;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.boneLvl8>()) level = 8;
            }

            damage = (int)(damage * (1 + 0.25f * (level - 1)));

            if (level >= 2) duration += 16;
            if (level >= 3) projectileCount += 1;
            if (level >= 3) damage += 20;
            if (level >= 4) speed *= 1.5f;
            if (level >= 5) projectileCount += 1;
            if (level >= 5) damage += 20;
            if (level >= 6) duration += 16;
            if (level >= 7) damage += 20;
            if (level >= 8) duration += 16;
            if (level >= 8) speed *= 1.5f;
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

        private void ShootAtTarget(Player player, NPC target)
        {
            Vector2 shootDirection = Vector2.Normalize(target.Center - player.Center);

            float shootSpeed = 12f;
            Vector2 velocity = shootDirection * shootSpeed;

            int projectileType = ModContent.ProjectileType<boneProjectile>();

            Terraria.Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                player.Center,
                velocity,
                projectileType,
                damage,
                2f,
                player.whoAmI,
                ai0: duration
            );

        }
    }
    public class boneProjectile : ModProjectile
    {
        private int penetrationsLeft;
        private float homingStrength = 1.0f;
        private int duration = 10;
        private int timeLeft = 0;

        private NPC LastHitNPC = null;
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
            Projectile.penetrate = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            duration = (int)Projectile.ai[0];
        }

        public override void AI()
        {
            if(timeLeft > duration) 
            {
                Projectile.Kill();
                return;
            }
            timeLeft++;
            Projectile.rotation += .05f;

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
                    if (npc == LastHitNPC) continue;

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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            LastHitNPC = target;
            SoundEngine.PlaySound(SoundID.DD2_SkeletonHurt, Projectile.position);

            NPC target2 = FindNearestEnemy(Projectile.Center, 200f);
            if (target2 != null)
            {
                Vector2 directionToTarget = Vector2.Normalize(target2.Center - Projectile.Center);
                float currentSpeed = Projectile.velocity.Length();

                Projectile.velocity = directionToTarget * currentSpeed;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.DD2_SkeletonHurt, Projectile.position);

            NPC target2 = FindNearestEnemy(Projectile.Center, 200f);
            if (target2 != null)
            {
                Vector2 directionToTarget = Vector2.Normalize(target2.Center - Projectile.Center);

                Projectile.velocity = Vector2.Lerp(Projectile.velocity, directionToTarget * Projectile.velocity.Length(), homingStrength);
            }

            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTexture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 mainDrawPosition = Projectile.Center - Main.screenPosition;
            Vector2 mainOrigin = new Vector2(mainTexture.Width / 2f, mainTexture.Height / 2f);

            Main.EntitySpriteDraw(
                mainTexture,
                mainDrawPosition,
                null,
                Color.LightBlue * (1f - Projectile.alpha / 255f),
                Projectile.rotation,
                mainOrigin,
                1f,
                SpriteEffects.None,
                0
            );

            return false;
        }
    }
}
