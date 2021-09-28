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
                .SetTypicalOpen(GameRelease.SkyrimSE, "Quest Journal Limit Bug Fixer.esp")
                .Run(args)
                .ConfigureAwait(false);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            int counter = 0;

            FormList list = new(state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease);
            list.EditorID = Constants.FormListEditorID;

            foreach ( var quest in state.LoadOrder.ListedOrder.Quest().WinningOverrides() )
            {
                if ( quest.EditorID == null || quest.Name == null )
                    continue;
                if ( quest.Objectives.Count > 0 )
                {
                    list.Items.Add(quest);
                    ++counter;
                }
            }

            state.PatchMod.FormLists.GetOrAddAsOverride(list);
            Console.WriteLine($"Added {counter} quests to the list.");
        }
    }
}
