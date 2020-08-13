using System;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace Insulation
{
	// Token: 0x02000008 RID: 8
	[StaticConstructorOnStartup]
	internal static class HarmonyPatching
	{
		// Token: 0x0600000D RID: 13 RVA: 0x00002364 File Offset: 0x00000564
		static HarmonyPatching()
		{
			new Harmony("com.Pelador.Rimworld.Insulation").PatchAll(Assembly.GetExecutingAssembly());
		}
	}
}
