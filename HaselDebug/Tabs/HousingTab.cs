using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.Game;
using HaselCommon.Services;
using HaselDebug.Abstracts;
using HaselDebug.Services;
using HaselDebug.Utils;
using ImGuiNET;
using Lumina.Excel.Sheets;

namespace HaselDebug.Tabs;

public unsafe class HousingTab(DebugRenderer DebugRenderer, LanguageProvider LanguageProvider) : DebugTab
{
    public override void Draw()
    {
        var housingManager = HousingManager.Instance();
        if (housingManager == null)
        {
            ImGui.TextUnformatted("HousingManager unavailable");
            return;
        }

        long houseId = 0;

        switch (housingManager->GetCurrentHousingTerritoryType())
        {
            case HousingTerritoryType.Outdoor:
                houseId = housingManager->OutdoorTerritory->HouseId;
                break;
            case HousingTerritoryType.Indoor:
                houseId = housingManager->IndoorTerritory->HouseId;
                break;
            case HousingTerritoryType.Workshop:
                houseId = housingManager->WorkshopTerritory->HouseId;
                break;
        }

        if (houseId != 0)
        {
            ImGui.TextUnformatted($"Current HouseId ({housingManager->GetCurrentHousingTerritoryType()})");
            ImGui.SameLine();
            DebugRenderer.DrawCopyableText($"{houseId}");
            ImGui.SameLine();
            DebugRenderer.DrawCopyableText($"0x{houseId:X}");
        }

        using (var node = ImRaii.TreeNode("Owned HouseIds", ImGuiTreeNodeFlags.SpanAvailWidth))
        {
            if (node)
            {
                using var table = ImRaii.Table("OwnedHouseIdsTable", 3, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg);
                if (table)
                {
                    ImGui.TableSetupColumn("EstateType", ImGuiTableColumnFlags.WidthFixed, 160);
                    ImGui.TableSetupColumn("HouseId", ImGuiTableColumnFlags.WidthFixed, 160);
                    ImGui.TableSetupColumn("HouseId (Hex)");
                    ImGui.TableHeadersRow();

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.TextUnformatted("FreeCompanyEstate");
                    ImGui.TableNextColumn();
                    DebugRenderer.DrawCopyableText($"{HousingManager.GetOwnedHouseId(EstateType.FreeCompanyEstate)}");
                    ImGui.TableNextColumn();
                    DebugRenderer.DrawCopyableText($"0x{HousingManager.GetOwnedHouseId(EstateType.FreeCompanyEstate):X}");

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.TextUnformatted("PersonalChambers");
                    ImGui.TableNextColumn();
                    DebugRenderer.DrawCopyableText($"{HousingManager.GetOwnedHouseId(EstateType.PersonalChambers)}");
                    ImGui.TableNextColumn();
                    DebugRenderer.DrawCopyableText($"0x{HousingManager.GetOwnedHouseId(EstateType.PersonalChambers):X}");

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.TextUnformatted("PersonalEstate");
                    ImGui.TableNextColumn();
                    DebugRenderer.DrawCopyableText($"{HousingManager.GetOwnedHouseId(EstateType.PersonalEstate)}");
                    ImGui.TableNextColumn();
                    DebugRenderer.DrawCopyableText($"0x{HousingManager.GetOwnedHouseId(EstateType.PersonalEstate):X}");

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.TextUnformatted("Unknown3");
                    ImGui.TableNextColumn();
                    DebugRenderer.DrawCopyableText($"{HousingManager.GetOwnedHouseId(EstateType.Unknown3)}");
                    ImGui.TableNextColumn();
                    DebugRenderer.DrawCopyableText($"0x{HousingManager.GetOwnedHouseId(EstateType.Unknown3):X}");

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.TextUnformatted("SharedEstate 0");
                    ImGui.TableNextColumn();
                    DebugRenderer.DrawCopyableText($"{HousingManager.GetOwnedHouseId(EstateType.SharedEstate, 0)}");
                    ImGui.TableNextColumn();
                    DebugRenderer.DrawCopyableText($"0x{HousingManager.GetOwnedHouseId(EstateType.SharedEstate, 0):X}");

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.TextUnformatted("SharedEstate 1");
                    ImGui.TableNextColumn();
                    DebugRenderer.DrawCopyableText($"{HousingManager.GetOwnedHouseId(EstateType.SharedEstate, 1)}");
                    ImGui.TableNextColumn();
                    DebugRenderer.DrawCopyableText($"0x{HousingManager.GetOwnedHouseId(EstateType.SharedEstate, 1):X}");

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.TextUnformatted("ApartmentBuilding");
                    ImGui.TableNextColumn();
                    DebugRenderer.DrawCopyableText($"{HousingManager.GetOwnedHouseId(EstateType.ApartmentBuilding)}");
                    ImGui.TableNextColumn();
                    DebugRenderer.DrawCopyableText($"0x{HousingManager.GetOwnedHouseId(EstateType.ApartmentBuilding):X}");

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.TextUnformatted("ApartmentRoom");
                    ImGui.TableNextColumn();
                    DebugRenderer.DrawCopyableText($"{HousingManager.GetOwnedHouseId(EstateType.ApartmentRoom)}");
                    ImGui.TableNextColumn();
                    DebugRenderer.DrawCopyableText($"0x{HousingManager.GetOwnedHouseId(EstateType.ApartmentRoom):X}");
                }
            }
        }

        var territoryTypeId = HousingManager.GetOriginalHouseTerritoryTypeId();
        ImGui.TextUnformatted($"OriginalHouseTerritoryTypeId:");
        ImGui.SameLine();
        DebugRenderer.DrawExdSheet(typeof(TerritoryType), territoryTypeId, 0, new NodeOptions()
        {
            Language = LanguageProvider.ClientLanguage
        });
        ImGui.Separator();

        DebugRenderer.DrawPointerType(housingManager, typeof(HousingManager), new NodeOptions() { DefaultOpen = true });
    }
}
