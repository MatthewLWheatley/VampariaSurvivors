using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace VampariaSurvivors.Content.Projectile
{
    public class MagicWandControllerProjectile : ModProjectile
    {
        public int level = 1;
        private int manaTimer = 0;
        private int shootTimer = 0;
        private float ManaCost = 10f;
        
        private int shootCooldown = 60;
        private int projectileCount = 2;
        private int projectilePenetration = 1;
        private int damage = 24;

        private int burstDelay = 6;
        private int burstCooldown = 0;

        private int burstShotCount = 0;
        
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse itemUse)
            {
                ManaCost = Main.player[Projectile.owner].GetManaCost(itemUse.Item) / 2;

                if (itemUse.Item.type == ModContent.ItemType<Content.Items.MagicWandLvl1>()) level = 1;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.MagicWandLvl2>()) level = 2;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.MagicWandLvl3>()) level = 3;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.MagicWandLvl4>()) level = 4;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.MagicWandLvl5>()) level = 5;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.MagicWandLvl6>()) level = 6;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.MagicWandLvl7>()) level = 7;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.MagicWandLvl8>()) level = 8;
            }

            damage = (int)(damage * (1 + 0.25f * (level - 1)));

            if (level >= 2) projectileCount = 2;
            if (level >= 3) shootCooldown = 48;
            if (level >= 4) projectileCount = 3;
            if (level >= 5) damage += 20;
            if (level >= 6) projectileCount = 4;
            if (level >= 7) projectilePenetration = 2;
            if (level >= 8) damage += 20;
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

            int projectileType = ModContent.ProjectileType<MagicWandProjectile>();

            Terraria.Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                player.Center,
                velocity,
                projectileType,
                damage,
                2f,
                player.whoAmI
            );

        }
    }
    public class MagicWandProjectile : ModProjectile
    { 
        private int penetrationsLeft;
        private List<Vector2> trailPositions = new List<Vector2>();
        private int maxTrailLength = 6;
        private float homingStrength = 0.05f;

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
        }

        public override void OnSpawn(IEntitySource source)
        {
            penetrationsLeft = (int)Projectile.ai[0];
            if (penetrationsLeft <= 0) penetrationsLeft = 1;
        }

        public override void AI()
        {
            NPC target = FindNearestEnemy(Projectile.Center, 200f);
            if (target != null)
            {
                Vector2 directionToTarget = Vector2.Normalize(target.Center - Projectile.Center);
                
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, directionToTarget * Projectile.velocity.Length(), homingStrength);
            }
            
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            
            trailPositions.Insert(0, Projectile.Center);
            if (trailPositions.Count > maxTrailLength)
            {
                trailPositions.RemoveAt(trailPositions.Count - 1);
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
                        
                        if (dotProduct > 0.6f)
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
            penetrationsLeft--;
            if (penetrationsLeft <= 0)
            {
                Projectile.Kill();
            }
        }

        // fucking ew, but it works
        public override bool PreDraw(ref Color lightColor)
        {
            if (trailPositions.Count > 1)
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
