using RoR2;
using System;
using UnityEngine;

namespace SavedGames.Data {
    [Serializable]
    public class ShrineBossData {
        private const string Path = "SpawnCards/InteractableSpawnCard/iscShrineBoss";
        public SerializableTransform transform;

        public bool available;

        public ShrineBossData(ShrineBossBehavior shrine) {
            transform = new SerializableTransform(shrine.transform);
            available = shrine.GetComponent<PurchaseInteraction>().available;
            
        }

        public void LoadShrineBoss() {
            var gameobject = Resources.Load<SpawnCard>(Path).DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion(), null);
            var shrineBoss = gameobject.GetComponent<ShrineBossBehavior>();
            var purchaseInteraction = shrineBoss.GetComponent<PurchaseInteraction>();

            purchaseInteraction.SetAvailable(available);
        }

    }
}
