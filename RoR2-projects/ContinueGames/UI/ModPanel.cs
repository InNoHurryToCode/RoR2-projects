using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace SavedGames.UI
{
    class ModPanel : ModUIObject {

        public Image panelImage;

        public ModPanel() {
            gameObject = new GameObject("BG Panel");
            panelImage = gameObject.AddComponent<Image>();
            rectTransform = gameObject.GetComponent<RectTransform>();
            panelImage.sprite = Utilities.Generic.FindResource<Sprite>("texUIPopupRect");
            panelImage.type = Image.Type.Sliced;
            panelImage.color = new Color32(41, 43, 45, 241);

        }
    }
}
