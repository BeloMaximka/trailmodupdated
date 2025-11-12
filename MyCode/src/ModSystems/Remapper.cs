using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace TrailModUpdated
{
    public class RemapTrailMod : ModSystem
    {
        ICoreServerAPI sapi;

        public override void StartServerSide(ICoreServerAPI api)
        {
            sapi = api;

            sapi.ChatCommands.Create("trailremap")
                .WithDescription("Remap Trail Mod blocks from trailmod -> trailmodupdated.")
                .RequiresPrivilege(Privilege.controlserver)
                .HandleWith(OnRunRemap);
        }

        private TextCommandResult OnRunRemap(TextCommandCallingArgs args)
        {
            int count = 0;
            void Exec(string cmd) { sapi.InjectConsole(cmd); count++; }

            // --- Cairns (unchanged from before) ---
            string[] stones = {
                "andesite","basalt","bauxite","chalk","chert","claystone","conglomerate",
                "granite","greenmarble","halite","kimberlite","limestone","obsidian",
                "peridotite","phyllite","redmarble","sandstone","scoria","shale","slate",
                "suevite","tuff","whitemarble"
            };
            foreach (var rock in stones)
            {
                Exec($"/bir remapq trailmodupdated:cairn-{rock}-free  trailmod:cairn-{rock}-free  force");
                Exec($"/bir remapq trailmodupdated:cairn-{rock}-ice   trailmod:cairn-{rock}-ice   force");
                Exec($"/bir remapq trailmodupdated:cairn-{rock}-snow  trailmod:cairn-{rock}-snow  force");
                Exec($"/bir remapq trailmodupdated:cairn-{rock}-water trailmod:cairn-{rock}-water force");
            }

            // --- Soil pretrail (new -> old, non-prefixed) ---
            string[] fertPretrail = { "compost", "high", "low", "medium", "verylow" };
            foreach (var f in fertPretrail)
                Exec($"/bir remapq trailmodupdated:soil-{f}-pretrail trailmod:soil-{f}-pretrail force");

            // --- Soil pretrail (new -> old, *prefixed* legacy codes) ---
            // Fixes 'unknown block trailmod:block-soil-<fert>-pretrail'
            foreach (var f in fertPretrail)
                Exec($"/bir remapq trailmodupdated:soil-{f}-pretrail trailmod:block-soil-{f}-pretrail force");

            // --- Trails (wear states x fertility tiers including compost) ---
            string[] ferts = { "compost", "high", "low", "medium", "verylow" };
            string[] wears = { "new", "established", "veryestablished", "old" };
            foreach (var f in ferts)
                foreach (var w in wears)
                    Exec($"/bir remapq trailmodupdated:trail-{f}-{w} trailmod:trail-{f}-{w} force");

            var sp = args.Caller.Player as IServerPlayer;
            sapi.SendIngameDiscovery(sp, "trailremap-done",
                $"TrailModUpdated: executed {count} remap lines. Now /save and reload the world/server.");

            return TextCommandResult.Success($"Executed {count} remap commands. Please /save and reload the world or restart the server.");
        }
    }
}
