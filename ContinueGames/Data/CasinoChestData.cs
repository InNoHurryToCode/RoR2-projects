using RoR2;
using System;
using System.Collections;
using UnityEngine;

namespace SavedGames.Data {
    [Serializable]
    public class CasinoChestData {
        private const string Path = "SpawnCards/InteractableSpawnCard/iscCasinoChest";
        public SerializableTransform transform;

        public int cost;
        public int costType;

        public bool opened;

        public CasinoChestData(RouletteChestController casinoChest) {
            var stateMachine = casinoChest.GetComponent<EntityStateMachine>();
            var purchaseInteraction = casinoChest.GetComponent<PurchaseInteraction>();

            transform = new SerializableTransform(casinoChest.transform);

            //index = casinoChest.GetFieldValue<PickupIndex>("dropPickup").value;

            opened = stateMachine.state.GetType().IsEquivalentTo(typeof(EntityStates.Barrel.Opened)) ? true : false;
            opened = !purchaseInteraction.Networkavailable;
            cost = purchaseInteraction.cost;
            costType = (int)purchaseInteraction.costType;

        }

        public void LoadCasinoChest() {
            var gameobject = Resources.Load<SpawnCard>(Path).DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion(), null);
            var casinoChest = gameobject.GetComponent<RouletteChestController>();
            var purchaseInteraction = gameobject.GetComponent<PurchaseInteraction>();
            purchaseInteraction.Networkcost = cost;
            purchaseInteraction.costType = (CostTypeIndex)costType;
            SavedGames.instance.StartCoroutine(WaitForStart(casinoChest));
        }


        IEnumerator WaitForStart(RouletteChestController casinoChest) {
            yield return null;
            if (opened) {
                casinoChest.GetFieldValue<PurchaseInteraction>("purchaseInteraction").SetAvailable(false);
                casinoChest.GetFieldValue<PurchaseInteraction>("purchaseInteraction").costType = CostTypeIndex.None;
            }
            casinoChest.transform.position = transform.position.GetVector3();
        }
    }
}
