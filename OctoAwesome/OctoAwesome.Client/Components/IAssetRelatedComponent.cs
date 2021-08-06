﻿namespace OctoAwesome.Client.Components
{
    /// <summary>
    ///     Interface für alle Componenten, die mit Assets aus dem Asset
    ///     Manager (AssetComponent) arbeiten.
    /// </summary>
    internal interface IAssetRelatedComponent
    {
        /// <summary>
        ///     Signalisiert das forcierte entladen aller Assets.
        /// </summary>
        void UnloadAssets();

        /// <summary>
        ///     Signalisiert das neu laden aller Assets.
        /// </summary>
        void ReloadAssets();
    }
}