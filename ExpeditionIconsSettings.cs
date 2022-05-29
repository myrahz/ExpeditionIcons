using System.Collections.Generic;
using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using SharpDX;

namespace ExpeditionIcons
{
    public class ExpeditionIconsSettings : ISettings
    {
        public int IconPickerSize = 20;
        public Dictionary<IconPickerIndex, IconDisplaySettings> IconMapping = new Dictionary<IconPickerIndex, IconDisplaySettings>();
        public ToggleNode Enable { get; set; } = new ToggleNode(false);
        public ToggleNode DrawEliteMonstersInWorld { get; set; } = new ToggleNode(true);
        public ToggleNode DrawEliteMonstersOnMap { get; set; } = new ToggleNode(true);
        public ToggleNode DrawChestsInWorld { get; set; } = new ToggleNode(true);
        public ToggleNode DrawChestsOnMap { get; set; } = new ToggleNode(true);
        public ToggleNode CacheEntityPosition { get; set; } = new ToggleNode(true);
        public RangeNode<int> WorldIconSize { get; set; } = new RangeNode<int>(50, 25, 200);
        public RangeNode<int> MapIconSize { get; set; } = new RangeNode<int>(30, 15, 200);

        [Menu("Good mods", 100)]
        public EmptyNode SettingsEmptyGood { get; set; }

        [Menu(nameof(DrawGoodModsOnMap), parentIndex = 100)]
        public ToggleNode DrawGoodModsOnMap { get; set; } = new ToggleNode(true);

        [Menu(nameof(DrawGoodModsInWorld), parentIndex = 100)]
        public ToggleNode DrawGoodModsInWorld { get; set; } = new ToggleNode(true);

        [Menu("Bad mods", 101)]
        public EmptyNode SettingsEmptyBad { get; set; }

        [Menu(nameof(DrawBadModsOnMap), parentIndex = 101)]
        public ToggleNode DrawBadModsOnMap { get; set; } = new ToggleNode(true);

        [Menu(nameof(DrawBadModsInWorld), parentIndex = 101)]
        public ToggleNode DrawBadModsInWorld { get; set; } = new ToggleNode(true);

        [Menu("Warn for physical immune", parentIndex = 101)]
        public ToggleNode PhysImmune { get; set; } = new ToggleNode(false);

        [Menu("Warn for fire immune", parentIndex = 101)]
        public ToggleNode FireImmune { get; set; } = new ToggleNode(false);

        [Menu("Warn for ignite immune", parentIndex = 101)]
        public ToggleNode IgniteImmune { get; set; } = new ToggleNode(false);

        [Menu("Warn for cold immune", parentIndex = 101)]
        public ToggleNode ColdImmune { get; set; } = new ToggleNode(false);

        [Menu("Warn for lightning immune", parentIndex = 101)]
        public ToggleNode LightningImmune { get; set; } = new ToggleNode(false);

        [Menu("Warn for chaos immune", parentIndex = 101)]
        public ToggleNode ChaosImmune { get; set; } = new ToggleNode(false);

        [Menu("Warn for crit immune", parentIndex = 101)]
        public ToggleNode CritImmune { get; set; } = new ToggleNode(false);

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


        [Menu("Explosive settings", 102)]
        public EmptyNode ExplosivesSettings { get; set; }
        [Menu("Show explosive radius", parentIndex = 102)]
        public ToggleNode ShowExplosives { get; set; } = new ToggleNode(true);
        [Menu("Color for explosive radius", parentIndex = 102)]
        public ColorNode ExplosiveColor { get; set; } = new ColorNode(Color.Red);
        [Menu("Map Explosive radius", parentIndex = 102)]
        public RangeNode<int> MapExplosiveRadius { get; set; } = new RangeNode<int>(310, 10, 600);
        [Menu("Logbook Explosive radius", parentIndex = 102)]
        public RangeNode<int> LogbookExplosiveRadius { get; set; } = new RangeNode<int>(310, 10, 600);
        [Menu("Merge explosive radii", parentIndex = 102)]
        public ToggleNode EnableExplosiveRadiusMerging { get; set; } = new ToggleNode(true);
    }
}
