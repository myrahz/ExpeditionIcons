using ExileCore.Shared.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ExpeditionIcons;

public record struct IconDisplaySettings(MapIconsIndex? Icon = null, bool Show = true)
{
    public IconDisplaySettings() : this(null)
    {
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public MapIconsIndex? Icon = Icon;

    public bool Show = Show;

    public bool ShouldSerializeIcon() => Icon != null;
}
