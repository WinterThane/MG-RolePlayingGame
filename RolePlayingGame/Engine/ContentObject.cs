﻿using Microsoft.Xna.Framework.Content;

namespace RolePlayingGame.Engine
{
    public abstract class ContentObject
    {
        private string _assetName;

        [ContentSerializerIgnore]
        public string AssetName
        {
            get => _assetName;
            set => _assetName = value;
        }
    }
}
