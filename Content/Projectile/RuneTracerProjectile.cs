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
    public class RuneTracerControllerProjectile : ModProjectile
    {
        public int level = 1;
        private int manaTimer = 0;
        private int shootTimer = 0;
        private float ManaCost = 10f;
        
        private int shootCooldown = 180;
        private int projectileCount = 1;
        private int projectilePenetration = 1;
        private int damage = 20;
        private float projectileSpeed = 10f;
        private int duration = 135;
        
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse itemUse)
            {
                ManaCost = Main.player[Projectile.owner].GetManaCost(itemUse.Item) / 2;

                if (itemUse.Item.type == ModContent.ItemType<Content.Items.RuneTracerLvl1>()) level = 1;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.RuneTracerLvl2>()) level = 2;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.RuneTracerLvl3>()) level = 3;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.RuneTracerLvl4>()) level = 4;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.RuneTracerLvl5>()) level = 5;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.RuneTracerLvl6>()) level = 6;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.RuneTracerLvl7>()) level = 7;
                else if (itemUse.Item.type == ModContent.ItemType<Content.Items.RuneTracerLvl8>()) level = 8;
            }

            damage = (int)(damage * (1 + 0.25f * (level - 1)));

            if (level >= 2) damage += 10;
            if (level >= 2) projectileSpeed *= 1.2f;
            if (level >= 3) duration += 18;
            if (level >= 3) damage += 10;
            if (level >= 4) projectileCount = 2;
            if (level >= 5) damage += 10;
            if (level >= 5) projectileSpeed *= 1.2f;
            if (level >= 6) duration += 18;
            if (level >= 6) damage += 10;
            if (level >= 7) projectileCount = 3;
            if (level >= 8) duration += 30;
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
                ShootAtTarget(player);
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

        private void ShootAtTarget(Player player)
        {
            
            for (int i = 0; i < projectileCount; i++)
            {
                Vector2 randomDirection = Main.rand.NextVector2Unit();

                float shootSpeed = projectileSpeed; // why was i multiplying this by 16? did i think it was pixels? fucking knob 
                Vector2 velocity = randomDirection * shootSpeed;

                int projectileType = ModContent.ProjectileType<RuneTracerProjectile>();

                // i should probably do this in the projectile itself, but fuck around and find out
                // i moved it

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
    }
    
    public class RuneTracerProjectile : ModProjectile
    {

        private bool shouldBounceX = false;
        private bool shouldBounceY = false;
        private Vector2 bounceVelocity;
        private int penetrationsLeft;
        private List<Vector2> trailPositions = new List<Vector2>();
        private int maxTrailLength = 80; // this might be too long not my problem
        private float homingStrength = 0.05f;
        private Color runeColor;

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
            Projectile.penetrate = -1; // does this mean it can hit multiple enemies? or just doesnt spawn? or doesnt hit at all? who the fuck diceded this 
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.timeLeft = (int)Projectile.ai[0];
            int red = Main.rand.Next(0, 256);
            int green = Main.rand.Next(0, 256);
            int blue = Main.rand.Next(0, 256);

            runeColor = new Color(red, green, blue);
        }
        
        public override void AI()
        {
            int maxWidth = 1920;
            int maxHeight = 1080;
            if (Main.graphics.GraphicsDevice.Viewport.Width < 1280 || Main.graphics.GraphicsDevice.Viewport.Height < 720)
            {
                maxWidth = 1280;
                maxHeight = 720;
            }
            int actualScreenWidth = Main.graphics.GraphicsDevice.Viewport.Width;
            int actualScreenHeight = Main.graphics.GraphicsDevice.Viewport.Height;

            int horizontalOffset = 0;
            if (actualScreenWidth > maxWidth)
            {
                horizontalOffset = (actualScreenWidth - maxWidth) / 2;
            }

            int verticalOffset = 0;
            if (actualScreenHeight > maxHeight)
            {
                verticalOffset = (actualScreenHeight - maxHeight) / 2;
            }
            if(verticalOffset < horizontalOffset)verticalOffset = (int)(horizontalOffset / Main.graphics.GraphicsDevice.Viewport.AspectRatio);
            if(verticalOffset > horizontalOffset)horizontalOffset = (int)(verticalOffset / Main.graphics.GraphicsDevice.Viewport.AspectRatio);
            Projectile.rotation += 0.01f;

            if (Projectile.position.X <= Main.screenPosition.X + horizontalOffset + (Projectile.height * 2))
            {
                Projectile.velocity.X = -Projectile.velocity.X;
            }
            if (Projectile.position.X >= Main.screenPosition.X + Main.screenWidth - horizontalOffset - (Projectile.height * 2))
            { 
                Projectile.velocity.X = -Projectile.velocity.X;
            }
            if (Projectile.position.Y <= Main.screenPosition.Y + verticalOffset + (Projectile.height * 2))
            {
                Projectile.velocity.Y = -Projectile.velocity.Y;
            }
            if (Projectile.position.Y > Main.screenPosition.Y + Main.screenHeight - verticalOffset - (Projectile.height * 2)) 
            {
                Projectile.velocity.Y = -Projectile.velocity.Y;
            }



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


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            
            Main.NewText("fuck X " + Projectile.position.X + " you " + (Main.screenPosition.X + Main.offScreenRange) + "cunt" + Main.player[Projectile.owner].position.X);

            // IM KEEPING THIS HERE AS A REMINDER OF HOW IM A FUCKING IDIOT
            // bounceVelocity = oldVelocity;
            // if (Projectile.velocity.X > oldVelocity.X || Projectile.velocity.X < -oldVelocity.X)
            // {
            //     shouldBounceX = true;
            //     bounceVelocity.X = -oldVelocity.X;
            //     Projectile.velocity.X = -oldVelocity.X;
            //     Projectile.position.X -= oldVelocity.X * 2;
            //     Main.NewText($"New X velocity: {Projectile.velocity.X:F2}");
            // }
            // if (Projectile.velocity.Y > oldVelocity.Y || Projectile.velocity.Y < -oldVelocity.Y)
            // {
            //     shouldBounceY = true;
            //     bounceVelocity.Y = -oldVelocity.Y;
            //     Projectile.velocity.Y = -oldVelocity.Y;
            //     Projectile.position.Y -= oldVelocity.Y * 2;
            //     Main.NewText($"New Y velocity: {Projectile.velocity.Y:F2}");
            // }

            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }

            if(Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }


            return false; // what does this do?
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

                    Color trailColor = runeColor * trailAlpha * (1f - Projectile.alpha / 255f);

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
