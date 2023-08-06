using System;
using SFS.Input;
using SFS.IO;
using SFS.UI;
using SFS.UI.ModGUI;
using UITools;
using UnityEngine;
using static SFS.UI.ModGUI.Builder;
using Type = SFS.UI.ModGUI.Type;

namespace SFSZoomKeys
{
    [Serializable]
    public class SettingsData
    {
        public float zoomSpeed = 1f;
    }

    public class Config : ModSettings<SettingsData>
    {
        private static Config main;

        private Action saveAction;

        protected override FilePath SettingsFile { get; } = new FolderPath(Main.main.ModFolder).ExtendToFile("Config.txt");

        public static void Load()
        {
            main = new Config();
            main.Initialize();
        }

        public static void Save()
        {
            main.saveAction?.Invoke();
        }

        protected override void RegisterOnVariableChange(Action onChange)
        {
            saveAction = onChange;
            Application.quitting += onChange;
        }
        
        private const float windowScale = 1f;
        private const float buttonTextScale = 0.875f;

        public static void Open()
        {
            
            var output = new MenuElement(delegate(GameObject root)
            {
                var containerObject = new GameObject("ModGUI Container");
                var rectTransform = containerObject.AddComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(0, 0);
                
                Window scroll = CreateWindow(rectTransform, GetRandomID(), 400, 210, 0, 0, false, false, 1, "Zoom Keys");

                scroll.Position = new Vector2(0, scroll.Size.y * windowScale / 2);
                scroll.CreateLayoutGroup(Type.Vertical, spacing: 5);

                Container container = CreateContainer(scroll);
                container.CreateLayoutGroup(Type.Vertical, TextAnchor.MiddleLeft, 0);
                
                
                CreateLabel(container, 150, 45, text: "Zoom Speed:");

                Container SliderLabel = CreateContainer(container);
                SliderLabel.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleRight);

                CreateSlider(SliderLabel, 370, settings.zoomSpeed, (0.5f, 3f),
                    false,
                    val => { settings.zoomSpeed = val; },
                    val => Math.Round(val * 100) + "%");

                CreateButton(scroll, 100, 45, onClick: () => ScreenManager.main.CloseCurrent(), text: "Okay");
                scroll.gameObject.transform.localScale = new Vector3(windowScale, windowScale);
                containerObject.transform.SetParent(root.transform);
            });
            
            MenuGenerator.OpenMenu(CancelButton.Cancel, CloseMode.Current, output);
        }
    }
}