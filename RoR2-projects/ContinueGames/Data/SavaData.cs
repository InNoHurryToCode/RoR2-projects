using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using Object = UnityEngine.Object;

namespace SavedGames.Data {
    [Serializable]
    public class SaveData {


        public RunData run;

        public List<PlayerData> players;
        public List<EnemyData> enemies;
        public List<ChestData> chests;
        public List<CasinoChestData> casinoChests;
        public List<BarrelData> barrels;
        public List<PrinterData> printers;
        public List<BrokenDroneData> brokenDrones;
        public List<FanData> fans;
        public List<MultiShopData> multiShops;
        public List<ShrineChanceData> chanceShrines;
        public List<ShrineBloodData> bloodShrines;
        public List<ShrineBossData> bossShrines;
        public List<ShrineCombatData> combatShrines;
        public List<ShrineHealingData> healingShrines;
        public List<ShrineRestackData> orderShrines;
        public List<ShrineGoldshoresAccessData> goldshoreShrines;
        public List<ShrineCleanseData> cleanseShrines;
        public List<ItemDropletData> itemDroplets;
        public List<PortalData> portals;
        public List<BazaarPodData> bazaarPods;
        public List<LunarCauldronData> lunarCauldrons;
        public List<BeaconData> beacons;

        public TeleporterData teleporter;

        public SaveData() {
            players = new List<PlayerData>();
            enemies = new List<EnemyData>();
            chests = new List<ChestData>();
            casinoChests = new List<CasinoChestData>();
            barrels = new List<BarrelData>();
            printers = new List<PrinterData>();
            multiShops = new List<MultiShopData>();
            brokenDrones = new List<BrokenDroneData>();
            fans = new List<FanData>();

            chanceShrines = new List<ShrineChanceData>();
            bloodShrines = new List<ShrineBloodData>();
            bossShrines = new List<ShrineBossData>();
            combatShrines = new List<ShrineCombatData>();
            goldshoreShrines = new List<ShrineGoldshoresAccessData>();
            bazaarPods = new List<BazaarPodData>();
            lunarCauldrons = new List<LunarCauldronData>();
            healingShrines = new List<ShrineHealingData>();
            orderShrines = new List<ShrineRestackData>();
            cleanseShrines = new List<ShrineCleanseData>();

            itemDroplets = new List<ItemDropletData>();
            portals = new List<PortalData>();
            beacons = new List<BeaconData>();

            foreach (var item in NetworkUser.readOnlyInstancesList) {
                players.Add(new PlayerData(item));
            }

            foreach (var item in Object.FindObjectsOfType<CharacterMaster>()) {
                if (item.GetBody() != null) {
                    if (!item.GetBody().isPlayerControlled) {
                        enemies.Add(new EnemyData(item));

                    }
                }
            }

            if (Stage.instance.sceneDef.baseSceneName != "bazaar")
            {
                foreach (var item in Object.FindObjectsOfType<ChestBehavior>())
                {
                    chests.Add(new ChestData(item));
                }
            }
            foreach (var item in Object.FindObjectsOfType<RouletteChestController>())
            {
                casinoChests.Add(new CasinoChestData(item));
            }
            
            foreach (var item in Object.FindObjectsOfType<BarrelInteraction>()) {
                barrels.Add(new BarrelData(item));
            }
            foreach (var item in Object.FindObjectsOfType<ShopTerminalBehavior>()) {
                if (item.name.Contains("Duplicator")) {
                    printers.Add(new PrinterData(item));
                }
                if (item.name.Contains("LunarShopTerminal")) {
                    bazaarPods.Add(new BazaarPodData(item));
                }
                if (item.name.Contains("LunarCauldron")) {
                    lunarCauldrons.Add(new LunarCauldronData(item));
                }
                if (item.name.Contains("ShrineCleanse")) 
                {
                    cleanseShrines.Add(new ShrineCleanseData(item));
                }
            }
            foreach (var item in Object.FindObjectsOfType<MultiShopController>()) {
                multiShops.Add(new MultiShopData(item));
            }
            foreach (var item in Object.FindObjectsOfType<ShrineChanceBehavior>()) {
                chanceShrines.Add(new ShrineChanceData(item));
            }
            foreach (var item in Object.FindObjectsOfType<ShrineBloodBehavior>()) {
                bloodShrines.Add(new ShrineBloodData(item));
            }
            foreach (var item in Object.FindObjectsOfType<ShrineBossBehavior>()) {
                bossShrines.Add(new ShrineBossData(item));
            }
            foreach (var item in Object.FindObjectsOfType<ShrineCombatBehavior>()) {
                combatShrines.Add(new ShrineCombatData(item));
            }
            foreach (var item in Object.FindObjectsOfType<ShrineHealingBehavior>()) {
                healingShrines.Add(new ShrineHealingData(item));
            }
            foreach (var item in Object.FindObjectsOfType<ShrineRestackBehavior>()) {
                orderShrines.Add(new ShrineRestackData(item));
            }
            foreach (var item in Object.FindObjectsOfType<PortalStatueBehavior>()) {
                if (item.name.Contains("Goldshores")) {
                    goldshoreShrines.Add(new ShrineGoldshoresAccessData(item));
                }

            }

            foreach (var item in Object.FindObjectsOfType<SummonMasterBehavior>()) {
                brokenDrones.Add(new BrokenDroneData(item));
            }
            foreach (var item in Object.FindObjectsOfType<GenericPickupController>()) {
                if (item.enabled) {
                    itemDroplets.Add(new ItemDropletData(item));
                }
            }
            foreach (var item in Object.FindObjectsOfType<SceneExitController>()) {
                if (!item.name.Contains("Teleporter")) {
                    if (item.destinationScene.baseSceneName != null) {
                        portals.Add(new PortalData(item));
                    }               
                }
            }
            foreach (var item in Object.FindObjectsOfType<PurchaseInteraction>()) {
                if (item.name.Contains("GoldshoresBeacon")) {
                    //Debug.Log(item.name);
                    beacons.Add(new BeaconData(item));
                }
                if (item.name.Contains("HumanFan")) {
                    //Debug.Log(item.name);
                    fans.Add(new FanData(item));
                }
            }
            if (TeleporterInteraction.instance) {
                teleporter = new TeleporterData(TeleporterInteraction.instance);
            }
            run = new RunData(Object.FindObjectOfType<Run>());

        }


