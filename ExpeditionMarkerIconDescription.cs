using System.Collections.Generic;
using ExileCore.Shared.Enums;

namespace ExpeditionIcons;

public class ExpeditionMarkerIconDescription
{
    public IconPickerIndex IconPickerIndex { get; init; }
    public MapIconsIndex Icon { get; init; }
    public List<string> BaseEntityMetadataSubstrings { get; set; } = new List<string>();
}