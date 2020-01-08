using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace SavedGames.Data {
    public class PortalData {
        public SerializableTransform transform;

        public string name;
        public bool useRunNextStageScene;


        public PortalData(SceneExitController portal) {
            transform = new SerializableTransform(portal.transform);
            name = portal.destinationScene.baseSceneName;
            useRunNextStageScene = portal.useRunNextStageScene;
        }

        public void LoadPortal() {

            switch (name) {
                //case "RoR2/Scenes/bazaar": {
                //        if (!Stage.instance.sceneDef.sceneName.Contains("bazaar")) {
                //            GameObject g = Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscShopPortal").DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion());
                //            NetworkServer.Spawn(g);
                //        }
                //        break;
                //    }
                case "RoR2/Scenes/goldshores": {
                        if (Stage.instance.sceneDef.baseSceneName.Contains("goldshores")) {
                            var gameobject = Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscGoldshoresPortal").DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion(), null);
                            NetworkServer.Spawn(gameobject);
                            gameobject.GetComponent<SceneExitController>().useRunNextStageScene = useRunNextStageScene;
                        }
                        break;
                    }
                    //case "RoR2/Scenes/mysteryspace": {
                    //        //GameObject g = Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscMSPortal").DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion());
                    //        //NetworkServer.Spawn(g);
                    //        break;
                    //    }
            }


        }

    }
}
