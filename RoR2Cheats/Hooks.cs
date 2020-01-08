﻿using System;
using Utilities;

#pragma warning disable 3026

namespace RoR2Cheats {
    public class Hooks {

        public static void InitializeHooks() {
            ConCommandHooks();

            GodModeHooks();

            SeedHooks();

            CameraFOVHooks();

            SetupNoEnemyIL();
            SetupFOVIL();
        }

        private static void CameraFOVHooks() {
            On.RoR2.CameraRigController.Start += (orig, self) => {
                self.baseFov = Cheats.fov;
                orig(self);
            };
        }

        private static void SeedHooks() {
            On.RoR2.Run.Start += (orig, self) => {
                self.seed = Cheats.seed == 0 ? self.seed : Cheats.seed;
                orig(self);
            };
        }

        private static void GodModeHooks() {
            On.RoR2.CharacterBody.Start += (orig, self) => {
                orig(self);

                if (self.healthComponent) {
                    if (self.isPlayerControlled)
                        self.healthComponent.godMode = Cheats.godMode;

                }

            };
        }

        private static void ConCommandHooks() {
            On.RoR2.Console.Awake += (orig, self) => {
                Generic.CommandHelper.RegisterCommands(self);
                orig(self);
            };
        }

        private static void SetupNoEnemyIL() {
            IL.RoR2.CombatDirector.FixedUpdate += il => {
                var c = new ILCursor(il);
                if (c.TryGotoNext(x => x.MatchStfld("RoR2.CombatDirector", "monsterCredit"))) {
                    c.EmitDelegate<Func<float, float>>((f) => {
                        return Cheats.noEnemies ? 0f : f;
                    });
                } else {
                    Debug.LogWarning("RoR2Cheats - Could not create IL hook IL.RoR2.CombatDirector.FixedUpdate");
                }
            };

            IL.RoR2.TeleporterInteraction.OnStateChanged += il => {
                var c = new ILCursor(il);
                if (c.TryGotoNext(x => x.MatchStfld("RoR2.CombatDirector", "monsterCredit"))) {
                    c.EmitDelegate<Func<float, float>>((f) => {
                        return Cheats.noEnemies ? 0f : f;

                    });
                } else {
                    Debug.LogWarning("RoR2Cheats - Could not create IL hook IL.RoR2.TeleporterInteraction.OnStateChanged");
                }
            };
            IL.RoR2.SceneDirector.Start += il => {
                var c = new ILCursor(il);
                if (c.TryGotoNext(x => x.MatchStfld("RoR2.SceneDirector", "monsterCredit"))) {
                    c.EmitDelegate<Func<int, int>>((i) => {
                        return Cheats.noEnemies ? 0 : i;
                    });
                } else {
                    Debug.LogWarning("RoR2Cheats - Could not create IL hook IL.RoR2.SceneDirector.Start");
                }
            };
        }

        private static void SetupFOVIL() {

            IL.RoR2.CameraRigController.Update += il => {
                var c = new ILCursor(il);
                if (c.TryGotoNext(
                    x => x.MatchLdcR4(1.3f)
                )) {
                    c.Index++;
                    c.EmitDelegate<Func<float, float>>((f) => { return 1; });
                } else {
                    Debug.LogWarning("RoR2Cheats - Could not create IL hook IL.RoR2.CameraRigController.Update");
                }
            };

            IL.EntityStates.Huntress.BackflipState.FixedUpdate += il => {
                var c = new ILCursor(il);
                if (c.TryGotoNext(x => x.MatchLdcR4(60f))) {
                    c.Index++;
                    c.EmitDelegate<Func<float, float>>(f => { return Cheats.fov - 10f; });
                } else {
                    Debug.LogWarning("RoR2Cheats - Could not create IL hook IL.EntityStates.Huntress.BackflipState.FixedUpdate");
                }
            };

            IL.EntityStates.Commando.DodgeState.FixedUpdate += il => {
                var c = new ILCursor(il);
                if (c.TryGotoNext(x => x.MatchLdcR4(60f))) {
                    c.Index++;
                    c.EmitDelegate<Func<float, float>>(f => { return Cheats.fov - 10f; });
                } else {
                    Debug.LogWarning("RoR2Cheats - Could not create IL hook IL.EntityStates.Commando.DodgeState.FixedUpdate");
                }
            };
        }

    }
}