        public void Load() {
            ClearStage();

            foreach (var item in chests)
            {
                item.LoadChest();
            }
            foreach (var item in casinoChests)
            {
                item.LoadCasinoChest();
            }
            foreach (var item in barrels) {
                item.LoadBarrel();
            }
            foreach (var item in printers) {
                item.LoadPrinter();
            }
            foreach (var item in multiShops) {
                item.LoadMultiShop();
            }
            foreach (var item in fans) {
                item.LoadFan();
            }
            foreach (var item in chanceShrines) {
                item.LoadShrineChance();
            }
            foreach (var item in bloodShrines) {
                item.LoadShrineBlood();
            }
            foreach (var item in bossShrines) {
                item.LoadShrineBoss();
            }
            foreach (var item in combatShrines) {
                item.LoadShrineCombat();
            }
            foreach (var item in healingShrines) {
                item.LoadShrineHealing();
            }
            foreach (var item in orderShrines) {
                item.LoadShrineRestack();
            }
            foreach (var item in goldshoreShrines) {
                item.LoadShrineGoldshores();
            }
            foreach (var item in cleanseShrines)
            {
                item.LoadShrineCleanse();
            }
            foreach (var item in brokenDrones) {
                item.LoadBrokenDrone();
            }
            foreach (var item in itemDroplets) {
                item.LoadDroplet();
            }
            foreach (var item in portals) {
                item.LoadPortal();
            }
            foreach (var item in beacons) {
                item.LoadBeacon();
            }
            foreach (var item in bazaarPods) {
                item.LoadPod();
            }
            foreach (var item in lunarCauldrons) {
                item.LoadLunarCauldron();
            }
            teleporter?.LoadTeleporter();

            foreach (var item in players) {
                item.LoadPlayer();
            }
            foreach (var item in enemies) {
                item.LoadEnemy();
            }
        }

        private static void ClearStage() {
            //Clear gold chests
            foreach (var item in Object.FindObjectsOfType<ChestBehavior>()) {
                Object.Destroy(item.gameObject);
            }
            //Clear Goldshores beacons
            foreach (var item in Object.FindObjectsOfType<PurchaseInteraction>()) {
                if (item.name.Contains("GoldshoresBeacon")) {
                    Object.Destroy(item.gameObject);
                }
            }
            //Clear Bazaar pods
            foreach (var item in Object.FindObjectsOfType<ShopTerminalBehavior>()) {
                if (item.name.Contains("LunarShopTerminal")) {
                    Object.Destroy(item.gameObject);
                }
                if (item.name.Contains("LunarCauldron")) {
                    Object.Destroy(item.gameObject);
                }
                if (item.name.Contains("Duplicator"))
                {
                    Object.Destroy(item.gameObject);
                }
            }
            if (GoldshoresMissionController.instance) {
                GoldshoresMissionController.instance.beaconInstanceList.Clear();
            }
        }
    }

    [Serializable]
    public struct SerializableTransform {

        public SerializableTransform(Vector3 position, Quaternion rotation) {
            this.position = new SerializableVector3(position);
            this.rotation = new SerializableQuaternion(rotation);
        }
        public SerializableTransform(Transform transform) {
            this.position = new SerializableVector3(transform.position);
            this.rotation = new SerializableQuaternion(transform.rotation);
        }

        public SerializableVector3 position;
        public SerializableQuaternion rotation;

    }

    [Serializable]
    public struct SerializableVector3 {

        public SerializableVector3(Vector3 position) {
            x = position.x;
            y = position.y;
            z = position.z;
        }

        public Vector3 GetVector3() {
            return new Vector3(x, y, z);
        }

        public float x, y, z;

    }

    [Serializable]
    public struct SerializableQuaternion {

        public SerializableQuaternion(Quaternion position) {
            x = position.x;
            y = position.y;
            z = position.z;
            w = position.w;
        }

        public Quaternion GetQuaternion() {
            return new Quaternion(x, y, z, w);
        }

        public float x, y, z, w;

    }
}
