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
    public class FireWandControllerProjectile : ModProjectile
    {
        public int level = 1;
        private int manaTimer = 0;
        private int shootTimer = 0;
        private float ManaCost = 10f;
        
        private int shootCooldown = 180;
        private int projectileCount = 3;
        private int projectilePenetration = 0;
        private int damage = 40;
        private float projectileSpeed = 3f;

        
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse itemUse)
            {
                ManaCost = Main.player[Projectile.owner].GetManaCost(itemUse.Item) / 2;

                if (itemUse.Item.type == ModContent.ItemType<Content.Items.FireWandLvl1>()) level = 1;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.FireWandLvl2>()) level = 2;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.FireWandLvl3>()) level = 3;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.FireWandLvl4>()) level = 4;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.FireWandLvl5>()) level = 5;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.FireWandLvl6>()) level = 6;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.FireWandLvl7>()) level = 7;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.FireWandLvl8>()) level = 8;
            }

            damage = (int)(damage * (1 + 0.25f * (level - 1)));

            if (level >= 2) damage += 20;
            if (level >= 3) damage += 20;
            if (level >= 3) projectileSpeed *= 1.2f;
            if (level >= 4) damage += 20;
            if (level >= 5) damage += 20;
            if (level >= 5) projectileSpeed *= 1.2f;
            if (level >= 6) damage += 20;
            if (level >= 7) damage += 20;
            if (level >= 7) projectileSpeed *= 1.2f;
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
                NPC target = FindNearestEnemy(Projectile.Center, 800f);
                if (target != null)
                {
                    ShootAtTarget(player, target);
                }
                shootTimer = 0;
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

        //why did this fucking suck so much also i hate radinas
        private void ShootAtTarget(Player player, NPC target)
        {

            for (int i = 0; i < projectileCount; i++)
            {
                // maybe dont be a idiot and update these values every time
                // if you dont you get repeated caclculated values leading to stupid directions
                Vector2 shootDirection = Vector2.Normalize(target.Center - player.Center);
                float angleOffset;
                if (projectileCount == 0)
                {
                    angleOffset = 0f;
                }
                else
                {
                    // if odd make projectile to left, if even to right
                    // step in 1 degree intervals on each side
                    angleOffset = (i - (projectileCount - 1) / 2f) * 8f;
                }

                shootDirection = shootDirection.RotatedBy(MathHelper.ToRadians(angleOffset));

                Vector2 velocity = shootDirection * projectileCount;

                int projectileType = ModContent.ProjectileType<FireWandProjectile>();

                Terraria.Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    player.Center,
                    velocity,
                    projectileType,
                    damage,
                    2f,
                    player.whoAmI,
                    ai0: projectilePenetration
                );
            }
        }
    }
    
    public class FireWandProjectile : ModProjectile
    {
        private int penetrationsLeft;
        private List<Vector2> trailPositions = new List<Vector2>();
        private int maxTrailLength = 12;
        private float homingStrength = 0.00f;

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
            Projectile.penetrate = 100;
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
            List<NPC> potentialTargets = new List<NPC>();
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && !npc.dontTakeDamage && npc.lifeMax > 5)
                {
                    potentialTargets.Add(npc);
                }
            }

            if (potentialTargets.Count > 0)
            {
                closest = potentialTargets[Main.rand.Next(potentialTargets.Count)];
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

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.position);

            return true;
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

                    float trailScale = trailAlpha * .8f;

                    Color trailColor = Color.OrangeRed * trailAlpha * (1f - Projectile.alpha / 255f);

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
                Color.OrangeRed * (1f - Projectile.alpha / 255f),
                Projectile.rotation - MathHelper.ToRadians(45f),
                mainOrigin,
                1f,
                SpriteEffects.None,
                0
            );

            return false;
        }
    }
}
