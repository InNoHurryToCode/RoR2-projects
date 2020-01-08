using RoR2;
using System;
using System.Collections;
using UnityEngine;

namespace SavedGames.Data {
    [Serializable]
    public class FanData {
        private const string Path = "Prefabs/NetworkedObjects/HumanFan";
        public SerializableTransform transform;

        public int cost;
        public bool available;

        public SerializableVector3 jumpVelocity;


        public FanData(PurchaseInteraction fan) {
            var jumpVolume = fan.GetComponentInChildren<JumpVolume>(true);

            transform = new SerializableTransform(fan.transform);

            cost = fan.cost;
            available = fan.available;
            jumpVelocity = new SerializableVector3(jumpVolume.jumpVelocity);

        }

        public void LoadFan() {
            var gameobject = GameObject.Instantiate(Resources.Load<GameObject>(Path), transform.position.GetVector3(), transform.rotation.GetQuaternion());
            var purchaseInteraction = gameobject.GetComponent<PurchaseInteraction>();
            var jumpVolume = purchaseInteraction.GetComponentInChildren<JumpVolume>(true);

            purchaseInteraction.Networkcost = cost;
            purchaseInteraction.SetAvailable(available);
            if (!available) {
                var stateMachine = gameobject.GetComponent<EntityStateMachine>();
                stateMachine.SetNextState(new EntityStates.Barrel.ActivateFan());
            }

            jumpVolume.jumpVelocity = jumpVelocity.GetVector3();
        }

    }
}
