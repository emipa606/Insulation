using Verse;

namespace Insulation
{
    // Token: 0x0200000A RID: 10
    internal class RoofValues : DefModExtension
    {
        // Token: 0x04000008 RID: 8
        public float InsulationFactor = 1f;

        // Token: 0x06000015 RID: 21 RVA: 0x00002884 File Offset: 0x00000A84
        internal static float GetRVInsulationFactor(RoofDef rdef)
        {
            var RIFactor = 1f;
            if (rdef.HasModExtension<RoofValues>())
            {
                RIFactor = rdef.GetModExtension<RoofValues>().InsulationFactor;
            }

            return RIFactor;
        }
    }
}