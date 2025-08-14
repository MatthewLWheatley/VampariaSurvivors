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
    public class LightningRingControllerProjectile : ModProjectile
    {
        private int manaTimer = 0;
        private int shootTimer = 0;
        private float ManaCost = 10f;
        private int burstCooldown = 0;
        private int burstShotCount = 0;

        private WeaponStats weaponStats;
        private WeaponStats originalWeaponStats;

        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse itemUse)
            {
                ManaCost = Main.player[Projectile.owner].GetManaCost(itemUse.Item) / 2;

                if (itemUse.Item.ModItem is VSWeapon weapon)
                {
                    weaponStats = weapon.GetWeaponStats();
                    originalWeaponStats = weapon.GetWeaponStats(); // Store original stats without player bonuses
                }
                else
                {
                    weaponStats = new WeaponStats
                    {
                        Damage = 25,
                        Amount = 1,
                        Area = 1.0f,
                        Cooldown = 120,
                        ProjectileInterval = 10
                    };
                    originalWeaponStats = weaponStats;
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

            // Check if the lightning ring weapon is still equipped/in inventory
            if (!IsWeaponEquipped(player))
            {
                Projectile.Kill();
                return;
            }

            // Check if accessories changed and kill if weapon stats would be different
            if (HasAccessoryStatsChanged(player))
            {
                Projectile.Kill();
                return;
            }

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
                StrikeLightning(player);
                burstCooldown = 0;
                burstShotCount++;
            }
        }

        private bool IsWeaponEquipped(Player player)
        {
            // Check if any LightningRing weapon is in the player's inventory
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].ModItem is VSWeapon weapon && weapon.WeaponIdentifier == "LightningRing")
                {
                    return true;
                }
            }
            return false;
        }

        private bool HasAccessoryStatsChanged(Player player)
        {
            // Find the current weapon and check if its current stats differ from when projectile was created
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].ModItem is VSWeapon weapon && weapon.WeaponIdentifier == "LightningRing")
                {
                    WeaponStats currentStats = weapon.GetWeaponStats(player);
                    
                    // Compare key stats that affect weapon performance
                    return currentStats.Damage != weaponStats.Damage ||
                           Math.Abs(currentStats.Area - weaponStats.Area) > 0.001f ||
                           currentStats.Amount != weaponStats.Amount ||
                           currentStats.Cooldown != weaponStats.Cooldown ||
                           currentStats.ProjectileInterval != weaponStats.ProjectileInterval;
                }
            }
            return false; // Weapon not found, let the IsWeaponEquipped check handle it
        }

        private void StrikeLightning(Player player)
        {
            // Find random enemy within range
            NPC target = FindRandomEnemy(player.Center, 600f);
            Vector2 strikePosition;
            
            if (target != null)
            {
                // Strike near the enemy with some randomness
                strikePosition = target.Center + new Vector2(
                    Main.rand.Next(-30, 31), 
                    Main.rand.Next(-20, 21)
                );
            }
            else
            {
                // Strike randomly around the player if no enemies
                strikePosition = player.Center + new Vector2(
                    Main.rand.Next(-200, 201),
                    Main.rand.Next(-150, 151)
                );
            }

            // Find ground level for the strike
            Vector2 groundPosition = FindGroundLevel(strikePosition);

            int projectileType = ModContent.ProjectileType<LightningBoltProjectile>();

            // Spawn lightning bolt high above the ground position
            Vector2 lightningStart = groundPosition + new Vector2(0, -800);

            Terraria.Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                lightningStart,
                Vector2.Zero, // Lightning doesn't move, it's just visual
                projectileType,
                weaponStats.Damage,
                weaponStats.Knockback,
                player.whoAmI,
                ai0: groundPosition.X, // Target X position
                ai1: groundPosition.Y, // Target Y position
                ai2: weaponStats.Area  // Pass area scale
            );
        }

        private Vector2 FindGroundLevel(Vector2 startPosition)
        {
            // Raycast downward to find ground
            for (int y = (int)startPosition.Y; y < Main.maxTilesY * 16; y += 16)
            {
                int tileX = (int)(startPosition.X / 16);
                int tileY = y / 16;
                
                if (tileX >= 0 && tileX < Main.maxTilesX && tileY >= 0 && tileY < Main.maxTilesY)
                {
                    if (Main.tile[tileX, tileY].HasTile && Main.tileSolid[Main.tile[tileX, tileY].TileType])
                    {
                        return new Vector2(startPosition.X, y);
                    }
                }
            }
            
            // If no ground found, use the start position
            return startPosition;
        }

        private NPC FindRandomEnemy(Vector2 position, float maxRange)
        {
            List<NPC> validEnemies = new List<NPC>();

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && !npc.dontTakeDamage && npc.lifeMax > 5)
                {
                    float distance = Vector2.Distance(npc.Center, position);
                    if (distance <= maxRange)
                    {
                        validEnemies.Add(npc);
                    }
                }
            }

            if (validEnemies.Count > 0)
            {
                return validEnemies[Main.rand.Next(validEnemies.Count)];
            }

            return null;
        }
    }

    public class LightningBoltProjectile : ModProjectile
    {
        private Vector2 targetPosition;
        private bool hasStruck = false;
        private int strikeTimer = 0;
        private const int STRIKE_DELAY = 15; // 0.25 seconds delay before strike

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 800;
            Projectile.friendly = false; // Lightning bolt itself doesn't damage
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 60; // 1 second
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.light = 1.0f;
            Projectile.penetrate = 1;
            Projectile.alpha = 255; // Start invisible
        }

        public override void OnSpawn(IEntitySource source)
        {
            targetPosition = new Vector2(Projectile.ai[0], Projectile.ai[1]);
        }

        public override void AI()
        {
            strikeTimer++;

            if (strikeTimer < STRIKE_DELAY)
            {
                // Warning phase - create pre-strike effects
                if (Main.rand.NextBool(3))
                {
                    Dust warning = Dust.NewDustDirect(
                        targetPosition - new Vector2(16, 8),
                        32, 16,
                        DustID.Electric,
                        0, -2f,
                        100,
                        Color.Yellow,
                        0.8f
                    );
                    warning.noGravity = true;
                }
            }
            else if (!hasStruck)
            {
                // Strike phase
                StrikeLightning();
                hasStruck = true;
            }

            if (hasStruck)
            {
                // Fade out
                Projectile.alpha += 15;
                if (Projectile.alpha >= 255)
                {
                    Projectile.Kill();
                }
            }
        }

        private void StrikeLightning()
        {
            // Play lightning sound
            SoundEngine.PlaySound(SoundID.Thunder, targetPosition);

            // Create the ground hitbox that actually deals damage
            int hitboxType = ModContent.ProjectileType<LightningStrikeHitboxProjectile>();
            
            Terraria.Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                targetPosition,
                Vector2.Zero,
                hitboxType,
                Projectile.damage,
                Projectile.knockBack,
                Projectile.owner,
                ai0: Projectile.ai[2] // Pass area scale
            );

            // Visual lightning effects
            CreateLightningEffects();
        }

        private void CreateLightningEffects()
        {
            // Lightning bolt visual effects
            for (int i = 0; i < 20; i++)
            {
                Vector2 lightningPos = Vector2.Lerp(Projectile.Center, targetPosition, (float)i / 19);
                lightningPos += new Vector2(Main.rand.Next(-8, 9), 0);

                Dust lightning = Dust.NewDustDirect(
                    lightningPos,
                    4, 4,
                    DustID.Electric,
                    Main.rand.NextFloat(-2f, 2f),
                    Main.rand.NextFloat(-1f, 1f),
                    100,
                    Color.White,
                    Main.rand.NextFloat(1.2f, 2.0f)
                );
                lightning.noGravity = true;
                lightning.fadeIn = 1.2f;
            }

            // Ground impact effects
            for (int i = 0; i < 15; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(3f, 8f);
                Dust impact = Dust.NewDustDirect(
                    targetPosition - new Vector2(16, 8),
                    32, 16,
                    DustID.Electric,
                    velocity.X,
                    velocity.Y,
                    100,
                    Color.Cyan,
                    Main.rand.NextFloat(1.0f, 1.8f)
                );
                impact.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (strikeTimer < STRIKE_DELAY || hasStruck) return false;

            // Draw lightning bolt
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 origin = new Vector2(texture.Width / 2f, 0);

            Color drawColor = Color.White * (1f - Projectile.alpha / 255f);

            Main.EntitySpriteDraw(
                texture,
                drawPosition,
                null,
                drawColor,
                0f,
                origin,
                1.0f,
                SpriteEffects.None,
                0
            );

            return false;
        }
    }

    public class LightningStrikeHitboxProjectile : ModProjectile
    {
        private float areaScale = 1.0f;
        private int damageTimer = 0;
        private Dictionary<int, int> enemyCooldowns = new Dictionary<int, int>();

        public override void SetDefaults()
        {
            Projectile.width = 32; // Base size for level 1
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 30; // 0.5 seconds
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.light = 0.6f;
            Projectile.penetrate = -1;
            Projectile.alpha = 100;
        }

        public override void OnSpawn(IEntitySource source)
        {
            // Get area scale from ai0 parameter passed from lightning bolt
            areaScale = Projectile.ai[0];
            if (areaScale <= 0) areaScale = 1.0f; // Fallback

            // Apply area scaling to hitbox size - balanced base, proper scaling
            int finalSize = (int)(32 * areaScale); // 32px base, scales with area bonuses
            if (finalSize < 24) finalSize = 24; // Minimum viable size
            
            Projectile.width = finalSize;
            Projectile.height = finalSize;

            // Center the hitbox on the spawn position
            Projectile.position -= new Vector2((finalSize - 32) / 2f, (finalSize - 32) / 2f);
        }

        public override void AI()
        {
            damageTimer++;

            // Damage enemies in the hitbox area
            if (damageTimer % 5 == 0) // Check every 5 ticks
            {
                DamageNearbyEnemies();
            }

            // Clean up expired enemy cooldowns
            List<int> expiredKeys = new List<int>();
            foreach (var kvp in enemyCooldowns)
            {
                enemyCooldowns[kvp.Key]--;
                if (enemyCooldowns[kvp.Key] <= 0)
                    expiredKeys.Add(kvp.Key);
            }
            foreach (int key in expiredKeys)
                enemyCooldowns.Remove(key);

            // Electric particle effects
            if (Main.rand.NextBool(2))
            {
                Vector2 particlePos = Projectile.position + new Vector2(
                    Main.rand.Next(Projectile.width),
                    Main.rand.Next(Projectile.height)
                );

                Dust electric = Dust.NewDustDirect(
                    particlePos,
                    4, 4,
                    DustID.Electric,
                    Main.rand.NextFloat(-3f, 3f),
                    Main.rand.NextFloat(-3f, 3f),
                    100,
                    Color.LightBlue,
                    Main.rand.NextFloat(0.6f, 1.2f)
                );
                electric.noGravity = true;
            }

            // Fade out
            Projectile.alpha += 8;
        }

        private void DamageNearbyEnemies()
        {
            float damageRadius = Math.Max(Projectile.width, Projectile.height) * 0.5f;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && !npc.dontTakeDamage)
                {
                    float distance = Vector2.Distance(npc.Center, Projectile.Center);

                    if (distance < damageRadius)
                    {
                        int uniqueKey = npc.whoAmI;

                        if (!enemyCooldowns.ContainsKey(uniqueKey))
                        {
                            npc.StrikeNPC(npc.CalculateHitInfo(Projectile.damage, 0, false, Projectile.knockBack));
                            enemyCooldowns[uniqueKey] = 15; // 0.25 second cooldown
                        }
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Draw electric circle effect
            Texture2D texture = ModContent.Request<Texture2D>("VampariaSurvivors/Content/Projectile/LightningStrikeHitbox").Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);

            Color drawColor = Color.Lerp(Color.LightBlue, Color.White, 0.5f) * (1f - Projectile.alpha / 255f);

            float visualScale = (float)Projectile.width / 32f; // Scale 32x32 texture to match hitbox size
            Main.EntitySpriteDraw(
                texture,
                drawPosition,
                null,
                drawColor,
                0f,
                origin,
                visualScale,
                SpriteEffects.None,
                0
            );

            return false;
        }
    }
}