using System.Collections.Generic;
using ExileCore.Shared.Enums;

namespace ExpeditionIcons;

public static class Icons
{
    public static IExpeditionRelic GetRelicType(string relicMod)
    {
        return relicMod switch
        {
            "ExpeditionRelicModifierExpeditionLogbookQuantityChest" => new LogbookChestRelic(),
            "ExpeditionRelicModifierExpeditionLogbookQuantityMonster" => new LogbookMonsterRelic(),
            "ExpeditionRelicModifierSirensScarabElite" => new OtherGoodMonsterRelic(),
            "ExpeditionRelicModifierSirensScarabChest" => new OtherGoodChestRelic(),
            "ExpeditionRelicModifierExpeditionFracturedItemsElite" => new FracturedMonsterRelic(),
            "ExpeditionRelicModifierExpeditionFracturedItemsChest" => new FracturedChestRelic(),
            "ExpeditionRelicModifierPackSize" => new PackSizeMonsterRelic(),
            "ExpeditionRelicModifierElitesDuplicated" => new DoubledMonstersRelic(),
            "ExpeditionRelicModifierExpeditionCurrencyQuantityChest" => new IncreasedChestArtifactsRelic(),
            "ExpeditionRelicModifierExpeditionCurrencyQuantityMonster" => new IncreasedMonsterArtifactsRelic(),
            "ExpeditionRelicModifierExpeditionVendorCurrency" => new OtherGoodChestRelic(),
            "ExpeditionRelicModifierItemQuantityChest" => new IncreasedChestLootRelic(),
            "ExpeditionRelicModifierItemQuantityMonster" => new IncreasedMonsterLootRelic(),
            "ExpeditionRelicModifierExpeditionBasicCurrencyChest" => new OtherGoodChestRelic(),
            "ExpeditionRelicModifierExpeditionBasicCurrencyElite" => new OtherGoodMonsterRelic(),
            "ExpeditionRelicModifierStackedDeckChest" => new OtherGoodChestRelic(),
            "ExpeditionRelicModifierStackedDeckElite" => new OtherGoodMonsterRelic(),
            _ when relicMod.Contains("Monster") => new OtherMonsterRelic(),
            _ when relicMod.Contains("Chest") => new OtherChestRelic(),
            _ => null,
        };
    }

    public static readonly List<ExpeditionMarkerIconDescription> ExpeditionRelicIcons = new()
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

