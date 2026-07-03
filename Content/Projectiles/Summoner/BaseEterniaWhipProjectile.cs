using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Eternia.Content.Items.Weapons.Promotion;
using Eternia.Content.Players;
using Eternia.Content.Souls;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Projectiles.Summoner
{
    public abstract class BaseEterniaWhipProjectile : ModProjectile
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/SummonerSoul";

        protected abstract int SegmentCount { get; }

        protected abstract float RangeMultiplier { get; }

        protected abstract Color WhipColor { get; }

        protected abstract int TagDamage { get; }

        protected virtual string RequiredSubclass => null;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.IsAWhip[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.DefaultToWhip();
            Projectile.DamageType = DamageClass.SummonMeleeSpeed;
            Projectile.WhipSettings.Segments = SegmentCount;
            Projectile.WhipSettings.RangeMultiplier = RangeMultiplier;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (!OwnerCanUseProjectile())
            {
                Projectile.Kill();
                return false;
            }

            return null;
        }

        public override void OnHitNPC(
            NPC target,
            NPC.HitInfo hit,
            int damageDone)
        {
            if (!OwnerCanUseProjectile())
            {
                Projectile.Kill();
                return;
            }

            Player player =
                Main.player[Projectile.owner];

            player.MinionAttackTargetNPC =
                target.whoAmI;

            Projectile.damage =
                (int)(Projectile.damage * 0.72f);
        }

        public override bool PreDraw(
            ref Color lightColor)
        {
            List<Vector2> controlPoints =
                new List<Vector2>();

            Projectile.FillWhipControlPoints(
                Projectile,
                controlPoints);

            Texture2D pixel =
                TextureAssets.MagicPixel.Value;

            int pointCount =
                controlPoints.Count;

            for (int i = 0; i < pointCount - 1; i++)
            {
                Vector2 start =
                    controlPoints[i] - Main.screenPosition;

                Vector2 end =
                    controlPoints[i + 1] - Main.screenPosition;

                Vector2 diff =
                    end - start;

                float progress =
                    i / (float)System.Math.Max(1, pointCount - 1);

                Color color =
                    Color.Lerp(
                        WhipColor,
                        Color.White,
                        progress * 0.35f);

                float thickness =
                    MathHelper.Lerp(
                        5f,
                        2f,
                        progress);

                Main.EntitySpriteDraw(
                    pixel,
                    start,
                    null,
                    color,
                    diff.ToRotation(),
                    Vector2.Zero,
                    new Vector2(diff.Length(), thickness),
                    SpriteEffects.None,
                    0);
            }

            return false;
        }

        public override void ModifyHitNPC(
            NPC target,
            ref NPC.HitModifiers modifiers)
        {
            if (!OwnerCanUseProjectile())
            {
                Projectile.Kill();
                return;
            }

            if (TagDamage > 0)
            {
                modifiers.FlatBonusDamage += TagDamage;
            }
        }

        private bool OwnerCanUseProjectile()
        {
            if (Projectile.owner < 0 ||
                Projectile.owner >= Main.maxPlayers)
            {
                return false;
            }

            Player player =
                Main.player[Projectile.owner];

            if (player == null ||
                !player.active ||
                player.dead)
            {
                return false;
            }

            var soul =
                player.GetModPlayer<EterniaPlayer>();

            if (!soul.HasClassSoul ||
                soul.ActiveSoul != SoulId.Summoner)
            {
                return false;
            }

            return string.IsNullOrEmpty(RequiredSubclass) ||
                SubclassLockHelper.PlayerHasSubclass(
                    player,
                    RequiredSubclass);
        }
    }
}
