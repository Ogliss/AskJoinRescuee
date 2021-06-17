using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RescueeJoinPlus
{
	// Token: 0x02000006 RID: 6 Pawn_GuestTracker.Notify_PawnUndowned
	[HarmonyPatch(typeof(Pawn_GuestTracker), "Notify_PawnUndowned")]
	public static class RJP_Pawn_GuestTracker_Notify_PawnUndowned_RescueeJoin_Patch
	{
		// Token: 0x0600000C RID: 12 RVA: 0x00002C5C File Offset: 0x00000E5C
		public static bool Prefix(ref Pawn_GuestTracker __instance, Pawn ___pawn) 
		{
			if (___pawn.RaceProps.Humanlike && (__instance.HostFaction == Faction.OfPlayer || (___pawn.IsWildMan() && ___pawn.InBed() && ___pawn.CurrentBed().Faction == Faction.OfPlayer)) && !__instance.IsPrisoner && ___pawn.SpawnedOrAnyParentSpawned)
			{
				if (__instance.getRescuedThoughtOnUndownedBecauseOfPlayer && ___pawn.needs != null && ___pawn.needs.mood != null)
				{;
					___pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.Rescued, null);
				}
				if (___pawn.Faction == null || ___pawn.Faction.def.rescueesCanJoin /* || !___pawn.Faction.def.rescueesCanJoin */)
				{
					Map mapHeld = ___pawn.MapHeld;
					float num;
					if (!___pawn.SafeTemperatureRange().Includes(mapHeld.mapTemperature.OutdoorTemp) || mapHeld.gameConditionManager.ConditionIsActive(GameConditionDefOf.ToxicFallout))
					{
						num = 1f;
					}
					else
					{
						num = 0.5f;
					}
					if (Rand.ValueSeeded(___pawn.thingIDNumber ^ 8976612) < num)
					{

						DiaNode diaNode = new DiaNode(RJP_Pawn_GuestTracker_Notify_PawnUndowned_RescueeJoin_Patch.CreateRescueeText(___pawn));
						DiaOption diaOptionDetails = new DiaOption(Translator.Translate("ClickForMoreInfo"));
						diaOptionDetails.action = delegate ()
						{
							Find.WindowStack.Add(new Dialog_InfoCard(___pawn));
						};
						diaNode.options.Add(diaOptionDetails);
						DiaOption diaOption = new DiaOption(Translator.Translate("RescueeJoinAccept"));
						TaggedString ttexta = "LetterLabelRescueeJoins".Translate(___pawn.Named("PAWN"));
						TaggedString tlabela = "LetterRescueeJoins".Translate(___pawn.Named("PAWN"));
						PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref ttexta, ref tlabela, ___pawn);
						GenPersonStatText.AppendStats(ref ttexta, ___pawn);
						diaOption.action = delegate ()
						{
							___pawn.SetFaction(Faction.OfPlayer, null);
							Find.LetterStack.ReceiveLetter(tlabela, ttexta, LetterDefOf.PositiveEvent, ___pawn, null, null);
						};
						diaOption.resolveTree = true;
						diaNode.options.Add(diaOption);
						DiaOption diaOption2 = new DiaOption(Translator.Translate("RescueeJoinReject"));
						diaOption2.action = delegate ()
						{
							Messages.Message("MessageRescueeDidntJoin".Translate().AdjustedFor(___pawn, "PAWN", true), ___pawn, MessageTypeDefOf.NeutralEvent, true);
						};
						diaOption2.resolveTree = true;
						diaNode.options.Add(diaOption2);
						string text = TranslatorFormattedStringExtensions.Translate("RescueeJoinTitle", ___pawn.Map.Parent.Label);
						Find.WindowStack.Add(new Dialog_NodeTree(diaNode, true, true, text));
					}
					else
					{
						Log.Message("MessageRescueeDidntJoin".Translate().AdjustedFor(___pawn, "PAWN", true));
						Messages.Message("MessageRescueeDidntJoin".Translate().AdjustedFor(___pawn, "PAWN", true), ___pawn, MessageTypeDefOf.NeutralEvent, true);
					}
				}
				__instance.getRescuedThoughtOnUndownedBecauseOfPlayer = false;
				return false;
			}
			return true;
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002EE8 File Offset: 0x000010E8
		private static string CreateRescueeText(Pawn Rescuee)
		{
			TaggedString text = TranslatorFormattedStringExtensions.Translate(Main.AlienRaces_Active ? "RescueeJoinAlienDesc" : "RescueeJoinDesc", Rescuee.Name.ToStringFull, Rescuee.story.Title.ToLower(), Rescuee.ageTracker.AgeBiologicalYears.ToString(), Rescuee.def.label);

			text = GenText.AdjustedFor(GrammarResolverSimpleStringExtensions.Formatted(text, NamedArgumentUtility.Named(Rescuee, "PAWN")), Rescuee, "PAWN");
			if (Rescuee.gender!= Gender.None)
			{
				text += " " + Rescuee.gender.GetLabel();
			}
			text += ".";
			PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref text, Rescuee);
			GenPersonStatText.AppendStats(ref text, Rescuee);
			return text;
		}
	}
}
