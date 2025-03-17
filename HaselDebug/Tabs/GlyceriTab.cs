using System.Numerics;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.UI.Arrays;
using HaselDebug.Abstracts;
using HaselDebug.Interfaces;
using HaselDebug.Services;
using HaselDebug.Utils;
using ImGuiNET;
using static FFXIVClientStructs.FFXIV.Client.UI.Arrays.ActionBarNumberArray;
using static FFXIVClientStructs.FFXIV.Client.UI.Arrays.ActionBarNumberArray.ActionBarBarNumberArray;
using static FFXIVClientStructs.FFXIV.Client.UI.Arrays.InventoryNumberArray;
using static FFXIVClientStructs.FFXIV.Client.UI.Arrays.LetterNumberArray;
using static FFXIVClientStructs.FFXIV.Client.UI.Arrays.PartyListNumberArray;


namespace HaselDebug.Tabs;

[RegisterSingleton<IDebugTab>(Duplicate = DuplicateStrategy.Append), AutoConstruct]
public unsafe partial class GlyceriTab : DebugTab
{
    private readonly DebugRenderer _debugRenderer;

    int index = 0;
    string setMemeberName = "Member";
    int memIndex = 0;

    public override void Draw()
    {
        using var tabBar = ImRaii.TabBar("ExcelTabs");
        if (!tabBar) return;

        index = 0;

        DrawPartyListTab();
        DrawInventoryTab();
        DrawHotbarTab();
        DrawHud2Tab();
        DrawLetterTab();
        DrawCastbarTab();
    }

    void DrawCastbarTab()
    {
        using var tab = ImRaii.TabItem("Castbar" + "###CastbarTab");
        if (!tab) return;

        _debugRenderer.DrawPointerType(CastBarStringArray.Instance(), typeof(CastBarStringArray), new NodeOptions() { DefaultOpen = false });
        _debugRenderer.DrawPointerType(CastBarNumberArray.Instance(), typeof(CastBarNumberArray), new NodeOptions() { DefaultOpen = false });
    }

    void DrawLetterTab()
    {
        using var tab = ImRaii.TabItem("Letter" + "###LetterTab");
        if (!tab) return;

        ImGui.TextUnformatted($"Total Letters: {LetterNumberArray.Instance()->LettersInMailbox}");
        ImGui.TextUnformatted($"Letters From GameMasters: {LetterNumberArray.Instance()->LettersFromGameMasters}");
        ImGui.TextUnformatted($"Letters From Store: {LetterNumberArray.Instance()->LettersFromStore}");
        ImGui.TextUnformatted($"Unclaimed Goods: {LetterNumberArray.Instance()->UnclaimedGoods}");
        ImGui.TextUnformatted($"Can Speed Up Store Process: {LetterNumberArray.Instance()->CanSpeedUpStoreProcess}");
        ImGui.TextUnformatted($"Client Occupied Sending Letter: {LetterNumberArray.Instance()->ClientOccupiedSendingLetter}");
        ImGui.TextUnformatted($"Client Acquiring Letters: {LetterNumberArray.Instance()->ClientAcquiringLetters}");

        for (int i = 0; i < LetterNumberArray.Instance()->LettersInMailbox; i++)
        {
            LetterLetterNumberArray letter = LetterNumberArray.Instance()->AllLetters[i];
            ImGui.TextUnformatted($"Letter: {letter.MessageStatus}, {letter.UnkFlag1}, {letter.UnkFlag2}, {letter.UnkFlag3}");
        }
    }

    void DrawHud2Tab()
    {
        using var tab = ImRaii.TabItem("Hud2" + "###hud2Tab");
        if (!tab) return;

        _debugRenderer.DrawPointerType(Hud2NumberArray.Instance(), typeof(Hud2NumberArray), new NodeOptions() { DefaultOpen = false });
    }

