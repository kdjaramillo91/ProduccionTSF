using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models
{
	public partial class Document
	{
		private DBContext db = new DBContext();

		public void RemoveDocument(User ActiveUser)
		{
			string codeState = "05";
			this.changeState(codeState, ActiveUser);
		}

		public void PendingDocument(User ActiveUser)
		{
			string codeState = "01";
			this.changeState(codeState, ActiveUser);
		}

		public void PartialApprove(User ActiveUser)
		{
			string codeState = "02";
			this.changeState(codeState, ActiveUser);
		}

		public void Approve(User ActiveUser)
		{
			string codeState = "03";
			this.changeState(codeState, ActiveUser);
		}

		public void PreAutorize(User ActiveUser)
		{
			string codeState = "09";
			this.changeState(codeState, ActiveUser);
		}

		public void Autorize(User ActiveUser)
		{
			string codeState = "06";
			this.changeState(codeState, ActiveUser);
		}

		public void Reverse(User ActiveUser, string codeFinal)
		{
			this.changeState(codeFinal, ActiveUser);
		}


		private void changeState(string codeStateChange, User ActiveUser)
		{
			this.dateUpdate = DateTime.Now;
			this.id_userUpdate = ActiveUser.id;
			DocumentState documentState = db.DocumentState.FirstOrDefault(r => r.code == codeStateChange);
			this.id_documentState = documentState.id;
		}
	}
}