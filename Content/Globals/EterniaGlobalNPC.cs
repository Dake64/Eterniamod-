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
            new EnemyRarityProfile(0.01f, EnemyRarity.Legendary, 3f, 2f, 2f, 5f, 1.22f, 18, 31),
            new EnemyRarityProfile(0.04f, EnemyRarity.SuperRare, 2f, 1.55f, 1.5f, 3f, 1.15f, 12, 21),
            new EnemyRarityProfile(0.12f, EnemyRarity.Rare, 1.55f, 1.25f, 1.25f, 2f, 1.08f, 6, 14),
            new EnemyRarityProfile(0.28f, EnemyRarity.Uncommon, 1.22f, 1.1f, 1.1f, 1.35f, 1.04f, 3, 8),
            new EnemyRarityProfile(1f, EnemyRarity.Common, 1f, 1f, 1f, 1f, 1f, 1, 5)
        };

        private static readonly EnemyRarityProfile[] BossProfiles =
        {
            new EnemyRarityProfile(0.05f, EnemyRarity.Legendary, 2f, 1.5f, 1.5f, 5f, 1.28f, 50, 71),
            new EnemyRarityProfile(0.15f, EnemyRarity.SuperRare, 1.5f, 1.3f, 1.3f, 3f, 1.18f, 40, 51),
            new EnemyRarityProfile(0.35f, EnemyRarity.Rare, 1.2f, 1.1f, 1.1f, 2f, 1.1f, 30, 41),
            new EnemyRarityProfile(1f, EnemyRarity.Common, 1f, 1f, 1f, 1f, 1f, 20, 31)
        };

        public override bool InstancePerEntity => true;

        public enum EnemyRarity
        {
            Common,
            Uncommon,
            Rare,
            SuperRare,
            Legendary
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

        public override void DrawEffects(
            NPC npc,
            ref Color drawColor)
        {
            if (rarity == EnemyRarity.Common)
            {
                return;
            }

            Color color =
                GetRarityColor(rarity);

            Lighting.AddLight(
                npc.Center,
                color.R / 255f * 0.65f,
                color.G / 255f * 0.65f,
                color.B / 255f * 0.65f);

            if (Main.rand.NextBool(5))
            {
                Dust dust =
                    Dust.NewDustDirect(
                        npc.position,
                        npc.width,
                        npc.height,
                        DustID.Enchanted_Gold);

                dust.noGravity = true;
                dust.scale = rarity == EnemyRarity.Legendary ? 1.35f : 1.05f;
                dust.velocity *= 0.25f;
                dust.color = color;
                dust.fadeIn = 1.1f;
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
                rarity == EnemyRarity.Common)
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
            string text =
                $"{GetRarityText(rarity)} Lv.{enemyLevel}";

            Color color =
                GetRarityColor(rarity);

            Vector2 textSize =
                FontAssets.MouseText.Value.MeasureString(text);

            Vector2 drawPosition =
                new Vector2(
                    npc.Center.X - screenPos.X,
                    npc.position.Y - screenPos.Y - 30f);

            drawPosition.X -=
                textSize.X * 0.34f;

            Utils.DrawBorderString(
                spriteBatch,
                text,
                drawPosition,
                color,
                0.68f);
        }

        private static string GetRarityText(EnemyRarity enemyRarity)
        {
            return enemyRarity switch
            {
                EnemyRarity.Uncommon => "Uncommon",
                EnemyRarity.Rare => "Rare",
                EnemyRarity.SuperRare => "Super Rare",
                EnemyRarity.Legendary => "Legendary",
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
                _ => Color.LightGray
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
