using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace Ark.Data.Image
{
    /// <summary>
    /// Repository for manipulating image binaries.
    /// + Leverages <see cref="Bitmap"/> and <see cref="Graphics"/> for high-quality resizing.
    /// - Depends on GDI+ which is not supported in headless environments.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.drawing"/>
    /// </summary>
    public class ImageRepository
    {
        #region Methods (Public)

        /// <summary>
        /// Resizes proportionally and optionally crops the center of an image.
        /// + Maintains aspect ratio for predictable output dimensions.
        /// - Allocates intermediate <see cref="Bitmap"/> instances.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.drawing.graphics"/>
        /// </summary>
        /// <param name="sourceContent">Binary content of the picture to resize/crop.</param>
        /// <param name="targetWidth">Width in pixels after resizing.</param>
        /// <param name="targetHeight">Height in pixels after resizing.</param>
        /// <param name="imageFormat">Format of the resulting image. Default: PNG.</param>
        /// <returns>
        /// Success : The resized/cropped image content.
        /// Failure : The image content could not be processed.
        /// Example JSON: { "isSuccess": true, "value": "iVBORw0KGgo..." }
        /// </returns>
        [SupportedOSPlatform("windows")]
        public Task<Result<byte[]>> ResizeProportionallyWithCenterCrop(byte[] sourceContent, int targetWidth, int targetHeight, ImageFormat imageFormat = null) => Result<byte[]>.SafeExecute(async () =>
        {
            await using var sourceStream = new MemoryStream(sourceContent);
            using var sourceImage = new Bitmap(sourceStream);

            // how many units are there to make the original length
            var widthRatio = (float)sourceImage.Width / targetWidth;
            var heightRatio = (float)sourceImage.Height / targetHeight;
            var ratio = Math.Min(widthRatio, heightRatio);

            // start cropping from the center
            var widthScaled = Convert.ToInt32(targetHeight * ratio);
            var heightScaled = Convert.ToInt32(targetWidth * ratio);
            var startX = (sourceImage.Width - widthScaled) / 2;
            var startY = (sourceImage.Height - heightScaled) / 2;

            // crop the image from the specified location and size
            var sourceRectangle = new Rectangle(startX, startY, widthScaled, heightScaled);
            var targetRectangle = new Rectangle(0, 0, targetWidth, targetHeight);
            using var resizedImage = new Bitmap(120, 120);
            using var graphics = Graphics.FromImage(resizedImage);
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.DrawImage(sourceImage, targetRectangle, sourceRectangle, GraphicsUnit.Pixel);

            await using var resizedStream = new MemoryStream();
            resizedImage.Save(resizedStream, imageFormat ?? ImageFormat.Png);
            var resizedImageContent = await resizedStream.ReadAllBytesAsync();

            return new Result<byte[]>(resizedImageContent);
        });

        #endregion Methods (Public)
    }
}