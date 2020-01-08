using RoR2;
using RoR2.UI;
using SavedGames.Data;
using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SavedGames.UI {
    public class SaveButton : ModButton {
        public string name;
        public long time;

        public GameObject container;

        public static string[] difficulties = new string[3] { "Drizzle", "Rainstorm", "Monsoon" };

        public SaveButton(string name, SaveData save) : base(name) {
            if (save == null) {
                throw new Exception($"Error loading save data for savegame '{name}'");
            }
            this.name = name;
            this.time = save.time;

            container = new GameObject("Container");
            container.AddComponent<RectTransform>().localScale = Vector3.one;
            container.AddComponent<HorizontalLayoutGroup>().childForceExpandWidth = false;
            container.AddComponent<LayoutElement>();

            gameObject.transform.SetParent(container.transform);
            gameObject.GetComponent<LayoutElement>().flexibleWidth = 1.0f;

            GameObject timeTextObject = new GameObject("Time Text");
            timeTextObject.transform.SetParent(gameObject.transform);
            GameObject statTextObject = new GameObject("Game Stats");
            statTextObject.transform.SetParent(gameObject.transform);

            // Position the time in the top right with a slight offset
            RectTransform timeRect = timeTextObject.AddComponent<RectTransform>();
            timeRect.anchorMin = new Vector2(0, 1);
            timeRect.anchorMax = new Vector2(0, 1);
            timeRect.pivot = new Vector2(0, 1);
            timeRect.anchoredPosition = new Vector2(10, -10);

            //Position stats in the top right
            RectTransform statRect = statTextObject.AddComponent<RectTransform>();
            statRect.anchorMin = new Vector2(1, 1);
            statRect.anchorMax = new Vector2(1, 1);
            statRect.pivot = new Vector2(1, 1);
            statRect.anchoredPosition = new Vector2(-10, -10);

            string date = DateTimeOffset.FromUnixTimeMilliseconds(time).Add(TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow)).ToString();

            // Format the time text and align it properly
            HGTextMeshProUGUI timeText = timeTextObject.AddComponent<HGTextMeshProUGUI>();
            timeText.text = "Created\n" + date.Substring(0, date.IndexOfAny(new char[] { '+', '-' }));
            timeText.color = Color.white;
            timeText.enableWordWrapping = false;
            timeText.alignment = TextAlignmentOptions.TopLeft;
            timeText.fontSize = 22;

            HGTextMeshProUGUI statText = statTextObject.AddComponent<HGTextMeshProUGUI>();
            statText.text = $"{save.run.stageClearCount} stages cleared\n" +
                $"{difficulties[save.run.difficulty]} difficulty\n" +
                $"{TimeSpan.FromSeconds(save.run.fixedTime).ToString("mm':'ss")} time\n" +
                $"{save.run.sceneName}";
            statText.color = Color.white;
            statText.enableWordWrapping = false;
            statText.alignment = TextAlignmentOptions.TopRight;
            statText.fontSize = 22;

            ModButton deleteButton = new ModButton("Delete");
            deleteButton.gameObject.transform.SetParent(container.transform);
            deleteButton.gameObject.GetComponent<LayoutElement>().preferredWidth = 200;
            deleteButton.gameObject.GetComponent<LayoutElement>().flexibleWidth = 0;
            deleteButton.gameObject.transform.SetAsLastSibling();

            deleteButton.customButtonTransition.onClick.AddListener(() => {
                SimpleDialogBox box = SimpleDialogBox.Create();
                box.headerLabel.text = "Delete save '" + name + "'";
                box.descriptionLabel.text = "Once you delete this save, the run will no longer be recoverable. Are you sure?";
                box.AddActionButton(() => {
                    File.Delete($"{SavedGames.directory}{name}.json");
                    UnityEngine.Object.Destroy(container.gameObject);
                    SavedGames.saveButtons.Remove(this);
                }, "Delete");
                box.AddCancelButton("Cancel");
            });

            GameObject iconContainer = new GameObject("Icon container");
            iconContainer.transform.SetParent(gameObject.transform);
            RectTransform iconContainerRect = iconContainer.AddComponent<RectTransform>();
            iconContainerRect.anchorMin = new Vector2(0, 0);
            iconContainerRect.anchorMax = new Vector2(1, 0);
            iconContainerRect.pivot = new Vector2(0, 0);
            iconContainerRect.anchoredPosition = new Vector2(10, 3);
            iconContainer.AddComponent<HorizontalLayoutGroup>().childForceExpandWidth = false;

            foreach (PlayerData pd in save.players) {
                GameObject bodyPrefab = BodyCatalog.FindBodyPrefab(pd.characterBodyName);
                Texture texture = bodyPrefab.GetComponent<CharacterBody>().portraitIcon;

                GameObject icon = new GameObject("Icon");
                icon.transform.SetParent(iconContainer.transform);
                icon.AddComponent<RectTransform>().anchorMin = new Vector2(0, 0);
                icon.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
                icon.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
                icon.AddComponent<RawImage>().texture = texture;
                icon.GetComponent<RawImage>().gameObject.AddComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;
                icon.GetComponent<RawImage>().gameObject.GetComponent<AspectRatioFitter>().aspectRatio = 1;
                icon.AddComponent<LayoutElement>().flexibleWidth = 0;
                icon.GetComponent<LayoutElement>().preferredWidth = 60;
            }

            // Modify colors to make the button look more like a button you shouldn't press unless you're sure
            ColorBlock colors = deleteButton.gameObject.GetComponent<CustomButtonTransition>().colors;
            colors.normalColor = new Color32(244, 67, 54, 255);
            colors.highlightedColor = new Color32(239, 83, 80, 187);
            colors.pressedColor = new Color32(211, 47, 47, 255);
            deleteButton.gameObject.GetComponent<CustomButtonTransition>().colors = colors;
        }
    }
}