    void DrawHotbarTab()
    {
        using var tab = ImRaii.TabItem("Hotbars" + "###hotbarTab");
        if (!tab) return;

        ActionBarNumberArray* barArr = ActionBarNumberArray.Instance();
        if (barArr == null) return;

        ImGui.TextUnformatted($"Bar Locked: " + barArr->HotBarLocked.ToString());
        ImGui.TextUnformatted($"Pet Bar Visible: " + barArr->DisplayPetBar.ToString());

        int barIndex = 0;

        foreach (ActionBarBarNumberArray barbar in barArr->Bars)
        {
            ImGui.TextUnformatted($"Hotbar: {barIndex++}");
            ImGui.SameLine();

            foreach (ActionBarSlotNumberArray slot in barbar.Slots)
            {
                _debugRenderer.DrawIcon(slot.IconId);
                /*
                ImGui.TextUnformatted($"ActionId: " + slot.ActionId);
                ImGui.TextUnformatted($"Executable: " + slot.Executable); 
                ImGui.TextUnformatted($"Executable2: " + slot.Executable2);
                ImGui.TextUnformatted($"MaxActionStacks: " + slot.MaxActionStacks);
                ImGui.TextUnformatted($"GlobalCoolDownPercentage: " + slot.GlobalCoolDownPercentage);
                ImGui.TextUnformatted($"StackCoolDownPercentage: " + slot.StackCoolDownPercentage);
                ImGui.TextUnformatted($"RechargeTime: " + slot.RechargeTime);
                ImGui.TextUnformatted($"ManaCost: " + slot.ManaCost);
                ImGui.TextUnformatted($"DisplayDot: " + slot.DisplayDot);
                ImGui.TextUnformatted($"CurrentStackCount: " + slot.CurrentStackCount);
                ImGui.TextUnformatted($"Glows: " + slot.Glows);
                ImGui.TextUnformatted($"Pulses: " + slot.Pulses);
                ImGui.TextUnformatted($"InRange: " + slot.InRange);*/
            }
            ImGui.NewLine();
        }
    } 

    void DrawInventoryTab()
    {
        using var tab = ImRaii.TabItem("Inventory" + "###invAddonTab");
        if (!tab) return;

        InventoryNumberArray* data = InventoryNumberArray.Instance();
        if (data == null)
        {
            ImGui.TextUnformatted($"data null");

            return;
        }

        foreach (InventoryItemNumberArray item in data->Items)
        {
            ImGui.TextUnformatted(item.IconId.ToString());
            ImGui.SameLine();
            _debugRenderer.DrawIcon(item.IconId);
            ImGui.SameLine();
            ImGui.TextUnformatted(item.StackCount.ToString());
            ImGui.SameLine();
            ImGui.TextUnformatted($"[{item.ItemFlags.ItemType}, {item.ItemFlags.Wearable}, {item.ItemFlags.MirageFlag}]");
            ImGui.SameLine();
            ImGui.TextUnformatted($"[{item.DyeSlot0.R}, {item.DyeSlot0.G}, {item.DyeSlot0.B}, [{item.DyeSlot0.DyeFlags}]]");
            ImGui.SameLine();
            ImGui.TextUnformatted($"[{item.DyeSlot1.R}, {item.DyeSlot1.G}, {item.DyeSlot1.B}, [{item.DyeSlot1.DyeFlags}]]");
        }
    }

    void DrawColour(uint dyeValue)
    {
        byte red    = (byte)((dyeValue >> 24) & 0xFF); // Alpha channel (most significant byte)
        byte green  = (byte)((dyeValue >> 16) & 0xFF); // Red channel
        byte blue   = (byte)((dyeValue >> 8)  & 0xFF); // Green channel

        byte dyeFlags  = (byte)( dyeValue     & 0xFF); // Blue channel (least significant byte)

        // dyeFlags 0 = can be dyed
        // dyeFlags 1 = dye locked
        // dyeFlags 2 = dyed

        float valR = (red   / 255.0f);
        float valG = (green / 255.0f);
        float valB = (blue  / 255.0f);

        ImGui.ColorButton("##dyeButt", new Vector4(valR, valG, valB, 1));
    }

