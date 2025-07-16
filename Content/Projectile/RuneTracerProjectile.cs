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
    public class RuneTracerControllerProjectile : ModProjectile
    {
        private int manaTimer = 0;
        private int shootTimer = 0;
        private float ManaCost = 10f;

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
                        Amount = 1,
                        Speed = 10f,
                        Duration = 135,
                        Cooldown = 180,
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
                ShootRunes(player);
                shootTimer = 0;
            }
        }

        private void ShootRunes(Player player)
        {
            for (int i = 0; i < weaponStats.Amount; i++)
            {
                Vector2 randomDirection = Main.rand.NextVector2Unit();

                float shootSpeed = weaponStats.Speed;
                Vector2 velocity = randomDirection * shootSpeed;

                int projectileType = ModContent.ProjectileType<RuneTracerProjectile>();

                Terraria.Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    player.Center,
                    velocity,
                    projectileType,
                    weaponStats.Damage,
                    weaponStats.Knockback,
                    player.whoAmI,
                    ai0: weaponStats.Duration
                );
            }
        }
    }

    public class RuneTracerProjectile : ModProjectile
    {
        private List<Vector2> trailPositions = new List<Vector2>();
        private int maxTrailLength = 80;
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
            Projectile.penetrate = -1;
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
            Projectile.rotation += 0.01f;

            Player owner = Main.player[Projectile.owner];

            if (Projectile.position.X <= owner.position.X - Main.ViewSize.X / 2 + Projectile.height * 2)
            {
                Projectile.velocity.X = Math.Abs(Projectile.velocity.X);
            }
            if (Projectile.position.X >= owner.position.X + Main.ViewSize.X / 2 - Projectile.height)
            {
                Projectile.velocity.X = -Math.Abs(Projectile.velocity.X);
            }
            if (Projectile.position.Y <= owner.position.Y - Main.ViewSize.Y / 2 + 32)
            {
                Projectile.velocity.Y = Math.Abs(Projectile.velocity.Y);
            }
            if (Projectile.position.Y >= owner.position.Y + Main.ViewSize.Y / 2)
            {
                Projectile.velocity.Y = -Math.Abs(Projectile.velocity.Y);
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
            SoundEngine.PlaySound(SoundID.Tink, Projectile.position);

            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }

            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }

            return false;
        }

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