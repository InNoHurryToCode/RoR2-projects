using System;
using System.Collections;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;

using RoR2;
using RoR2.Networking;
using UnityEngine;
using Utilities;
using SavedGames.Data;


using ArgsHelper = Utilities.Generic.ArgsHelper;
using SavedGames.UI;
using System.Reflection;
using System.IO;
using UnityEngine.UI;
using RoR2.UI;
using TMPro;
using RoR2.UI.SkinControllers;
using System.Text;
using UnityEngine.Events;

namespace SavedGames
{

    [BepInPlugin("com.morris1927.SavedGames", "SavedGames", "2.2.0")]
    public class SavedGames : BaseUnityPlugin {

        public static SavedGames instance { get; set; }

        public static bool loadingScene;

        public static ConfigWrapper<int> loadKey { get; set; }
        public static ConfigWrapper<int> saveKey { get; set; }

        public static string directory = Assembly.GetExecutingAssembly().Location.Replace("SavedGames.dll", "SaveStates\\");

        
        public void Awake() {
            //Singleton used for hacky crap :)
            if (instance == null) {
                instance = this;
            } else {
                Destroy(this);
            }
            
            loadKey = Config.Wrap<int>(
                "Keybinds", "LoadKey", null,
                (int) KeyCode.F5);
            saveKey = Config.Wrap<int>(
                "Keybinds", "SaveKey", null,
                (int) KeyCode.F8);

            //Register our Commands
            On.RoR2.Console.Awake += (orig, self) => {
                Generic.CommandHelper.RegisterCommands(self);
                orig(self);
            };


            //Stop the scene loading objects so we can load our own
            On.RoR2.SceneDirector.PopulateScene += (orig, self) => {
                if (!loadingScene) {
                    orig(self);
                }
                loadingScene = false;
            };

            //Removing targetGraphic null error
            On.RoR2.UI.CustomButtonTransition.DoStateTransition += (orig, self, state, instant) => {
                if (self.targetGraphic != null) {
                    orig(self, state, instant);

                }
            };

            //Removing targetGraphic null error
            On.RoR2.UI.CustomScrollbar.DoStateTransition += (orig, self, state, instant) => {
                if (self.targetGraphic != null) {
                    orig(self, state, instant);

                }
            };

            //Setup "Load Game" Main Menu button and loading menu
            On.RoR2.UI.MainMenu.MainMenuController.Start += (orig, self) => {
                ModButton button = new ModButton("Load Game");
                Transform buttonTransform = button.gameObject.transform;
                buttonTransform.SetParent(self.titleMenuScreen.transform.GetChild(2).transform);
                buttonTransform.SetSiblingIndex(1);

                Color32 c = button.baseImage.color;
                c.a = 73;
                button.baseImage.color = c;

                button.rectTransform.localScale = Vector3.one;
                button.rectTransform.sizeDelta = new Vector2(320, 48);
                button.buttonSkinController.useRecommendedAlignment = false;
                button.tmpText.alignment = TextAlignmentOptions.Left;
                button.tmpText.rectTransform.sizeDelta = new Vector2(-24, -8);

                GameObject submenuPrefab = new GameObject("Load Game");
                GameObject scaledSpace = new GameObject("Scaled Space");
                RectTransform scaledRect = scaledSpace.AddComponent<RectTransform>();
                scaledRect.anchorMin = new Vector2(0.05f, 0.05f);
                scaledRect.anchorMax = new Vector2(0.95f, 0.95f);
                scaledRect.offsetMax = new Vector2(0, 0);
                scaledRect.offsetMin = new Vector2(0, 0);

                scaledSpace.transform.SetParent(submenuPrefab.transform);

                ModPanel topPanel = new ModPanel();
                topPanel.gameObject.transform.SetParent(scaledSpace.transform);
                topPanel.rectTransform.anchorMin = new Vector2(0, 1);
                topPanel.rectTransform.anchorMax = new Vector2(1, 1);

                topPanel.rectTransform.sizeDelta = new Vector2(0, 96);
                topPanel.rectTransform.pivot = new Vector2(0.5f, 1);

                ModButton backButton = new ModButton("Back");
                backButton.gameObject.transform.SetParent(scaledSpace.transform);
                backButton.customButtonTransition.onClick.AddListener(delegate () { self.SetDesiredMenuScreen(self.titleMenuScreen); });

                //ModButton loadButton = new ModButton("Load");
                //loadButton.gameObject.transform.SetParent(scaledSpace.transform);
                //loadButton.customButtonTransition.onClick.AddListener(delegate () { });

                ModSubMenu saveMenu = new ModSubMenu("Load Menu", self, submenuPrefab);
                submenuPrefab.transform.SetParent(saveMenu.subMenuObject.transform);

                ModScrollBar sb = new ModScrollBar();
                sb.gameObject.transform.SetParent(scaledSpace.transform);

                submenuPrefab.SetActive(false);

                GameObject contentObject = new GameObject("Content");
                contentObject.transform.SetParent(sb.viewportObject.transform);
                contentObject.AddComponent<CanvasRenderer>();

                instance.StartCoroutine(SetupUI(delegate () {
                    backButton.rectTransform.anchoredPosition = new Vector2(0, 0);
                    backButton.rectTransform.anchorMin = new Vector2(0, 0);
                    backButton.rectTransform.anchorMax = new Vector2(0, 0);
                    backButton.rectTransform.sizeDelta = new Vector2(200, 50);
                    backButton.rectTransform.pivot = new Vector2(0, 0);

                    //loadButton.rectTransform.anchorMin = new Vector2(1, 0);
                    //loadButton.rectTransform.anchorMax = new Vector2(1, 0);
                    //loadButton.rectTransform.sizeDelta = new Vector2(200, 50);
                    //loadButton.rectTransform.pivot = new Vector2(1, 0);


                    
                    RectTransform contentRect = contentObject.AddComponent<RectTransform>();
                    contentRect.sizeDelta = new Vector2(100, 300);


                    sb.scrollRect.content = contentRect;
                    sb.customScrollbar.direction = Scrollbar.Direction.BottomToTop;
                    sb.scrollRect.movementType = ScrollRect.MovementType.Clamped;

                    sb.rectTransform.anchorMin = new Vector2(0, 0);
                    sb.rectTransform.anchorMax = new Vector2(1, 1);
                    sb.rectTransform.sizeDelta = new Vector2(-60, -200);
                    sb.rectTransform.anchoredPosition = new Vector2(0, 0);

                    sb.gameObject.GetComponent<Image>().color = new Color32(16,16,16,150);
                    sb.gameObject.GetComponent<Image>().sprite = Generic.FindResource<Sprite>("texUIHighlightHeader");
                    sb.gameObject.GetComponent<Image>().color = new Color32(14, 14, 14, 150);
                    sb.gameObject.GetComponent<Image>().type = Image.Type.Sliced;

                    sb.customScrollbar.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
                    sb.customScrollbar.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
                    sb.customScrollbar.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 0);

                    sb.handleAreaObject.GetComponent<RectTransform>().anchorMin = new Vector2(1, 0);
                    sb.handleAreaObject.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
                    sb.handleAreaObject.GetComponent<RectTransform>().sizeDelta = new Vector2(10, 0);
                    sb.handleAreaObject.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
                    sb.handleAreaObject.GetComponent<Image>().sprite = Generic.FindResource<Sprite>("texUICleanButton");
                    sb.handleAreaObject.GetComponent<Image>().color = new Color32(44, 44, 44, 150);
                    sb.handleAreaObject.GetComponent<Image>().type = Image.Type.Sliced;

                    sb.handleObject.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 0);
                    sb.handleObject.GetComponent<Image>().sprite = Generic.FindResource<Sprite>("texUICleanButton");
                    sb.handleObject.GetComponent<Image>().type = Image.Type.Sliced;

                    sb.handleOutlineObject.GetComponent<Image>().enabled = false;

                    sb.viewportObject.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
                    sb.viewportObject.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
                    sb.viewportObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);

                    contentObject.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
                    contentObject.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
                    contentObject.GetComponent<RectTransform>().pivot = new Vector2(1, 1);
                    contentObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);


                    VerticalLayoutGroup contentLayoutGroup = contentObject.AddComponent<VerticalLayoutGroup>();
                    ContentSizeFitter contentSizeFitter = contentObject.AddComponent<ContentSizeFitter>();

                    contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.MinSize;


                    foreach (var file in Directory.GetFiles(directory)) {
                        FileStream saveFile = File.Open(file, FileMode.Open);
                        string fileName = saveFile.Name.Replace(directory, "")
                                                       .Replace(".json", "");
                        ModButton fileSelectButton = new ModButton(fileName);
                        fileSelectButton.customButtonTransition.onClick.AddListener(delegate () {
                            RoR2.Console.instance.SubmitCmd(null, $"load {fileName}");
                        });
                        fileSelectButton.gameObject.GetComponent<LayoutElement>().minHeight = 200;
                        fileSelectButton.gameObject.transform.SetParent(contentObject.transform);

                        saveFile.Close();
                    }

                }));


                button.customButtonTransition.onClick.AddListener(delegate () {
                    self.SetDesiredMenuScreen(saveMenu.submenuMainMenuScreen);
                    submenuPrefab.SetActive(true);
                });

                orig(self);
            };

            //Add save button to pause screen
            On.RoR2.UI.PauseScreenController.Awake += (orig, self) => {

                ModButton button = new ModButton("Save");
                GameObject buttonObject = button.gameObject;
                buttonObject.transform.SetParent(self.mainPanel.GetChild(0));
                buttonObject.transform.SetSiblingIndex(1);
                buttonObject.GetComponent<RectTransform>().localScale = Vector3.one;
                buttonObject.GetComponent<RectTransform>().sizeDelta = new Vector2(320, 48);

                ModInputField inputField = new ModInputField();
                GameObject inputObject = inputField.gameObject;

                inputObject.transform.SetParent(self.mainPanel.GetChild(0));
                inputObject.transform.SetSiblingIndex(1);

                RectTransform inputRect = inputField.rectTransform;
                inputRect.sizeDelta = new Vector2(320, 48);
                inputRect.localScale = new Vector3(1, 1, 1);

                CustomButtonTransition buttonEvents = buttonObject.GetComponent<CustomButtonTransition>();

                buttonEvents.onClick.AddListener(() => { RoR2.Console.instance.SubmitCmd(null, $"save {inputField.tmpInputField.text}"); });

                orig(self);
            };

        }

        IEnumerator SetupUI(Action uiDelegate) {
            yield return new WaitForEndOfFrame();
            uiDelegate?.Invoke();
        }

        public void Update() {
            HandleInputs();

        }

        private void HandleInputs() {
            if (Input.GetKeyDown((KeyCode)loadKey.Value)) {
                //Save
                RoR2.Console.instance.SubmitCmd(null, "save quicksave");
            }
            if (Input.GetKeyDown((KeyCode)saveKey.Value)) {
                //Load
                RoR2.Console.instance.SubmitCmd(null, "load quicksave ");
            }
            //---------------//---------------//---------------//---------------//---------------//---------------//---------------//---------------//---------------
           // if (Input.GetKeyDown(KeyCode.F6)) {
           //     //Quick cheats
           //     RoR2.Console.instance.SubmitCmd(NetworkUser.readOnlyLocalPlayersList[0], "give_item hoof 30; god; kill_all; no_enemies");
           // }
            //---------------//---------------//---------------//---------------//---------------//---------------//---------------//---------------//---------------
        }

        [ConCommand(commandName = "load", flags = ConVarFlags.None, helpText = "Load game")]
        private static void CCLoad(ConCommandArgs args) {
            if (args.Count != 1) {
                Debug.Log("Command failed, requires 1 argument: load <filename>");
                return;
            }
            if (loadingScene) {
                return;
            }
            
            string fileName = ArgsHelper.GetValue(args.userArgs, 0);
            string saveJSON = File.ReadAllText($"{directory}{fileName}.json");

            SaveData save = TinyJson.JSONParser.FromJson<SaveData>(saveJSON);
            instance.StartCoroutine(instance.StartLoading(save));
            
        }


        [ConCommand(commandName = "save", flags = ConVarFlags.None, helpText = "Save game")]
        private static void CCSave(ConCommandArgs args) {
            if (args.Count != 1) {
                Debug.Log("Command failed, requires 1 argument: save <filename>");
                return;
            }


            SaveData save = new SaveData();

            string json = TinyJson.JSONWriter.ToJson(save);
            string fileName = ArgsHelper.GetValue(args.userArgs, 0);

            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllText($"{directory}{fileName}.json", json);

        }

         private IEnumerator StartLoading(SaveData save) {

            loadingScene = true;
            if (Run.instance == null) {
                GameNetworkManager.singleton.desiredHost = new GameNetworkManager.HostDescription(new GameNetworkManager.HostDescription.HostingParameters {
                    listen = false,
                    maxPlayers = 1
                });
                yield return new WaitUntil(() => PreGameController.instance != null);
                PreGameController.instance?.StartLaunch();
                yield return new WaitUntil(() => Run.instance != null);
            }
            save.run.LoadData();


            //yield return new WaitForSeconds(Stage.instance == null ? 1.5f : 0.75f);
            Debug.Log("[SavedGames] Waiting on Stage.instace to load");
            yield return new WaitUntil( () => Stage.instance != null );
            Debug.Log("[SavedGames] Stage.instance loaded");
            save.Load();
            
            loadingScene = false;
        }

        public static NetworkUser GetPlayerFromUsername(string username) {

            foreach (var item in NetworkUser.readOnlyInstancesList) {
                if (username == item.userName) {
                    return item;
                }
            }

            return null;
        }
    }
}
