using Verse;

namespace Insulation
{
    // Token: 0x02000003 RID: 3
    public class CompProperties_BuildInsulation : CompProperties
    {
        // Token: 0x04000004 RID: 4
        public bool FuelNeeded;

        // Token: 0x04000001 RID: 1
        public float HeatTransferFactor = 1f;

        // Token: 0x04000006 RID: 6
        public float HitPointsNeed;

        // Token: 0x04000002 RID: 2
        public bool InsulationStuff;

        // Token: 0x04000003 RID: 3
        public bool PowerNeeded;

        // Token: 0x04000005 RID: 5
        public bool RepairNeeded;

        // Token: 0x06000003 RID: 3 RVA: 0x00002082 File Offset: 0x00000282
        public CompProperties_BuildInsulation()
        {
            compClass = typeof(CompBuildInsulate);
        }
    }
}