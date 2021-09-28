using System;
using System.Linq;
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
            foreach ( var fl in state.LoadOrder.PriorityOrder.FormList().WinningOverrides().Where(fl => fl.FormKey.ModKey == Constants.PluginKey) )
            {
                var flCopy = fl.DeepCopy();
                var count = flCopy.Items.Count;
                foreach ( var quest in state.LoadOrder.PriorityOrder.Quest().WinningOverrides() )
                {
                    if ( !flCopy.Items.Contains(quest) && quest.Objectives.Count > 0 && quest.EditorID != null && quest.Name != null )
                    {
                        flCopy.Items.Add(quest);
                    }
                }
                count = flCopy.Items.Count - count;
                if ( count > 0 )
                {
                    state.PatchMod.FormLists.Set(flCopy);
                    Console.WriteLine($"Added {count} quests to the list.");
                }
                else
                {
                    Console.WriteLine($"FormList size changed by {count}.");
                }
                return;
            }
            Console.WriteLine("Failed, couldn't find target FormList.");
            //foreach ( var quest in state.LoadOrder.PriorityOrder.Quest().WinningOverrides() )
            //{
            //    if ( quest.EditorID == null || quest.Name == null )
            //        continue;
            //    if ( quest.Objectives.Count > 0 )
            //    {
            //        list.Items.Add(quest);
            //    }
            //}

            //   state.PatchMod.FormLists.GetOrAddAsOverride(list);
            //   Console.WriteLine($"Added {counter} quests to the list.");
        }
    }
}
