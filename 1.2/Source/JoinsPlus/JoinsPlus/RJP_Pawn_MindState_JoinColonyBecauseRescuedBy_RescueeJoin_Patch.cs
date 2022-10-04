using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace RescueeJoinPlus
{
	[HarmonyPatch(typeof(Pawn_MindState), "JoinColonyBecauseRescuedBy")]
	public static class RJP_Pawn_MindState_JoinColonyBecauseRescuedBy_RescueeJoin_Patch
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var instructionsList = new List<CodeInstruction>(instructions);
			MethodInfo tryRecruit = AccessTools.Method(typeof(InteractionWorker_RecruitAttempt), "DoRecruit", new Type[] {typeof(Pawn),typeof(Pawn),typeof(float),typeof(bool)});
			// add dialog to allow player to choose if the pawn should join or not
            for (int i = 0; i < instructionsList.Count; i++)
            { 
				CodeInstruction instruction = instructionsList[i];
				if (instruction.OperandIs(tryRecruit))
				{
					instruction = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RJP_Pawn_MindState_JoinColonyBecauseRescuedBy_RescueeJoin_Patch), "DoRescueDialog"));
				}
				yield return instruction;
			}

		}

		static TaggedString ttexta;
		static TaggedString tlabela;
		public static void DoRescueDialog(Pawn by, Pawn pawn, float recruitChance, bool useAudiovisualEffects = true)
        {
			ttexta = null;
			tlabela = null;
			DiaNode diaNode = new DiaNode(GenPersonStatText.CreateRescueeText(pawn));
			DiaOption diaOptionDetails = new DiaOption(Translator.Translate("ClickForMoreInfo"));
			diaOptionDetails.action = delegate ()
			{
				Find.WindowStack.Add(new Dialog_InfoCard(pawn));
			};
			diaNode.options.Add(diaOptionDetails);
			DiaOption diaOption = new DiaOption(Translator.Translate("RescueeAskJoin.Accept"));
			ttexta = "LetterLabelRescueeJoins".Translate(pawn.Named("PAWN"));
			tlabela = "LetterRescueeJoins".Translate(pawn.Named("PAWN"));
			PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref ttexta, ref tlabela, pawn);
			GenPersonStatText.AppendStats(ref ttexta, pawn);
			diaOption.action = delegate ()
			{
				InteractionWorker_RecruitAttempt.DoRecruit(by, pawn, recruitChance, useAudiovisualEffects);
			};
			diaOption.resolveTree = true;
			diaNode.options.Add(diaOption);
			DiaOption diaOption2 = new DiaOption(Translator.Translate("RescueeAskJoin.Reject"));
			diaOption2.action = delegate ()
			{
				ttexta = "RescueeAskJoin.MessageRescueeDidntJoin".Translate().AdjustedFor(pawn, "PAWN", true);
			};
			diaOption2.resolveTree = true;
			diaNode.options.Add(diaOption2);
			string text = TranslatorFormattedStringExtensions.Translate("RescueeAskJoin.WindowTitle", pawn.Map.Parent.Label);
			Find.WindowStack.Add(new Dialog_NodeTree(diaNode, true, true, text));
		}
		
	}
}
