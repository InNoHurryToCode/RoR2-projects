using RoR2.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace SavedGames.UI
{
    class ModScrollBar : ModUIObject
    {

        public CustomScrollbar customScrollbar;
        public GameObject sbVerticalObject;
        public GameObject viewportObject;
        public GameObject handleAreaObject;
        public GameObject handleObject;
        public GameObject handleOutlineObject;
        public ScrollRect scrollRect;

        public ModScrollBar() {
            gameObject = new GameObject("Base Scroll Area");
            rectTransform = gameObject.AddComponent<RectTransform>();
            sbVerticalObject = new GameObject("Vertical Scrollbar");
            handleAreaObject = new GameObject("Handle Area");
            handleObject = new GameObject("Handle");
            handleOutlineObject = new GameObject("Handle Outline");
            viewportObject = new GameObject("Viewport");

            sbVerticalObject.transform.SetParent(gameObject.transform);
            handleAreaObject.transform.SetParent(sbVerticalObject.transform);
            handleObject.transform.SetParent(handleAreaObject.transform);
            handleOutlineObject.transform.SetParent(handleObject.transform);
            viewportObject.transform.SetParent(gameObject.transform);

            scrollRect = gameObject.AddComponent<ScrollRect>();
            gameObject.AddComponent<Image>();
            handleAreaObject.AddComponent<Image>();
            viewportObject.AddComponent<Image>();

            Image handleImage = handleObject.AddComponent<Image>();
            Image handleOutlineImage = handleOutlineObject.AddComponent<Image>();
            RectTransform handleOutlineRect = handleOutlineObject.GetComponent<RectTransform>();
            Mask mask = viewportObject.AddComponent<Mask>();
            RectTransform handleAreaRect = handleAreaObject.GetComponent<RectTransform>();
            RectTransform handleRect = handleObject.GetComponent<RectTransform>();


            handleOutlineRect.sizeDelta = new Vector2(0, 0);


            handleAreaRect.pivot = new Vector2(0, 1);
            handleAreaRect.anchorMin = new Vector2(0, 1);
            handleAreaRect.anchorMax = new Vector2(0, 1);

            customScrollbar = sbVerticalObject.AddComponent<CustomScrollbar>();
            customScrollbar.imageOnHover = handleOutlineImage;
            customScrollbar.allowAllEventSystems = true;
            customScrollbar.handleRect = handleObject.GetComponent<RectTransform>();
            customScrollbar.targetGraphic = handleImage;
            customScrollbar.GetComponent<MPEventSystemLocator>().SetProperyValue("eventSystemProvider", GameObject.Find("MainMenu").GetComponent<MPEventSystemProvider>());

            scrollRect.viewport = viewportObject.GetComponent<RectTransform>();
            scrollRect.verticalScrollbar = customScrollbar;
            scrollRect.scrollSensitivity = 50f;

            mask.showMaskGraphic = false;
                


        }

    }
}
