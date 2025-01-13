using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using HaselCommon.Graphics;
using HaselCommon.Gui.ImGuiTable;
using HaselDebug.Services;
using HaselDebug.Utils;
using ImGuiNET;

namespace HaselDebug.Tabs.UnlocksTabs.AchievementsTableColumns;

public class NameColumn(DebugRenderer debugRenderer, UnlocksTabUtils unlocksTabUtils) : ColumnString<AchievementEntry>
{
    public override string ToName(AchievementEntry entry)
    {
        return entry.Name;
    }

    public override unsafe void DrawColumn(AchievementEntry entry)
    {
        debugRenderer.DrawIcon(entry.Row.Icon);

        var canClick = entry.CanShowName && entry.CanShowCategory;
        var clicked = false;
        using (Color.Transparent.Push(ImGuiCol.HeaderActive, !canClick))
        using (Color.Transparent.Push(ImGuiCol.HeaderHovered, !canClick))
            clicked = ImGui.Selectable(entry.Name);

        if (canClick && clicked)
            AgentAchievement.Instance()->OpenById(entry.Row.RowId);

        if (ImGui.IsItemHovered())
        {
            if (canClick)
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);

            unlocksTabUtils.DrawTooltip(entry.Row.Icon, entry.Name, entry.CategoryName, entry.Description);
        }
    }
}
