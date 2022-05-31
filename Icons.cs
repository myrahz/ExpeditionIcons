using System.Collections.Generic;
using ExileCore.Shared.Enums;

namespace ExpeditionIcons;

public static class Icons
{
    public static readonly List<ExpeditionMarkerIconDescription> ExpeditionRelicWorldIcons = new()
    {
        new()
        {
            IconPickerIndex = IconPickerIndex.Legion,
            Icon = MapIconsIndex.LegionGeneric,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierLegionSplintersElite",
                "ExpeditionRelicModifierEternalEmpireLegionElite",
                "ExpeditionRelicModifierLegionSplintersChest",
                "ExpeditionRelicModifierEternalEmpireLegionChest"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.Uniques,
            Icon = MapIconsIndex.RewardUniques,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierExpeditionUniqueElite",
                "ExpeditionRelicModifierLostMenUniqueElite",
                "ExpeditionRelicModifierExpeditionUniqueChest",
                "ExpeditionRelicModifierLostMenUniqueChest"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.Essences,
            Icon = MapIconsIndex.RewardEssences,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierEssencesElite",
                "ExpeditionRelicModifierLostMenEssenceElite",
                "ExpeditionRelicModifierLostMenEssenceChest",
                "ExpeditionRelicModifierEssencesChest"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.VaalGems,
            Icon = MapIconsIndex.RewardGems,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierVaalGemsElite",
                "ExpeditionRelicModifierExpeditionGemsElite",
                "ExpeditionRelicModifierVaalGemsChest",
                "ExpeditionRelicModifierExpeditionGemsChest"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.Logbooks,
            Icon = MapIconsIndex.QuestItem,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierExpeditionLogbookQuantityChest",
                "ExpeditionRelicModifierExpeditionLogbookQuantityMonster"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.Jewellery,
            Icon = MapIconsIndex.RewardJewellery,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierExpeditionRareTrinketElite",
                "ExpeditionRelicModifierExpeditionRareTrinketChest"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.Enchants,
            Icon = MapIconsIndex.LabyrinthEnchant,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierEternalEmpireEnchantElite",
                "ExpeditionRelicModifierEternalEmpireEnchantChest"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.Scarabs,
            Icon = MapIconsIndex.RewardScarabs,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierSirensScarabElite",
                "ExpeditionRelicModifierSirensScarabChest"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.Breach,
            Icon = MapIconsIndex.RewardBreach,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierBreachSplintersElite",
                "ExpeditionRelicModifierSirensBreachElite",
                "ExpeditionRelicModifierBreachSplintersChest",
                "ExpeditionRelicModifierSirensBreachChest"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.Influenced,
            Icon = MapIconsIndex.LootFilterLargeYellowCross,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierExpeditionInfluencedItemsElite",
                "ExpeditionRelicModifierExpeditionInfluencedItemsChest"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.Maps,
            Icon = MapIconsIndex.RewardMaps,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierExpeditionMapsElite",
                "ExpeditionRelicModifierExpeditionMapsChest"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.Fractured,
            Icon = MapIconsIndex.LootFilterLargeBlueDiamond,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierExpeditionFracturedItemsElite",
                "ExpeditionRelicModifierExpeditionFracturedItemsChest"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.Harbinger,
            Icon = MapIconsIndex.RewardHarbinger,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierHarbingerCurrencyElite",
                "ExpeditionRelicModifierHarbingerCurrencyChest"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.MonsterMods,
            Icon = MapIconsIndex.LootFilterLargeGreenTriangle,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierPackSize",
                "ExpeditionRelicModifierRareMonsterChance"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.RunicMonsterDuplication,
            Icon = MapIconsIndex.IncursionArchitectReplace,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierElitesDuplicated"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.Artifacts,
            Icon = MapIconsIndex.LootFilterLargePurpleSquare,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierExpeditionCurrencyQuantityChest",
                "ExpeditionRelicModifierExpeditionCurrencyQuantityMonster"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.Rerolls,
            Icon = MapIconsIndex.LootFilterLargePurpleCircle,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierExpeditionVendorCurrency"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.Quantity,
            Icon = MapIconsIndex.RewardGenericItems,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierItemQuantityChest",
                "ExpeditionRelicModifierItemQuantityMonster"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.Currency,
            Icon = MapIconsIndex.RewardCurrency,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierExpeditionBasicCurrencyChest",
                "ExpeditionRelicModifierExpeditionBasicCurrencyElite"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.StackedDecks,
            Icon = MapIconsIndex.RewardDivinationCards,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierStackedDeckChest",
                "ExpeditionRelicModifierStackedDeckElite"
            },
        },
    };
}