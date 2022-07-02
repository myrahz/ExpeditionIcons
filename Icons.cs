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
            DefaultIcon = MapIconsIndex.LegionGeneric,
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
            DefaultIcon = MapIconsIndex.RewardUniques,
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
            DefaultIcon = MapIconsIndex.RewardEssences,
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
            DefaultIcon = MapIconsIndex.RewardGems,
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
            DefaultIcon = MapIconsIndex.QuestItem,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierExpeditionLogbookQuantityChest",
                "ExpeditionRelicModifierExpeditionLogbookQuantityMonster"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.Jewellery,
            DefaultIcon = MapIconsIndex.RewardJewellery,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierExpeditionRareTrinketElite",
                "ExpeditionRelicModifierExpeditionRareTrinketChest"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.Enchants,
            DefaultIcon = MapIconsIndex.LabyrinthEnchant,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierEternalEmpireEnchantElite",
                "ExpeditionRelicModifierEternalEmpireEnchantChest"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.Scarabs,
            DefaultIcon = MapIconsIndex.RewardScarabs,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierSirensScarabElite",
                "ExpeditionRelicModifierSirensScarabChest"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.Breach,
            DefaultIcon = MapIconsIndex.RewardBreach,
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
            DefaultIcon = MapIconsIndex.LootFilterLargeYellowCross,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierExpeditionInfluencedItemsElite",
                "ExpeditionRelicModifierExpeditionInfluencedItemsChest"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.Maps,
            DefaultIcon = MapIconsIndex.RewardMaps,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierExpeditionMapsElite",
                "ExpeditionRelicModifierExpeditionMapsChest"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.Fractured,
            DefaultIcon = MapIconsIndex.LootFilterLargeBlueDiamond,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierExpeditionFracturedItemsElite",
                "ExpeditionRelicModifierExpeditionFracturedItemsChest"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.Harbinger,
            DefaultIcon = MapIconsIndex.RewardHarbinger,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierHarbingerCurrencyElite",
                "ExpeditionRelicModifierHarbingerCurrencyChest"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.MonsterMods,
            DefaultIcon = MapIconsIndex.LootFilterLargeGreenTriangle,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierPackSize",
                "ExpeditionRelicModifierRareMonsterChance"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.RunicMonsterDuplication,
            DefaultIcon = MapIconsIndex.IncursionArchitectReplace,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierElitesDuplicated"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.Artifacts,
            DefaultIcon = MapIconsIndex.LootFilterLargePurpleSquare,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierExpeditionCurrencyQuantityChest",
                "ExpeditionRelicModifierExpeditionCurrencyQuantityMonster"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.Rerolls,
            DefaultIcon = MapIconsIndex.LootFilterLargePurpleCircle,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierExpeditionVendorCurrency"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.Quantity,
            DefaultIcon = MapIconsIndex.RewardGenericItems,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierItemQuantityChest",
                "ExpeditionRelicModifierItemQuantityMonster"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.Currency,
            DefaultIcon = MapIconsIndex.RewardCurrency,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierExpeditionBasicCurrencyChest",
                "ExpeditionRelicModifierExpeditionBasicCurrencyElite"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.StackedDecks,
            DefaultIcon = MapIconsIndex.RewardDivinationCards,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierStackedDeckChest",
                "ExpeditionRelicModifierStackedDeckElite"
            },
        },
    };
}