using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.Elements;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using ExileCore.Shared.Nodes;
using SharpDX;

namespace ExpeditionIcons;

public class ExpeditionMarkerIconDescription
{
    public MapIconsIndex Icon { get; set; }
    public List<string> BaseEntityMetadataSubstrings { get; set; } = new List<string>();
    public Func<Settings, ToggleNode> EnableSelector { get; set; } = _ => null;
}

public class EntityPosWrapper
{
    public readonly Vector3 Pos;

    public EntityPosWrapper(Vector3 pos)
    {
        Pos = pos;
    }
}

public class Core : BaseSettingsPlugin<Settings>
{
    private static readonly List<ExpeditionMarkerIconDescription> _expeditionRelicWorldIcons = new()
    {
        new()
        {
            Icon = MapIconsIndex.LegionGeneric,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierLegionSplintersElite",
                "ExpeditionRelicModifierEternalEmpireLegionElite",
                "ExpeditionRelicModifierLegionSplintersChest",
                "ExpeditionRelicModifierEternalEmpireLegionChest"
            },
            EnableSelector = s => s.ShowLegion
        },
        new()
        {
            Icon = MapIconsIndex.RewardUniques,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierExpeditionUniqueElite",
                "ExpeditionRelicModifierLostMenUniqueElite",
                "ExpeditionRelicModifierExpeditionUniqueChest",
                "ExpeditionRelicModifierLostMenUniqueChest"
            },
            EnableSelector = s => s.ShowUniques
        },
        new()
        {
            Icon = MapIconsIndex.RewardEssences,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierEssencesElite",
                "ExpeditionRelicModifierLostMenEssenceElite",
                "ExpeditionRelicModifierLostMenEssenceChest",
                "ExpeditionRelicModifierEssencesChest"
            },
            EnableSelector = s => s.ShowEssences
        },
        new()
        {
            Icon = MapIconsIndex.RewardGems,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierVaalGemsElite",
                "ExpeditionRelicModifierExpeditionGemsElite",
                "ExpeditionRelicModifierVaalGemsChest",
                "ExpeditionRelicModifierExpeditionGemsChest"
            },
            EnableSelector = s => s.ShowGems
        },
        new()
        {
            Icon = MapIconsIndex.QuestItem,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierExpeditionLogbookQuantityChest",
                "ExpeditionRelicModifierExpeditionLogbookQuantityMonster"
            },
            EnableSelector = s => s.ShowLogbooks
        },
        new()
        {
            Icon = MapIconsIndex.RewardJewellery,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierExpeditionRareTrinketElite",
                "ExpeditionRelicModifierExpeditionRareTrinketChest"
            },
            EnableSelector = s => s.ShowRares
        },
        new()
        {
            Icon = MapIconsIndex.LabyrinthEnchant,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierEternalEmpireEnchantElite",
                "ExpeditionRelicModifierEternalEmpireEnchantChest"
            },
            EnableSelector = s => s.ShowEnchant
        },
        new()
        {
            Icon = MapIconsIndex.RewardScarabs,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierSirensScarabElite",
                "ExpeditionRelicModifierSirensScarabChest"
            },
            EnableSelector = s => s.ShowScarabs
        },
        new()
        {
            Icon = MapIconsIndex.RewardBreach,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierBreachSplintersElite",
                "ExpeditionRelicModifierSirensBreachElite",
                "ExpeditionRelicModifierBreachSplintersChest",
                "ExpeditionRelicModifierSirensBreachChest"
            },
            EnableSelector = s => s.ShowBreach
        },
        new()
        {
            Icon = MapIconsIndex.LootFilterLargeYellowStar,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierExpeditionInfluencedItemsElite",
                "ExpeditionRelicModifierExpeditionInfluencedItemsChest"
            },
            EnableSelector = s => s.ShowInfluencedItems
        },
        new()
        {
            Icon = MapIconsIndex.RewardMaps,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierExpeditionMapsElite",
                "ExpeditionRelicModifierExpeditionMapsChest"
            },
            EnableSelector = s => s.ShowMaps
        },
        new()
        {
            Icon = MapIconsIndex.LootFilterLargeBlueDiamond,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierExpeditionFracturedItemsElite",
                "ExpeditionRelicModifierExpeditionFracturedItemsChest"
            },
            EnableSelector = s => s.ShowFractured
        },
        new()
        {
            Icon = MapIconsIndex.RewardHarbinger,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierHarbingerCurrencyElite",
                "ExpeditionRelicModifierHarbingerCurrencyChest"
            },
            EnableSelector = s => s.ShowHarbinger
        },
        new()
        {
            Icon = MapIconsIndex.LootFilterLargeGreenTriangle,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierPackSize",
                "ExpeditionRelicModifierRareMonsterChance",
                "ExpeditionRelicModifierElitesDuplicated"
            },
            EnableSelector = s => s.ShowMonsterMods
        },
        new()
        {
            Icon = MapIconsIndex.LootFilterLargePurpleSquare,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierExpeditionCurrencyQuantityChest",
                "ExpeditionRelicModifierExpeditionCurrencyQuantityMonster"
            },
            EnableSelector = s => s.ShowArtifacts
        },
        new()
        {
            Icon = MapIconsIndex.LootFilterLargePurpleCircle,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierExpeditionVendorCurrency"
            },
            EnableSelector = s => s.ShowExpeditionRerollCurrency
        },
        new()
        {
            Icon = MapIconsIndex.RewardGenericItems,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierItemQuantityChest",
                "ExpeditionRelicModifierItemQuantityMonster"
            },
            EnableSelector = s => s.ShowItemQuantity
        },
        new()
        {
            Icon = MapIconsIndex.RewardCurrency,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierExpeditionBasicCurrencyChest",
                "ExpeditionRelicModifierExpeditionBasicCurrencyElite"
            },
            EnableSelector = s => s.ShowBasicCurrency
        },
        new()
        {
            Icon = MapIconsIndex.RewardDivinationCards,
            BaseEntityMetadataSubstrings =
            {
                "ExpeditionRelicModifierStackedDeckChest",
                "ExpeditionRelicModifierStackedDeckElite"
            },
            EnableSelector = s => s.ShowStackedDecks
        },
    };

    private const string TextureName = "Icons.png";
    private const double CameraAngle = 38.7 * Math.PI / 180;
    private static readonly float CameraAngleCos = (float)Math.Cos(CameraAngle);
    private static readonly float CameraAngleSin = (float)Math.Sin(CameraAngle);
    private const float GridToWorldMultiplier = 250 / 23f;

    private double _mapScale;
    private Vector2 _mapCenter;
    private bool _largeMapOpen;
    private readonly ConditionalWeakTable<Entity, string> _baseAnimatedEntityMetadata = new ConditionalWeakTable<Entity, string>();
    private readonly ConditionalWeakTable<Entity, EntityPosWrapper> _entityPos = new ConditionalWeakTable<Entity, EntityPosWrapper>();
    private Vector2 _playerPos;
    private float _playerZ;

    private Camera Camera => GameController.Game.IngameState.Camera;

    public override bool Initialise()
    {
        Graphics.InitImage(TextureName);
        return base.Initialise();
    }

    public override Job Tick()
    {
        return null;
    }

    private static TValue GetOrAdd<TKey, TValue>(ConditionalWeakTable<TKey, TValue> table, TKey key, Func<TKey, TValue> createFunc)
        where TKey : class
        where TValue : class
    {
        if (!table.TryGetValue(key, out var result))
        {
            result = createFunc(key);
            table.Add(key, result);
        }

        return result;
    }

    public override void Render()
    {
        var ingameUi = GameController.Game.IngameState.IngameUi;
        var map = ingameUi.Map;
        var largeMap = map.LargeMap.AsObject<SubMap>();
        _largeMapOpen = largeMap.IsVisible;
        _mapScale = GameController.IngameState.Camera.Height / 677f * largeMap.Zoom;
        _mapCenter = largeMap.GetClientRect().TopLeft + largeMap.Shift + largeMap.DefaultShift;
        _playerPos = GameController.Player.GetComponent<Positioned>().GridPos;
        _playerZ = GameController.Player.GetComponent<Render>().Z;

        const string markerPath = "Metadata/MiscellaneousObjects/Expedition/ExpeditionMarker";
        foreach (var e in GameController.EntityListWrapper.ValidEntitiesByType[EntityType.IngameIcon].OrderBy(x => x.Path != markerPath))
        {
            if (e.Path == markerPath)
            {
                var animatedMetaData = GetOrAdd(_baseAnimatedEntityMetadata, e, ee => ee.GetComponent<Animated>()?.BaseAnimatedObjectEntity?.Metadata);
                if (animatedMetaData != null)
                {
                    if (animatedMetaData.Contains("elitemarker"))
                    {
                        if (Settings.DrawEliteMonstersOnMap)
                        {
                            DrawIconOnMap(e, MapIconsIndex.MissionTarget, Color.Blue, Vector2.Zero);
                        }

                        if (Settings.DrawEliteMonstersInWorld)
                        {
                            DrawIconInWorld(e, MapIconsIndex.MissionTarget, Color.Blue, Vector2.Zero);
                        }
                    }
                    else
                    {
                        var color = Color.Transparent;
                        var alreadyHasMapIcon = false;
                        if (animatedMetaData.Contains("chestmarker3"))
                        {
                            color = Color.Orange;
                        }
                        else if (animatedMetaData.Contains("chestmarker2"))
                        {
                            color = Color.Yellow;
                        }
                        else if (animatedMetaData.Contains("chestmarker1") || animatedMetaData.Contains("chestmarker_signpost"))
                        {
                            color = Color.White;
                        }
                        else if (animatedMetaData.Contains("Metadata/Terrain/Doodads/Leagues/Expedition/ChestMarkers"))
                        {
                            color = Color.Red;
                            alreadyHasMapIcon = true;
                        }

                        if (color != Color.Transparent)
                        {
                            if (!alreadyHasMapIcon && Settings.DrawChestsOnMap)
                            {
                                DrawIconOnMap(e, MapIconsIndex.MissionAlly, color, Vector2.Zero);
                            }

                            if (Settings.DrawChestsInWorld)
                            {
                                DrawIconInWorld(e, MapIconsIndex.MissionAlly, color, Vector2.Zero);
                            }
                        }
                    }
                }

                continue;
            }

            var renderComponent = e.GetComponent<Render>();
            if (renderComponent == null) continue;
            var expeditionChestComponent = e.GetComponent<ObjectMagicProperties>();
            if (expeditionChestComponent == null) continue;
            var mods = expeditionChestComponent.Mods;
            if (!mods.Any(x => x.Contains("ExpeditionRelicModifier"))) continue;

            var icons = new List<MapIconsIndex>();
            if (Settings.PhysImmune.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierImmunePhysicalDamage")) ||
                Settings.FireImmune.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierImmuneFireDamage")) ||
                Settings.ColdImmune.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierImmuneColdDamage")) ||
                Settings.LightningImmune.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierImmuneLightningDamage")) ||
                Settings.ChaosImmune.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierImmuneChaosDamage")) ||
                Settings.CritImmune.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierCannotBeCrit")) ||
                Settings.IgniteImmune.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierImmuneStatusAilments")) ||
                Settings.WarnArmorPen.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierIgnoreArmour")) ||
                Settings.WarnNoEvade.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierHitsCannotBeEvaded")) ||
                Settings.WarnNoLeech.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierCannotBeLeechedFrom")) ||
                Settings.WarnNoFlask.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierGrantNoFlaskCharges")) ||
                Settings.WarnPetrify.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierElitesPetrifyOnHit")) ||
                Settings.WarnCurseImmune.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierImmuneToCurses")) ||
                Settings.WarnCull.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierCullingStrikeTwentyPercent"))
               )
            {
                if (Settings.DrawBadModsInWorld)
                {
                    DrawIconInWorld(e, MapIconsIndex.RedFlag, Color.White, -Vector2.UnitY);
                }

                if (Settings.DrawBadModsOnMap)
                {
                    DrawIconOnMap(e, MapIconsIndex.RedFlag, Color.White, Vector2.Zero);
                }

                continue;
            }

            if (Settings.DrawGoodModsInWorld || Settings.DrawGoodModsOnMap)
            {
                foreach (var worldIcon in _expeditionRelicWorldIcons.Where(x => x.EnableSelector(Settings)?.Value ?? true))
                {
                    if (mods.Any(mod => worldIcon.BaseEntityMetadataSubstrings.Any(mod.Contains)))
                    {
                        icons.Add(worldIcon.Icon);
                    }
                }

                var offset = new Vector2(-icons.Count * 0.5f + 0.5f, 0);
                foreach (var icon in icons)
                {
                    if (Settings.DrawGoodModsInWorld)
                    {
                        DrawIconInWorld(e, icon, Color.White, offset);
                    }

                    if (Settings.DrawGoodModsOnMap)
                    {
                        DrawIconOnMap(e, icon, Color.White, offset);
                    }

                    offset += Vector2.UnitX;
                }
            }
        }
    }

    private void DrawIconOnMap(Entity entity, MapIconsIndex icon, Color color, Vector2 offset)
    {
        if (_largeMapOpen)
        {
            const int halfsize = 15;
            var point = GetEntityPosOnMapScreen(entity) + offset * halfsize * 2;
            var rect = new RectangleF(point.X, point.Y, 0, 0);
            rect.Inflate(halfsize, halfsize);
            Graphics.DrawImage(TextureName, rect, SpriteHelper.GetUV(icon), color);
        }
    }

    private void DrawIconInWorld(Entity entity, MapIconsIndex icon, Color color, Vector2 offset)
    {
        const int halfsize = 25;
        var entityPos = Settings.CacheEntityPosition ? GetOrAdd(_entityPos, entity, e => new EntityPosWrapper(e.Pos)).Pos : entity.Pos;
        var point = Camera.WorldToScreen(entityPos) + offset * halfsize * 2;
        var rect = new RectangleF(point.X, point.Y, 0, 0);
        rect.Inflate(halfsize, halfsize);
        Graphics.DrawImage(TextureName, rect, SpriteHelper.GetUV(icon), color);
    }

    private Vector2 GetEntityPosOnMapScreen(Entity entity)
    {
        var iconZ = entity.GetComponent<Render>()?.Z ?? 0;
        var point = _mapCenter + TranslateGridDeltaToMapDelta(entity.GridPos - _playerPos, iconZ - _playerZ);
        return point;
    }

    private Vector2 TranslateGridDeltaToMapDelta(Vector2 delta, float deltaZ)
    {
        deltaZ /= GridToWorldMultiplier; //z is normally "world" units, translate to grid
        return (float)_mapScale * new Vector2((delta.X - delta.Y) * CameraAngleCos, (deltaZ - (delta.X + delta.Y)) * CameraAngleSin);
    }
}


