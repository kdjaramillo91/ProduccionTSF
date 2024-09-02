using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DXPANACEASOFT.Models;
using System;
using DXPANACEASOFT.Models.QualityControls;

namespace DXPANACEASOFT.DataProviders
{
	public class DataProviderQualityControl
	{
		private static DBContext db = null;

		public static IEnumerable QualityControls()
		{
			db = new DBContext();
			return db.QualityControl.OrderBy(t => t.id).ToList();
		}

        public static QualityControl QualityControlById(int? id)
        {
            db = new DBContext();
            return db.QualityControl.FirstOrDefault(t => t.id == id);
        }
        public static QualityControlConfiguration QualityControlConfiguration(int? id_qualityControlConfiguration)
        {
            db = new DBContext();
            return db.QualityControlConfiguration.FirstOrDefault(t => t.id == id_qualityControlConfiguration);
        }
        public static IEnumerable QualityControlConfigurationsByCompany(int? id_company)
		{
			db = new DBContext();
			return db.QualityControlConfiguration.Where(t => t.isActive &&
											  t.id_company == id_company).OrderBy(t => t.id).ToList();
		}

		public static IEnumerable QualityControlConfigurationsByCompanyAndCurrent(int? id_company, int? id_current)
		{
			db = new DBContext();
			List<QualityControlConfiguration> qualityControlConfigurationTmp = db.QualityControlConfiguration.Where(t => t.isActive && (!t.isSystem) &&
											  t.id_company == id_company || (t.id == id_current)).OrderBy(t => t.id).ToList();
			return qualityControlConfigurationTmp;
		}

		public static IEnumerable QualityControlConfigurationAnalysisDataTypeValidateDetailByAnalysis(int? id_qualityControlConfiguration, int? id_qualityAnalysis)
		{
			db = new DBContext();


			int? id_qualityControlConfiguartionDataTypeValidate = db.QualityControlConfigurationAnalysisDataTypeValidate
			.FirstOrDefault(fod => fod.id_qualityAnalysis == id_qualityAnalysis && fod.id_qualityControlConfiguration == id_qualityControlConfiguration)?.id ?? 0;

			//int idQualityControlDataTypeValidate = 0;
			//if (id_qualityControlConfiguartionDataTypeValidate != null)
			//{
			//    idQualityControlDataTypeValidate = (int)id_qualityControlConfiguartionDataTypeValidate.id;
			//}

			List<QualityControlConfigurationAnalysisDataTypeValidateDetailValue> qualityControlConfigurationDataTypeValidateDetailValue = db.QualityControlConfigurationAnalysisDataTypeValidateDetailValue
			.Where(w => w.id_qualityControlConfigurationAnalysisDataTypeValidate == id_qualityControlConfiguartionDataTypeValidate)
			.ToList();

			qualityControlConfigurationDataTypeValidateDetailValue = qualityControlConfigurationDataTypeValidateDetailValue ?? new List<QualityControlConfigurationAnalysisDataTypeValidateDetailValue>();

			return qualityControlConfigurationDataTypeValidateDetailValue;
		}

		public static string QualityControlVisualizationTypeDataByAnalysis(int? id_analysisGroup)
		{
			db = new DBContext();

			var qualityControlAnalysisGroupTmp = db.QualityControlAnalysisGroup.FirstOrDefault(w => w.id == id_analysisGroup);

			var visualizationTypeData = db.VisualizationTypeData.FirstOrDefault(fod => fod.id == qualityControlAnalysisGroupTmp.id_VisualizationTypeData);


			return visualizationTypeData.code;
		}

		public static IEnumerable QualityControlConformityResult(int? id_company)
		{
			db = new DBContext();

			var qualityControlConformityResultList = db.QualityControlResultConformity.Where(w => w.id_Company == id_company);

			return qualityControlConformityResultList.ToList();
		}
		public static bool QualityControlConfigurationConformity(int? id_qualityControlConfiguration)
		{
			db = new DBContext();
			var qualityControlConfigurationConfirmity = db.QualityControlConfigurationConformity.FirstOrDefault(fod => fod.id_qualityControlConfiguration == id_qualityControlConfiguration);
			return (bool)qualityControlConfigurationConfirmity.isConformityOnHeader;

		}

