using RoR2;
using System;
using System.Collections;
using UnityEngine;

namespace SavedGames.Data {
    [Serializable]
    public class ChestData {
        private const string Path = "SpawnCards/InteractableSpawnCard/isc";
        public SerializableTransform transform;

        public string name;

        public bool isEquipment;
        public int index;

        public int cost;
        public int costType;

        public bool opened;

        public ChestData(ChestBehavior chest) {
            var stateMachine = chest.GetComponent<EntityStateMachine>();
            var purchaseInteraction = chest.GetComponent<PurchaseInteraction>();

            name = chest.name.Replace("(Clone)", "");
            transform = new SerializableTransform(chest.transform);

            index = chest.GetFieldValue<PickupIndex>("dropPickup").value;
            isEquipment = index >= (int)ItemIndex.Count;

            opened = stateMachine.state.GetType().IsEquivalentTo(typeof(EntityStates.Barrel.Opened)) ? true : false;
            cost = purchaseInteraction.cost;
            costType = (int)purchaseInteraction.costType;

        }

        public void LoadChest() {
            if (name.Contains("HumanFan")) {
                return;
            }
            var gameobject = Resources.Load<SpawnCard>(Path + name).DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion(), null);
            var chest = gameobject.GetComponent<ChestBehavior>();
            var purchaseInteraction = gameobject.GetComponent<PurchaseInteraction>();

            if (isEquipment) {
                chest.SetFieldValue("dropPickup", new PickupIndex((EquipmentIndex)index - (int)ItemIndex.Count));
            } else {
                chest.SetFieldValue("dropPickup", new PickupIndex((ItemIndex)index));
            }

            purchaseInteraction.Networkcost = cost;
            purchaseInteraction.costType = (CostTypeIndex)costType;

            chest.dropRoller = new UnityEngine.Events.UnityEvent();

            SavedGames.instance.StartCoroutine(WaitForStart(chest));
        }

        IEnumerator WaitForStart(ChestBehavior chest) {
            yield return null;
            if (opened) {
                chest.Open();
                chest.GetComponent<PurchaseInteraction>().SetAvailable(false);
            }
            chest.transform.position = transform.position.GetVector3();
        }

    }
}
