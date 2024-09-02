using System;
using System.Collections.Generic;
using System.Linq;

namespace DXPANACEASOFT.Models
{
	partial class ProductionCostCoefficientDetail
	{
		#region Clases para implementar cachés para elementos externos

		private class TblCiCuentaCache
		{
			private class TblCiCuentaKey : Tuple<string, string>
			{
				internal string IdPlanDeCuentas => this.Item1;
				internal string IdCuentaContable => this.Item2;

				internal TblCiCuentaKey(string idPlanDeCuentas, string idCuentaContable)
					: base(NormalizeCacheKey(idPlanDeCuentas), NormalizeCacheKey(idCuentaContable))
				{
				}
			}

			private readonly Dictionary<TblCiCuentaKey, TblciCuenta> m_cache = new Dictionary<TblCiCuentaKey, TblciCuenta>();

			internal TblciCuenta GetCuentaContable(string idPlanDeCuentas, string idCuentaContable)
			{
				// Recuperar del caché
				var key = new TblCiCuentaKey(idPlanDeCuentas, idCuentaContable);

				if (this.m_cache.TryGetValue(key, out var value))
				{
					return value;
				}

				// Si no hay en caché, consultar y traer de la base
				value = new DBContextIntegration()
					.TblciCuenta
					.FirstOrDefault(c => c.CCiPlanCta == idPlanDeCuentas
											&& c.CCiCuenta == idCuentaContable);

				this.m_cache[key] = value;

				return value;
			}
		}

		private class TblCiTipoAuxiliarCache
		{
			private readonly Dictionary<string, TblCiTipoAuxiliar> m_cache = new Dictionary<string, TblCiTipoAuxiliar>();

			internal TblCiTipoAuxiliar GetTipoAuxiliarContable(string idTipoAuxContable)
			{
				// Recuperar del caché
				var key = NormalizeCacheKey(idTipoAuxContable);

				if (this.m_cache.TryGetValue(key, out var value))
				{
					return value;
				}

				// Si no hay en caché, consultar y traer de la base
				value = new DBContextIntegration()
					.TblCiTipoAuxiliar
					.FirstOrDefault(c => c.CCiTipoAuxiliar == idTipoAuxContable);

				this.m_cache[key] = value;

				return value;
			}
		}

		private class TblCiAuxiliarCache
		{
			private readonly Dictionary<string, TblCiAuxiliar> m_cache = new Dictionary<string, TblCiAuxiliar>();

			internal TblCiAuxiliar GetAuxiliarContable(string idAuxiliarContable)
			{
				// Recuperar del caché
				var key = NormalizeCacheKey(idAuxiliarContable);

				if (this.m_cache.TryGetValue(key, out var value))
				{
					return value;
				}

				// Si no hay en caché, consultar y traer de la base
				value = new DBContextIntegration()
					.TblCiAuxiliar
					.FirstOrDefault(c => c.CCiAuxiliar == idAuxiliarContable);

				this.m_cache[key] = value;

				return value;
			}
		}

		private class TblCiTipoPresObraCache
		{
			private readonly Dictionary<string, TblCiTipoPresObra> m_cache = new Dictionary<string, TblCiTipoPresObra>();

			internal TblCiTipoPresObra GetTipoPresupuesto(string idTipoPresupuesto)
			{
				// Recuperar del caché
				var key = NormalizeCacheKey(idTipoPresupuesto);

				if (this.m_cache.TryGetValue(key, out var value))
				{
					return value;
				}

				// Si no hay en caché, consultar y traer de la base
				value = new DBContextIntegration()
					.TblCiTipoPresObra
					.FirstOrDefault(c => c.CCiTipoPres == idTipoPresupuesto);

				this.m_cache[key] = value;

				return value;
			}
		}

		private class TblCiProyectoCache
		{
			private class TblCiProyectoKey : Tuple<string, string>
			{
				internal string IdTipoPresupuesto => this.Item1;
				internal string IdCentroCosto => this.Item2;

				internal TblCiProyectoKey(string idTipoPresupuesto, string idCentroCosto)
					: base(NormalizeCacheKey(idTipoPresupuesto), NormalizeCacheKey(idCentroCosto))
				{
				}
			}

			private readonly Dictionary<TblCiProyectoKey, TblCiProyecto> m_cache = new Dictionary<TblCiProyectoKey, TblCiProyecto>();

			internal TblCiProyecto GetCentroCosto(string idTipoPresupuesto, string idCentroCosto)
			{
				// Recuperar del caché
				var key = new TblCiProyectoKey(idTipoPresupuesto, idCentroCosto);

				if (this.m_cache.TryGetValue(key, out var value))
				{
					return value;
				}

				// Si no hay en caché, consultar y traer de la base
				value = new DBContextIntegration()
					.TblCiProyecto
					.FirstOrDefault(c => c.CCiTipoPres == idTipoPresupuesto
											&& c.CCiProyecto == idCentroCosto);

				this.m_cache[key] = value;

				return value;
			}
		}

		private class TblCiSubProyectoCache
		{
			private class TblCiSubProyectoKey : Tuple<string, string, string>
			{
				internal string IdTipoPresupuesto => this.Item1;
				internal string IdCentroCosto => this.Item2;
				internal string IdSubcentroCosto => this.Item3;

