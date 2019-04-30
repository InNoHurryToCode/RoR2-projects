﻿using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace SavedGames.Data
{
    public class PortalData
    {
        public SerializableTransform transform;

        public string name;

        public static PortalData SavePortal(SceneExitController portal) {
            PortalData portalData = new PortalData();
            portalData.transform = new SerializableTransform(portal.transform);
            portalData.name = portal.destinationScene.SceneName;

            return portalData;
        }

        public void LoadPortal() {
            switch (name) {
                case "RoR2/Scenes/bazaar": {
                        if (!Stage.instance.sceneDef.sceneName.Contains("bazaar")) {
                            GameObject g = Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscShopPortal").DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion());
                            NetworkServer.Spawn(g);
                        }
                        break;
                    }
                case "RoR2/Scenes/goldshores": {
                        GameObject g = Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscGoldshoresPortal").DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion());
                        NetworkServer.Spawn(g);
                        break;
                    }
                case "RoR2/Scenes/mysteryspace": {
                        GameObject g = Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscMSPortal").DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion());
                        NetworkServer.Spawn(g);
                        break;
                    }
            }


        }

    }
}
