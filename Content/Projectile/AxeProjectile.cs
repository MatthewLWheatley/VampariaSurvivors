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
	public class AxeControllerProjectile : ModProjectile
	{
		public int level = 1;
		private int manaTimer = 0;
		private int shootTimer = 0;
		private float ManaCost = 10f;

		private int shootCooldown = 60;
		private int projectileCount = 2;
		private int projectilePenetration = 3;
		private int damage = 20;

		private int burstDelay = 6;
		private int burstCooldown = 0;
		private int burstShotCount = 0;

		public override void OnSpawn(IEntitySource source)
		{
			if (source is EntitySource_ItemUse itemUse)
			{
				ManaCost = Main.player[Projectile.owner].GetManaCost(itemUse.Item) / 2;

				if (itemUse.Item.type == ModContent.ItemType<Content.Items.AxeLvl1>()) level = 1;
				else if (itemUse.Item.type == ModContent.ItemType<Content.Items.AxeLvl2>()) level = 2;
				else if (itemUse.Item.type == ModContent.ItemType<Content.Items.AxeLvl3>()) level = 3;
				else if (itemUse.Item.type == ModContent.ItemType<Content.Items.AxeLvl4>()) level = 4;
				else if (itemUse.Item.type == ModContent.ItemType<Content.Items.AxeLvl5>()) level = 5;
				else if (itemUse.Item.type == ModContent.ItemType<Content.Items.AxeLvl6>()) level = 6;
				else if (itemUse.Item.type == ModContent.ItemType<Content.Items.AxeLvl7>()) level = 7;
				else if (itemUse.Item.type == ModContent.ItemType<Content.Items.AxeLvl8>()) level = 8;
			}

			if (level >= 2) projectileCount = 3;
			if (level >= 3) damage += 20;
			if (level >= 4) projectilePenetration += 2;
			if (level >= 5) projectileCount = 4;
			if (level >= 6) damage += 20;
			if (level >= 7) projectilePenetration += 2;
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
				ShootAxe(player, burstShotCount);
				burstCooldown = 0;
				burstShotCount++;
			}
		}

		private void ShootAxe(Player player, int axeIndex)
		{
			Vector2 shootPosition = player.Center;

			Vector2 velocity;

			if (axeIndex == 0)
			{
				velocity = new Vector2(0, -8f);
			}
			else
			{
				float facingDirection = player.direction;
				float arcAngle = axeIndex * 0.5f;

				velocity = new Vector2(
					facingDirection * arcAngle * 3f,
					-8f + (arcAngle * 0.5f)
				);
			}

			int projectileType = ModContent.ProjectileType<AxeProjectile>();

			Terraria.Projectile.NewProjectile(
				Projectile.GetSource_FromThis(),
				shootPosition,
				velocity,
				projectileType,
				damage,
				2f,
				player.whoAmI,
				ai0: projectilePenetration
			);
		}
	}

	public class AxeProjectile : ModProjectile
	{
		private int penetrationsLeft;
		private List<Vector2> trailPositions = new List<Vector2>();
		private int maxTrailLength = 6;

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.timeLeft = 300;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = true;
			Projectile.light = 0.3f;
			Projectile.penetrate = 100;
		}

		public override void OnSpawn(IEntitySource source)
		{
			penetrationsLeft = (int)Projectile.ai[0];
			if (penetrationsLeft <= 0) penetrationsLeft = 3;
		}

		public override void AI()
		{
			Projectile.velocity.Y += 0.2f;

			Projectile.rotation += 0.3f;

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

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
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
			if (trailPositions.Count > 1)
			{
				for (int i = 1; i < trailPositions.Count; i++)
				{
					Vector2 drawPosition = trailPositions[i] - Main.screenPosition;
					float trailAlpha = (float)(maxTrailLength - i) / maxTrailLength * 0.6f;
					Color trailColor = Color.Brown * trailAlpha * (1f - Projectile.alpha / 255f);

					Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
					Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);

					Main.EntitySpriteDraw(texture, drawPosition, null, trailColor,
										Projectile.rotation, origin, trailAlpha * 0.8f,
										SpriteEffects.None, 0);
				}
			}

			Texture2D mainTexture = ModContent.Request<Texture2D>(Texture).Value;
			Vector2 mainDrawPosition = Projectile.Center - Main.screenPosition;
			Vector2 mainOrigin = new Vector2(mainTexture.Width / 2f, mainTexture.Height / 2f);

			Main.EntitySpriteDraw(mainTexture, mainDrawPosition, null,
								Color.White * (1f - Projectile.alpha / 255f),
								Projectile.rotation, mainOrigin, 1f,
								SpriteEffects.None, 0);

			return false;
		}
	}
}