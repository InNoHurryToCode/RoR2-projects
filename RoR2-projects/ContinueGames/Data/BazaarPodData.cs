using RoR2;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace SavedGames.Data {
    public class BazaarPodData {
        private const string Path = "Prefabs/NetworkedObjects/LunarShopTerminal";
        public SerializableTransform transform;

        public bool available;
        public int itemIndex;
        public int cost;

        public BazaarPodData(ShopTerminalBehavior pod) {
            transform = new SerializableTransform(pod.transform);
            
            var purchaseInteraction = pod.GetComponent<PurchaseInteraction>();
            
            available = purchaseInteraction.available;
            itemIndex = (int)pod.CurrentPickupIndex().value;
            cost = purchaseInteraction.cost;
        }

        public void LoadPod() {
            var gameobject = Object.Instantiate(Resources.Load<GameObject>(Path), transform.position.GetVector3(), transform.rotation.GetQuaternion());
            var pod = gameobject.GetComponent<ShopTerminalBehavior>();
            var purchaseInteraction = gameobject.GetComponent<PurchaseInteraction>();
            gameobject.transform.localScale = Vector3.one;
            NetworkServer.Spawn(gameobject);

            purchaseInteraction.SetAvailable(available);
            purchaseInteraction.Networkcost = cost;
            SavedGames.instance.StartCoroutine(WaitForStart(pod));
        }


        IEnumerator WaitForStart(ShopTerminalBehavior pod) {
            yield return null;

            if (itemIndex >= (int)ItemIndex.Count) {
                pod.SetPickupIndex(new PickupIndex((EquipmentIndex)itemIndex - (int)ItemIndex.Count));
            } else {
                pod.SetPickupIndex(new PickupIndex((ItemIndex)itemIndex));
            }


        }
    }
}