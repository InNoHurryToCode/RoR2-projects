using System;
using System.Reflection;
using BepInEx;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.CharacterAI;
using UnityEngine;

namespace AllyPing {

    [BepInPlugin("com.morris1927.AllyPing", "AllyPing", "1.1.0")]
    public class AllyPing : BaseUnityPlugin {
        public static FieldInfo currentEnemy = typeof(BaseAI).GetField("currentEnemy", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo pingerController = typeof(PlayerCharacterMasterController).GetField("pingerController", BindingFlags.NonPublic | BindingFlags.Instance);

        public static FieldInfo pingDuration = typeof(RoR2.UI.PingIndicator).GetField("pingDuration", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo pingIndicator = typeof(PingerController).GetField("pingerIndicator", BindingFlags.NonPublic | BindingFlags.Instance);


        private void Awake() {


            IL.RoR2.CharacterBody.HandleConstructTurret += (il) => {
                var c = new ILCursor(il);

                c.GotoNext(x => x.MatchStloc(4));
                c.Emit(OpCodes.Ldloc_2);
                c.EmitDelegate<Func<CharacterMaster, CharacterMaster, CharacterMaster>>((turret, master) => {
                    turret.gameObject.AddComponent<AIOwnership>().ownerMaster = master;

                    return turret;
                });

            };

            On.RoR2.CharacterAI.BaseAI.FixedUpdate += (orig, self) => {

                if (self.leader.characterBody != null) {
                    BaseAI.Target leader = self.leader;


                    if (leader.characterBody.isPlayerControlled) {
                        PingerController p = null;
                        foreach (PlayerCharacterMasterController item in PlayerCharacterMasterController.instances) {
                            if (item.master.alive) {
                                if (item.master.GetBody().GetUserName() == leader.characterBody.GetUserName()) {
                                    p = (PingerController)pingerController.GetValue(item);
                                }
                            }
                        }
                        if (p != null) {
                            if (p.currentPing.targetGameObject != null) {
                                //Yes, I know this is ugly
                                if (p.currentPing.targetGameObject.name.Contains("Body")) {
                                    ((BaseAI.Target)currentEnemy.GetValue(self)).gameObject = p.currentPing.targetGameObject;
                                }
                            }
                        }
                    }
                }
                orig(self);
            };
            //IL.RoR2.CharacterAI.BaseAI.FixedUpdate += (il2) => {
            //    var c2 = new ILCursor(il2);
            //    c2.Emit(OpCodes.Ldarg_0);
            //
            //    c2.EmitDelegate<Action<BaseAI>>((ai) => {
            //        if (ai.leader.characterBody != null) {
            //            BaseAI.Target leader = ai.leader;
            //
            //
            //            if (leader.characterBody.isPlayerControlled) {
            //                PingerController p = null; 
            //                foreach (PlayerCharacterMasterController item in PlayerCharacterMasterController.instances) {
            //                    if (item.master.alive) {
            //                        if (item.master.GetBody().GetUserName() == leader.characterBody.GetUserName()) {
            //                            p = (PingerController)pingerController.GetValue(item);
            //                        }
            //                    }
            //                }
            //                if (p != null) {
            //                    if (p.currentPing.targetGameObject != null) {
            //                        //Yes, I know this is ugly
            //                        if (p.currentPing.targetGameObject.name.Contains("Body")) {
            //                            ((BaseAI.Target)currentEnemy.GetValue(ai)).gameObject = p.currentPing.targetGameObject;
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    });
            //};

        }
    }
}
