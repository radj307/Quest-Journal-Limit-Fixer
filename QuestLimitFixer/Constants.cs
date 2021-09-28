using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;

namespace QuestLimitFixer
{
    public readonly struct Constants
    {
        public const string FormListEditorID = "AllVanillaQuests";
        public const string PluginName = "Quest Journal Limit Bug Fixer.esp";
        public static ModKey PluginKey = ModKey.FromNameAndExtension(PluginName);
        public static FormKey TargetKey = PluginKey.MakeFormKey(0x001800);
    }
}
