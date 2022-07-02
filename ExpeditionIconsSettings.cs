using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore.Shared.Attributes;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using ImGuiNET;
using Newtonsoft.Json;
using SharpDX;
using Vector4 = System.Numerics.Vector4;

namespace ExpeditionIcons;

public class ExpeditionIconsSettings : ISettings
{
    public const MapIconsIndex DefaultBadModsIcon = MapIconsIndex.RedFlag;
    public const MapIconsIndex DefaultEliteMonsterIcon = MapIconsIndex.HeistSpottedMiniBoss;

    internal IntPtr _iconsImageId;
    private IconPickerIndex? _shownIconPicker;
    private string _iconFilter = "";
    public int IconPickerSize = 20;
    public int IconsPerRow = 15;
    public Dictionary<IconPickerIndex, IconDisplaySettings> IconMapping = new Dictionary<IconPickerIndex, IconDisplaySettings>();

    public ExpeditionIconsSettings()
    {
        GoodModsIconPicker = new CustomNode
        {
            DrawDelegate = () =>
            {
                foreach (var expeditionMarkerIconDescription in Icons.ExpeditionRelicWorldIcons)
                {
                    ImGui.PushID($"IconLine{expeditionMarkerIconDescription.IconPickerIndex}");
                    PickIcon(expeditionMarkerIconDescription.IconPickerIndex, expeditionMarkerIconDescription.DefaultIcon);
                    ImGui.PopID();
                }
            }
        };
        DrawEliteMonstersInWorld = new CustomNode
        {
            DrawDelegate = () => { PickIcon(IconPickerIndex.EliteMonstersWorldIndicator, DefaultEliteMonsterIcon); }
        };
        DrawEliteMonstersOnMap = new CustomNode
        {
            DrawDelegate = () => { PickIcon(IconPickerIndex.EliteMonstersMapIndicator, DefaultEliteMonsterIcon); }
        };
        DrawBadModsInWorld = new CustomNode
        {
            DrawDelegate = () => { PickIcon(IconPickerIndex.BadModsWorldIndicator, DefaultBadModsIcon); }
        };
        DrawBadModsOnMap = new CustomNode
        {
            DrawDelegate = () => { PickIcon(IconPickerIndex.BadModsMapIndicator, DefaultBadModsIcon); }
        };
    }

    public ToggleNode Enable { get; set; } = new ToggleNode(false);

    [JsonIgnore]
    public CustomNode DrawEliteMonstersInWorld { get; set; }

    [JsonIgnore]
    public CustomNode DrawEliteMonstersOnMap { get; set; }

    public ToggleNode DrawChestsInWorld { get; set; } = new ToggleNode(true);
    public ToggleNode DrawChestsOnMap { get; set; } = new ToggleNode(true);
    public ToggleNode CacheEntityPosition { get; set; } = new ToggleNode(true);
    public RangeNode<int> WorldIconSize { get; set; } = new RangeNode<int>(50, 25, 200);
    public RangeNode<int> MapIconSize { get; set; } = new RangeNode<int>(30, 15, 200);

    [Menu("Good mods", 100)]
    [JsonIgnore]
    public EmptyNode SettingsEmptyGood { get; set; }

    [Menu(null, parentIndex = 100)]
    public ToggleNode DrawGoodModsOnMap { get; set; } = new ToggleNode(true);

    [Menu(null, parentIndex = 100)]
    public ToggleNode DrawGoodModsInWorld { get; set; } = new ToggleNode(true);

    [JsonIgnore]
    [Menu(null, parentIndex = 100)]
    public CustomNode GoodModsIconPicker { get; }

    [Menu("Bad mods", 101)]
    [JsonIgnore]
    public EmptyNode SettingsEmptyBad { get; set; }

    [Menu(null, parentIndex = 101)]
    [JsonIgnore]
    public CustomNode DrawBadModsOnMap { get; }

    [Menu(null, parentIndex = 101)]
    [JsonIgnore]
    public CustomNode DrawBadModsInWorld { get; }

    [Menu("Warn for physical immune", parentIndex = 101)]
    public ToggleNode WarnPhysImmune { get; set; } = new ToggleNode(false);

    [Menu("Warn for fire immune", parentIndex = 101)]
    public ToggleNode WarnFireImmune { get; set; } = new ToggleNode(false);

    [Menu("Warn for ignite immune", parentIndex = 101)]
    public ToggleNode WarnIgniteImmune { get; set; } = new ToggleNode(false);

    [Menu("Warn for cold immune", parentIndex = 101)]
    public ToggleNode WarnColdImmune { get; set; } = new ToggleNode(false);

    [Menu("Warn for lightning immune", parentIndex = 101)]
    public ToggleNode WarnLightningImmune { get; set; } = new ToggleNode(false);

    [Menu("Warn for chaos immune", parentIndex = 101)]
    public ToggleNode WarnChaosImmune { get; set; } = new ToggleNode(false);

    [Menu("Warn for crit immune", parentIndex = 101)]
    public ToggleNode WarnCritImmune { get; set; } = new ToggleNode(false);

    [Menu("Warn for curse immune", parentIndex = 101)]
    public ToggleNode WarnCurseImmune { get; set; } = new ToggleNode(false);

    [Menu("Warn for armor pen", parentIndex = 101)]
    public ToggleNode WarnArmorPen { get; set; } = new ToggleNode(false);

    [Menu("Warn for no flask", parentIndex = 101)]
    public ToggleNode WarnNoFlask { get; set; } = new ToggleNode(false);

