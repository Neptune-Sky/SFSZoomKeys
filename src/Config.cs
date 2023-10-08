using System;
using System.Globalization;
using SFS.Input;
using SFS.IO;
using SFS.UI;
using SFS.UI.ModGUI;
using TMPro;
using UITools;
using UnityEngine;
using UnityEngine.UI;
using static SFS.UI.ModGUI.Builder;
using Button = SFS.UI.ModGUI.Button;
using Slider = SFS.UI.ModGUI.Slider;
using Type = SFS.UI.ModGUI.Type;

namespace SFSZoomKeys
{
    [Serializable]
    public class SettingsData
    {
        public float zoomSpeed = 1f;
        public bool scrollFromMiddle;
        public bool useTextInput;
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
                
                Window scroll = CreateWindow(rectTransform, GetRandomID(), 470, 260, 0, 0, false, false, 1, "Zoom Keys");

                scroll.Position = new Vector2(0, scroll.Size.y * windowScale / 2);
                scroll.CreateLayoutGroup(Type.Vertical, spacing: 5);

                Container container = CreateContainer(scroll);
                container.CreateLayoutGroup(Type.Vertical, TextAnchor.MiddleLeft, 2);
                
                
                CreateLabel(container, 410, 35, text: "Zoom Speed:").TextAlignment = TextAlignmentOptions.MidlineLeft;

                Container SliderLabel = CreateContainer(container);
                SliderLabel.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleRight, -70);

                Slider slider = CreateSlider(SliderLabel, 370, settings.zoomSpeed, (0.5f, 3f));
                Button sliderButton = CreateButton(SliderLabel, 100, 50, text: (slider.Value * 100).Round(0).ToString(CultureInfo.InvariantCulture) + "%");
                slider.OnSliderChanged = val =>
                {
                    settings.zoomSpeed = val;
                    sliderButton.Text = (slider.Value * 100).Round(0).ToString(CultureInfo.InvariantCulture) + "%";
                };
                
                Container InputLabel = CreateContainer(container);
                
                InputLabel.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleRight, 2);
                
                NumberInput input = CustomUI.CreateNumberInput(InputLabel, 150, 50, 100, 0, 10000000, 25);

                Button inputButton = CreateButton(InputLabel, 40, 50, text: "%");
                input.textInput.OnChange += _ =>
                {
                    settings.zoomSpeed = (float)input.currentVal / 100;
                };

                CreateToggleWithLabel(container, 370, 50, () => settings.scrollFromMiddle, () => settings.scrollFromMiddle ^= true,
                    labelText: "Zoom From Screen's Center");
                CreateSpace(container, 0, 5);
                CreateButton(scroll, 100, 45, onClick: () => ScreenManager.main.CloseCurrent(), text: "Okay");
                scroll.gameObject.transform.localScale = new Vector3(windowScale, windowScale);
                containerObject.transform.SetParent(root.transform);

                sliderButton.OnClick = () =>
                {
                    settings.useTextInput = true;
                    HandleInputChange();
                };

                inputButton.OnClick = () =>
                {
                    settings.useTextInput = false;
                    HandleInputChange();
                };
                HandleInputChange();
                
                void HandleInputChange()
                {
                    if (settings.useTextInput)
                    {
                        SliderLabel.gameObject.SetActive(false);
                        InputLabel.gameObject.SetActive(true);
                        input.textInput.Text = (settings.zoomSpeed * 100).Round(0).ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        SliderLabel.gameObject.SetActive(true);
                        InputLabel.gameObject.SetActive(false);
                        slider.Value = settings.zoomSpeed;
                    }

                    LayoutRebuilder.ForceRebuildLayoutImmediate(scroll.ChildrenHolder as RectTransform);
                }
            });
            
            MenuGenerator.OpenMenu(CancelButton.Cancel, CloseMode.Current, output);
        }
    }
}