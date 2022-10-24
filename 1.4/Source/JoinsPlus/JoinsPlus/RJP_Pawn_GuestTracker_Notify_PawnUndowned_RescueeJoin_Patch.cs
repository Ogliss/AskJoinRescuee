using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RescueeJoinPlus
{
	[HarmonyPatch(typeof(Pawn_GuestTracker), "Notify_PawnUndowned")]
	public static class RJP_Pawn_GuestTracker_Notify_PawnUndowned_RescueeJoin_Patch
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var instructionsList = new List<CodeInstruction>(instructions);
			FieldInfo pawn = AccessTools.Field(typeof(Pawn_GuestTracker), "pawn");
			bool act = false;
			bool replace = false;
			// replace entire section where its decided if the pawn joins or leaves
			for (int i = 0; i < instructionsList.Count; i++)
			{
				CodeInstruction instruction = instructionsList[i];
				if (!act)
				{
					if (instruction.opcode == OpCodes.Bge_Un_S)
					{
						yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RJP_Pawn_GuestTracker_Notify_PawnUndowned_RescueeJoin_Patch), "AskToJoin"));
						instruction = new CodeInstruction(OpCodes.Brfalse, instruction.operand);
					}
					if (i > 1 && instructionsList[i - 1].opcode == OpCodes.Bge_Un_S)
					{
						replace = true;
					}
					if (replace && instruction.opcode == OpCodes.Br_S)
					{
						yield return new CodeInstruction(OpCodes.Ldarg_0); // Pawn_GuestTracker
						yield return new CodeInstruction(OpCodes.Ldfld, pawn);
						yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RJP_Pawn_GuestTracker_Notify_PawnUndowned_RescueeJoin_Patch), "DoRescueDialog"));
						act = true;
						replace = false;
					}
					if (replace)
					{
						continue;
					}
				}
				yield return instruction;
			}

		}

		public static bool AskToJoin(float A, float B)
		{
			if (DebugSettings.godMode || A < B)
			{
				return true;
			}
			return false;
		}

		public static void DoRescueDialog(Pawn pawn)
		{
			DiaNode diaNode = new DiaNode(GenPersonStatText.CreateRescueeText(pawn));
			DiaOption diaOptionDetails = new DiaOption(Translator.Translate("ClickForMoreInfo"));
			diaOptionDetails.action = delegate ()
			{
				Find.WindowStack.Add(new Dialog_InfoCard(pawn));
			};
			diaNode.options.Add(diaOptionDetails);
			DiaOption diaOption = new DiaOption(Translator.Translate("RescueeAskJoin.Accept"));
			TaggedString ttexta = "LetterLabelRescueeJoins".Translate(pawn.Named("PAWN"));
			TaggedString tlabela = "LetterRescueeJoins".Translate(pawn.Named("PAWN"));
			PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref ttexta, ref tlabela, pawn);
			GenPersonStatText.AppendStats(ref ttexta, pawn);
			diaOption.action = delegate ()
			{
				pawn.SetFaction(Faction.OfPlayer, null);
				Find.LetterStack.ReceiveLetter(tlabela, ttexta, LetterDefOf.PositiveEvent, pawn, null, null);
			};
			diaOption.resolveTree = true;
			diaNode.options.Add(diaOption);
			DiaOption diaOption2 = new DiaOption(Translator.Translate("RescueeAskJoin.Reject"));
			diaOption2.action = delegate ()
			{
				Messages.Message("RescueeAskJoin.MessageRescueeDidntJoin".Translate().AdjustedFor(pawn, "PAWN", true), pawn, MessageTypeDefOf.NeutralEvent, true);
			};
			diaOption2.resolveTree = true;
			diaNode.options.Add(diaOption2);
			string text = TranslatorFormattedStringExtensions.Translate("RescueeAskJoin.WindowTitle", pawn.Map.Parent.Label);
			Find.WindowStack.Add(new Dialog_NodeTree(diaNode, true, true, text));
		}

	}
}
