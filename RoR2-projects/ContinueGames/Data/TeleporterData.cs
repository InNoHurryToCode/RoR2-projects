using RoR2;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

namespace SavedGames.Data {
    [Serializable]
    public class TeleporterData {
        private const string Path = "SpawnCards/InteractableSpawnCard/iscTeleporter";
        public static PropertyInfo getActivationState = typeof(TeleporterInteraction).GetProperty("activationState", BindingFlags.NonPublic | BindingFlags.Instance);


        public SerializableTransform transform;

        public int activationState;

        public float remainingCharge;

        public int bossShrineStacks;
        public bool blueOrb;
        public bool goldOrb;
        public bool celestialOrb;


        public TeleporterData(TeleporterInteraction teleporter) {
            transform = new SerializableTransform(teleporter.transform);
            remainingCharge = teleporter.remainingChargeTimer;
            activationState = (int) getActivationState.GetValue(teleporter);
            bossShrineStacks = teleporter.shrineBonusStacks;

            blueOrb = teleporter.Network_shouldAttemptToSpawnShopPortal;
            goldOrb = teleporter.Network_shouldAttemptToSpawnGoldshoresPortal;
            celestialOrb = teleporter.Network_shouldAttemptToSpawnMSPortal;
            
        }

        public void LoadTeleporter() {
            typeof(TeleporterInteraction).GetProperty("instance", BindingFlags.Public | BindingFlags.Static).SetValue(null, null);
            var gameobject = Resources.Load<SpawnCard>(Path).DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion(), null);
            var teleporter = gameobject.GetComponent<TeleporterInteraction>();

            if (activationState == 2) {
                var bossGroup = teleporter.gameObject.AddComponent<BossGroup>();
                    //GameObject.Instantiate(Resources.Load<BossGroup>("Prefabs/NetworkedObjects/BossGroup"));
                //NetworkServer.Spawn(bossGroup.gameObject);
                //bossGroup.dropPosition = teleporter.bossDirector.
                teleporter.SetFieldValue("bossGroup", bossGroup);
            }
            if (activationState >= 2) {
                teleporter.bossDirector.monsterCredit = float.MinValue;
                activationState = 2;
            }

            teleporter.remainingChargeTimer = remainingCharge;


            SavedGames.instance.StartCoroutine(WaitForStart(teleporter));
        }

        IEnumerator WaitForStart(TeleporterInteraction teleporter) {
            yield return null;

            teleporter.shrineBonusStacks = bossShrineStacks;
            teleporter.Network_shouldAttemptToSpawnShopPortal = blueOrb;
            teleporter.Network_shouldAttemptToSpawnGoldshoresPortal = goldOrb;
            teleporter.Network_shouldAttemptToSpawnMSPortal = celestialOrb;

            getActivationState.SetValue(teleporter, activationState);
            teleporter.SetFieldValue("previousActivationState", 1);

        }
    }
}
