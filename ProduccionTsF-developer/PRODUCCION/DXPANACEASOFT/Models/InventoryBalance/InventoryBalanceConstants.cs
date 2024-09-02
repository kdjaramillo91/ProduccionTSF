namespace DXPANACEASOFT.Models.InventoryBalance
{

    public static class InventoryBalanceConstants
    {
        public const string m_InProcessState = "INPROCESS";
        public const string m_InProcessState_Description = "EN PROCESO";
        public const string m_AvailableState = "AVAILABLE";
        public const string m_AvailableState_Descrition = "DISPONIBLE";

        public const string m_KG_CodeMetricUnit = "KG";
        public const string m_LBS_CodeMetricUnit = "LBS";

        public const decimal m_Factor_LBS_LBS = 1;
        public const decimal m_Factor_KG_LBS = 2.2046M;
    }
}