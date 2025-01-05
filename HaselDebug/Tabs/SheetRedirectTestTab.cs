using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using HaselCommon.Graphics;
using HaselCommon.Services;
using HaselDebug.Abstracts;
using ImGuiNET;
using InteropGenerator.Runtime.Attributes;

namespace HaselDebug.Tabs;

[GenerateInterop]
public unsafe partial struct HaselRaptureTextModule
{
    public static HaselRaptureTextModule* Instance() => (HaselRaptureTextModule*)RaptureTextModule.Instance();

    [MemberFunction("E8 ?? ?? ?? ?? 44 8B E8 A8 10")]
    public partial uint ResolveSheetRedirect(
        Utf8String* sheetName,
        uint* rowId,
        ushort* flags);
}

public unsafe partial class SheetRedirectTestTab(SeStringEvaluatorService seStringEvaluator) : DebugTab
{
    public override bool DrawInChild => false;

    public override void Draw()
    {
        using var table = ImRaii.Table("SheetRedirectTestTable", 4, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollY);
        if (!table) return;

        ImGui.TableSetupColumn("Input", ImGuiTableColumnFlags.WidthFixed, 200);
        ImGui.TableSetupColumn("Game", ImGuiTableColumnFlags.WidthFixed, 200);
        ImGui.TableSetupColumn("Mine", ImGuiTableColumnFlags.WidthFixed, 200);
        ImGui.TableSetupColumn("Matches", ImGuiTableColumnFlags.WidthFixed, 50);
        ImGui.TableSetupScrollFreeze(0, 1);
        ImGui.TableHeadersRow();

        PrintRedirect("Item", 10);
        PrintRedirect("ItemHQ", 10);
        PrintRedirect("ItemMP", 10);
        PrintRedirect("Item", 35588);
        PrintRedirect("Item", 1035588);
        PrintRedirect("Item", 2000217);
        PrintRedirect("Item", 3000217);
        PrintRedirect("ActStr", 10);       // Trait
        PrintRedirect("ActStr", 1000010);  // Action
        PrintRedirect("ActStr", 2000010);  // Item
        PrintRedirect("ActStr", 3000010);  // EventItem
        PrintRedirect("ActStr", 4000010);  // EventAction
        PrintRedirect("ActStr", 5000010);  // GeneralAction
        PrintRedirect("ActStr", 6000010);  // BuddyAction
        PrintRedirect("ActStr", 7000010);  // MainCommand
        PrintRedirect("ActStr", 8000010);  // Companion
        PrintRedirect("ActStr", 9000010);  // CraftAction
        PrintRedirect("ActStr", 10000010); // Action
        PrintRedirect("ActStr", 11000010); // PetAction
        PrintRedirect("ActStr", 12000010); // CompanyAction
        PrintRedirect("ActStr", 13000010); // Mount
        // PrintRedirect("ActStr", 14000010);
        // PrintRedirect("ActStr", 15000010);
        // PrintRedirect("ActStr", 16000010);
        // PrintRedirect("ActStr", 17000010);
        // PrintRedirect("ActStr", 18000010);
        PrintRedirect("ActStr", 19000010); // BgcArmyAction
        PrintRedirect("ActStr", 20000010); // Ornament
        PrintRedirect("ObjStr", 10);       // BNpcName
        PrintRedirect("ObjStr", 1000010);  // ENpcResident
        PrintRedirect("ObjStr", 2000010);  // Treasure
        PrintRedirect("ObjStr", 3000010);  // Aetheryte
        PrintRedirect("ObjStr", 4000010);  // GatheringPointName
        PrintRedirect("ObjStr", 5000010);  // EObjName
        PrintRedirect("ObjStr", 6000010);  // Mount
        PrintRedirect("ObjStr", 7000010);  // Companion
        // PrintRedirect("ObjStr", 8000010);
        // PrintRedirect("ObjStr", 9000010);
        PrintRedirect("ObjStr", 10000010); // Item
        PrintRedirect("EObj", 2003479); // EObj => EObjName
        PrintRedirect("Treasure", 1473); // Treasure (without name, falls back to rowId 0)
        PrintRedirect("Treasure", 1474); // Treasure (with name)
        PrintRedirect("WeatherPlaceName", 0);
        PrintRedirect("WeatherPlaceName", 28);
        PrintRedirect("WeatherPlaceName", 40);
        PrintRedirect("WeatherPlaceName", 52);
        PrintRedirect("WeatherPlaceName", 2300);
    }

    private void PrintRedirect(string sheetName, uint rowId)
    {
        var sheetName1 = Utf8String.FromString(sheetName);
        try
        {
            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.TextUnformatted($"{sheetName}#{rowId}");

            ImGui.TableNextColumn();
            var rowId1 = rowId;
            ushort flags = 0xFFFF;
            HaselRaptureTextModule.Instance()->ResolveSheetRedirect(sheetName1, &rowId1, &flags);
            ImGui.TextUnformatted($"{sheetName1->ToString()}#{rowId1}");

            ImGui.TableNextColumn();
            var sheetName2 = sheetName;
            var rowId2 = rowId;
            seStringEvaluator.ResolveSheetRedirect(ref sheetName2, ref rowId2);
            ImGui.TextUnformatted($"{sheetName2}#{rowId2}");

            ImGui.TableNextColumn();
            var matches = sheetName1->ToString() == sheetName2 && rowId1 == rowId2;
            using (ImRaii.PushColor(ImGuiCol.Text, (uint)(matches ? Color.Green : Color.Red)))
                ImGui.TextUnformatted(matches.ToString());
        }
        finally
        {
            sheetName1->Dtor(true);
        }
    }
}
