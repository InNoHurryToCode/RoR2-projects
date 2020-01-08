using RoR2.UI;
using RoR2.UI.SkinControllers;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace SavedGames.UI {
    public class ModButton : ModUIObject {
        public HGTextMeshProUGUI tmpText;
        public HGTextMeshProUGUI timeText;
        public ButtonSkinController buttonSkinController;
        public CustomButtonTransition customButtonTransition;

        public Image image;
        public Image hoverImage;
        public Image baseImage;

        public ModButton(string text) {
            gameObject = new GameObject("Button");
            GameObject hoverOutline = new GameObject("Hover Outline");
            GameObject baseOutline = new GameObject("Base Outline");
            GameObject textObject = new GameObject("Text");

            rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.localScale = Vector3.one;

            RectTransform hoverRect = hoverOutline.AddComponent<RectTransform>();
            RectTransform baseRect = baseOutline.AddComponent<RectTransform>();
            RectTransform textRect = textObject.AddComponent<RectTransform>();
            MPEventSystemLocator systemLocator = gameObject.AddComponent<MPEventSystemLocator>();
            LayoutElement layoutElement = gameObject.AddComponent<LayoutElement>();
            customButtonTransition = gameObject.AddComponent<CustomButtonTransition>();
            buttonSkinController = gameObject.AddComponent<ButtonSkinController>();
            image = gameObject.AddComponent<Image>();
            customButtonTransition.targetGraphic = image;

            ColorBlock c = customButtonTransition.colors;
            hoverImage = hoverOutline.AddComponent<Image>();
            baseImage = baseOutline.AddComponent<Image>();

            hoverOutline.transform.SetParent(gameObject.transform);
            baseOutline.transform.SetParent(gameObject.transform);
            textObject.transform.SetParent(gameObject.transform);

            hoverRect.anchorMin = new Vector2(0, 0);
            hoverRect.anchorMax = new Vector2(1, 1);
            hoverRect.sizeDelta = new Vector2(0, 0);

            baseRect.anchorMin = new Vector2(0, 0);
            baseRect.anchorMax = new Vector2(1, 1);
            baseRect.sizeDelta = new Vector2(0, 0);

            tmpText = textObject.AddComponent<HGTextMeshProUGUI>();
            tmpText.text = text;

            textRect.anchorMin = new Vector2(0, 0);
            textRect.anchorMax = new Vector2(1, 1);
            textRect.sizeDelta = new Vector2(0, 0);

            image.sprite = Generic.FindResource<Sprite>("texUICleanButton");
            hoverImage.sprite = Generic.FindResource<Sprite>("texUIHighlightBoxOutline");
            baseImage.sprite = Generic.FindResource<Sprite>("texUIOutlineOnly");

            image.type = Image.Type.Sliced;
            hoverImage.type = Image.Type.Sliced;
            baseImage.type = Image.Type.Sliced;

            customButtonTransition.imageOnHover = hoverImage;
            customButtonTransition.imageOnInteractable = baseImage;
            customButtonTransition.scaleButtonOnHover = false;
            customButtonTransition.showImageOnHover = true;
            customButtonTransition.allowAllEventSystems = true;
            customButtonTransition.pointerClickOnly = true;

            buttonSkinController.skinData = Generic.FindResource<UISkinData>("skinMenu");

            c.normalColor = new Color32(83, 102, 120, 255);
            c.highlightedColor = new Color32(251, 255, 176, 187);
            c.pressedColor = new Color32(188, 192, 113, 251);
            c.disabledColor = new Color32(64, 51, 51, 182);
            customButtonTransition.colors = c;

        }
    }
}