//ExpeditionRelicModifierImmuneChaosDamage
//ExpeditionRelicModifierImmuneColdDamage
//ExpeditionRelicModifierImmuneFireDamage
//ExpeditionRelicModifierImmuneLightningDamage
//ExpeditionRelicModifierImmunePhysicalDamage
//ExpeditionRelicModifierImmuneStatusAilments
//ExpeditionRelicModifierImmuneToCurses
//ExpeditionRelicModifierCannotBeCrit


//ExpeditionRelicModifierElitesDuplicated
//ExpeditionRelicModifierStackedDeckChest
//ExpeditionRelicModifierStackedDeckElite
//ExpeditionRelicModifierExpeditionBasicCurrencyChest
//ExpeditionRelicModifierExpeditionBasicCurrencyElite
//ExpeditionRelicModifierExpeditionLogbookQuantityChest
//ExpeditionRelicModifierExpeditionLogbookQuantityMonster
//ExpeditionRelicModifierExpeditionCurrencyQuantityChest
//ExpeditionRelicModifierExpeditionCurrencyQuantityMonster
//ExpeditionRelicModifierItemQuantityChest
//ExpeditionRelicModifierItemQuantityMonster
// Modifier:ExpeditionChestImplicitRarity~~, Modifier:ExpeditionLogbookMapExpeditionChestDoubleDropsChance~, Modifier:ExpeditionLogbookMapExpeditionExplosionRadius, Modifier:ExpeditionLogbookMapExpeditionExplosives, Modifier:ExpeditionLogbookMapExpeditionExtraRelicSuffixChance~~, Modifier:ExpeditionLogbookMapExpeditionMaximumPlacementDistance, Modifier:ExpeditionLogbookMapExpeditionNumberOfMonsterMarkers~, Modifier:ExpeditionLogbookMapExpeditionRelics, Modifier:ExpeditionLogbookMapExpeditionSagaAdditionalTerrainFeatures~~, Modifier:ExpeditionLogbookMapExpeditionSagaContainsBoss2~, Modifier:ExpeditionLogbookMapExpeditionSagaContainsBoss3, Modifier:ExpeditionLogbookMapExpeditionSagaContainsBoss4, Modifier:ExpeditionLogbookMapExpeditionSagaContainsBoss~, Modifier:ExpeditionNPCLifeRegen~, Modifier:ExpeditionNPCStealth~, Modifier:ExpeditionReducedBeyondPortalChance, Modifier:ExpeditionRelicModifierAcrobatic~~, Modifier:ExpeditionRelicModifierAllDamageFreezesFreezeDuration, Modifier:ExpeditionRelicModifierAllDamageIgnitesIgniteDuration, Modifier:ExpeditionRelicModifierAllDamagePoisonsPoisonDuration, Modifier:ExpeditionRelicModifierAllDamageShocksShockEffect, Modifier:ExpeditionRelicModifierAlwaysCrit, Modifier:ExpeditionRelicModifierAttackBlockSpellBlockMaxBlockChance, Modifier:ExpeditionRelicModifierAtziriFragmentsChest, Modifier:ExpeditionRelicModifierAtziriFragmentsElite~, Modifier:ExpeditionRelicModifierBleedOnHitBleedDuration, Modifier:ExpeditionRelicModifierBlightOilsChest, Modifier:ExpeditionRelicModifierBlightOilsElite, Modifier:ExpeditionRelicModifierBreachSplintersChest~, Modifier:ExpeditionRelicModifierBreachSplintersElite ~~ ~~, Modifier:ExpeditionRelicModifierCannotBeCrit, Modifier:ExpeditionRelicModifierCannotBeLeechedFrom~~, Modifier:ExpeditionRelicModifierChaosPenetration~, Modifier:ExpeditionRelicModifierColdPenetration, Modifier:ExpeditionRelicModifierCriticalAgainstFullLife, Modifier:ExpeditionRelicModifierCullingStrikeTwentyPercent, Modifier:ExpeditionRelicModifierDamage, Modifier:ExpeditionRelicModifierDamageAddedAsChaos, Modifier:ExpeditionRelicModifierDamageAttackCastMovementSpeedLowLife ~~ ~~, Modifier:ExpeditionRelicModifierDeliriumSplintersChest, Modifier:ExpeditionRelicModifierDeliriumSplintersElite, Modifier:ExpeditionRelicModifierElitesDeathGeasOnHit, Modifier:ExpeditionRelicModifierElitesDuplicated, Modifier:ExpeditionRelicModifierElitesPetrifyOnHit, Modifier:ExpeditionRelicModifierElitesRandomCurseOnHit, Modifier:ExpeditionRelicModifierElitesRegenerateLifeEveryFourSeconds, Modifier:ExpeditionRelicModifierEssencesChest, Modifier:ExpeditionRelicModifierEssencesElite, Modifier:ExpeditionRelicModifierEternalEmpireEnchantChest, Modifier:ExpeditionRelicModifierEternalEmpireEnchantElite, Modifier:ExpeditionRelicModifierEternalEmpireLegionChest, Modifier:ExpeditionRelicModifierEternalEmpireLegionElite, Modifier:ExpeditionRelicModifierExpeditionBasicCurrencyChest, Modifier:ExpeditionRelicModifierExpeditionBasicCurrencyElite ~~ ~~, Modifier:ExpeditionRelicModifierExpeditionCorruptedItemsElite, Modifier:ExpeditionRelicModifierExpeditionCurrencyQuantityChest, Modifier:ExpeditionRelicModifierExpeditionCurrencyQuantityMonster~~, Modifier:ExpeditionRelicModifierExpeditionFracturedItemsChest, Modifier:ExpeditionRelicModifierExpeditionFracturedItemsElite, Modifier:ExpeditionRelicModifierExpeditionFullyLinkedElite~~, Modifier:ExpeditionRelicModifierExpeditionGemsChest, Modifier:ExpeditionRelicModifierExpeditionGemsElite, Modifier:ExpeditionRelicModifierExpeditionInfluencedItemsElite ~~ ~~, Modifier:ExpeditionRelicModifierExpeditionInfluencedtemsChest~~, Modifier:ExpeditionRelicModifierExpeditionLogbookQuantityChest, Modifier:ExpeditionRelicModifierExpeditionLogbookQuantityMonster, Modifier:ExpeditionRelicModifierExpeditionLogbookQuantityMonster, Modifier:ExpeditionRelicModifierExpeditionMapsChest~, Modifier:ExpeditionRelicModifierExpeditionMapsElite~, Modifier:ExpeditionRelicModifierExpeditionRareArmourChest~, Modifier:ExpeditionRelicModifierExpeditionRareArmourElite~, Modifier:ExpeditionRelicModifierExpeditionRareTrinketChest, Modifier:ExpeditionRelicModifierExpeditionRareTrinketElite, Modifier:ExpeditionRelicModifierExpeditionRareWeaponChest~~, Modifier:ExpeditionRelicModifierExpeditionRareWeaponElite, Modifier:ExpeditionRelicModifierExpeditionTalismanChest, Modifier:ExpeditionRelicModifierExpeditionTalismanElite ~~ ~~, Modifier:ExpeditionRelicModifierExpeditionUniqueChest~, Modifier:ExpeditionRelicModifierExpeditionUniqueElite, Modifier:ExpeditionRelicModifierExpeditionVendorCurrencyFactionChest1, Modifier:ExpeditionRelicModifierExpeditionVendorCurrencyFactionChest2, Modifier:ExpeditionRelicModifierExpeditionVendorCurrencyFactionChest3, Modifier:ExpeditionRelicModifierExpeditionVendorCurrencyFactionChest4, Modifier:ExpeditionRelicModifierExpeditionVendorCurrencyFactionElite1, Modifier:ExpeditionRelicModifierExpeditionVendorCurrencyFactionElite2, Modifier:ExpeditionRelicModifierExpeditionVendorCurrencyFactionElite3, Modifier:ExpeditionRelicModifierExpeditionVendorCurrencyFactionElite4, Modifier:ExpeditionRelicModifierExperience~1 ~~ ~~, Modifier:ExpeditionRelicModifierExperience~2, Modifier:ExpeditionRelicModifierExperience~3~, Modifier:ExpeditionRelicModifierExperience~4, Modifier:ExpeditionRelicModifierFirePenetration~, Modifier:ExpeditionRelicModifierFossilsChest, Modifier:ExpeditionRelicModifierFossilsElite, Modifier:ExpeditionRelicModifierGrantNoFlaskCharges, Modifier:ExpeditionRelicModifierHarbingerCurrencyChest~, Modifier:ExpeditionRelicModifierHarbingerCurrencyElite, Modifier:ExpeditionRelicModifierHeistContractChest, Modifier:ExpeditionRelicModifierHeistContractElite, Modifier:ExpeditionRelicModifierHitsCannotBeEvaded, Modifier:ExpeditionRelicModifierHitsRemoveManaAndEnergyShieldOnHit, Modifier:ExpeditionRelicModifierIgnoreArmour~, Modifier:ExpeditionRelicModifierImmuneChaosDamage, Modifier:ExpeditionRelicModifierImmuneColdDamage, Modifier:ExpeditionRelicModifierImmuneFireDamage~, Modifier:ExpeditionRelicModifierImmuneLightningDamage, Modifier:ExpeditionRelicModifierImmunePhysicalDamage, Modifier:ExpeditionRelicModifierImmuneStatusAilments~, Modifier:ExpeditionRelicModifierImmuneToCurses~~, Modifier:ExpeditionRelicModifierImpaleOnHitImpaleEffect~, Modifier:ExpeditionRelicModifierItemQuantityChest~, Modifier:ExpeditionRelicModifierItemQuantityMonster, Modifier:ExpeditionRelicModifierItemRarityChest, Modifier:ExpeditionRelicModifierItemRarityMonster ~~ ~~ ~, Modifier:ExpeditionRelicModifierKaruiFossilChest~, Modifier:ExpeditionRelicModifierKaruiFossilElite, Modifier:ExpeditionRelicModifierKaruiShardsChest, Modifier:ExpeditionRelicModifierKaruiShardsElite, Modifier:ExpeditionRelicModifierLegionSplintersChest, Modifier:ExpeditionRelicModifierLegionSplintersElite, Modifier:ExpeditionRelicModifierLife~~, Modifier:ExpeditionRelicModifierLightningPenetration, Modifier:ExpeditionRelicModifierLostMenEssenceChest, Modifier:ExpeditionRelicModifierLostMenEssenceElite~~, Modifier:ExpeditionRelicModifierLostMenUniqueChest, Modifier:ExpeditionRelicModifierLostMenUniqueElite, Modifier:ExpeditionRelicModifierMagicMonsterChance, Modifier:ExpeditionRelicModifierMarakethDivinationChest, Modifier:ExpeditionRelicModifierMarakethDivinationElite, Modifier:ExpeditionRelicModifierMarakethIncubatorChest~, Modifier:ExpeditionRelicModifierMarakethIncubatorElite~, Modifier:ExpeditionRelicModifierMetamorphCatalystsChest, Modifier:ExpeditionRelicModifierMetamorphCatalystsElite, Modifier:ExpeditionRelicModifierMonkeyTribeAbyssChest~, Modifier:ExpeditionRelicModifierMonkeyTribeAbyssElite, Modifier:ExpeditionRelicModifierMonkeyTribeFragmentsChest ~~ ~~, Modifier:ExpeditionRelicModifierMonkeyTribeFragmentsElite, Modifier:ExpeditionRelicModifierPackSize, Modifier:ExpeditionRelicModifierRareMonsterChance, Modifier:ExpeditionRelicModifierReducedDamageTaken~~, Modifier:ExpeditionRelicModifierResistancesAndMaxResistances, Modifier:ExpeditionRelicModifierSirensBreachChest~, Modifier:ExpeditionRelicModifierSirensBreachElite, Modifier:ExpeditionRelicModifierSirensScarabChest, Modifier:ExpeditionRelicModifierSirensScarabElite, Modifier:ExpeditionRelicModifierSpeed, Modifier:ExpeditionRelicModifierStackedDeckChest~, Modifier:ExpeditionRelicModifierStackedDeckElite~~, Modifier:ExpeditionRelicModifierTemplarCatalystChest~, Modifier:ExpeditionRelicModifierTemplarCatalystElite, Modifier:ExpeditionRelicModifierTemplarDeliriumChest ~~ ~~, Modifier:ExpeditionRelicModifierTemplarDeliriumElite, Modifier:ExpeditionRelicModifierVaalGemsChest~~, Modifier:ExpeditionRelicModifierVaalGemsElite~~, Modifier:ExpeditionRelicModifierVaalOilsChest, Modifier:ExpeditionRelicModifierVaalOilsElite, Modifier:ExpeditionRelicModifierWard,