    [Menu("Warn for no evade", parentIndex = 101)]
    public ToggleNode WarnNoEvade { get; set; } = new ToggleNode(false);

    [Menu("Warn for no leech", parentIndex = 101)]
    public ToggleNode WarnNoLeech { get; set; } = new ToggleNode(true);

    [Menu("Warn for petrify", parentIndex = 101)]
    public ToggleNode WarnPetrify { get; set; } = new ToggleNode(true);

    [Menu("Warn for 20% cull", parentIndex = 101)]
    public ToggleNode WarnCull { get; set; } = new ToggleNode(true);

    [Menu("Warn for monster regen", parentIndex = 101)]
    public ToggleNode WarnMonsterRegen { get; set; } = new ToggleNode(false);

    [Menu("Warn for monster block", parentIndex = 101)]
    public ToggleNode WarnMonsterBlock { get; set; } = new ToggleNode(false);

    [Menu("Warn for monster resistances", parentIndex = 101)]
    public ToggleNode WarnMonsterResist { get; set; } = new ToggleNode(false);


    [Menu("Explosive settings", 102)]
    [JsonIgnore]
    public EmptyNode ExplosivesSettings { get; set; }

    [Menu("Show explosive radius", parentIndex = 102)]
    public ToggleNode ShowExplosives { get; set; } = new ToggleNode(true);

    [Menu("Color for explosive radius", parentIndex = 102)]
    public ColorNode ExplosiveColor { get; set; } = new ColorNode(Color.Red);

    [Menu("Explosive radius", parentIndex = 102)]
    public RangeNode<int> ExplosiveRadius { get; set; } = new RangeNode<int>(326, 10, 600);

    [Menu("Automatically calculate Radius from Mapmods", parentIndex = 102)]
    public ToggleNode AutoCalculateRadius { get; set; } = new ToggleNode(true);

    [Menu("Merge explosive radii", parentIndex = 102)]
    public ToggleNode EnableExplosiveRadiusMerging { get; set; } = new ToggleNode(true);

    [Menu("Mark entities captured by explosives", parentIndex = 102)]
    public ToggleNode MarkCapturedEntities { get; set; } = new ToggleNode(true);

    [Menu("Color for marked entities", parentIndex = 102)]
    public ColorNode MarkCapturedEntitiesColor { get; set; } = new ColorNode(Color.Green);

    [Menu("Rectangle Thickness for marked entities", parentIndex = 102)]
    public RangeNode<int> MarkCapturedEntitiesFrameThickness { get; set; } = new RangeNode<int>(1, 1, 20);

    private bool PickIcon(string iconName, ref MapIconsIndex icon)
    {
        var isOpen = true;
        ImGui.Begin($"Pick icon for {iconName}", ref isOpen, ImGuiWindowFlags.AlwaysAutoResize);
        if (!isOpen)
        {
            return true;
        }

        ImGui.InputTextWithHint("##Filter", "Filter", ref _iconFilter, 100);
        ImGui.SliderInt("Icon size (only in this menu)", ref IconPickerSize, 15, 60);
        ImGui.SliderInt("Icons per row", ref IconsPerRow, 5, 60);
        var icons = Enum.GetValues<MapIconsIndex>()
           .Where(x => string.IsNullOrEmpty(_iconFilter) || x.ToString().Contains(_iconFilter, StringComparison.InvariantCultureIgnoreCase))
           .ToArray();
        for (var i = 0; i < icons.Length; i++)
        {
            var testIcon = icons[i];
            var rect = SpriteHelper.GetUV(testIcon);
            if (icon == testIcon)
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(1, 0, 1, 1));
            }
            else
            {
                ImGui.PushStyleColor(ImGuiCol.Button, ImGui.GetColorU32(ImGuiCol.WindowBg));
            }

            ImGui.PushID(i.ToString());
            try
            {
                if (ImGui.ImageButton(_iconsImageId, System.Numerics.Vector2.One * IconPickerSize,
                        rect.TopLeft.ToVector2Num(), rect.BottomRight.ToVector2Num()))
                {
                    icon = testIcon;
                    return true;
                }
            }
            finally
            {
                ImGui.PopID();
                ImGui.PopStyleColor();
            }

            if ((i + 1) % IconsPerRow != 0)
            {
                ImGui.SameLine();
            }
        }

        ImGui.End();
        return false;
    }

    private void PickIcon(IconPickerIndex iconKey, MapIconsIndex defaultIcon)
    {
        var iconSettings = IconMapping.GetValueOrDefault(iconKey, new IconDisplaySettings());
        ImGui.Checkbox($"Show {iconKey}", ref iconSettings.Show);
        ImGui.SameLine();
        var effectiveIcon = iconSettings.Icon ?? defaultIcon;
        var uv = SpriteHelper.GetUV(effectiveIcon);
        var uv0 = uv.TopLeft.ToVector2Num();
        var uv1 = uv.BottomRight.ToVector2Num();
        ImGui.PushID(iconKey.ToString());
        var buttonClicked = ImGui.ImageButton(_iconsImageId, System.Numerics.Vector2.One * 15, uv0, uv1);
        if (buttonClicked)
        {
            _iconFilter = "";
        }

        if (buttonClicked || iconKey == _shownIconPicker)
        {
            _shownIconPicker = iconKey;
            if (PickIcon(iconKey.ToString(), ref effectiveIcon))
            {
                iconSettings.Icon = effectiveIcon != defaultIcon ? effectiveIcon : null;
                _shownIconPicker = null;
            }
        }

        ImGui.PopID();
        IconMapping[iconKey] = iconSettings;
    }
}
