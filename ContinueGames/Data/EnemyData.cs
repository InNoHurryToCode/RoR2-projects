using EntityStates.Missions.Goldshores;
using RoR2;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;


namespace SavedGames.Data {
    [Serializable]
    public class EnemyData {

        public SerializableTransform transform;

        public string enemyName;

        public int teamIndex;

        public int[] items;
        public int equipmentIndex;

        public float health;
        public float shields;

        public int expReward;
        public int goldReward;

        public bool isBoss;


        public EnemyData(CharacterMaster enemy) {
            var inventory = enemy.inventory;
            var healthComponent = enemy.GetBody().healthComponent;
            var deathRewards = enemy.GetBody().GetComponent<DeathRewards>();

            transform = new SerializableTransform(enemy.GetBody().transform);
            enemyName = enemy.name.Replace("Master(Clone)", "");
            teamIndex = (int)enemy.teamIndex;

            items = new int[(int)ItemIndex.Count];
            for (int i = 0; i < (int)ItemIndex.Count; i++) {
                items[i] = inventory.GetItemCount((ItemIndex)i);
            }

            equipmentIndex = (int)inventory.GetEquipmentIndex();
            health = (float)healthComponent.health;
            shields = (float)healthComponent.shield;

            if (deathRewards != null) {
                expReward = (int)deathRewards.expReward;
                goldReward = (int)deathRewards.goldReward;
            }

            isBoss = enemy.isBoss;
        }

        public void LoadEnemy() {

            if (enemyName.Contains("EngiTurret") || enemyName.Contains("BeetleGuardAlly") || enemyName.Contains("ShopkeeperMaster")) {
                return;
            }

            var gameobject = GameObject.Instantiate(MasterCatalog.FindMasterPrefab(enemyName + "Master"));
            var enemy = gameobject.GetComponent<CharacterMaster>();
            var inventory = enemy.inventory;

            NetworkServer.Spawn(gameobject);

            if (enemyName == "BeetleQueen")
                enemyName = "BeetleQueen2";

            enemy.SpawnBody(BodyCatalog.FindBodyPrefab(enemyName + "Body"), transform.position.GetVector3(), transform.rotation.GetQuaternion());

            enemy.teamIndex = (TeamIndex)teamIndex;
            for (int i = 0; i < (int)ItemIndex.Count; i++) {
                inventory.GiveItem((ItemIndex)i, items[i]);
            }
            inventory.SetEquipmentIndex((EquipmentIndex)equipmentIndex);

            enemy.isBoss = isBoss;

            if (enemyName.Contains("TitanGold")) {
                var bossFight = new GoldshoresBossfight();
                var bossGroupGameObject = Object.Instantiate(Resources.Load<GameObject>("Prefabs/NetworkedObjects/Bossgroup"));
                var bossGroup = bossGroupGameObject.GetComponent<BossGroup>();

                NetworkServer.Spawn(bossGroupGameObject);

                bossGroup.bossDropChance = 1f;
                bossGroup.dropPosition = GoldshoresMissionController.instance.bossSpawnPosition;

                bossGroup.combatSquad.AddMember(enemy);
                TeleporterInteraction.instance.SetFieldValue("bossGroup", bossGroup);
                bossFight.SetFieldValue("hasSpawnedBoss", true);
                bossFight.SetFieldValue("bossInstanceBody", enemy.GetBody());

                enemy.GetBody().AddBuff(BuffIndex.Immune);

                GoldshoresMissionController.instance.GetComponent<EntityStateMachine>().SetNextState(bossFight);
            } else if (isBoss) {
                TeleporterInteraction.instance.GetFieldValue<BossGroup>("bossGroup").combatSquad.AddMember(enemy);
            }

            SavedGames.instance.StartCoroutine(WaitForStart(enemy));
        }

        IEnumerator WaitForStart(CharacterMaster enemy) {
            yield return null;
            var healthComponent = enemy.GetBody().healthComponent;

            healthComponent.health = health;
            healthComponent.shield = shields;

            var deathRewards = enemy.GetBody().GetComponent<DeathRewards>();

            if (deathRewards != null) {
                deathRewards.expReward = (uint)expReward;
                deathRewards.goldReward = (uint)goldReward;
            }
        }
    }
}
