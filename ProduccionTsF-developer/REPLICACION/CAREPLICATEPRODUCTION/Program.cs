using System;
using System.Configuration;
using System.Reflection;
using System.Timers;
using Utilitarios.Logs;

namespace CAREPLICATEPRODUCTION
{
	class Program
	{
		private static string m_RutaLog = ConfigurationManager.AppSettings["pathLog"];

		public static void Main(string[] args)
		{
			#region Variables

			int _timeExpire = Convert.ToInt32(ConfigurationManager.AppSettings["timeExpire"]);
			int _idRegisterToReplicate = 0;

			string _codeReplicationClass = "";
			string _methodReplicationClass = "";
			string _idsRegisterToReplicate = "";
			string _classReplication = "";

			#endregion

			#region Obtiene Parámetros

			if (args.Length >= 3)
			{
				_idRegisterToReplicate = Convert.ToInt32(args[0]);
				_codeReplicationClass = args[1];
				_methodReplicationClass = args[2];
			}
			if (args.Length == 4)
			{
				_idsRegisterToReplicate = args[3];
			}

			_classReplication = ConfigurationManager.AppSettings[_codeReplicationClass.Trim()];

			#endregion

			MetodosEscrituraLogs.EscribeMensajeLog(
				"Invocación recibida: " + String.Join(" | ", args),
				m_RutaLog, "Replicación" + _classReplication, "PROD");

			try
			{
#if !DEBUG
				// En modo RELEASE, se crea un timer para controlar el TimeOut del proceso...
				var timeoutTimer = new System.Timers.Timer(1000 * 60);
				timeoutTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
#endif

				#region Instancia e Invoca Método

				// Obtiene el constructor y crea una instancia de Replication Class
				var replicationType = Type.GetType(_classReplication);
				Type[] types = new Type[3];
				types[0] = typeof(int);
				types[1] = typeof(string);
				types[2] = typeof(string);

				var replicationConstructor = replicationType.GetConstructor(
					BindingFlags.Instance | BindingFlags.Public, null,
					CallingConventions.HasThis, types, null);

				var replicationClassObject = replicationConstructor.Invoke(
					new object[] { _idRegisterToReplicate, m_RutaLog, _idsRegisterToReplicate });

				// Obtiene el metodo de replicación e invoca con un parámetro obtenido de args[]
				var replicationMethod = replicationType.GetMethod(_methodReplicationClass);
				var replicationAnswer = replicationMethod.Invoke(replicationClassObject, null);

				#endregion
			}
			catch (Exception ex)
			{
				MetodosEscrituraLogs.EscribeExcepcionLog(
					ex, m_RutaLog, "Replicacion" + _classReplication, "PROD");
				throw;
			}
		}
		private static void OnTimedEvent(object source, ElapsedEventArgs e)
		{
			MetodosEscrituraLogs.EscribeMensajeLog(
				"Tiempo Excedido", m_RutaLog, "ReplicacionExcedioTiempo", "PROD");
			Environment.Exit(0);
		}
	}
}