				internal TblCiSubProyectoKey(string idTipoPresupuesto, string idCentroCosto, string idSubcentroCosto)
					: base(NormalizeCacheKey(idTipoPresupuesto), NormalizeCacheKey(idCentroCosto), NormalizeCacheKey(idSubcentroCosto))
				{
				}
			}

			private readonly Dictionary<TblCiSubProyectoKey, TblCiSubProyecto> m_cache = new Dictionary<TblCiSubProyectoKey, TblCiSubProyecto>();

			internal TblCiSubProyecto GetSubcentroCosto(string idTipoPresupuesto, string idCentroCosto, string idSubcentroCosto)
			{
				// Recuperar del caché
				var key = new TblCiSubProyectoKey(idTipoPresupuesto, idCentroCosto, idSubcentroCosto);

				if (this.m_cache.TryGetValue(key, out var value))
				{
					return value;
				}

				// Si no hay en caché, consultar y traer de la base
				value = new DBContextIntegration()
					.TblCiSubProyecto
					.FirstOrDefault(c => c.CCiTipoPres == idTipoPresupuesto
											&& c.CCiProyecto == idCentroCosto
											&& c.CCiSubProyecto == idSubcentroCosto);

				this.m_cache[key] = value;

				return value;
			}
		}

		private static string NormalizeCacheKey(string key)
		{
			return key?.TrimEnd().ToUpper();
		}

		#endregion


		private readonly TblCiCuentaCache m_tblCiCuentaCache = new TblCiCuentaCache();
		private readonly TblCiTipoAuxiliarCache m_tblCiTipoAuxiliarCache = new TblCiTipoAuxiliarCache();
		private readonly TblCiAuxiliarCache m_tblCiAuxiliarCache = new TblCiAuxiliarCache();
		private readonly TblCiTipoPresObraCache m_tblCiTipoPresObraCache = new TblCiTipoPresObraCache();
		private readonly TblCiProyectoCache m_tblCiProyectoCache = new TblCiProyectoCache();
		private readonly TblCiSubProyectoCache m_tblCiSubProyectoCache = new TblCiSubProyectoCache();


		public string name_cuentaContab
		{
			get
			{
				return this.m_tblCiCuentaCache
					.GetCuentaContable(this.id_planDeCuentas, this.id_cuentaContab)?
					.CDsCuenta;
			}
		}

		public string description_tipoAuxContab
		{
			get
			{
				if (String.IsNullOrEmpty(this.id_tipoAuxContab))
				{
					return null;
				}

				var descripcion = this.m_tblCiTipoAuxiliarCache
					.GetTipoAuxiliarContable(this.id_tipoAuxContab)?
					.CDsTipoAuxiliar;

				if (String.IsNullOrEmpty(descripcion))
				{
					return this.id_tipoAuxContab;
				}

				return $"{this.id_tipoAuxContab} - {descripcion}";
			}
		}
		public string description_auxiliarContab
		{
			get
			{
				if (String.IsNullOrEmpty(this.id_auxiliarContab))
				{
					return null;
				}

				var descripcion = this.m_tblCiAuxiliarCache
					.GetAuxiliarContable(this.id_auxiliarContab)?
					.CDsAuxiliar;

				if (String.IsNullOrEmpty(descripcion))
				{
					return this.id_auxiliarContab;
				}

				return $"{this.id_auxiliarContab} - {descripcion}";
			}
		}

		public string description_tipoPresContab
		{
			get
			{
				if (String.IsNullOrEmpty(this.id_tipoPresContab))
				{
					return null;
				}

				var descripcion = this.m_tblCiTipoPresObraCache
					.GetTipoPresupuesto(this.id_tipoPresContab)?
					.CDsTipoPres;

				if (String.IsNullOrEmpty(descripcion))
				{
					return this.id_tipoPresContab;
				}

				return $"{this.id_tipoPresContab} - {descripcion}";
			}
		}
		public string description_centroCtoContab
		{
			get
			{
				if (String.IsNullOrEmpty(this.id_tipoPresContab)
					|| String.IsNullOrEmpty(this.id_centroCtoContab))
				{
					return null;
				}

				var descripcion = this.m_tblCiProyectoCache
					.GetCentroCosto(this.id_tipoPresContab, this.id_centroCtoContab)?
					.CDsProyecto;

				if (String.IsNullOrEmpty(descripcion))
				{
					return this.id_centroCtoContab;
				}

				return $"{this.id_centroCtoContab} - {descripcion}";
			}
		}
		public string description_subcentroCtoContab
		{
			get
			{
				if (String.IsNullOrEmpty(this.id_tipoPresContab)
					|| String.IsNullOrEmpty(this.id_centroCtoContab)
					|| String.IsNullOrEmpty(this.id_subcentroCtoContab))
				{
					return null;
				}

				var descripcion = this.m_tblCiSubProyectoCache
					.GetSubcentroCosto(this.id_tipoPresContab, this.id_centroCtoContab, this.id_subcentroCtoContab)?
					.CDsSubProyecto;

				if (String.IsNullOrEmpty(descripcion))
				{
					return this.id_subcentroCtoContab;
				}

				return $"{this.id_subcentroCtoContab} - {descripcion}";
			}
		}
	}
}