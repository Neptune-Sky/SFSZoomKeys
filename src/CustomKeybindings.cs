using HarmonyLib;
using ModLoader;
using ModLoader.Helpers;
using SFS.Builds;
using SFS.Input;
using SFS.World;
using SFS.World.Maps;
using UnityEngine;
using static SFS.Input.KeybindingsPC;
// ReSharper disable MemberCanBePrivate.Global

namespace SFSZoomKeys
{
    public class DefaultKeys
    {
        public readonly Key[] Zoom =
        {
            KeyCode.Equals,
            KeyCode.Minus
        };

        public readonly Key OpenMenu = KeyCode.F9;
    }

    public class ZoomKeybindings : ModKeybindings
    {
        private static readonly DefaultKeys DefaultKeys = new();

        #region Keys

        public Key[] Zoom = DefaultKeys.Zoom;
        public Key OpenMenu = DefaultKeys.OpenMenu;

        #endregion

        private static ZoomKeybindings main;

        public static void LoadKeybindings()
        {
            main = SetupKeybindings<ZoomKeybindings>(Main.main);
            SceneHelper.OnWorldSceneLoaded += OnWorldLoad;
            SceneHelper.OnBuildSceneLoaded += OnBuildLoad;
        }

        private static void OnBuildLoad()
        {
            BuildManager.main.build_Input.keysNode.AddOnKey(main.Zoom[0], () => ZoomExec(BuildManager.main));
            BuildManager.main.build_Input.keysNode.AddOnKey(main.Zoom[1], () => ZoomExec(BuildManager.main, true));
            
            AddOnKeyDown_Build(main.OpenMenu, Config.Open);
        }
        private static void OnWorldLoad()
        {
            GameManager.main.world_Input.keysNode.AddOnKey(main.Zoom[0], () => ZoomExec(GameManager.main)); 
            GameManager.main.world_Input.keysNode.AddOnKey(main.Zoom[1], () => ZoomExec(GameManager.main, true)); 
            
            GameManager.main.map_Input.keysNode.AddOnKey(main.Zoom[0], () => ZoomExec(Map.view)); 
            GameManager.main.map_Input.keysNode.AddOnKey(main.Zoom[1], () => ZoomExec(Map.view, true)); 
            
            AddOnKeyDown_World(main.OpenMenu, Config.Open);
        }

        public override void CreateUI()
        {
            CreateUI_Space();
            CreateUI_Text("Zoom Keybindings");
            CreateUI_Keybinding(Zoom, DefaultKeys.Zoom, "Zoom In/Out");
            CreateUI_Keybinding(OpenMenu, DefaultKeys.OpenMenu, "Change Zoom Speed");
            CreateUI_Space();
        }
        
        public static void ZoomExec(object zoomableObject, bool zoomOut = false)
        {
            float zoomDelta = 1 + (zoomOut ?  Config.settings.zoomSpeed : -Config.settings.zoomSpeed) / 100;
            Vector3 zoomPosition = Config.settings.scrollFromMiddle ? new Vector2((float)Screen.width / 2, (float)Screen.height / 2) : Input.mousePosition;
            var parameters = new object[] { new ZoomData(zoomDelta, new TouchPosition(zoomPosition)) };

            Traverse.Create(zoomableObject).Method("OnZoom", parameters).GetValue();
        }

        public static float ZoomDelta(bool zoomingOut)
        {
            return 1 + (zoomingOut ?  Config.settings.zoomSpeed : -Config.settings.zoomSpeed) / 100;
        }
        public static void Zoom_Build(bool zoomOut = false)
        {
            Traverse.Create(BuildManager.main).Method("OnZoom", new ZoomData(ZoomDelta(zoomOut), new TouchPosition(Input.mousePosition)))
                .GetValue();
        }

        public static void Zoom_World(bool zoomOut = false)
        {
            Traverse.Create(PlayerController.main).Method("OnZoom", new ZoomData(ZoomDelta(zoomOut), new TouchPosition(Input.mousePosition)))
                .GetValue();
        }

        public static void Zoom_Map(bool zoomOut = false)
        {
            Traverse.Create(Map.view).Method("OnZoom", new ZoomData(ZoomDelta(zoomOut), new TouchPosition(Input.mousePosition)))
                .GetValue();
        }
    }
}