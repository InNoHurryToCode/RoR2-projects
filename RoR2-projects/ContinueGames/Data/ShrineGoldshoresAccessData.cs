using RoR2;
using System;
using UnityEngine;

namespace SavedGames.Data {
    [Serializable]
    public class ShrineGoldshoresAccessData {
        private const string Path = "SpawnCards/InteractableSpawnCard/iscShrineGoldshoresAccess";
        public SerializableTransform transform;

        public int portalIndex;

        public bool available;
        public int cost;

        public ShrineGoldshoresAccessData(PortalStatueBehavior shrine) {
            var purchaseInteraction = shrine.GetComponent<PurchaseInteraction>();

            transform = new SerializableTransform(shrine.transform);
            cost = purchaseInteraction.cost;
            available = purchaseInteraction.available;

            portalIndex = (int) shrine.portalType;
        }

        public void LoadShrineGoldshores() {
            var gameobject = Resources.Load<SpawnCard>(Path).DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion(), null);
            var goldShrine = gameobject.GetComponent<PortalStatueBehavior>();
            var purchaseInteraction = gameobject.GetComponent<PurchaseInteraction>();

            purchaseInteraction.Networkcost = cost;
            purchaseInteraction.SetAvailable(available);

            goldShrine.portalType = (PortalStatueBehavior.PortalType) portalIndex;

        }

    }
}
