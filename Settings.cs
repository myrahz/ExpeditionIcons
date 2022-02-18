using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;

namespace ExpeditionIcons
{
    public class Settings : ISettings
    {
        public ToggleNode Enable { get; set; } = new ToggleNode(false);
        public ToggleNode DrawEliteMonstersInWorld { get; set; } = new ToggleNode(true);
        public ToggleNode DrawEliteMonstersOnMap { get; set; } = new ToggleNode(true);
        public ToggleNode DrawChestsInWorld { get; set; } = new ToggleNode(true);
        public ToggleNode DrawChestsOnMap { get; set; } = new ToggleNode(true);
        public ToggleNode CacheEntityPosition { get; set; } = new ToggleNode(true);

        [Menu("Good mods", 100)]
        public EmptyNode SettingsEmptyGood { get; set; }

        [Menu(nameof(DrawGoodModsOnMap), parentIndex = 100)]
        public ToggleNode DrawGoodModsOnMap { get; set; } = new ToggleNode(true);

        [Menu(nameof(DrawGoodModsInWorld), parentIndex = 100)]
        public ToggleNode DrawGoodModsInWorld { get; set; } = new ToggleNode(true);

        [Menu("Show monsters mods", parentIndex = 100)]
        public ToggleNode ShowMonsterMods { get; set; } = new ToggleNode(true);

        [Menu("Show logbook quantity", parentIndex = 100)]
        public ToggleNode ShowLogbooks { get; set; } = new ToggleNode(true);

        [Menu("Show artifact quantity", parentIndex = 100)]
        public ToggleNode ShowArtifacts { get; set; } = new ToggleNode(true);

        [Menu(nameof(ShowExpeditionRerollCurrency), parentIndex = 100)]
        public ToggleNode ShowExpeditionRerollCurrency { get; set; } = new ToggleNode(true);

        [Menu("Show basic currency", parentIndex = 100)]
        public ToggleNode ShowBasicCurrency { get; set; } = new ToggleNode(true);

        [Menu("Show stacked decks", parentIndex = 100)]
        public ToggleNode ShowStackedDecks { get; set; } = new ToggleNode(true);

        [Menu("Show item quantity", parentIndex = 100)]
        public ToggleNode ShowItemQuantity { get; set; } = new ToggleNode(true);

        [Menu(nameof(ShowLegion), parentIndex = 100)]
        public ToggleNode ShowLegion { get; set; } = new ToggleNode(true);

        [Menu(nameof(ShowUniques), parentIndex = 100)]
        public ToggleNode ShowUniques { get; set; } = new ToggleNode(true);

        [Menu(nameof(ShowEssences), parentIndex = 100)]
        public ToggleNode ShowEssences { get; set; } = new ToggleNode(true);

        [Menu(nameof(ShowGems), parentIndex = 100)]
        public ToggleNode ShowGems { get; set; } = new ToggleNode(true);

        [Menu(nameof(ShowRares), parentIndex = 100)]
        public ToggleNode ShowRares { get; set; } = new ToggleNode(true);

        [Menu(nameof(ShowEnchant), parentIndex = 100)]
        public ToggleNode ShowEnchant { get; set; } = new ToggleNode(true);

        [Menu(nameof(ShowScarabs), parentIndex = 100)]
        public ToggleNode ShowScarabs { get; set; } = new ToggleNode(true);

        [Menu(nameof(ShowBreach), parentIndex = 100)]
        public ToggleNode ShowBreach { get; set; } = new ToggleNode(true);

        [Menu(nameof(ShowInfluencedItems), parentIndex = 100)]
        public ToggleNode ShowInfluencedItems { get; set; } = new ToggleNode(true);

        [Menu(nameof(ShowMaps), parentIndex = 100)]
        public ToggleNode ShowMaps { get; set; } = new ToggleNode(true);

        [Menu(nameof(ShowFractured), parentIndex = 100)]
        public ToggleNode ShowFractured { get; set; } = new ToggleNode(true);

        [Menu(nameof(ShowHarbinger), parentIndex = 100)]
        public ToggleNode ShowHarbinger { get; set; } = new ToggleNode(true);

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
    }
}
