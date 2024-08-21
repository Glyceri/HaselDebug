using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game;
using Dalamud.Interface.ImGuiSeStringRenderer;
using Dalamud.Interface.Utility;
using HaselCommon.Services;
using HaselCommon.Services.SeStringEvaluation;
using HaselCommon.Utils;
using HaselCommon.Windowing;
using HaselDebug.Services;
using HaselDebug.Utils;
using ImGuiNET;
using Lumina.Text.Expressions;
using Lumina.Text.ReadOnly;

namespace HaselDebug.Windows;

#pragma warning disable SeStringRenderer
public class SeStringInspectorWindow(
    WindowManager windowManager,
    DebugRenderer DebugRenderer,
    SeStringEvaluatorService SeStringEvaluator,
    ReadOnlySeString SeString,
    ClientLanguage Language,
    string windowName = "SeString") : SimpleWindow(windowManager, windowName)
{
    private SeStringParameter[]? LocalParameters = null;

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

        Flags |= ImGuiWindowFlags.NoSavedSettings;

        RespectCloseHotkey = true;
        DisableWindowSounds = true;
    }

    public override void OnClose()
    {
        base.OnClose();

        if (ImGui.IsKeyDown(ImGuiKey.LeftShift))
            WindowManager.Close<SeStringInspectorWindow>();
    }

    public override void Draw()
    {
        LocalParameters ??= GetLocalParameters(SeString.AsSpan(), []);

        DrawPreview();

        if (LocalParameters.Length != 0)
        {
            ImGui.Spacing();
            DrawParameters();
        }

        ImGui.Spacing();
        DrawPayloads();
    }

    private void DrawPreview()
    {
        using var node = DebugRenderer.DrawTreeNode(new NodeOptions() { AddressPath = new(1), Title = "Preview", TitleColor = Colors.Green, DefaultOpen = true });
        if (!node) return;

        var evaluated = SeStringEvaluator.Evaluate(SeString.AsSpan(), new()
        {
            LocalParameters = LocalParameters ?? [],
            Language = Language
        });

        ImGui.Dummy(new Vector2(0, ImGui.GetTextLineHeight()));
        ImGui.SameLine(0, 0);
        ImGuiHelpers.SeStringWrapped(evaluated, new SeStringDrawParams()
        {
            ForceEdgeColor = true,
        });
    }

    private void DrawParameters()
    {
        using var node = DebugRenderer.DrawTreeNode(new NodeOptions() { AddressPath = new(2), Title = "Parameters", TitleColor = Colors.Green, DefaultOpen = true });
        if (!node) return;

        for (var i = 0; i < LocalParameters!.Length; i++)
        {
            if (LocalParameters[i].IsString)
            {
                var str = LocalParameters[i].StringValue.ExtractText();
                if (ImGui.InputText($"lstr({i + 1})", ref str, 255))
                {
                    LocalParameters[i] = new(str);
                }
            }
            else
            {
                var num = (int)LocalParameters[i].UIntValue;
                if (ImGui.InputInt($"lnum({i + 1})", ref num))
                {
                    LocalParameters[i] = new((uint)num);
                }
            }
        }
    }

    private void DrawPayloads()
    {
        using var node = DebugRenderer.DrawTreeNode(new NodeOptions() { AddressPath = new(3), Title = "Payloads", TitleColor = Colors.Green, DefaultOpen = true });
        if (!node) return;

        DebugRenderer.DrawSeString(SeString.AsSpan(), false, new NodeOptions()
        {
            RenderSeString = false,
            DefaultOpen = true
        });
    }

    private static SeStringParameter[] GetLocalParameters(ReadOnlySeStringSpan rosss, Dictionary<uint, SeStringParameter>? parameters)
    {
        parameters ??= [];

        foreach (var payload in rosss)
        {
            foreach (var expression in payload)
            {
                if (expression.TryGetString(out var exprString))
                {
                    GetLocalParameters(exprString, parameters);
                    continue;
                }

                if (!expression.TryGetParameterExpression(out var expressionType, out var operand))
                    continue;

                if (!operand.TryGetUInt(out var index))
                    continue;

                if (parameters.ContainsKey(index))
                    continue;

                if (expressionType == (int)ExpressionType.LocalNumber)
                {
                    parameters[index] = new SeStringParameter(0);
                }
                else if (expressionType == (int)ExpressionType.LocalString)
                {
                    parameters[index] = new SeStringParameter("");
                }
            }
        }

        if (parameters.Count > 0)
        {
            var last = parameters.OrderBy(x => x.Key).Last();

            if (parameters.Count != last.Key)
            {
                // fill missing local parameter slots, so we can go off the array index in SeStringContext

                for (var i = 1u; i <= last.Key; i++)
                {
                    if (!parameters.ContainsKey(i))
                        parameters[i] = new SeStringParameter(0);
                }
            }
        }

        return parameters.OrderBy(x => x.Key).Select(x => x.Value).ToArray();
    }
}
