﻿using BepInEx;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace SandBox
{
    [BepInPlugin("com.morris1927.sandbox", "sandbox", "1.0.0")]
    public class SandBox : BaseUnityPlugin {









      //  public void Awake() {
      //      // GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
      //
      //      IL.EntityStates.Treebot.Weapon.FireSyringe.OnEnter += (il) => {
      //          var c = new ILCursor(il);
      //
      //          c.GotoNext((x) => (x.MatchRet()));
      //          //c.Emit(OpCodes.Ldfld, typeof(RoR2.RoR2Application).Assembly.GetType("EntityStates.Treebot.Weapon.FireSyringe").GetField("duration", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
      //          c.Emit(OpCodes.Ldarg_0);
      //          c.EmitDelegate<Action<EntityStates.BaseState>>((x) => {
      //              Debug.Log(x.GetFieldValue<float>("duration"));
      //          });
      //          
      //      };
      //      IL.RoR2.BodyCatalog.GeneratePortraits += il => {
      //          var c = new ILCursor(il);
      //
      //          Debug.Log(il.ToString());
      //      };
      //
      //      IL.RoR2.CameraRigController.Update += (il) => {
      //          var c = new ILCursor(il);
      //
      //          //We use GotoNext to locate the code we want to edit
      //          //Notice we can specify a block of instructions to match, rather than only a single instruction
      //          //This is preferable as it is less likely to break something else because of an update
      //
      //          c.GotoNext(
      //              x => x.MatchLdloc(4),      // num14 *= 0.5f;
      //              x => x.MatchLdcR4(0.5f),   // 
      //              x => x.MatchMul(),         // 
      //              x => x.MatchStloc(4),      // 
      //              x => x.MatchLdloc(5),      // num15 *= 0.5f;
      //              x => x.MatchLdcR4(0.5f),   //
      //              x => x.MatchMul(),         //
      //              x => x.MatchStloc(5));     //
      //
      //          //When we GotoNext, the cursor is before the first instruction we match.
      //          //So we remove the next 8 instructions
      //          c.RemoveRange(8);
      //          //Logger.LogError(il.ToString());
      //      };
      //  }
      //  //
      //  //     public void Update() {
      //  //
      //  //         if (Time.frameCount % 60 == 0) {
      //  //              Debug.Log(GC.GetTotalMemory(false));
      //  //         }
      //  //         if (Input.GetKeyDown(KeyCode.U)) {
      //  //             GarbageCollector.GCMode = GarbageCollector.GCMode == GarbageCollector.Mode.Enabled ? GarbageCollector.Mode.Disabled : GarbageCollector.Mode.Enabled;
      //  //             Debug.Log(GarbageCollector.GCMode);
      //  //         }
      //  //
      //  //
      //  //         if (Input.GetKeyDown(KeyCode.I)) {
      //  //             GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;
      //  //             GC.Collect();
      //  //             GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
      //  //         }
      //  //
      //  //     }
      //
      //  //    public void Awake() {
      //  //        On.RoR2.SceneCamera.Awake += (orig, self) => {
      //  //
      //  //            orig(self);
      //  //            self.gameObject.AddComponent<Test>();
      //  //        };
      //  //
      //  //        IL.RoR2.CameraRigController.Update += (il) => {
      //  //            var c = new ILCursor(il);
      //  //
      //  //            //We use GotoNext to locate the code we want to edit
      //  //            //Notice we can specify a block of instructions to match, rather than only a single instruction
      //  //            //This is preferable as it is less likely to break something else because of an update
      //  //
      //  //            c.GotoNext(
      //  //                x => x.MatchLdloc(4),      // num14 *= 0.5f;
      //  //                x => x.MatchLdcR4(0.5f),   // 
      //  //                x => x.MatchMul(),         // 
      //  //                x => x.MatchStloc(4),      // 
      //  //                x => x.MatchLdloc(5),      // num15 *= 0.5f;
      //  //                x => x.MatchLdcR4(0.5f),   //
      //  //                x => x.MatchMul(),         //
      //  //                x => x.MatchStloc(5));     //
      //  //
      //  //            //When we GotoNext, the cursor is before the first instruction we match.
      //  //            //So we remove the next 8 instructions
      //  //            c.RemoveRange(8);
      //  //            
      //  //        };
      //  //
      //  //    //   On.RoR2.UI.PauseScreenController.Update += (orig, self) => {
      //  //    //       Debug.Log("sdfgsdfg");
      //  //    //       orig(self);
      //  //    //   };
      //  //        
      //  //        On.RoR2.PositionIndicator.UpdatePositions += (orig, uiCamera) => {
      //  //            
      //  //            if (uiCamera.cameraRigController.target != null)
      //  //                orig(uiCamera);
      //  //
      //  //        };
      //  //
      //  //        On.RoR2.CombatDirector.Simulate += (orig, self, deltaTime) => {
      //  //            self.monsterCredit = Mathf.Min(self.monsterCredit, maxMonsterCredits);
      //  //           // if (self.monsterCredit < 0) {
      //  //           //     self.monsterCredit = maxMonsterCredits;
      //  //           // }
      //  //            orig(self, deltaTime);
      //  //        };
      //  //    };

    }
}
