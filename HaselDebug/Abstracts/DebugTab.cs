using System.Text.RegularExpressions;

namespace HaselDebug.Abstracts;

public abstract partial class DebugTab : IDebugTab
{
    private string? _title = null;
    public virtual string GetTitle() => _title ??= NameRegex().Replace(TabRegex().Replace(GetType().Name, ""), "$1 $2");
    public virtual bool DrawInChild => true;

    [GeneratedRegex("Tab$")]
    private static partial Regex TabRegex();

    [GeneratedRegex("([a-z])([A-Z])")]
    private static partial Regex NameRegex();

    public virtual void SetupVTableHooks() { }
    public virtual void Draw() { }

    public bool Equals(IDebugTab? other)
    {
        return other?.GetTitle() == _title;
    }
}
