using System;
using UnityEngine;
using Verse;

namespace Insulation
{
	// Token: 0x0200000B RID: 11
	public class Settings : ModSettings
	{
		// Token: 0x06000017 RID: 23 RVA: 0x000028C0 File Offset: 0x00000AC0
		public void DoWindowContents(Rect canvas)
		{
			Listing_Standard listing_Standard = new Listing_Standard();
			listing_Standard.ColumnWidth = canvas.width;
			listing_Standard.Begin(canvas);
			listing_Standard.Gap(12f);
			listing_Standard.Label(Translator.Translate("Insulation.pctEffective") + "  " + this.pctEffective, -1f, null);
			this.pctEffective = checked((int)listing_Standard.Slider((float)this.pctEffective, 50f, 200f));
			listing_Standard.Gap(12f);
			listing_Standard.End();
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002957 File Offset: 0x00000B57
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.pctEffective, "pctEffective", 100, false);
		}

		// Token: 0x04000009 RID: 9
		public int pctEffective = 100;
	}
}
