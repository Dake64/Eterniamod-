using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Players
{
    public class VirtuosoPlayer : ModPlayer
    {
        // =================================================
        // NOTES
        // =================================================

        public List<string> Notes = new();

        public int SelectedNote;

        private readonly string[] AvailableNotes =
        {
            "Do",
            "Re",
            "Mi"
        };

        // =================================================
        // BUFF TIMERS
        // =================================================

        public int DamageBuffTimer;

        public int SpeedBuffTimer;

        // =================================================
        // POST UPDATE
        // =================================================

        public override void PostUpdate()
        {
            var subclass =
                Player.GetModPlayer<SubclassPlayer>();

            if (subclass.CurrentSubclass
                != "Virtuoso")
            {
                Notes.Clear();

                SelectedNote = 0;

                DamageBuffTimer = 0;
                SpeedBuffTimer = 0;

                return;
            }

            // =============================================
            // CHANGE NOTE
            // =============================================

            if (EterniaKeybinds.ChangeNote.JustPressed)
            {
                SelectedNote++;

                if (SelectedNote >= AvailableNotes.Length)
                {
                    SelectedNote = 0;
                }

                CombatText.NewText(
                    Player.Hitbox,
                    Color.Yellow,
                    AvailableNotes[SelectedNote]
                );
            }

            // =============================================
            // PLAY MELODY
            // =============================================

            if (EterniaKeybinds.SkillKey.JustPressed)
            {
                PlayMelody();
            }

            // =============================================
            // TIMERS
            // =============================================

            if (DamageBuffTimer > 0)
            {
                DamageBuffTimer--;
            }

            if (SpeedBuffTimer > 0)
            {
                SpeedBuffTimer--;
            }
        }

        // =================================================
        // ON HIT ITEM
        // =================================================

        public override void OnHitNPCWithItem(
            Item item,
            NPC target,
            NPC.HitInfo hit,
            int damageDone)
        {
            AddCurrentNote();
        }

        // =================================================
        // ON HIT PROJECTILE
        // =================================================

        public override void OnHitNPCWithProj(
            Projectile proj,
            NPC target,
            NPC.HitInfo hit,
            int damageDone)
        {
            AddCurrentNote();
        }

        // =================================================
        // ADD CURRENT NOTE
        // =================================================

        private void AddCurrentNote()
        {
            var subclass =
                Player.GetModPlayer<SubclassPlayer>();

            if (subclass.CurrentSubclass
                != "Virtuoso")
            {
                return;
            }

            if (Notes.Count >= 3)
            {
                return;
            }

            string note =
                AvailableNotes[SelectedNote];

            Notes.Add(note);

            CombatText.NewText(
                Player.Hitbox,
                Color.Cyan,
                note
            );
        }

        // =================================================
        // PLAY MELODY
        // =================================================

        private void PlayMelody()
        {
            if (Notes.Count < 3)
            {
                return;
            }

            string melody =
                string.Join("-", Notes);

            // =============================================
            // DO RE MI
            // =============================================

            if (melody == "Do-Re-Mi")
            {
                DamageBuffTimer = 600;

                for (int i = 0; i < 15; i++)
                {
                    Dust.NewDust(
                        Player.position,
                        Player.width,
                        Player.height,
                        DustID.Torch
                    );
                }

                CombatText.NewText(
                    Player.Hitbox,
                    Color.Red,
                    "Melody of War"
                );
            }

            // =============================================
            // MI RE DO
            // =============================================

            else if (melody == "Mi-Re-Do")
            {
                SpeedBuffTimer = 600;

                for (int i = 0; i < 15; i++)
                {
                    Dust.NewDust(
                        Player.position,
                        Player.width,
                        Player.height,
                        DustID.GemEmerald
                    );
                }

                CombatText.NewText(
                    Player.Hitbox,
                    Color.Green,
                    "Melody of Speed"
                );
            }

            // =============================================
            // RE MI RE
            // =============================================

            else if (melody == "Re-Mi-Re")
            {
                foreach (NPC npc in Main.npc)
                {
                    if (!npc.active || npc.friendly)
                    {
                        continue;
                    }

                    if (Vector2.Distance(
                        npc.Center,
                        Player.Center) <= 500f)
                    {
                        npc.AddBuff(
                            BuffID.Confused,
                            300
                        );

                        npc.AddBuff(
                            BuffID.Slow,
                            300
                        );
                    }
                }

                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(
                        Player.position,
                        Player.width,
                        Player.height,
                        DustID.PurpleTorch
                    );
                }

                CombatText.NewText(
                    Player.Hitbox,
                    Color.Purple,
                    "Dissonance"
                );
            }

            else
            {
                CombatText.NewText(
                    Player.Hitbox,
                    Color.Gray,
                    "Failed Melody"
                );
            }

            Notes.Clear();
        }

        // =================================================
        // DAMAGE BUFF
        // =================================================

        public override void ModifyWeaponDamage(
            Item item,
            ref StatModifier damage)
        {
            if (DamageBuffTimer > 0)
            {
                damage += 0.10f;
            }
        }

        // =================================================
        // SPEED BUFF
        // =================================================

        public override void PostUpdateRunSpeeds()
        {
            if (SpeedBuffTimer > 0)
            {
                Player.moveSpeed += 0.20f;
            }
        }

        // =================================================
        // GET CURRENT NOTE
        // =================================================

        public string GetCurrentNote()
        {
            return AvailableNotes[SelectedNote];
        }
    }
}