    public static readonly List<ExpeditionMarkerIconDescription> LogbookChestIcons = new()
    {
        new()
        {
            IconPickerIndex = IconPickerIndex.BlightChest,
            DefaultIcon = ExpeditionIconsSettings.DefaultChestIcon,
            BaseEntityMetadataSubstrings =
            {
                "Metadata/Terrain/Doodads/Leagues/Expedition/ChestMarkers/ChestBlight.ao"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.FragmentChest,
            DefaultIcon = ExpeditionIconsSettings.DefaultChestIcon,
            BaseEntityMetadataSubstrings =
            {
                "Metadata/Terrain/Doodads/Leagues/Expedition/ChestMarkers/ChestFragments.ao"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.LeagueChest,
            DefaultIcon = ExpeditionIconsSettings.DefaultChestIcon,
            BaseEntityMetadataSubstrings =
            {
                "Metadata/Terrain/Doodads/Leagues/Expedition/ChestMarkers/ChestLeague.ao"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.JewelleryChest,
            DefaultIcon = ExpeditionIconsSettings.DefaultChestIcon,
            BaseEntityMetadataSubstrings =
            {
                "Metadata/Terrain/Doodads/Leagues/Expedition/ChestMarkers/ChestTrinkets.ao"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.WeaponChest,
            DefaultIcon = ExpeditionIconsSettings.DefaultChestIcon,
            BaseEntityMetadataSubstrings =
            {
                "Metadata/Terrain/Doodads/Leagues/Expedition/ChestMarkers/ChestWeapon.ao"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.CurrencyChest,
            DefaultIcon = ExpeditionIconsSettings.DefaultChestIcon,
            BaseEntityMetadataSubstrings =
            {
                "Metadata/Terrain/Doodads/Leagues/Expedition/ChestMarkers/ChestCurrency.ao"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.HeistChest,
            DefaultIcon = ExpeditionIconsSettings.DefaultChestIcon,
            BaseEntityMetadataSubstrings =
            {
                "Metadata/Terrain/Doodads/Leagues/Expedition/ChestMarkers/ChestHeist.ao"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.BreachChest,
            DefaultIcon = ExpeditionIconsSettings.DefaultChestIcon,
            BaseEntityMetadataSubstrings =
            {
                "Metadata/Terrain/Doodads/Leagues/Expedition/ChestMarkers/ChestBreach.ao"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.RitualChest,
            DefaultIcon = ExpeditionIconsSettings.DefaultChestIcon,
            BaseEntityMetadataSubstrings =
            {
                "Metadata/Terrain/Doodads/Leagues/Expedition/ChestMarkers/ChestRitual.ao"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.MetamorphChest,
            DefaultIcon = ExpeditionIconsSettings.DefaultChestIcon,
            BaseEntityMetadataSubstrings =
            {
                "Metadata/Terrain/Doodads/Leagues/Expedition/ChestMarkers/ChestMetamorph.ao"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.MapsChest,
            DefaultIcon = ExpeditionIconsSettings.DefaultChestIcon,
            BaseEntityMetadataSubstrings =
            {
                "Metadata/Terrain/Doodads/Leagues/Expedition/ChestMarkers/ChestMaps.ao"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.GemsChest,
            DefaultIcon = ExpeditionIconsSettings.DefaultChestIcon,
            BaseEntityMetadataSubstrings =
            {
                "Metadata/Terrain/Doodads/Leagues/Expedition/ChestMarkers/ChestGems.ao"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.FossilsChest,
            DefaultIcon = ExpeditionIconsSettings.DefaultChestIcon,
            BaseEntityMetadataSubstrings =
            {
                "Metadata/Terrain/Doodads/Leagues/Expedition/ChestMarkers/ChestFossils.ao"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.DivinationCardsChest,
            DefaultIcon = ExpeditionIconsSettings.DefaultChestIcon,
            BaseEntityMetadataSubstrings =
            {
                "Metadata/Terrain/Doodads/Leagues/Expedition/ChestMarkers/ChestDivinationCards.ao"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.EssenceChest,
            DefaultIcon = ExpeditionIconsSettings.DefaultChestIcon,
            BaseEntityMetadataSubstrings =
            {
                "Metadata/Terrain/Doodads/Leagues/Expedition/ChestMarkers/ChestEssence.ao"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.ArmourChest,
            DefaultIcon = ExpeditionIconsSettings.DefaultChestIcon,
            BaseEntityMetadataSubstrings =
            {
                "Metadata/Terrain/Doodads/Leagues/Expedition/ChestMarkers/ChestArmour.ao"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.LegionChest,
            DefaultIcon = ExpeditionIconsSettings.DefaultChestIcon,
            BaseEntityMetadataSubstrings =
            {
                "Metadata/Terrain/Doodads/Leagues/Expedition/ChestMarkers/ChestLegion.ao"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.DeliriumChest,
            DefaultIcon = ExpeditionIconsSettings.DefaultChestIcon,
            BaseEntityMetadataSubstrings =
            {
                "Metadata/Terrain/Doodads/Leagues/Expedition/ChestMarkers/ChestDelirium.ao"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.UniquesChest,
            DefaultIcon = ExpeditionIconsSettings.DefaultChestIcon,
            BaseEntityMetadataSubstrings =
            {
                "Metadata/Terrain/Doodads/Leagues/Expedition/ChestMarkers/ChestUniques.ao"
            },
        },
        new()
        {
            IconPickerIndex = IconPickerIndex.OtherChests,
            DefaultIcon = MapIconsIndex.MissionAlly,
            BaseEntityMetadataSubstrings =
            {
                "chestmarker1",
                "chestmarker2",
                "chestmarker3",
                "chestmarker_signpost",
                "Metadata/Terrain/Doodads/Leagues/Expedition/ChestMarkers"
            },
        },
    };
}