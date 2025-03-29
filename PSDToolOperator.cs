using Beutl.Graphics;
using Beutl.Operation;
using Beutl.Graphics.Effects;
using Beutl.Graphics.Transformation;

namespace BeutlPSDTool;

public class PSDToolOperator() : PublishOperator<PSDToolObject>([
    PSDToolObject.PSDFileProperty,
    (Drawable.TransformProperty, () => new TransformGroup()),
    Drawable.AlignmentXProperty,
    Drawable.AlignmentYProperty,
    Drawable.TransformOriginProperty,
    (Drawable.FilterEffectProperty, () => new FilterEffectGroup()),
    Drawable.BlendModeProperty,
    Drawable.OpacityProperty
]);
