using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Phone.Shell;

namespace BodyArchitect.WP7.Utils
{
    static class LargeLiveTileHelper
    {
        private static Version TargetedVersion = new Version(7, 10, 8858);

        public static bool IsTargetedVersion { get { return Environment.OSVersion.Version >= TargetedVersion; } }

        public static void UpdateFlipTile(
            string title,
            string backTitle,
            string backContent,
            string wideBackContent,
            int? count,
            Uri tileId,
            Uri smallBackgroundImage,
            Uri backgroundImage,
            Uri backBackgroundImage,
            Uri wideBackgroundImage,
            Uri wideBackBackgroundImage)
        {
            if (IsTargetedVersion)
            {
                // Get the new FlipTileData type.
                Type flipTileDataType = Type.GetType("Microsoft.Phone.Shell.FlipTileData, Microsoft.Phone");

                // Get the ShellTile type so we can call the new version of "Update" that takes the new Tile templates.
                Type shellTileType = Type.GetType("Microsoft.Phone.Shell.ShellTile, Microsoft.Phone");

                // Loop through any existing Tiles that are pinned to Start.
                foreach (var tileToUpdate in ShellTile.ActiveTiles)
                {
                    // Look for a match based on the Tile's NavigationUri (tileId).
                    if (tileToUpdate.NavigationUri.ToString() == tileId.ToString())
                    {
                        // Get the constructor for the new FlipTileData class and assign it to our variable to hold the Tile properties.
                        var UpdateTileData = flipTileDataType.GetConstructor(new Type[] { }).Invoke(null);

                        // Set the properties. 
                        SetProperty(UpdateTileData, "Title", title);
                        if (count.HasValue)
                        {
                            SetProperty(UpdateTileData, "Count", count);
                        }
                        SetProperty(UpdateTileData, "BackTitle", backTitle);
                        SetProperty(UpdateTileData, "BackContent", backContent);
                        SetProperty(UpdateTileData, "SmallBackgroundImage", smallBackgroundImage);
                        SetProperty(UpdateTileData, "BackgroundImage", backgroundImage);
                        SetProperty(UpdateTileData, "BackBackgroundImage", backBackgroundImage);
                        SetProperty(UpdateTileData, "WideBackgroundImage", wideBackgroundImage);
                        SetProperty(UpdateTileData, "WideBackBackgroundImage", wideBackBackgroundImage);
                        SetProperty(UpdateTileData, "WideBackContent", wideBackContent);

                        // Invoke the new version of ShellTile.Update.
                        shellTileType.GetMethod("Update").Invoke(tileToUpdate, new Object[] { UpdateTileData });
                        break;
                    }
                }
            }

        }

        private static void SetProperty(object instance, string name, object value)
        {
            var setMethod = instance.GetType().GetProperty(name).GetSetMethod();
            setMethod.Invoke(instance, new object[] { value });
        }
    }
}
