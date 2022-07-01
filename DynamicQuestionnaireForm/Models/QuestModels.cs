using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicQuestionnaireForm.Models
{
    public class QuestModels
    {
		public class QuestForm
		{
			public short QuestNo { get; set; }
			public string QuestTitle { get; set; }
			public string QuestOptionType { get; set; }
			public List<QuestOption> QuestOptions { get; set; }
		}

		public class QuestOption
		{
			public short QuestNo { get; set; }
			public short QuestOtpionId { get; set; }
			public string QuestOptionText { get; set; }
			public string QuestOptionWithTextbox { get; set; }
		}

		public class UserAnswerModel
		{
			public List<UserAnswerObj> QuestUserAnswer { get; set; }
		}

		public class UserAnswerObj
		{
			public short QuestNo { get; set; }
			public string UserAnswer { get; set; }
		}
	}
}