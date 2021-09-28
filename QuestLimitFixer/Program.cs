using System;
using System.Linq;
using System.Threading.Tasks;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
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
            if ( TargetKey.TryResolveFormList(state.LinkCache, out var formlist) )
            {
                // Copy the default formlist
                var formlistCopy = formlist.DeepCopy();
                // iterate through all non-duplicate quests
                foreach ( var quest in state.LoadOrder.PriorityOrder.Quest().WinningOverrides().Where(q => !formlist.Items.Contains(q)) )
                {
                    if ( quest.Objectives.Count == 0 || quest.EditorID == null || quest.Name == null )
                        continue;
                    formlistCopy.Items.Add(quest);
                }
                // get the number of additions
                var count = formlistCopy.Items.Count - formlist.Items.Count;
                if ( count > 0 ) // changes were made
                {
                    state.PatchMod.FormLists.Set(formlistCopy); // add formlistCopy as an override to the patch
                    Console.WriteLine($"Successfully added {count} quests to the patch.");
                }
                else // no changes made
                {
                    Console.WriteLine("Patcher didn't find any additional quests to add.");
                }
            }
            else
            {
                throw new Exception($"Couldn't resolve target formlist, is \"{Constants.PluginName}\" installed?");
            }
        }
    }
}
