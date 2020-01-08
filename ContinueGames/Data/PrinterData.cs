using RoR2;
using System;
using System.Collections;
using UnityEngine;

namespace SavedGames.Data {
    [Serializable]
    public class PrinterData {
        private const string Path = "SpawnCards/InteractableSpawnCard/isc";
        public SerializableTransform transform;

        public string name;

        public int itemIndex;

        public PrinterData(ShopTerminalBehavior printer) {
            transform = new SerializableTransform(printer.transform);
            itemIndex = (int)printer.CurrentPickupIndex().itemIndex;
            name = printer.name.Replace("(Clone)", "");

        }

        public void LoadPrinter() {
            var gameobject = Resources.Load<SpawnCard>(Path + name).DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion(), null);
            var printer = gameobject.GetComponent<ShopTerminalBehavior>();
            SavedGames.instance.StartCoroutine(WaitForStart(printer));
        }

        IEnumerator WaitForStart(ShopTerminalBehavior printer) {

            yield return new WaitWhile(() => printer.CurrentPickupIndex().value != -1);
            printer.SetPickupIndex(new PickupIndex((ItemIndex)itemIndex));
        }
    }
}
