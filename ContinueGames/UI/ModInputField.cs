using UnityEngine;

namespace SavedGames.UI {
    class ModInputField : ModUIObject {

        public TMPro.TMP_InputField tmpInputField;

        public ModInputField() {
            gameObject = new UnityEngine.GameObject("Input Field");
            rectTransform = gameObject.AddComponent<RectTransform>();

            GameObject inputField = TMPro.TMP_DefaultControls.CreateInputField(new TMPro.TMP_DefaultControls.Resources { });
            inputField.transform.SetParent(rectTransform);

            RectTransform inputRect = inputField.GetComponent<RectTransform>();
            inputRect.anchorMin = new Vector2(0, 1);
            inputRect.anchorMax = new Vector2(0, 1);
            inputRect.pivot = new Vector2(0, 1);
            inputRect.sizeDelta = new Vector2(320, 48);
            tmpInputField = inputField.GetComponent<TMPro.TMP_InputField>();
            tmpInputField.pointSize = 30;
        }
    }
}
