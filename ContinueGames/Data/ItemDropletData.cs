using RoR2;
using System;
using UnityEngine;

namespace SavedGames.Data {
    [Serializable]
    public class ItemDropletData {
        public SerializableTransform transform;

        public int itemIndex;
        public bool isEquipment;

        public ItemDropletData(GenericPickupController pickupDroplet) {
            transform = new SerializableTransform(pickupDroplet.transform);
            itemIndex = (int)pickupDroplet.pickupIndex.value;
            isEquipment = pickupDroplet.pickupIndex.value >= (int)ItemIndex.Count;

        }


        public void LoadDroplet() {
            if (isEquipment) {
                PickupDropletController.CreatePickupDroplet(new PickupIndex((EquipmentIndex)(itemIndex - ItemIndex.Count)), transform.position.GetVector3(), Vector3.zero);
            } else {
                PickupDropletController.CreatePickupDroplet(new PickupIndex((ItemIndex)itemIndex), transform.position.GetVector3(), Vector3.zero);
            }
        }
    }
}
