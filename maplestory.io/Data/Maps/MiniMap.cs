﻿using PKG1;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace maplestory.io.Data.Maps
{
    public class MiniMap
    {
        public int centerX, centerY, height, width, magnification;
        public Image<Rgba32> canvas;

        public static MiniMap Parse(WZProperty data)
        {
            if (data == null) return null;
            MiniMap result = new MiniMap();
            result.canvas = data.ResolveForOrNull<Image<Rgba32>>("canvas");
            result.centerX = data.ResolveFor<int>("centerX") ?? -1;
            result.centerY = data.ResolveFor<int>("centerY") ?? -1;
            result.height = data.ResolveFor<int>("height") ?? -1;
            result.width = data.ResolveFor<int>("width") ?? -1;
            result.magnification = data.ResolveFor<int>("mag") ?? -1;
            return result;
        }
    }
}
