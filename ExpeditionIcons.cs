using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.Elements;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using ImGuiNET;
using SharpDX;
using Vector2 = SharpDX.Vector2;

namespace ExpeditionIcons;

public class ExpeditionIcons : BaseSettingsPlugin<ExpeditionIconsSettings>
{
    private const string TextureName = "Icons.png";
    private const double CameraAngle = 38.7 * Math.PI / 180;
    private static readonly float CameraAngleCos = (float)Math.Cos(CameraAngle);
    private static readonly float CameraAngleSin = (float)Math.Sin(CameraAngle);
    private const float GridToWorldMultiplier = 250 / 23f;
    private const int ExplosiveBaseRadius = 30;
    private double _mapScale;
    private Vector2 _mapCenter;
    private bool _largeMapOpen;
    private readonly ConditionalWeakTable<Entity, string> _baseAnimatedEntityMetadata = new ConditionalWeakTable<Entity, string>();
    private readonly ConditionalWeakTable<Entity, EntityPosWrapper> _entityPos = new ConditionalWeakTable<Entity, EntityPosWrapper>();
    private Vector2 _playerPos;
    private float _playerZ;
    private readonly ConcurrentDictionary<string, List<ExpeditionMarkerIconDescription>> _relicModIconMapping = new();
    private readonly ConcurrentDictionary<string, ExpeditionMarkerIconDescription> _metadataIconMapping = new();

    private Camera Camera => GameController.Game.IngameState.Camera;

