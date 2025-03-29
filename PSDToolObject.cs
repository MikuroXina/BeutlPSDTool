using System.ComponentModel;
using Beutl;
using Beutl.Graphics;
using Beutl.Graphics.Rendering;
using Beutl.Media;
using Beutl.Media.Pixel;
using Beutl.Media.Source;

namespace BeutlPSDTool;

public class PSDToolObject : Drawable
{
    public static readonly CoreProperty<FileInfo> PSDFileProperty;

    static PSDToolObject()
    {
        PSDFileProperty = ConfigureProperty<FileInfo, PSDToolObject>(nameof(PSDFile)).Register();
        AffectsRender<PSDToolObject>(PSDFileProperty);
    }

    public FileInfo PSDFile
    {
        get => GetValue(PSDFileProperty);
        set => SetValue(PSDFileProperty, value);
    }
    private PSDToolAsset? asset = null;
    private Bgra8888[]? cache = null;

    protected override void OnPropertyChanged(PropertyChangedEventArgs args)
    {
        base.OnPropertyChanged(args);

        bool isDirty = false;
        if (args is CorePropertyChangedEventArgs<FileInfo> typedArgs && typedArgs.Property == PSDFileProperty)
        {
            if (typedArgs.NewValue == null)
            {
                asset = null;
                return;
            }
            asset = new PSDToolAsset(typedArgs.NewValue.FullName);
            isDirty = true;
        }

        if (isDirty && asset != null)
        {
            var size = asset.ImageSize();
            var pixels = asset.Bgra8888Pixels();
            cache = [.. pixels.Select(px => new Bgra8888(
                (byte)(px >> 24),
                (byte)(px >> 16),
                (byte)(px >> 8),
                (byte)px
            ))];
        }
    }

    protected override Size MeasureCore(Size _)
    {
        if (asset == null)
        {
            return new Size();
        }
        var size = asset.ImageSize();
        return new Size(size.Width, size.Height);
    }

    protected override void OnDraw(GraphicsContext2D context)
    {
        if (asset == null || cache == null)
        {
            return;
        }

        var size = asset.ImageSize();
        unsafe
        {
            fixed (Bgra8888* head = &cache[0])
            {
                var bitmap = new Bitmap<Bgra8888>(size.Width, size.Height, head);
                var source = new BitmapSource(Ref<IBitmap>.Create(bitmap), "");
                context.DrawImageSource(source, null, null);
            }
        }
    }
}
