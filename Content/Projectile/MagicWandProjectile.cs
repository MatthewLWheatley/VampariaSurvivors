// Example: Updated MagicWandProjectile.cs with area scaling

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using VampariaSurvivors.Content.Items;

namespace VampariaSurvivors.Content.Projectile
{
    public class MagicWandControllerProjectile : ModProjectile
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
                        Pierce = 1,
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
            float shootSpeed = 12f * weaponStats.Speed;
            Vector2 velocity = shootDirection * shootSpeed;

            int projectileType = ModContent.ProjectileType<MagicWandProjectile>();

            Terraria.Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                player.Center,
                velocity,
                projectileType,
                weaponStats.Damage,
                weaponStats.Knockback,
                player.whoAmI,
                ai0: weaponStats.Pierce,
                ai1: weaponStats.Duration,
                ai2: weaponStats.Area
            );
        }
    }

    public class MagicWandProjectile : ModProjectile
    {
        private int penetrationsLeft;
        private int maxTrailLength = 6;
        private float homingStrength = 0.05f;
        private float areaScale = 1.0f;
        private int baseWidth = 16;
        private int baseHeight = 16;

        public override void SetDefaults()
        {
            Projectile.width = baseWidth;
            Projectile.height = baseHeight;
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

            areaScale = Projectile.ai[2];
            if (areaScale <= 0) areaScale = 1.0f;

            Projectile.width = (int)(baseWidth * areaScale);
            Projectile.height = (int)(baseHeight * areaScale);
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

            Main.EntitySpriteDraw(
                mainTexture,
                mainDrawPosition,
                null,
                Color.LightBlue * (1f - Projectile.alpha / 255f),
                Projectile.rotation,
                mainOrigin,
                areaScale,
                SpriteEffects.None,
                0
            );

            return false;
        }
    }
}