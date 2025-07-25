// -----------------------------------------------------------------------
//  <copyright file = "Helpers.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using ImageMagick;

namespace Prism.ProAssistant.Api.Helpers;

public static class ImageProcessor
{
    public static byte[] Resize(byte[] data, int width, int height)
    {
        var image = new MagickImage(data);

        if (image.Height < height && image.Width < width)
        {
            return data;
        }

        image.Resize((uint)width, (uint)height);
        return image.ToByteArray();
    }
}