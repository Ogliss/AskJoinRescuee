using System;
using System.Collections.Generic;
using System.Text;
using RimWorld;
using Verse;

namespace RescueeJoinPlus
{
	public static class GenPersonStatText
	{
		public static string CreateRescueeText(Pawn Rescuee)
		{
			TaggedString text = TranslatorFormattedStringExtensions.Translate(Main.AlienRaces_Active ? "RescueeAskJoin.WindowAlienDesc" : "RescueeAskJoin.WindowDesc", Rescuee.Name.ToStringFull, Rescuee.story.Title.ToLower(), Rescuee.ageTracker.AgeBiologicalYears.ToString(), Rescuee.def.label);
			text = GenText.AdjustedFor(GrammarResolverSimpleStringExtensions.Formatted(text, NamedArgumentUtility.Named(Rescuee, "PAWN")), Rescuee, "PAWN");
			if (Rescuee.gender != Gender.None) text += " " + Rescuee.gender.GetLabel();
			text += ".";
			PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref text, Rescuee);
			GenPersonStatText.AppendStats(ref text, Rescuee);
			return text;
		}
		public static void AppendStats(ref TaggedString text, Pawn pawn)
		{
			StringBuilder stringBuilder = new StringBuilder(text);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine();
			GenPersonStatText.AppendStats(stringBuilder, pawn);
			text = stringBuilder.ToString();
		}

		public static void AppendStats(StringBuilder sb, Pawn pawn)
		{
			GenPersonStatText.AppendDisabledWorkTags(sb, pawn.story.DisabledWorkTagsBackstoryAndTraits);
			sb.AppendLine();
			sb.AppendLine();
			GenPersonStatText.AppendTraits(sb, pawn.story.traits.allTraits);
			sb.AppendLine();
			sb.AppendLine();
			GenPersonStatText.AppendPassions(sb, pawn.skills.skills);
		}

		private static void AppendDisabledWorkTags(StringBuilder sb, WorkTags disabledWorkTags)
		{
			sb.Append(GenPersonStatText.CreateTitle(Translator.Translate("IncapableOf")));
			sb.AppendLine();
			if (disabledWorkTags == WorkTags.None)
			{
				sb.Append("(" + Translator.Translate("NoneLower") + ")");
				return;
			}
			int num = 0;
			bool flag = true;
			foreach (object obj in Enum.GetValues(typeof(WorkTags)))
			{
				WorkTags workTags = (WorkTags)obj;
				if (workTags != WorkTags.None && (disabledWorkTags & workTags) == workTags)
				{
					if (num > 0)
					{
						sb.Append(", ");
					}
					if (!flag)
					{
						sb.Append(WorkTypeDefsUtility.LabelTranslated(workTags).ToLower());
					}
					else
					{
						sb.Append(WorkTypeDefsUtility.LabelTranslated(workTags));
					}
					num++;
					flag = false;
				}
			}
		}

		private static void AppendTraits(StringBuilder sb, List<Trait> traits)
		{
			sb.Append(GenPersonStatText.CreateTitle(Translator.Translate("Traits")));
			if (traits.Count == 0)
			{
				sb.AppendLine();
				sb.Append("(" + Translator.Translate("NoneLower") + ")");
				return;
			}
			foreach (Trait trait in traits)
			{
				sb.AppendLine();
				sb.Append(trait.LabelCap);
			}
		}

		private static void AppendPassions(StringBuilder sb, List<SkillRecord> skills)
		{
			sb.Append(GenPersonStatText.CreateTitle(Translator.Translate("RescueeAskJoin.PassionateFor")));
			sb.AppendLine();
			if (skills.Count == 0)
			{
				sb.Append("(" + Translator.Translate("NoneLower") + ")");
				return;
			}
			int num = 0;
			foreach (SkillRecord skillRecord in skills)
			{
				if (skillRecord.passion > 0)
				{
					if (num > 0)
					{
						sb.Append(", ");
					}
					string str = "RescueeAskJoin.PassionateMajor".Translate();

					sb.Append(skillRecord.def.skillLabel + ((skillRecord.passion == (Passion)2) ? str : ""));
					num++;
				}
			}
		}

		private static string CreateTitle(string title)
		{
			return string.Concat(new object[]
			{
				"<b><size=",
				TITLE_SIZE,
				">",
				title,
				"</size></b>"
			});
		}

		private const int TITLE_SIZE = 16;
	}
}
