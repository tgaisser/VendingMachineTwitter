using System;
using System.Collections.Generic;
using System.Linq;
using VendingMachine.Model;

namespace VendingMachine.Repository
{
	public class CodeRepository
	{
		VendingMachineContext vendingMachineContext = new VendingMachineContext();
		public Code GetCode(int id) 
		{
			var ret = vendingMachineContext.Codes.Find(id);
			return ret;
		}

		public Code GetCodeByCodeValue(string CodeValue)
		{
			var ret = vendingMachineContext.Codes.FirstOrDefault(x => x.CodeValue.ToUpper() == CodeValue.ToUpper());
			return ret;
		}

		public List<Code> GetCodesByEvent(int EventId)
		{
			var ret = vendingMachineContext.Codes.Where(x => x.DenormalizedEventId == EventId).ToList();
			return ret;
		}

		public List<Code> GetCodesByTweetUserId(string TweetUserId)
		{
			var ret = vendingMachineContext.Codes.Where(x => x.TweetUserId == TweetUserId).ToList();
			return ret;
		}

		public Code GetCodeUsedTodayByTweetUserId(string UsersTweetId, int EventId, int MachineTwitterAccountId)
		{
			List<Code> codes = vendingMachineContext.Codes.Where(x =>
				x.TweetUserId == UsersTweetId
				&& x.DenormalizedEventId == EventId
				&& x.MachineTwitterAccountId == MachineTwitterAccountId
				&& x.DateAssigned != null).ToList();

			Code code = codes.FirstOrDefault(x => x.DateAssigned != null && x.DateAssigned.Value.Date == DateTime.Now.Date);

			Code ret = code;
			code = null;
			codes = null;
			return ret;
		}

		public string SecureAndReturnNewCodeForUser(TwitterMention Mention, int EventId, int MachineTwitterAccountId)
		{
			List<Code> codes = vendingMachineContext.Codes.Where(x =>
				x.DenormalizedEventId == EventId &&
				x.MachineTwitterAccountId == MachineTwitterAccountId &&
				x.IsActive &&
				x.DateAssigned == null).ToList();

			string newCode = string.Empty;

			int count = codes.Count();

			if (count > 0)
			{
				int index = new Random().Next(count);

				Code ret = codes.Skip(index).FirstOrDefault();

				newCode = ret.CodeValue;

				ret.DateAssigned = DateTime.Now;
				ret.DateTweetCreated = Mention.TweetCreated;
				ret.TweetId = Mention.MentionId;
				ret.TweetMessage = Mention.Text;
				ret.TweetUserId = Mention.TwitterUser.TwitterId;

				vendingMachineContext.SaveChanges();
				
				ret = null;
			}

			codes = null;
			return newCode;

		}

		//public string SecureAndReturnNewCodeForUser(string UsersTweetId, int EventId, int MachineTwitterAccountId)
		//{
		//	List<Code> codes = vendingMachineContext.Codes.Where(x => 
		//		x.DenormalizedEventId == EventId &&
		//		x.MachineTwitterAccountId == MachineTwitterAccountId &&
		//		x.IsActive && 
		//		x.DateAssigned == null).ToList();

		//	Code ret = codes.Where(x =>
		//		x.DateAssigned.Value.Date == DateTime.Now.Date
		//		).FirstOrDefault();

		//}
	}
}
