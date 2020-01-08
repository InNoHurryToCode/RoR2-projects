using RoR2;
using System;
using UnityEngine;

namespace SavedGames.Data {
    [Serializable]
    public class ShrineChanceData {
        private const string Path = "SpawnCards/InteractableSpawnCard/iscShrineChance";
        public SerializableTransform transform;

        public bool available;

        public int successfulPurchaseCount;
        public int cost;

        public ShrineChanceData(ShrineChanceBehavior shrine) {
            var purchaseInteraction = shrine.GetComponent<PurchaseInteraction>();

            transform = new SerializableTransform(shrine.transform);
            successfulPurchaseCount = shrine.GetFieldValue<int>("successfulPurchaseCount");
            cost = purchaseInteraction.cost;
            available = purchaseInteraction.available;

        }

        public void LoadShrineChance() {
            var gameobject = Resources.Load<SpawnCard>(Path).DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion(), null);
            var shrineChance = gameobject.GetComponent<ShrineChanceBehavior>();
            var purchaseInteraction = shrineChance.GetComponent<PurchaseInteraction>();

            shrineChance.SetFieldValue("successfulPurchaseCount", successfulPurchaseCount);

            purchaseInteraction.Networkcost = cost;
            purchaseInteraction.SetAvailable(available);

        }

    }
}
