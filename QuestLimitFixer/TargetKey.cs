using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;

namespace QuestLimitFixer
{
    public static class TargetKey
    {
        private readonly static ModKey ModKey = ModKey.FromNameAndExtension(Constants.PluginName);
        private readonly static FormKey FormKey = ModKey.MakeFormKey(Constants.FormListID);

        public static bool TryResolveFormList(ILinkCache linkCache, [MaybeNullWhen(false)] out IFormListGetter formlist)
        {
            return linkCache.TryResolve(FormKey, out formlist);
        }
    }
}
