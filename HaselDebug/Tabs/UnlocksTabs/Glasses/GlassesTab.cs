using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using HaselDebug.Abstracts;
using HaselDebug.Interfaces;

namespace HaselDebug.Tabs.UnlocksTabs.Glasses;

[RegisterSingleton<IUnlockTab>(Duplicate = DuplicateStrategy.Append)]
public unsafe class GlassesTab(GlassesTable table) : DebugTab, IUnlockTab
{
    public override string Title => "Glasses";
    public override bool DrawInChild => false;

    public UnlockProgress GetUnlockProgress()
    {
        if (table.Rows.Count == 0)
            table.LoadRows();

        return new UnlockProgress()
        {
            TotalUnlocks = table.Rows.Count,
            NumUnlocked = table.Rows.Count(row => PlayerState.Instance()->IsGlassesUnlocked((ushort)row.RowId)),
        };
    }

    public override void Draw()
    {
        table.Draw();
    }
}
