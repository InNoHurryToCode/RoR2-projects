using RoR2.UI.MainMenu;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SavedGames.UI
{
    class ModSubMenu
    {

        public GameObject menuObject;
        public GameObject subMenuObject;
        public SubmenuMainMenuScreen submenuMainMenuScreen;

        public ModSubMenu(string name, MainMenuController mainMenuController, GameObject submenuPrefab) {
            menuObject = new GameObject($"MENU: {name}");
            menuObject.transform.SetParent(mainMenuController.transform);
            
            subMenuObject = new GameObject(name);
            submenuMainMenuScreen = subMenuObject.AddComponent<SubmenuMainMenuScreen>();
            submenuPrefab.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler canvasScaler = submenuPrefab.AddComponent<CanvasScaler>();

            subMenuObject.transform.SetParent(menuObject.transform);

            submenuPrefab.AddComponent<GraphicRaycaster>();

            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);

            submenuMainMenuScreen.submenuPanelPrefab = new GameObject("Dummy");
            submenuMainMenuScreen.onEnter = new UnityEvent();
            submenuMainMenuScreen.onExit = new UnityEvent();
            submenuMainMenuScreen.desiredCameraTransform = GameObject.Find("World Position").transform;

        }

    }
}
