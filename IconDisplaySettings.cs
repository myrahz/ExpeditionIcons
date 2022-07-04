using ExileCore.Shared.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SharpDX;

namespace ExpeditionIcons;

public record struct IconDisplaySettings(MapIconsIndex? Icon = null, Color? Tint = null, bool ShowOnMap = true, bool ShowInWorld = true)
{
    public IconDisplaySettings() : this(null)
    {
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public MapIconsIndex? Icon = Icon;

    public bool ShowOnMap = ShowOnMap;
    public bool ShowInWorld = ShowInWorld;
    public Color? Tint = Tint;

    public bool ShouldSerializeIcon() => Icon != null;
}
