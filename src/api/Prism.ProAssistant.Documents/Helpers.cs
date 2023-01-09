// -----------------------------------------------------------------------
//  <copyright file = "Helpers.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using ImageMagick;

namespace Prism.ProAssistant.Documents;

public static class Helpers
{
    public static byte[] ResizeImage(byte[] data, int width, int height)
    {
        var image = new MagickImage(data);

        if (image.Height < height && image.Width < width)
        {
            return data;
        }

        image.Resize(width, height);
        return image.ToByteArray();
    }
}