using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using HaselDebug.Abstracts;
using HaselDebug.Interfaces;

namespace HaselDebug.Tabs.UnlocksTabs.UnlockLinks;

[RegisterSingleton<IUnlockTab>(Duplicate = DuplicateStrategy.Append)]
public unsafe class UnlockLinksTab(UnlockLinksTable table) : DebugTab, IUnlockTab
{
    public override string Title => "Unlock Links";
    public override bool DrawInChild => false;

    public UnlockProgress GetUnlockProgress()
    {
        if (table.Rows.Count == 0)
            table.LoadRows();

        return new UnlockProgress()
        {
            TotalUnlocks = table.Rows.Count,
            NumUnlocked = table.Rows.Count(entry => UIState.Instance()->IsUnlockLinkUnlocked((ushort)entry.Index)),
        };
    }

    public override void Draw()
    {
        table.Draw();
    }
}
