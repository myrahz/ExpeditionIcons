using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ExpeditionIcons;

[JsonConverter(typeof(StringEnumConverter))]
public enum IconPickerIndex
{
    Legion,
    Uniques,
    Essences,
    VaalGems,
    Logbooks,
    Jewellery,
    Enchants,
    Scarabs,
    Breach,
    Influenced,
    Maps,
    Fractured,
    Harbinger,
    MonsterMods,
    Artifacts,
    Rerolls,
    Quantity,
    Currency,
    StackedDecks,
    RunicMonsterDuplication,
}