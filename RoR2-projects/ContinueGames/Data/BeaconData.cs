﻿using EntityStates.Interactables.GoldBeacon;
using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SavedGames.Data
{
    public class BeaconData
    {
        private const string Path = "SpawnCards/InteractableSpawnCard/iscGoldshoresBeacon";
        public SerializableTransform transform;


        public bool available;
        public int cost;


        public static BeaconData SaveBeacon(PurchaseInteraction beacon) {
            var beaconData = new BeaconData();

            beaconData.transform = new SerializableTransform(beacon.transform);
            beaconData.available = beacon.available;
            beaconData.cost = beacon.cost;

            return beaconData;
        }

        public void LoadBeacon() {
            var gameobject = Resources.Load<SpawnCard>(Path).DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion(), null);
            var beacon = gameobject.GetComponent<PurchaseInteraction>();

            beacon.SetAvailable(available);
            beacon.Networkcost = cost;
            GoldshoresMissionController.instance.beaconInstanceList.Add(gameobject);

            if (!available) {
                SavedGames.instance.StartCoroutine(WaitForTitanSpawn(gameobject));//beacon.GetComponent<EntityStateMachine>().SetNextState(new Ready());
            }

        }

        IEnumerator WaitForTitanSpawn(GameObject g) {
            yield return new WaitForSeconds(5f);
            g.GetComponent<EntityStateMachine>().SetNextState(new Ready());
        }
    }
}
