using RoR2;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace SavedGames.Data {

    [Serializable]
    public class BarrelData {
        private const string Path = "SpawnCards/InteractableSpawnCard/iscBarrel1";
        private static FieldInfo getOpened = typeof(BarrelInteraction).GetField("opened", BindingFlags.Instance | BindingFlags.NonPublic);

        public SerializableTransform transform;

        public bool opened;

        public BarrelData(BarrelInteraction barrel) {
            transform = new SerializableTransform(barrel.transform);

            opened = barrel.GetFieldValue<bool>("opened");
        }

        public void LoadBarrel() {
            var g = Resources.Load<SpawnCard>(Path).DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion(), null);
            var barrel = g.GetComponent<BarrelInteraction>();

            SavedGames.instance.StartCoroutine(WaitForStart(barrel));
        }

        IEnumerator WaitForStart(BarrelInteraction barrel) {
            yield return null;
            if (opened) {
                barrel.SetFieldValue("opened", opened);
                barrel.GetComponent<EntityStateMachine>().SetNextState(new EntityStates.Barrel.Opening());
            }
            barrel.transform.position = transform.position.GetVector3();
        }
    }
}
