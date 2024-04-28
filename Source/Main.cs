using Verse.AI;
using HarmonyLib;
using Verse;
using RimWorld;

namespace BotsChargeWhenIdle
{
    [StaticConstructorOnStartup]
    public static class Main
    {
        static Main()
        {
            Harmony instance = new Harmony("doug.BotsChargeWhenIdle");
            instance.PatchAll();
        }
    }

    [HarmonyPatch(typeof(JobGiver_Wander), "TryGiveJob", null)]
    public static class JobMaker_Patch
    {
        public static void Postfix(Pawn pawn, ref Job __result)
        {
            if (pawn.IsColonyMech && pawn.GetMechWorkMode() == MechWorkModeDefOf.Work && pawn.kindDef.defName != "Mech_Militor")
            {
                Building_MechCharger charger = JobGiver_GetEnergy_Charger.GetClosestCharger(pawn, pawn, false);
                if (charger != null && pawn.needs.energy.CurLevelPercentage < pawn.GetMechControlGroup().mechRechargeThresholds.max)
                {
                    __result = JobMaker.MakeJob(JobDefOf.MechCharge, charger);
                }
                else
                {
                    RCellFinder.TryFindNearbyMechSelfShutdownSpot(pawn.Position, pawn, pawn.Map, out var result, allowForbidden: true);
                    __result = JobMaker.MakeJob(JobDefOf.SelfShutdown, result);
                    __result.expiryInterval = 625;
                }
            }
        }
    }
}