    void DrawPartyListTab()
    {
        using var tab = ImRaii.TabItem("PartyList" + "###AddonTab2");
        if (!tab) return;

        PartyListNumberArray* data = PartyListNumberArray.Instance();
        if (data == null) return;

        using (var table = ImRaii.Table($"PartyTable{index++}", 2, ImGuiTableFlags.RowBg | ImGuiTableFlags.Borders | ImGuiTableFlags.NoSavedSettings))
        {
            if (!table) return;

            ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthFixed, 200);
            ImGui.TableSetupColumn("Value", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupScrollFreeze(2, 1);
            ImGui.TableHeadersRow();

            DrawRow("IsCrossRealmParty", data->IsCrossRealmParty);
            DrawRow("PartyLeaderIndex", data->PartyLeaderIndex);
            DrawRow("PartyHasMembers", data->PartyHasMembers);
            DrawRow("HideWhenInSoloParty", data->HideWhenInSoloParty);
            DrawRow("PartyListCount", data->PartyListCount);
        }

        //DrawRow("Members:", string.Empty);

        setMemeberName = "Member";
        memIndex = 0;

        foreach (PartyListMemberNumberArray member in data->PartyMembers)
        {
            DrawPartyMemberArray(member);
        }

        //DrawRow("Trust:", string.Empty);
        setMemeberName = "Trust";
        memIndex = 0;

        foreach (PartyListMemberNumberArray member in data->TrustMembers)
        {
            DrawPartyMemberArray(member);
        }

        //DrawRow("Pets:", string.Empty);
        setMemeberName = "Pet";
        memIndex = 0;

        foreach (PartyListMemberNumberArray member in data->Pets)
        {
            DrawPartyMemberArray(member);
        }

    }

    void DrawPartyMemberArray(PartyListMemberNumberArray memberData)
    {
        try
        {
            if (!ImGui.CollapsingHeader($"{setMemeberName}: {memIndex++}")) return;

            using (var table = ImRaii.Table($"HOAGH{index++}", 2, ImGuiTableFlags.RowBg | ImGuiTableFlags.Borders | ImGuiTableFlags.NoSavedSettings))
            {
                if (!table) return;

                ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthFixed, 200);
                ImGui.TableSetupColumn("Value", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupScrollFreeze(2, 1);
                ImGui.TableHeadersRow();

                DrawRow("Level", memberData.Level);
                DrawRow("ClassIconId", memberData.ClassIconId);
                DrawRow("CurrentHealth", memberData.CurrentHealth);
                DrawRow("MaxHealth", memberData.MaxHealth);
                DrawRow("ShieldsPercentage", memberData.ShieldsPercentage);
                DrawRow("CurrentMana", memberData.CurrentMana);
                DrawRow("MaxMana", memberData.MaxMana);
                DrawRow("EnmityPercent", memberData.EnmityPercent);
                DrawRow("EnmityLevel", memberData.EnmityLevel);
                DrawRow("StatusCount", memberData.StatusCount);


                for (int i = 0; i < memberData.StatusCount; i++)
                {
                    int iconId = memberData.StatusIconIds[i];
                    bool dispellable = memberData.StatusIsDispellable[i];

                    DrawStatus(iconId, dispellable);
                }

                DrawRow("CastTime", memberData.CastTime);
                DrawRow("CastId", memberData.CastId);
                DrawRow("ContentId", memberData.ContentId);
                DrawRow("Targetable", memberData.Targetable);
            }
        }
        catch(Exception e)
        {
            
        }
    }

    void DrawStatus(int iconId, bool dispellable)
    {
        DrawRow("Status", $"IconId: {iconId}, Dispellable: {dispellable}");
    }

    void DrawRow(string name, object value)
    {
        ImGui.TableNextRow();

        ImGui.TableNextColumn(); // RowId
        ImGui.TextUnformatted(name);

        ImGui.TableNextColumn(); // Text
        ImGui.TextUnformatted($"{value}");
    }
}
