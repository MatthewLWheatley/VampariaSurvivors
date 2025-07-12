using Terraria;
using Terraria.ID;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace VampariaSurvivors.Content
{
    public class VSGlobalNPC : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.EyeofCthulhu)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.VamparieChest>(), 2));
            }
        }
    }
}