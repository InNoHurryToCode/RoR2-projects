using RoR2;
using System;
using UnityEngine;

namespace SavedGames.Data {
    [Serializable]
    public class ShrineHealingData {
        private const string Path = "SpawnCards/InteractableSpawnCard/iscShrineHealing";

        public SerializableTransform transform;

        public int purchaseCount;
        public bool available;


        public ShrineHealingData(ShrineHealingBehavior shrine) {
            var purchaseInteraction = shrine.GetComponent<PurchaseInteraction>();
            transform = new SerializableTransform(shrine.transform);
            purchaseCount = shrine.purchaseCount;
            available = purchaseInteraction.available;
        }

        public void LoadShrineHealing() {
            var gameobject = Resources.Load<SpawnCard>(Path).DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion(), null);
            var shrineHealing = gameobject.GetComponent<ShrineHealingBehavior>();
            var purchaseInteraction = gameobject.GetComponent<PurchaseInteraction>();

            purchaseInteraction.SetAvailable(available);

            for (int i = 0; i < purchaseCount; i++) {
                var interactionDriver = NetworkUser.readOnlyInstancesList[0].master.GetBody().GetComponent<InteractionDriver>();
                shrineHealing.AddShrineStack(interactionDriver.interactor);
            }
        }

    }
}
