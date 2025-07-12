using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.DataStructures;

namespace VampariaSurvivors.Content.Projectile
{
    public class GarlicAuraProjectile : ModProjectile
    {
        public int level = 1;

        private int manaTimer = 0;
        private float currentRotation = 0f;
        private float rotationSpeed = 0.02f;
        
        private Dictionary<int, int> enemyCooldowns = new Dictionary<int, int>();
        private int DAMAGE_COOLDOWN = 78;

        private int Damage = 20;

        
        private float scale = 3.5f;

        private float ManaCost = 10f;

        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse itemUse)
            {
                ManaCost = Main.player[Projectile.owner].GetManaCost(itemUse.Item)/2;
                if (itemUse.Item.type == ModContent.ItemType<Content.Items.GarlicLvl1>()) level = 1;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.GarlicLvl2>()) level = 2;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.GarlicLvl3>()) level = 3;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.GarlicLvl4>()) level = 4;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.GarlicLvl5>()) level = 5;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.GarlicLvl6>()) level = 6;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.GarlicLvl7>()) level = 7;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.GarlicLvl8>()) level = 8;

            }
            Damage = (int)(Damage*(1+.2*(level-1)));
            if (level > 1) scale = 3.5f * 1.4f;
            if (level > 2) DAMAGE_COOLDOWN = 72;
            if (level > 3) scale = 3.5f * 1.6f;
            if (level > 4) DAMAGE_COOLDOWN = 66;
            if (level > 5) scale = 3.5f * 1.8f;
            if (level > 6) DAMAGE_COOLDOWN = 60;
            if (level > 7) scale = 3.5f * 2.0f; 
        }

        public override void SetDefaults()
        {
            Projectile.width = 160;
            Projectile.height = 160;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 360000;
            Projectile.alpha = 0;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.Center = player.Center;

            currentRotation += rotationSpeed;
            Projectile.rotation = currentRotation;

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

            float auraRadius = (scale +.5f)*16;
            
            //update cooldowns and remove expired ones
            List<int> expiredKeys = new List<int>();
            foreach (var kvp in enemyCooldowns)
            {
                enemyCooldowns[kvp.Key]--;
                if (enemyCooldowns[kvp.Key] <= 0)
                    expiredKeys.Add(kvp.Key);
            }
            foreach (int key in expiredKeys)
                enemyCooldowns.Remove(key);

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && !npc.dontTakeDamage)
                {
                    float distance = Vector2.Distance(npc.Center, Projectile.Center);
                    if (distance < auraRadius)
                    {
                        int uniqueKey = npc.whoAmI;

                        // Check enemy
                        if (!enemyCooldowns.ContainsKey(uniqueKey))
                        {
                            // damage
                            int damage = Damage;
                            npc.StrikeNPC(npc.CalculateHitInfo(damage, 0, false, 0));

                            if (Main.netMode != NetmodeID.Server)
                            {
                                Main.NewText($"Garlic hit NPC {npc.whoAmI} for {damage} damage");
                            }

                            // Add enemy
                            enemyCooldowns[uniqueKey] = DAMAGE_COOLDOWN;
                        }
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
            
            
            Main.EntitySpriteDraw(
                texture,
                drawPosition,
                null,
                Color.White * 0.8f,
                Projectile.rotation,
                origin,
                scale,
                SpriteEffects.None,
                0
            );
            
            return false;
        }
    }
}