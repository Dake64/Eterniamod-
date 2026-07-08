using System.IO;
using ETERNIA;
using Eternia.Content.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Eternia.Content.Globals
{
    public class EterniaGlobalNPC : GlobalNPC
    {
        private static readonly EnemyRarityProfile[] NormalProfiles =
        {
            new EnemyRarityProfile(0.001f, EnemyRarity.Nightmare, 5f, 2.6f, 2.6f, 25f, 1.5f, 40, 61),
            new EnemyRarityProfile(0.004f, EnemyRarity.Ancient, 4.2f, 2.4f, 2.4f, 15f, 1.4f, 32, 51),
            new EnemyRarityProfile(0.012f, EnemyRarity.Mythic, 3.6f, 2.2f, 2.2f, 9f, 1.3f, 24, 41),
            new EnemyRarityProfile(0.03f, EnemyRarity.Legendary, 3f, 2f, 2f, 5f, 1.22f, 18, 31),
            new EnemyRarityProfile(0.07f, EnemyRarity.SuperRare, 2f, 1.55f, 1.5f, 3f, 1.15f, 12, 21),
            new EnemyRarityProfile(0.15f, EnemyRarity.Rare, 1.55f, 1.25f, 1.25f, 2f, 1.08f, 6, 14),
            new EnemyRarityProfile(0.32f, EnemyRarity.Uncommon, 1.22f, 1.1f, 1.1f, 1.35f, 1.04f, 3, 8),
            new EnemyRarityProfile(1f, EnemyRarity.Common, 1f, 1f, 1f, 1f, 1f, 1, 5)
        };

        private static readonly EnemyRarityProfile[] BossProfiles =
        {
            new EnemyRarityProfile(0.02f, EnemyRarity.Nightmare, 3.2f, 2f, 2f, 25f, 1.6f, 80, 101),
            new EnemyRarityProfile(0.06f, EnemyRarity.Ancient, 2.8f, 1.8f, 1.8f, 15f, 1.5f, 68, 86),
            new EnemyRarityProfile(0.13f, EnemyRarity.Mythic, 2.4f, 1.6f, 1.6f, 9f, 1.4f, 58, 71),
            new EnemyRarityProfile(0.25f, EnemyRarity.Legendary, 2f, 1.5f, 1.5f, 5f, 1.28f, 50, 71),
            new EnemyRarityProfile(0.45f, EnemyRarity.SuperRare, 1.5f, 1.3f, 1.3f, 3f, 1.18f, 40, 51),
            new EnemyRarityProfile(0.68f, EnemyRarity.Rare, 1.2f, 1.1f, 1.1f, 2f, 1.1f, 30, 41),
            new EnemyRarityProfile(1f, EnemyRarity.Common, 1f, 1f, 1f, 1f, 1f, 20, 31)
        };

        public override bool InstancePerEntity => true;

        public enum EnemyRarity
        {
            Common,
            Uncommon,
            Rare,
            SuperRare,
            Legendary,
            Mythic,
            Ancient,
            Nightmare
        }

        public EnemyRarity rarity = EnemyRarity.Common;

        public int enemyLevel = 1;

        public float lifeMultiplier = 1f;
        public float damageMultiplier = 1f;
        public float defenseMultiplier = 1f;
        public float expMultiplier = 1f;
        public float scaleMultiplier = 1f;

        private bool applied;

        public override void SetDefaults(NPC npc)
        {
            if (ShouldIgnore(npc))
            {
                return;
            }

            // Only the server (or singleplayer) rolls rarity. Multiplayer clients
            // receive it via ReceiveExtraAI so every client sees the same enemy.
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            ApplyRarityProfile(
                npc,
                RollRarityProfile(npc));

            applied = true;
        }

        public override void SendExtraAI(
            NPC npc,
            BitWriter bitWriter,
            BinaryWriter binaryWriter)
        {
            binaryWriter.Write((byte)rarity);
            binaryWriter.Write(enemyLevel);
            binaryWriter.Write(lifeMultiplier);
            binaryWriter.Write(scaleMultiplier);
        }

        public override void ReceiveExtraAI(
            NPC npc,
            BitReader bitReader,
            BinaryReader binaryReader)
        {
            rarity = (EnemyRarity)binaryReader.ReadByte();
            enemyLevel = binaryReader.ReadInt32();
            lifeMultiplier = binaryReader.ReadSingle();
            scaleMultiplier = binaryReader.ReadSingle();

            // Re-apply the visual/health scaling once on the client so the enemy
            // matches the server. Guarded so repeated syncs don't compound it.
            if (!applied)
            {
                npc.lifeMax =
                    (int)(npc.lifeMax * lifeMultiplier) + enemyLevel * 5;
                npc.scale *= scaleMultiplier;
                applied = true;
            }
        }

        public override bool PreDraw(
            NPC npc,
            SpriteBatch spriteBatch,
            Vector2 screenPos,
            Color drawColor)
        {
            if (rarity == EnemyRarity.Common)
            {
                return true;
            }

            float intensity = GetRarityIntensity(rarity);

            float pulse =
                0.5f + 0.5f * (float)System.Math.Sin(
                    Main.GlobalTimeWrappedHourly * (2f + intensity * 3f));

            Texture2D texture =
                TextureAssets.Npc[npc.type].Value;

            Rectangle frame = npc.frame;

            Vector2 origin =
                new Vector2(frame.Width / 2f, frame.Height / 2f);

            Vector2 drawPos =
                npc.Center - screenPos + new Vector2(0f, npc.gfxOffY);

            SpriteEffects effects =
                npc.spriteDirection == 1
                    ? SpriteEffects.FlipHorizontally
                    : SpriteEffects.None;

            // A pulsing ring of tinted echoes behind the sprite = glowing aura.
            Color glow =
                GetRarityColor(rarity) *
                (0.14f + intensity * 0.24f) *
                (0.65f + 0.35f * pulse);

            int copies = System.Math.Min(18, 4 + (int)(intensity * 6f));

            float radius =
                (2.5f + intensity * 6f) * (0.7f + 0.3f * pulse);

            // Nightmare aura shakes for an unstable, dreadful feel.
            if (rarity == EnemyRarity.Nightmare)
            {
                float shake = Main.GlobalTimeWrappedHourly * 40f;
                drawPos +=
                    new Vector2(
                        (float)System.Math.Sin(shake * 1.3f),
                        (float)System.Math.Cos(shake)) * 1.8f;
            }

            for (int i = 0; i < copies; i++)
            {
                float angle =
                    MathHelper.TwoPi * i / copies +
                    Main.GlobalTimeWrappedHourly * intensity;

                Vector2 offset = angle.ToRotationVector2() * radius;

                spriteBatch.Draw(
                    texture,
                    drawPos + offset,
                    frame,
                    glow,
                    npc.rotation,
                    origin,
                    npc.scale,
                    effects,
                    0f);
            }

            // Top tiers get a second, slower, counter-rotating outer ring.
            if (rarity >= EnemyRarity.Mythic)
            {
                Color outerGlow =
                    GetRarityColor(rarity) *
                    (0.09f + intensity * 0.08f) *
                    (0.5f + 0.5f * pulse);

                float outerRadius = radius * 1.8f;

                for (int i = 0; i < 6; i++)
                {
                    float angle =
                        MathHelper.TwoPi * i / 6f -
                        Main.GlobalTimeWrappedHourly * intensity * 0.6f;

                    Vector2 offset = angle.ToRotationVector2() * outerRadius;

                    spriteBatch.Draw(
                        texture,
                        drawPos + offset,
                        frame,
                        outerGlow,
                        npc.rotation,
                        origin,
                        npc.scale,
                        effects,
                        0f);
                }
            }

            return true;
        }

        public override void DrawEffects(
            NPC npc,
            ref Color drawColor)
        {
            if (rarity == EnemyRarity.Common)
            {
                return;
            }

            float intensity = GetRarityIntensity(rarity);
            Color color = GetRarityColor(rarity);

            float pulse =
                0.6f + 0.4f * (float)System.Math.Sin(
                    Main.GlobalTimeWrappedHourly * (2f + intensity * 3f));

            float lightScale = (0.4f + intensity * 0.7f) * pulse;

            Lighting.AddLight(
                npc.Center,
                color.R / 255f * lightScale,
                color.G / 255f * lightScale,
                color.B / 255f * lightScale);

            if (Main.rand.NextFloat() < 0.10f + intensity * 0.32f)
            {
                Vector2 spawn =
                    npc.position +
                    new Vector2(
                        Main.rand.NextFloat(npc.width),
                        Main.rand.NextFloat(npc.height));

                Dust dust =
                    Dust.NewDustPerfect(spawn, RarityDust(rarity));

                dust.noGravity = true;
                dust.scale = 0.85f + intensity * 0.85f;
                dust.velocity =
                    new Vector2(
                        Main.rand.NextFloat(-1f, 1f),
                        -1.1f - intensity);
                dust.color = color;
                dust.fadeIn = 1f + intensity;
            }
        }

        public override void PostDraw(
            NPC npc,
            SpriteBatch spriteBatch,
            Vector2 screenPos,
            Color drawColor)
        {
            if (!npc.active ||
                npc.life <= 0 ||
                ShouldIgnore(npc))
            {
                return;
            }

            DrawEnemyBadge(
                npc,
                spriteBatch,
                screenPos);
        }

        public override void OnKill(NPC npc)
        {
            if (npc.CountsAsACritter ||
                npc.friendly ||
                npc.townNPC)
            {
                return;
            }

            // NPC death is server-authoritative; multiplayer clients do nothing.
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            int exp =
                npc.boss
                    ? (int)((1000 + enemyLevel * 20) * expMultiplier)
                    : (int)(((npc.lifeMax / 10f) + enemyLevel * 2) * expMultiplier);

            if (exp < 5)
            {
                exp = 5;
            }

            int killer = npc.lastInteraction;

            bool validKiller =
                killer >= 0 &&
                killer < Main.maxPlayers &&
                Main.player[killer].active;

            if (Main.netMode == NetmodeID.Server)
            {
                if (!validKiller)
                {
                    return;
                }

                // Send the earned XP to the client that killed the enemy; that
                // client applies it and shows the level-up feedback locally.
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)EterniaMessageType.AddExperience);
                packet.Write(exp);
                packet.Send(killer);
                return;
            }

            // Singleplayer: award directly to the local player.
            Player player =
                validKiller
                    ? Main.player[killer]
                    : Main.LocalPlayer;

            if (!player.GetModPlayer<EterniaLevelPlayer>().AddExperience(exp))
            {
                return;
            }

            CombatText.NewText(
                npc.getRect(),
                Color.Gold,
                $"+{exp} EXP");
        }

        private static bool ShouldIgnore(NPC npc)
        {
            return npc.CountsAsACritter ||
                npc.friendly ||
                npc.townNPC ||
                npc.lifeMax <= 5;
        }

        private static EnemyRarityProfile RollRarityProfile(NPC npc)
        {
            EnemyRarityProfile[] profiles =
                npc.boss
                    ? BossProfiles
                    : NormalProfiles;

            float roll =
                Main.rand.NextFloat();

            for (int i = 0; i < profiles.Length; i++)
            {
                if (roll <= profiles[i].MaxRoll)
                {
                    return profiles[i];
                }
            }

            return profiles[profiles.Length - 1];
        }

        private void ApplyRarityProfile(
            NPC npc,
            EnemyRarityProfile profile)
        {
            rarity = profile.Rarity;
            lifeMultiplier = profile.LifeMultiplier;
            damageMultiplier = profile.DamageMultiplier;
            defenseMultiplier = profile.DefenseMultiplier;
            expMultiplier = profile.ExpMultiplier;
            scaleMultiplier = profile.ScaleMultiplier;
            enemyLevel =
                Main.rand.Next(
                    profile.MinLevel,
                    profile.MaxLevelExclusive);

            npc.lifeMax =
                (int)(npc.lifeMax * lifeMultiplier) +
                enemyLevel * 5;

            npc.damage =
                (int)(npc.damage * damageMultiplier) +
                enemyLevel / 2;

            npc.defense =
                (int)(npc.defense * defenseMultiplier) +
                enemyLevel / 3;

            npc.scale *= scaleMultiplier;
            npc.life = npc.lifeMax;
        }

        private void DrawEnemyBadge(
            NPC npc,
            SpriteBatch spriteBatch,
            Vector2 screenPos)
        {
            // Common non-boss enemies get only a tiny level tag to avoid clutter.
            if (rarity == EnemyRarity.Common && !npc.boss)
            {
                string levelText = $"Lv.{enemyLevel}";
                float levelScale = 0.5f;

                Vector2 size =
                    FontAssets.MouseText.Value.MeasureString(levelText) * levelScale;

                Vector2 tagPos =
                    new Vector2(
                        npc.Center.X - screenPos.X - size.X / 2f,
                        npc.position.Y - screenPos.Y - 24f);

                Utils.DrawBorderString(
                    spriteBatch,
                    levelText,
                    tagPos,
                    Color.LightGray * 0.9f,
                    levelScale);

                return;
            }

            float intensity = GetRarityIntensity(rarity);
            Color color = GetRarityColor(rarity);

            string text =
                $"{GetRarityText(rarity)}  Lv.{enemyLevel}";

            float pulse =
                0.85f + 0.15f * (float)System.Math.Sin(
                    Main.GlobalTimeWrappedHourly * (3f + intensity * 4f));

            float scale =
                (0.6f + intensity * 0.32f) *
                (rarity >= EnemyRarity.SuperRare ? pulse : 1f);

            Vector2 textSize =
                FontAssets.MouseText.Value.MeasureString(text) * scale;

            Vector2 drawPosition =
                new Vector2(
                    npc.Center.X - screenPos.X,
                    npc.position.Y - screenPos.Y - 34f - intensity * 10f);

            drawPosition.X -= textSize.X / 2f;

            Texture2D pixel = TextureAssets.MagicPixel.Value;

            Rectangle plate =
                new Rectangle(
                    (int)drawPosition.X - 8,
                    (int)drawPosition.Y - 3,
                    (int)textSize.X + 16,
                    (int)textSize.Y + 6);

            spriteBatch.Draw(pixel, plate, Color.Black * (0.34f + intensity * 0.3f));

            spriteBatch.Draw(
                pixel,
                new Rectangle(plate.X, plate.Bottom - 2, plate.Width, 2),
                color * 0.9f);

            // Glow halo behind the text for high rarities.
            int glowSteps =
                rarity >= EnemyRarity.Rare
                    ? 3 + (int)(intensity * 3f)
                    : 0;

            for (int i = 0; i < glowSteps; i++)
            {
                float angle =
                    MathHelper.TwoPi * i / System.Math.Max(1, glowSteps);

                Vector2 offset =
                    angle.ToRotationVector2() *
                    (1.4f + intensity * 1.6f) * pulse;

                Utils.DrawBorderString(
                    spriteBatch,
                    text,
                    drawPosition + offset,
                    color * 0.45f,
                    scale);
            }

            Utils.DrawBorderString(
                spriteBatch,
                text,
                drawPosition,
                color,
                scale);
        }

        private static string GetRarityText(EnemyRarity enemyRarity)
        {
            return enemyRarity switch
            {
                EnemyRarity.Uncommon => "Uncommon",
                EnemyRarity.Rare => "Rare",
                EnemyRarity.SuperRare => "Super Rare",
                EnemyRarity.Legendary => "Legendary",
                EnemyRarity.Mythic => "Mythic",
                EnemyRarity.Ancient => "Ancient",
                EnemyRarity.Nightmare => "Nightmare",
                _ => "Common"
            };
        }

        private static Color GetRarityColor(EnemyRarity enemyRarity)
        {
            return enemyRarity switch
            {
                EnemyRarity.Uncommon => Color.LightBlue,
                EnemyRarity.Rare => Color.LightGreen,
                EnemyRarity.SuperRare => Color.Gold,
                EnemyRarity.Legendary => Color.OrangeRed,
                EnemyRarity.Mythic => new Color(200, 70, 255),
                EnemyRarity.Ancient => new Color(60, 230, 210),
                EnemyRarity.Nightmare => new Color(210, 24, 44),
                _ => Color.LightGray
            };
        }

        private static float GetRarityIntensity(EnemyRarity enemyRarity)
        {
            return enemyRarity switch
            {
                EnemyRarity.Uncommon => 0.35f,
                EnemyRarity.Rare => 0.55f,
                EnemyRarity.SuperRare => 0.8f,
                EnemyRarity.Legendary => 1.15f,
                EnemyRarity.Mythic => 1.45f,
                EnemyRarity.Ancient => 1.8f,
                EnemyRarity.Nightmare => 2.2f,
                _ => 0f
            };
        }

        private static int RarityDust(EnemyRarity enemyRarity)
        {
            return enemyRarity switch
            {
                EnemyRarity.Nightmare => DustID.Shadowflame,
                EnemyRarity.Ancient => DustID.IceTorch,
                EnemyRarity.Mythic => DustID.PurpleTorch,
                EnemyRarity.Legendary => DustID.RedTorch,
                EnemyRarity.SuperRare => DustID.GoldFlame,
                _ => DustID.Enchanted_Gold
            };
        }

        private readonly struct EnemyRarityProfile
        {
            public EnemyRarityProfile(
                float maxRoll,
                EnemyRarity rarity,
                float lifeMultiplier,
                float damageMultiplier,
                float defenseMultiplier,
                float expMultiplier,
                float scaleMultiplier,
                int minLevel,
                int maxLevelExclusive)
            {
                MaxRoll = maxRoll;
                Rarity = rarity;
                LifeMultiplier = lifeMultiplier;
                DamageMultiplier = damageMultiplier;
                DefenseMultiplier = defenseMultiplier;
                ExpMultiplier = expMultiplier;
                ScaleMultiplier = scaleMultiplier;
                MinLevel = minLevel;
                MaxLevelExclusive = maxLevelExclusive;
            }

            public float MaxRoll { get; }

            public EnemyRarity Rarity { get; }

            public float LifeMultiplier { get; }

            public float DamageMultiplier { get; }

            public float DefenseMultiplier { get; }

            public float ExpMultiplier { get; }

            public float ScaleMultiplier { get; }

            public int MinLevel { get; }

            public int MaxLevelExclusive { get; }
        }
    }
}
