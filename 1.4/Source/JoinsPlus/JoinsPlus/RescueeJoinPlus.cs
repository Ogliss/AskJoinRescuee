using System;
using System.Collections.Generic;
using Verse;
using RimWorld;
using HarmonyLib;
using RimWorld.Planet;
using System.Reflection;
using System.Linq;

namespace RescueeJoinPlus
{
	[StaticConstructorOnStartup]
	public class Main
	{
		public static bool AlienRaces_Active;
		static Main()
		{
			AlienRaces_Active = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == "erdelf.HumanoidAlienRaces");
			var harmony = new Harmony("com.ogliss.rimworld.mod.RescueeJoinPlus");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
			if (Prefs.DevMode) Log.Message(string.Format("RescueeJoinPlus: successfully completed {0} harmony patches.", harmony.GetPatchedMethods().Select(new Func<MethodBase, Patches>(Harmony.GetPatchInfo)).SelectMany((Patches p) => p.Prefixes.Concat(p.Postfixes).Concat(p.Transpilers)).Count((Patch p) => p.owner.Contains(harmony.Id))));
		}
	}

}