		public static bool QualityControlAnalysisGroupHasWholePerformance(int? id_analysisGroup)
		{
			db = new DBContext();
			return (bool)(db.QualityControlAnalysisGroup.FirstOrDefault(fod => fod.id == id_analysisGroup)?.calculateWholePerformanceValue);
		}
		public static string QualityControlAnalysisGroupHasColor(int? id_analysisGroup)
		{
			db = new DBContext();
			return (db.QualityControlAnalysisGroup.FirstOrDefault(fod => fod.id == id_analysisGroup)?.code);
		}
		public static string RemissionGuideNumber(int? id_pld)
		{
			db = new DBContext();
			return db.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == id_pld)?.RemissionGuideDetail?.RemissionGuide?.Document?.number ?? "";
		}
        public static string RemissionGuideNumberExterna(int? id_pld)
        {
            db = new DBContext();
            return db.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == id_pld)?.RemissionGuideDetail?.RemissionGuide?.RemissionGuideTransportationCustomizedInformation?.numberGuide ?? "";
        }
        public static string RemissionGuideProcess(int? id_pld)
		{
			db = new DBContext();
			return db.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == id_pld)?.RemissionGuideDetail?.RemissionGuide?.Person2?.processPlant ?? "";
		}
		public static string RemissionGuideNumberByQualityControl(int? id_qc)
		{
			db = new DBContext();
			var id = db.ProductionLotDetailQualityControl
						.FirstOrDefault(fod => fod.id_qualityControl == id_qc)?
						.ProductionLotDetail?.id ?? 0;
			string answer = "";
			if (id != 0)
			{
				answer = db.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == id)?.RemissionGuideDetail?.RemissionGuide?.Document?.number ?? "";
			}
			return answer;
		}
		public static string RemissionGuideProcessByQualityControl(int? id_qc)
		{
			db = new DBContext();
			var id = db.ProductionLotDetailQualityControl
						 .FirstOrDefault(fod => fod.id_qualityControl == id_qc)?
						 .ProductionLotDetail?.id ?? 0;
			string answer = "";
			if (id != 0)
			{
				answer = db.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == id)?.RemissionGuideDetail?.RemissionGuide?.Person2?.processPlant ?? "";
			}
			return answer;
		}

		public static  QualityControlConfigDataTypeValidateDto[] GetValidation( QualityControlDetail[] detalles, 
																				int id_qualityControlConfiguration)
        {
			db = new DBContext();

			int[] id_qualityAnalysisAR = detalles.Select(r => r.id_qualityAnalysis).ToArray();
			var qualityConfiguration = db.QualityControlConfigurationAnalysisDataTypeValidate
										.Where(r => r.id_qualityControlConfiguration == id_qualityControlConfiguration
											   && id_qualityAnalysisAR.Contains(r.id_qualityAnalysis))
										.ToList();

			var result = (from det in detalles join
						  config in qualityConfiguration on
						  det.id_qualityAnalysis equals config.id_qualityAnalysis
						  select new QualityControlConfigDataTypeValidateDto
						  {
							    
							  id_qualityControlDetail = det.id,
							  id_qualityAnalysis = config.id_qualityAnalysis,
							  id_qualityControlConfiguration = config.id_qualityControlConfiguration,
							  id_qualityDataType = config.id_qualityDataType,
							  id_qualityValidate = config.id_qualityValidate,
							  valueValidate = config.valueValidate,
							  valueValidateMax = config.valueValidateMax,
							  valueValidateMin = config.valueValidateMin

						  })
						  .ToArray();

						

			return result;


		}
	}
}