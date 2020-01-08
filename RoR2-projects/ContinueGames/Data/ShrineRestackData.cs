using RoR2;
using System;
using UnityEngine;

namespace SavedGames.Data {
    [Serializable]
    public class ShrineRestackData {
        private const string Path = "SpawnCards/InteractableSpawnCard/iscShrineRestack";

        public SerializableTransform transform;

        public bool available;

        public ShrineRestackData(ShrineRestackBehavior shrine) {
            var purchaseInteraction = shrine.GetComponent<PurchaseInteraction>();

            transform = new SerializableTransform(shrine.transform);
            available = purchaseInteraction.available;
            
        }

        public void LoadShrineRestack() {
            var gameobject = Resources.Load<SpawnCard>(Path).DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion(), null);
            var shrine = gameobject.GetComponent<ShrineRestackBehavior>();
            var purchaseInteraction = shrine.GetComponent<PurchaseInteraction>();

            purchaseInteraction.SetAvailable(available);

        }

    }
}
