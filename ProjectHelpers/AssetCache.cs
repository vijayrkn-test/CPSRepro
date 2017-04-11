using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ProjectHelpers
{
    public class AssetCache
    {
        private static readonly ConcurrentDictionary<string, BitmapSource> Cache = new ConcurrentDictionary<string, BitmapSource>(StringComparer.Ordinal);

        public static ImageSource Get(Expression<Func<Bitmap>> memberAccess)
        {
            BitmapSource image = GetInternal(memberAccess);
            return image;
        }

        public static ImageSource Get(Expression<Func<Bitmap>> memberAccess, int sizeX, int sizeY)
        {
            BitmapSource image = GetInternal(memberAccess);

            if (image != null)
            {
                if (sizeX != (int)image.Width || sizeY != (int)image.Height)
                {
                    TransformedBitmap transformed = new TransformedBitmap(image, new ScaleTransform
                    {
                        CenterX = image.Width / 2,
                        CenterY = image.Height / 2,
                        ScaleX = sizeX / image.Width,
                        ScaleY = sizeY / image.Height
                    });

                    transformed.SetValue(RenderOptions.BitmapScalingModeProperty, BitmapScalingMode.Fant);
                    return transformed;
                }
            }

            return image;
        }

        private static BitmapSource GetInternal(Expression<Func<Bitmap>> memberAccess)
        {
            if (memberAccess == null)
            {
                return null;
            }

            MemberExpression expr = memberAccess.Body as MemberExpression;

            if (expr == null)
            {
                return null;
            }

            string key = string.Concat(expr.Type.FullName, "::", expr.Member.Name);
            return Cache.GetOrAdd(key, k => CreateFrozenAsset(memberAccess));
        }

        private static BitmapSource CreateFrozenAsset(Expression<Func<Bitmap>> expr)
        {
            Bitmap b = expr.Compile()();
            BitmapSource source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(b.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            source.SetValue(RenderOptions.BitmapScalingModeProperty, BitmapScalingMode.Fant);

            if (source.CanFreeze)
            {
                source.Freeze();
            }

            return source;
        }
    }
}
