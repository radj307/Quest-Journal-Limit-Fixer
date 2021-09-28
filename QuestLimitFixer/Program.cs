using System;
using System.Threading.Tasks;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;

namespace QuestLimitFixer
{
    public static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetTypicalOpen(GameRelease.SkyrimSE, Constants.PluginName)
                .Run(args)
                .ConfigureAwait(false);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            // Create a FormList and set its Editor ID
            FormList list = new(Constants.FormListKey, state.PatchMod.SkyrimRelease)
            {
                EditorID = Constants.FormListEditorID
            };

            // iterate through winning quest overrides
            foreach ( var quest in state.LoadOrder.ListedOrder.Quest().WinningOverrides() )
            {
                if ( quest.EditorID == null || quest.Name == null ) // skip quests with null editor IDs / names
                    continue;

                if ( quest.Objectives.Count > 0 ) // if quest has objectives, add it to the list.
                    list.Items.Add(quest);
            }

            SkyrimMod mod = new(Constants.PluginKey, state.PatchMod.SkyrimRelease);
            mod.FormLists.GetOrAddAsOverride(list);
            // Add the FormList to the target patcher
            //state.PatchMod.FormLists.GetOrAddAsOverride(list);

            Console.WriteLine($"Added {list.Items.Count} quests to the list.");
        }
    }
}
