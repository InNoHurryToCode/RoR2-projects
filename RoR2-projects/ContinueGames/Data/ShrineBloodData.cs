using RoR2;
using System;
using UnityEngine;

namespace SavedGames.Data {
    [Serializable]
    public class ShrineBloodData {
        private const string Path = "SpawnCards/InteractableSpawnCard/iscShrineBlood";
        public SerializableTransform transform;

        public bool available;

        public int purchaseCount;
        public int cost;

        public ShrineBloodData(ShrineBloodBehavior shrine) {
            var purchaseInteraction = shrine.GetComponent<PurchaseInteraction>();
            transform = new SerializableTransform(shrine.transform);
            purchaseCount = shrine.GetFieldValue<int>("purchaseCount");
            cost = purchaseInteraction.cost;
            available = purchaseInteraction.available;
        }

        public void LoadShrineBlood() {
            var gameobject = Resources.Load<SpawnCard>(Path).DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion(), null);
            var shrineBlood = gameobject.GetComponent<ShrineBloodBehavior>();
            var purchaseInteraction = shrineBlood.GetComponent<PurchaseInteraction>();

            shrineBlood.SetFieldValue("purchaseCount", purchaseCount);

            purchaseInteraction.Networkcost = cost;
            purchaseInteraction.SetAvailable(available);
        }

    }
}
