using RoR2;
using System;
using UnityEngine;

namespace SavedGames.Data {
    [Serializable]
    public class ShrineCleanseData {
        private const string Path = "SpawnCards/InteractableSpawnCard/iscShrineCleanse";
        public SerializableTransform transform;

        //public bool available;

        //public int cost;
        //public int costType;

        //public int tier;
        //public int itemIndex;

        public ShrineCleanseData(ShopTerminalBehavior shrineCleanse) {
            //var purchaseInteraction = shrineCleanse.GetComponent<PurchaseInteraction>();
            //var purchaseInteraction = shrineCleanse;

            transform = new SerializableTransform(shrineCleanse.transform);
            //cost = purchaseInteraction.cost;
            //costType = (int)purchaseInteraction.costType;
            //tier = (int)shrineCleanse.itemTier;
            //itemIndex = (int)shrineCleanse.CurrentPickupIndex().value;
            //available = purchaseInteraction.available;

        }

        public void LoadShrineCleanse() {
            var gameobject = Resources.Load<SpawnCard>(Path).DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion(), null);
            //var shrineCleanse = gameobject.GetComponent<ShrineCleanseBehavior>();
        }

    }
}
