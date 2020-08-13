using System;
using UnityEngine;
using Verse;

namespace Insulation
{
	// Token: 0x02000005 RID: 5
	public class Controller : Mod
	{
		// Token: 0x06000006 RID: 6 RVA: 0x000020BA File Offset: 0x000002BA
		public override string SettingsCategory()
		{
			return Translator.Translate("Insulation.Name");
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000020CB File Offset: 0x000002CB
		public override void DoSettingsWindowContents(Rect canvas)
		{
			Controller.Settings.DoWindowContents(canvas);
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000020D8 File Offset: 0x000002D8
		public Controller(ModContentPack content) : base(content)
		{
			Controller.Settings = base.GetSettings<Settings>();
		}

		// Token: 0x04000007 RID: 7
		public static Settings Settings;
	}
}
