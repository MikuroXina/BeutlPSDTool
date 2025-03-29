using Beutl.Extensibility;
using Beutl.Services;

namespace BeutlPSDTool;

[Export]
public class BeutlPSDToolExtension : Extension
{
    public override string Name => "PSDTool loader for Beutl";
    public override string DisplayName => "PSDTool";

    public override void Load()
    {
        base.Load();

        LibraryService.Current.AddMultiple("PSD", m => m
            .BindSourceOperator<PSDToolOperator>()
            .BindDrawable<PSDToolObject>()
        );
    }
}
