using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using VampariaSurvivors.Content.Items;

namespace VampariaSurvivors.Content.Projectile
{
    public class PentagramControllerProjectile : ModProjectile
    {
        private int cooldownTimer = 0;
        private float ManaCost = 40f;

        private WeaponStats weaponStats;
        private WeaponStats originalWeaponStats;

        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse itemUse)
            {
                ManaCost = Main.player[Projectile.owner].GetManaCost(itemUse.Item);

                if (itemUse.Item.ModItem is VSWeapon weapon)
                {
                    weaponStats = weapon.GetWeaponStats();
                    originalWeaponStats = weapon.GetWeaponStats();
                }
                else
                {
                    weaponStats = new WeaponStats
                    {
                        Damage = 660,
                        Cooldown = 3600, // 60 seconds in ticks
                        Chance = 0.0f
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

        private bool IsWeaponEquipped(Player player)
        {
            // Check if any Pentagram weapon is in the player's inventory
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].ModItem is VSWeapon weapon && weapon.WeaponIdentifier == "Pentagram")
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
                if (player.inventory[i].ModItem is VSWeapon weapon && weapon.WeaponIdentifier == "Pentagram")
                {
                    WeaponStats currentStats = weapon.GetWeaponStats(player);
                    
                    // Compare key stats that affect weapon performance
                    return currentStats.Damage != weaponStats.Damage ||
                           currentStats.Cooldown != weaponStats.Cooldown ||
                           Math.Abs(currentStats.Chance - weaponStats.Chance) > 0.001f;
                }
            }
            return false; // Weapon not found, let the IsWeaponEquipped check handle it
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.Center;

            // Check if the Pentagram weapon is still equipped/in inventory
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

            cooldownTimer++;

            // Check if it's time to activate Pentagram
            if (cooldownTimer >= weaponStats.Cooldown)
            {
                ActivatePentagram(player);
                cooldownTimer = 0;
            }
        }

        private void ActivatePentagram(Player player)
        {
            // Play activation sound
            SoundEngine.PlaySound(SoundID.Item121, player.Center);
            
            // Show activation message
            Main.NewText("PENTAGRAM ACTIVATED!", 255, 50, 50);

            // Create screen-covering visual effect
            CreatePentagramVisual(player);

            // Create the actual damage projectile that covers the screen
            Terraria.Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                player.Center,
                Vector2.Zero,
                ModContent.ProjectileType<PentagramDamageProjectile>(),
                weaponStats.Damage,
                0f,
                player.whoAmI,
                ai0: weaponStats.Chance // Pass preservation chance
            );

            // Erase ALL enemies on screen immediately (no drops)
            EraseAllEnemies(player);

