using System.Buffers.Binary;
using Aspose.PSD;
using Aspose.PSD.FileFormats.Psd;
using Aspose.PSD.FileFormats.Psd.Layers;

namespace BeutlPSDTool;

public class PSDToolAsset
{
    private readonly PsdImage psd;
    private readonly List<IPSDToolLayer> children = [];

    public PSDToolAsset(string path)
    {
        var format = Image.GetFileFormat(path);
        if (format != FileFormat.Psd)
        {
            throw new Exception("file at the path is not a PSD");
        }
        psd = (PsdImage)Image.Load(path);
        foreach (var layer in psd.Layers)
        {
            if (layer is LayerGroup group)
            {
                children.Add(new PSDToolLayerGroup(group));
            }
            else
            {
                children.Add(new PSDToolLayerImage(layer));
            }
        }
    }

    public Size ImageSize()
    {
        return psd.Size;
    }

    public int[] Bgra8888Pixels()
    {
        var argb = psd.LoadArgb32Pixels(psd.Bounds);
        var ret = new int[psd.Size.Width * psd.Size.Height];
        for (int i = 0; i < psd.Size.Height; ++i)
        {
            for (int j = 0; j < psd.Size.Width; ++j)
            {
                var index = i * psd.Size.Width + j;
                ret[index] = BinaryPrimitives.ReverseEndianness(argb[index]);
            }
        }
        return ret;
    }
}

public interface IPSDToolLayer
{
    string Name { get; }
    bool CanBeToggled { get; }
    bool IsRadio { get; }
    bool IsVisible { get; set; }
}

public class PSDToolLayerGroup : IPSDToolLayer
{
    private readonly LayerGroup group;
    private readonly List<IPSDToolLayer> children = [];

    public string Name { get => group.Name; }
    public bool CanBeToggled { get => !group.Name.StartsWith('!'); }
    public bool IsRadio { get => group.Name.StartsWith('*'); }
    public bool IsVisible
    {
        get => group.IsVisible;
        set
        {
            if (CanBeToggled)
            {
                group.IsVisible = value;
            }
            else
            {
                group.IsVisible = true;
            }
        }
    }

    public PSDToolLayerGroup(LayerGroup group)
    {
        this.group = group;
        foreach (var layer in group.Layers)
        {
            if (layer is LayerGroup subGroup)
            {
                children.Add(new PSDToolLayerGroup(subGroup));
            }
            else
            {
                children.Add(new PSDToolLayerImage(layer));
            }
        }
        if (!CanBeToggled)
        {
            IsVisible = true;
        }
    }

    public void Toggle(int index)
    {
        var item = children[index];
        if (item.IsVisible)
        {
            item.IsVisible = false;
        }
        foreach (var child in children)
        {
            child.IsVisible = false;
        }
        item.IsVisible = true;
    }
}

public class PSDToolLayerImage : IPSDToolLayer
{
    private readonly Layer layer;

    public PSDToolLayerImage(Layer layer)
    {
        this.layer = layer;
        if (!CanBeToggled)
        {
            IsVisible = true;
        }
    }

    public string Name { get => layer.Name; }
    public bool CanBeToggled { get => !layer.Name.StartsWith('!'); }
    public bool IsRadio { get => layer.Name.StartsWith('*'); }
    public bool IsVisible
    {
        get => layer.IsVisible;
        set
        {
            if (CanBeToggled)
            {
                layer.IsVisible = value;
            }
            else
            {
                layer.IsVisible = true;
            }
        }
    }
}