    public override bool Initialise()
    {
        Graphics.InitImage(TextureName);
        Settings._iconsImageId = Graphics.GetTextureId(TextureName);
        return base.Initialise();
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

    private void DrawCirclesInWorld(List<Vector3> positions, float radius, Color color)
    {
        const int segments = 90;
        const int segmentSpan = 360 / segments;
        foreach (var position in positions)
        {
            foreach (var segmentId in Enumerable.Range(0, segments))
            {
                (Vector2, Vector2) GetVector(int i)
                {
                    var p = position + new Vector3((float)(Math.Cos(Math.PI / 180 * i) * radius), (float)(Math.Sin(Math.PI / 180 * i) * radius), 0);
                    var vector2 = Camera.WorldToScreen(p);
                    return (new Vector2(p.X, p.Y), vector2);
                }

                var segmentOrigin = segmentId * segmentSpan;
                var (w1, c1) = GetVector(segmentOrigin);
                var (w2, c2) = GetVector(segmentOrigin + segmentSpan);
                if (Settings.EnableExplosiveRadiusMerging)
                {
                    if (positions
                        .Where(x => x != position)
                        .Select(x => new Vector2(x.X, x.Y))
                        .Any(x => Vector2.Distance(w1, x) < radius &&
                                  Vector2.Distance(w2, x) < radius))
                    {
                        continue;
                    }
                }

                ImGui.GetForegroundDrawList().AddLine(c1.ToVector2Num(), c2.ToVector2Num(), color.ToImgui());
            }
        }
    }

    public override void Render()
    {
        Settings._iconsImageId = Graphics.GetTextureId(TextureName);
        var ingameUi = GameController.Game.IngameState.IngameUi;
        var map = ingameUi.Map;
        var largeMap = map.LargeMap.AsObject<SubMap>();
        _largeMapOpen = largeMap.IsVisible;
        _mapScale = GameController.IngameState.Camera.Height / 677f * largeMap.Zoom;
        _mapCenter = largeMap.GetClientRect().TopLeft + largeMap.Shift + largeMap.DefaultShift;
        _playerPos = GameController.Player.GetComponent<Positioned>().GridPos;
        _playerZ = GameController.Player.GetComponent<Render>().Z;

        const string markerPath = "Metadata/MiscellaneousObjects/Expedition/ExpeditionMarker";
        const string explosivePath = "Metadata/MiscellaneousObjects/Expedition/ExpeditionExplosive";
        const string relicPath = "Metadata/MiscellaneousObjects/Expedition/ExpeditionRelic";
        var explosiveRadius = Settings.AutoCalculateRadius
                                  //ReSharper disable once PossibleLossOfFraction
                                  //rounding here is extremely important to get right, this is taken from the game's code
                                  ? ExplosiveBaseRadius * (100 + GameController.IngameState.Data.MapStats.GetValueOrDefault(GameStat.MapExpeditionExplosionRadiusPct)) / 100 * GridToWorldMultiplier
                                  : Settings.ExplosiveRadius.Value;
        var explosives = GameController.EntityListWrapper.ValidEntitiesByType[EntityType.IngameIcon]
           .Where(x => x.Path == explosivePath)
           .Select(x => x.Pos)
           .ToList();
        var explosives2D = explosives.Select(x => new Vector2(x.X, x.Y)).ToList();
        if (Settings.ShowExplosives)
        {
            DrawCirclesInWorld(
                positions: explosives,
                radius: explosiveRadius,
                color: Settings.ExplosiveColor.Value);
        }

        foreach (var e in GameController.EntityListWrapper.ValidEntitiesByType[EntityType.IngameIcon].OrderBy(x => x.Path != markerPath))
        {
            switch (e.Path)
            {
                case markerPath:
                {
                    var animatedMetaData = GetOrAdd(_baseAnimatedEntityMetadata, e, ee => ee.GetComponent<Animated>()?.BaseAnimatedObjectEntity?.Metadata);
                    if (animatedMetaData != null)
                    {
                        if (animatedMetaData.Contains("elitemarker"))
                        {
                            var mapSettings = Settings.IconMapping.GetValueOrDefault(IconPickerIndex.EliteMonstersIndicator, new IconDisplaySettings());
                            if (mapSettings.ShowOnMap)
                            {
                                DrawIconOnMap(e, mapSettings.Icon ?? ExpeditionIconsSettings.DefaultEliteMonsterIcon, mapSettings.Tint, Vector2.Zero);
                            }
                            
                            if (mapSettings.ShowInWorld)
                            {
                                DrawIconInWorld(e, mapSettings.Icon ?? ExpeditionIconsSettings.DefaultEliteMonsterIcon, mapSettings.Tint, Vector2.Zero, explosives2D,
                                    explosiveRadius);
                            }
                        }
                        else
                        {
                            var iconDescription = _metadataIconMapping.GetOrAdd(animatedMetaData,
                                a =>
                                    Icons.LogbookChestIcons.FirstOrDefault(icon =>
                                        icon.BaseEntityMetadataSubstrings.Any(a.Contains)));
                            if (iconDescription != null)
                            {
                                var settings = Settings.IconMapping.GetValueOrDefault(iconDescription.IconPickerIndex, new IconDisplaySettings());
                                var icon = settings.Icon ?? iconDescription.DefaultIcon;
                                if (settings.ShowOnMap)
                                {
                                    DrawIconOnMap(e, icon, settings.Tint, Vector2.Zero);
                                }

                                if (settings.ShowInWorld)
                                {
                                    DrawIconInWorld(e, icon, settings.Tint, Vector2.Zero, explosives2D, explosiveRadius);
                                }
                            }
                        }
                    }

                    continue;
                }
                case relicPath:
                {
                    var renderComponent = e.GetComponent<Render>();
                    if (renderComponent == null) continue;
                    var expeditionChestComponent = e.GetComponent<ObjectMagicProperties>();
                    if (expeditionChestComponent == null) continue;
                    var mods = expeditionChestComponent.Mods;
                    if (!e.TryGetComponent<MinimapIcon>(out var iconComponent) || iconComponent.IsHide) continue;
                    if (!mods.Any(x => x.Contains("ExpeditionRelicModifier"))) continue;

                    if (Settings.WarnPhysImmune.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierImmunePhysicalDamage")) ||
                        Settings.WarnFireImmune.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierImmuneFireDamage")) ||
                        Settings.WarnColdImmune.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierImmuneColdDamage")) ||
                        Settings.WarnLightningImmune.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierImmuneLightningDamage")) ||
                        Settings.WarnChaosImmune.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierImmuneChaosDamage")) ||
                        Settings.WarnCritImmune.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierCannotBeCrit")) ||
                        Settings.WarnIgniteImmune.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierImmuneStatusAilments")) ||
                        Settings.WarnArmorPen.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierIgnoreArmour")) ||
                        Settings.WarnNoEvade.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierHitsCannotBeEvaded")) ||
                        Settings.WarnNoLeech.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierCannotBeLeechedFrom")) ||
                        Settings.WarnNoFlask.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierGrantNoFlaskCharges")) ||
                        Settings.WarnPetrify.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierElitesPetrifyOnHit")) ||
                        Settings.WarnCurseImmune.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierImmuneToCurses")) ||
                        Settings.WarnCull.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierCullingStrikeTwentyPercent")) ||
                        Settings.WarnMonsterBlock.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierAttackBlockSpellBlockMaxBlockChance")) ||
                        Settings.WarnMonsterResist.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierResistancesAndMaxResistances")) ||
                        Settings.WarnMonsterRegen.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierElitesRegenerateLifeEveryFourSeconds"))
                       )
                    {

                        var mapSettings = Settings.IconMapping.GetValueOrDefault(IconPickerIndex.BadModsIndicator, new IconDisplaySettings());
                        if (mapSettings.ShowOnMap)
                        {
                            DrawIconOnMap(e, mapSettings.Icon ?? ExpeditionIconsSettings.DefaultBadModsIcon, mapSettings.Tint, Vector2.Zero);
                        }
                        
                        if (mapSettings.ShowInWorld)
                        {
                            DrawIconInWorld(e, mapSettings.Icon ?? ExpeditionIconsSettings.DefaultBadModsIcon, mapSettings.Tint, -Vector2.UnitY, explosives2D, explosiveRadius);
                        }

                        continue;
                    }

                    if (Settings.DrawGoodModsInWorld || Settings.DrawGoodModsOnMap)
                    {
                        var worldIcons = new HashSet<(MapIconsIndex, Color?)>();
                        var mapIcons = new HashSet<(MapIconsIndex, Color?)>();
                        var iconDescriptions = mods.SelectMany(mod =>
                            _relicModIconMapping.GetOrAdd(mod, s =>
                                Icons.ExpeditionRelicIcons.Where(icon =>
                                    icon.BaseEntityMetadataSubstrings.Any(s.Contains)).ToList())).Distinct();
                        foreach (var iconDescription in iconDescriptions)
                        {
                            var settings = Settings.IconMapping.GetValueOrDefault(iconDescription.IconPickerIndex, new IconDisplaySettings());
                            var icon = settings.Icon ?? iconDescription.DefaultIcon;
                            if (settings.ShowOnMap)
                            {
                                mapIcons.Add((icon, settings.Tint));
                            }

                            if (settings.ShowInWorld)
                            {
                                worldIcons.Add((icon, settings.Tint));
                            }
                        }

                        var offset = new Vector2(-worldIcons.Count * 0.5f + 0.5f, 0);
                        foreach (var (icon, tint) in worldIcons)
                        {
                            if (Settings.DrawGoodModsInWorld)
                            {
                                DrawIconInWorld(e, icon, tint, offset, explosives2D, explosiveRadius);
                            }

                            offset += Vector2.UnitX;
                        }

                        offset = new Vector2(-mapIcons.Count * 0.5f + 0.5f, 0);
                        foreach (var (icon, tint) in mapIcons)
                        {
                            if (Settings.DrawGoodModsOnMap)
                            {
                                DrawIconOnMap(e, icon, tint, offset);
                            }

                            offset += Vector2.UnitX;
                        }
                    }

                    break;
                }
            }
        }
    }

    private void DrawIconOnMap(Entity entity, MapIconsIndex icon, Color? color, Vector2 offset)
    {
        if (_largeMapOpen)
        {
            float halfsize = Settings.MapIconSize / 2.0f;
            var point = GetEntityPosOnMapScreen(entity) + offset * halfsize * 2;
            var rect = new RectangleF(point.X, point.Y, 0, 0);
            rect.Inflate(halfsize, halfsize);
            Graphics.DrawImage(TextureName, rect, SpriteHelper.GetUV(icon), color ?? Color.White);
        }
    }

    private void DrawIconInWorld(Entity entity, MapIconsIndex icon, Color? color, Vector2 offset, List<Vector2> explosivePositions, float explosiveRadius)
    {
        float halfsize = Settings.WorldIconSize / 2.0f;
        var entityPos = GetEntityInfo(entity).Pos;
        var entityPos2 = new Vector2(entityPos.X, entityPos.Y);
        var point = Camera.WorldToScreen(entityPos) + offset * halfsize * 2;
        var rect = new RectangleF(point.X, point.Y, 0, 0);
        rect.Inflate(halfsize, halfsize);
        if (Settings.MarkCapturedEntities && explosivePositions.Any(x => Vector2.Distance(x, entityPos2) < explosiveRadius))
        {
            Graphics.DrawFrame(rect, Settings.MarkCapturedEntitiesColor, Settings.MarkCapturedEntitiesFrameThickness);
        }

        Graphics.DrawImage(TextureName, rect, SpriteHelper.GetUV(icon), color ?? Color.White);
    }

    private Vector2 GetEntityPosOnMapScreen(Entity entity)
    {
        var (_, gridPos, iconZ) = GetEntityInfo(entity);
        var point = _mapCenter + TranslateGridDeltaToMapDelta(gridPos - _playerPos, iconZ - _playerZ);
        return point;
    }

    private Vector2 TranslateGridDeltaToMapDelta(Vector2 delta, float deltaZ)
    {
        deltaZ /= GridToWorldMultiplier; //z is normally "world" units, translate to grid
        return (float)_mapScale * new Vector2((delta.X - delta.Y) * CameraAngleCos, (deltaZ - (delta.X + delta.Y)) * CameraAngleSin);
    }

    private EntityPosWrapper GetEntityInfo(Entity entity)
    {
        return Settings.CacheEntityPosition
                   ? GetOrAdd(_entityPos, entity, e => new EntityPosWrapper(e.Pos, e.GridPos, e.GetComponent<Render>()?.Z ?? 0))
                   : new EntityPosWrapper(entity.Pos, entity.GridPos, entity.GetComponent<Render>()?.Z ?? 0);
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