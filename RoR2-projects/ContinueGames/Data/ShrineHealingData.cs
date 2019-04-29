﻿using RoR2;
using System;
using UnityEngine;

namespace SavedGames.Data {
    [Serializable]
    public class ShrineHealingData {

        public SerializableTransform transform;

        public static ShrineHealingData SaveShrineHealing(ShrineHealingBehavior shrine) {
            ShrineHealingData shrineHealingData = new ShrineHealingData();
            shrineHealingData.transform = new SerializableTransform(shrine.transform);

            return shrineHealingData;
        }

        public void LoadShrineHealing() {
            GameObject g = Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscShrineHealing").DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion());
            ShrineHealingBehavior shrineHealing = g.GetComponent<ShrineHealingBehavior>();
        }

    }
}
