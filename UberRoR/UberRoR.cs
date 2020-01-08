using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using BepInEx;
using System.Reflection;
using UnityEngine;

namespace UberRoR {

    [BepInPlugin("com.morris1927.uberror", "UberRoR", "0.0.1")]
    public class UberRoR : BaseUnityPlugin {

        public static FieldInfo[] fields = typeof(CombatDirector).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
        public static FieldInfo bossDirector = typeof(Run).GetField("bossDirector", BindingFlags.NonPublic);
        public static FieldInfo get_rng = typeof(CombatDirector).GetField("rng", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo get_moneyWaves = typeof(CombatDirector).GetField("moneyWaves", BindingFlags.NonPublic | BindingFlags.Instance);

        // public static FieldInfo get_moneyWaveIntervals = typeof(CombatDirector).GetField("moneyWaveIntervals", Bindi)
        public void Awake() {


            SetupCombatDirector();

            On.RoR2.Chat.CCSay += (orig, self) => {
                orig(self);

                Utilities.Generic.PrintFields(typeof(CombatDirector), TeleporterInteraction.instance.bossDirector);
                Utilities.Generic.PrintFields(typeof(CombatDirector), uberDirector);

                //Debug.Log( TeleporterInteraction.instance.bossDirector.moneyWaveIntervals);
                uberDirector.enabled = true;
                uberDirector.monsterCredit += (float)((int)(600f * Mathf.Pow(Run.instance.compensatedDifficultyCoefficient, 0.5f) * (float)(1)));
                uberDirector.currentSpawnTarget = base.gameObject;
                uberDirector.SetNextSpawnAsBoss();

            };
        }

        private void SetupCombatDirector() {
            uberDirector = gameObject.AddComponent<CombatDirector>();
            uberDirector.isBoss = true;
            //uberDirector.moneyWaveIntervals = TeleporterInteraction.instance.bossDirector.moneyWaveIntervals;

            get_rng.SetValue(uberDirector, new Xoroshiro128Plus(Run.instance.seed));
            get_moneyWaves.SetValue(uberDirector, get_moneyWaves.GetValue(TeleporterInteraction.instance.bossDirector));
            uberDirector.dropPosition = this.transform;
            uberDirector.minSeriesSpawnInterval = 0.1f;
            uberDirector.maxSeriesSpawnInterval = 1f;
            uberDirector.minRerollSpawnInterval = 2.333333f;
            uberDirector.maxRerollSpawnInterval = 4.333333f;
            uberDirector.shouldSpawnOneWave = true;
            uberDirector.targetPlayers = false;
            uberDirector.skipSpawnIfTooCheap = true;
            uberDirector.monsterSpawnTimer = 0;
            uberDirector.enabled = false;

        }


        CombatDirector uberDirector;
    }
}
