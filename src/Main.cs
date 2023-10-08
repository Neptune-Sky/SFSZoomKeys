using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ModLoader;
using SFS.IO;
using UITools;

namespace SFSZoomKeys
{
    [UsedImplicitly]
    public class Main : Mod, IUpdatable
    {
        public override string ModNameID => "Neptune.ZoomKeys.Mod";
        public override string DisplayName => "Zoom Keys";
        public override string Author => "NeptuneSky";
        public override string MinimumGameVersionNecessary => "1.5.10.2";
        public override string ModVersion => "v1.0.2";
        public override string Description => "Adds bindable zoom keys to the game. For trackpad users or people who are otherwise unable to zoom.";

        public Dictionary<string, FilePath> UpdatableFiles => new ()
        {
            {
                "https://github.com/Neptune-Sky/SFSZoomKeys/releases/latest/download/SFSZoomKeys.dll",
                new FolderPath(ModFolder).ExtendToFile("SFSZoomKeys.dll")
            }
        };

        public static Main main;
        
        public override Action LoadKeybindings => ZoomKeybindings.LoadKeybindings;
        public override void Load()
        {
        }

        public override void Early_Load()
        {
            main = this;
            Config.Load();
        }

        
    }
}