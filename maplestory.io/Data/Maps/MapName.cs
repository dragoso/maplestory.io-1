﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using PKG1;

namespace maplestory.io.Data.Maps
{
    public class MapName
    {
        public string Name, StreetName;
        public int Id;

        public static MapName Parse(WZProperty mapEntry)
        {
            if (mapEntry == null) return null;
            return new MapName()
            {
                Id = int.Parse(mapEntry.NameWithoutExtension),
                Name = mapEntry.ResolveForOrNull<string>("mapName"),
                StreetName = mapEntry.ResolveForOrNull<string>("streetName")
            };
        }

        public static IEnumerable<MapName> GetMapNames(WZProperty stringWz)
        {
            IEnumerable<MapName> names = null;
            if (stringWz.FileContainer.Collection.VersionCache.TryGetValue("mapNames", out object mapNamesCached))
                names = (IEnumerable<MapName>)mapNamesCached;
            else
            {
                names = stringWz.Resolve("Map").Children.SelectMany(c => c.Children).Select(c => MapName.Parse(c));
                stringWz.FileContainer.Collection.VersionCache.AddOrUpdate("mapNames", names, (a, b) => b);
            }

            return names;
        }

        public static ILookup<int, MapName> GetMapNameLookup(WZProperty anyWz)
        {
            ILookup<int, MapName> lookup = null;

            if (anyWz.FileContainer.Collection.VersionCache.TryGetValue("mapNames", out object mapNamesCached))
                lookup = (ILookup<int, MapName>)mapNamesCached;
            else
            {
                lookup = anyWz.ResolveOutlink("String/Map").Children.SelectMany(c => c.Children).ToLookup(c => int.Parse(c.NameWithoutExtension), c => MapName.Parse(c));
                anyWz.FileContainer.Collection.VersionCache.AddOrUpdate("mapNameLookup", lookup, (a, b) => b);
            }

            return lookup;
        }
    }
}
