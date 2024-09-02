namespace DXPANACEASOFT.Models
{
	public partial class DBContext
	{
		public override int SaveChanges()
		{
			return this.PrivateSaveChanges(true);
		}

		public int SaveChanges(bool notify)
		{
			return this.PrivateSaveChanges(notify);
		}

		private int PrivateSaveChanges(bool notify)
		{
			if (notify)
			{
				// Notificaciones queda inhabilitado hasta posterior
				// Revisión de posibles problemas de bloqueo
				//Extensions.Functions.DBNotification.Execute(this);
			}

			return base.SaveChanges();
		}
	}
}