            // Erase all items immediately with preservation chance
            EraseAllItems(player);
        }

        private void CreatePentagramVisual(Player player)
        {
            // Create massive visual effect covering the screen
            for (int i = 0; i < 300; i++)
            {
                Vector2 position = player.Center + Main.rand.NextVector2Circular(1200, 800);
                Dust pentagram = Dust.NewDustDirect(
                    position,
                    4, 4,
                    DustID.RedTorch,
                    0, 0,
                    100,
                    Color.DarkRed,
                    Main.rand.NextFloat(2.0f, 5.0f)
                );
                pentagram.noGravity = true;
                pentagram.fadeIn = 3.0f;
            }
        }

        private void EraseAllEnemies(Player player)
        {
            int enemiesErased = 0;
            int enemiesKilled = 0;

            // Process ALL enemies on screen (not NPCs like town NPCs)
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && !npc.townNPC)
                {
                    // Check if enemy is on screen
                    Vector2 screenPos = Main.screenPosition;
                    Rectangle screenRect = new Rectangle((int)screenPos.X, (int)screenPos.Y, Main.screenWidth, Main.screenHeight);
                    
                    if (screenRect.Intersects(npc.getRect()))
                    {
                        // Check preservation chance - if it hits, kill normally (with drops)
                        if (Main.rand.NextFloat() < weaponStats.Chance)
                        {
                            // Kill normally with drops
                            npc.StrikeInstantKill();
                            enemiesKilled++;
                            
                            // Create normal death effect
                            for (int j = 0; j < 10; j++)
                            {
                                Dust death = Dust.NewDustDirect(
                                    npc.Center,
                                    npc.width, npc.height,
                                    DustID.Blood,
                                    Main.rand.NextFloat(-5f, 5f),
                                    Main.rand.NextFloat(-5f, 5f),
                                    100,
                                    Color.Red,
                                    Main.rand.NextFloat(1.0f, 2.0f)
                                );
                                death.noGravity = true;
                            }
                        }
                        else
                        {
                            // Erase completely (no drops, no death effects)
                            CreateEraseEffect(npc.Center);
                            npc.active = false;
                            enemiesErased++;
                        }
                    }
                }
            }

            if (enemiesErased > 0 || enemiesKilled > 0)
            {
                Main.NewText($"Erased {enemiesErased} enemies, killed {enemiesKilled} normally", 255, 50, 50);
            }
        }

        private void EraseAllItems(Player player)
        {
            int itemsErased = 0;

            // Erase all items on screen with preservation chance
            for (int i = 0; i < Main.maxItems; i++)
            {
                Item item = Main.item[i];
                if (item.active)
                {
                    // Check distance to screen bounds rather than player
                    Vector2 screenPos = Main.screenPosition;
                    Rectangle screenRect = new Rectangle((int)screenPos.X, (int)screenPos.Y, Main.screenWidth, Main.screenHeight);
                    
                    if (screenRect.Contains((int)item.Center.X, (int)item.Center.Y))
                    {
                        // Check if this item should be preserved
                        bool shouldPreserve = ShouldPreserveItem(item);
                        
                        if (!shouldPreserve)
                        {
                            CreateEraseEffect(item.Center);
                            item.active = false;
                            itemsErased++;
                        }
                    }
                }
            }

            if (itemsErased > 0)
            {
                Main.NewText($"Erased {itemsErased} items from existence", 200, 100, 100);
            }
        }

        private bool ShouldPreserveItem(Item item)
        {
            // Apply preservation chance to pickups and treasure chests
            if (item.type == ItemID.Chest || 
                item.type == ItemID.GoldChest ||
                item.type == ItemID.ShadowChest ||
                item.type == ItemID.IvyChest ||
                item.rare >= ItemRarityID.Orange || // Preserve rare items with chance
                item.maxStack > 1) // Most pickups have maxStack > 1
            {
                return Main.rand.NextFloat() < weaponStats.Chance;
            }

            // Other items get erased without chance
            return false;
        }

        private void CreateEraseEffect(Vector2 position)
        {
            // Create dramatic erase effect
            for (int i = 0; i < 8; i++)
            {
                Dust erase = Dust.NewDustDirect(
                    position,
                    4, 4,
                    DustID.Shadowflame,
                    Main.rand.NextFloat(-5f, 5f),
                    Main.rand.NextFloat(-5f, 5f),
                    100,
                    Color.Purple,
                    Main.rand.NextFloat(1.5f, 3.0f)
                );
                erase.noGravity = true;
                erase.fadeIn = 2.0f;
            }
        }
    }

    // Damage projectile that covers the entire screen and erases enemies
    public class PentagramDamageProjectile : ModProjectile
    {
        private float preservationChance = 0f;

        public override void SetDefaults()
        {
            Projectile.width = Main.screenWidth;
            Projectile.height = Main.screenHeight;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 10; // Very short duration
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.alpha = 200; // Semi-transparent
        }

        public override void OnSpawn(IEntitySource source)
        {
            preservationChance = Projectile.ai[0];
            
            // Position to cover the screen
            Player player = Main.player[Projectile.owner];
            Projectile.Center = new Vector2(
                Main.screenPosition.X + Main.screenWidth / 2,
                Main.screenPosition.Y + Main.screenHeight / 2
            );
        }

        public override void AI()
        {
            // Create visual effect
            if (Projectile.timeLeft > 5)
            {
                for (int i = 0; i < 50; i++)
                {
                    Vector2 pos = Projectile.position + new Vector2(
                        Main.rand.Next(Projectile.width),
                        Main.rand.Next(Projectile.height)
                    );
                    
                    Dust visual = Dust.NewDustDirect(
                        pos, 4, 4,
                        DustID.RedTorch,
                        0, 0, 100,
                        Color.Red,
                        2.0f
                    );
                    visual.noGravity = true;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Damage projectile doesn't handle erasure - EraseAllEnemies() handles complete enemy removal
            // This allows normal damage to be dealt while the controller handles the erasure
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Draw faded pentagram image covering screen
            Texture2D texture = ModContent.Request<Texture2D>("VampariaSurvivors/Content/Items/Pentagram").Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
            
            // Scale to cover most of the screen
            float scale = Math.Max(Main.screenWidth / (float)texture.Width, Main.screenHeight / (float)texture.Height) * 0.8f;
            
            Color drawColor = Color.Red * 0.3f; // Faded red
            
            Main.EntitySpriteDraw(
                texture,
                drawPos,
                null,
                drawColor,
                0f,
                origin,
                scale,
                SpriteEffects.None,
                0
            );
            
            return false;
        }
    }
}