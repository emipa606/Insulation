using System.Reflection;
using HarmonyLib;
using Verse;

namespace Insulation;

[StaticConstructorOnStartup]
internal static class HarmonyPatching
{
    static HarmonyPatching()
    {
        new Harmony("com.Pelador.Rimworld.Insulation").PatchAll(Assembly.GetExecutingAssembly());
    }
}