using System;
using Verse;

namespace Insulation
{
	// Token: 0x02000004 RID: 4
	public class CompBuildInsulate : ThingComp
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000004 RID: 4 RVA: 0x000020A5 File Offset: 0x000002A5
		public CompProperties_BuildInsulation Props
		{
			get
			{
				return (CompProperties_BuildInsulation)this.props;
			}
		}
	}
}
