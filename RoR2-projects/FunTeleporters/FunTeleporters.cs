using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using RoR2;
using UnityEngine;

namespace FunTeleporters
{

    [BepInPlugin(
        "com.morris1927.FunTeleporters",
        "FunTeleporters",
        "1.0.0"
    )]
    public class FunTeleporters : BaseUnityPlugin
    {
        public static FieldInfo bossDirector = typeof(Run).GetField("bossDirector", BindingFlags.NonPublic);
        public static FieldInfo get_rng = typeof(CombatDirector).GetField("rng", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo get_moneyWaves = typeof(CombatDirector).GetField("moneyWaves", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo get_instancesList = typeof(BossGroup).GetField("_instancesList", BindingFlags.NonPublic | BindingFlags.Static);
        public static PropertyInfo get_instance = typeof(BossGroup).GetProperty("instance", BindingFlags.Public | BindingFlags.Static);


        public void Awake() {
            On.RoR2.BossGroup.Awake += (orig, self) => {
                if (BossGroup.instance == null) {
                    orig(self);
                } else {
                    BossGroup instance = BossGroup.instance;

                    orig(self);

                    get_instance.SetValue(null, instance);
                    List<BossGroup> temp = (List<BossGroup>) get_instancesList.GetValue(null);
                    temp.Remove(self);
                    get_instancesList.SetValue(null, temp);

                }
            };

            On.RoR2.TeleporterInteraction.OnInteractionBegin += (orig, self, interactor) => {
                orig(self, interactor);
            };

            On.RoR2.Chat.CCSay += (orig, self) => {
                orig(self);
                SetupCombatDirector();
                //Utilities.Generic.PrintFields(typeof(CombatDirector), TeleporterInteraction.instance.bossDirector);
            
                Utilities.Generic.PrintFields(typeof(CombatDirector), uberDirector);
                //Debug.Log( TeleporterInteraction.instance.bossDirector.moneyWaveIntervals);
                uberDirector.enabled = true;
                uberDirector.monsterCredit += (float)((int)(600f * Mathf.Pow(Run.instance.compensatedDifficultyCoefficient, 0.5f) * (float)(1)));
                uberDirector.currentSpawnTarget = TeleporterInteraction.instance?.gameObject;
                
                uberDirector.SetNextSpawnAsBoss();
                //List<BossGroup> _instancesList = (List<BossGroup>)get_instancesList.GetValue(null);
                //_instancesList.Remove(uberDirector.bossGroup);
                //get_instancesList.SetValue(null, _instancesList);
                //get_instance.SetValue(null, _instancesList[0]);
                Utilities.Generic.PrintFields(typeof(CombatDirector), uberDirector);
            };
        }

        private void SetupCombatDirector() {
            uberDirector = gameObject.AddComponent<CombatDirector>();
            uberDirector.isBoss = true;
            //uberDirector.moneyWaveIntervals = TeleporterInteraction.instance.bossDirector.moneyWaveIntervals;

            get_rng.SetValue(uberDirector, new Xoroshiro128Plus(Run.instance.seed));
            get_moneyWaves.SetValue(uberDirector, get_moneyWaves.GetValue(TeleporterInteraction.instance.bossDirector));
            uberDirector.dropPosition = TeleporterInteraction.instance?.gameObject?.transform;
            uberDirector.dropPosition.Translate(Vector3.up);
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
