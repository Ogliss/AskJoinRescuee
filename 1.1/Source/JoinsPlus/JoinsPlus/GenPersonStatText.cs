﻿using System;
using System.Collections.Generic;
using System.Text;
using RimWorld;
using Verse;

namespace RescueeJoinPlus
{
	// Token: 0x02000002 RID: 2
	public static class GenPersonStatText
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public static void AppendStats(ref TaggedString text, Pawn pawn)
		{
			StringBuilder stringBuilder = new StringBuilder(text);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine();
			GenPersonStatText.AppendStats(stringBuilder, pawn);
			text = stringBuilder.ToString();
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002084 File Offset: 0x00000284
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


		// Token: 0x06000004 RID: 4 RVA: 0x00002134 File Offset: 0x00000334
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

		// Token: 0x06000005 RID: 5 RVA: 0x0000221C File Offset: 0x0000041C
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

		// Token: 0x06000006 RID: 6 RVA: 0x000022C0 File Offset: 0x000004C0
		private static void AppendPassions(StringBuilder sb, List<SkillRecord> skills)
		{
			sb.Append(GenPersonStatText.CreateTitle(Translator.Translate("PassionateFor")));
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
					string str = "PassionateMajor".Translate();

					sb.Append(skillRecord.def.skillLabel + ((skillRecord.passion == (Passion)2) ? str : ""));
					num++;
				}
			}
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000023A0 File Offset: 0x000005A0
		private static string CreateTitle(string title)
		{
			return string.Concat(new object[]
			{
				"<b><size=",
				16,
				">",
				title,
				"</size></b>"
			});
		}

		// Token: 0x04000001 RID: 1
		private const int TITLE_SIZE = 16;
	}
}
