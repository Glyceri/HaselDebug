using System.Numerics;
using Dalamud.Interface.ImGuiSeStringRenderer;
using Dalamud.Interface.Utility;
using HaselCommon.Services;
using HaselCommon.Services.SeStringEvaluation;
using HaselCommon.Windowing;
using HaselDebug.Services;
using HaselDebug.Utils;
using ImGuiNET;
using Lumina.Text.Payloads;
using Lumina.Text.ReadOnly;

namespace HaselDebug.Windows;

#pragma warning disable SeStringRenderer
public class SeStringInspectorWindow(
    WindowManager windowManager,
    DebugRenderer DebugRenderer,
    SeStringEvaluatorService SeStringEvaluator,
    ReadOnlySeString SeString,
    string windowName = "SeString") : SimpleWindow(windowManager, windowName)
{
    public override void OnOpen()
    {
        base.OnOpen();

        Size = new Vector2(800, 600);
        SizeConstraints = new()
        {
            MinimumSize = new Vector2(250, 250),
            MaximumSize = new Vector2(4096, 2160)
        };

        SizeCondition = ImGuiCond.Appearing;
        RespectCloseHotkey = true;
        DisableWindowSounds = true;
    }

    public override void Draw()
    {
        ImGuiHelpers.SeStringWrapped(SeStringEvaluator.Evaluate(SeString.AsSpan()), new SeStringDrawParams()
        {
            ForceEdgeColor = true,
        });

        ImGui.Spacing();
        ImGui.Separator();

        DebugRenderer.DrawSeString(SeString.AsSpan(), new NodeOptions()
        {
            RenderSeString = false,
            DefaultOpen = true
        });
    }
}
