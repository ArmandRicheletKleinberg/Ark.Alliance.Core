using System.ComponentModel;
using Ark.Net.Models.CrossCutting;

namespace Ark.Net.CrossCutting
{
    /// <summary>
    /// This enumeration holds all the mappings available for the application.
    /// As this is used by a MemoryWithEnumKeyRepository, RESPECT THE +1 INCREMENT FOR THE ENUMERATION VALUE.
    /// The name of the enumeration are also really important, IT WILL SET THE NAME OF THE MAPPING IN THE DATABASE.
    /// This enumeration will be used by the Cross Cutting server to fill automatically the database of the dynamic mappings.
    /// </summary>
    public enum DynamicMappingEnum
    {
        /// <summary>
        /// Mapping for GOLD_DOMAIN_TRANSCO export.
        /// </summary>
        [DynamicMappingApp(nameof(AppEnum.Hub))]
        [Description("ORD_Erp_Export_GOLD_DOMAIN_TRANSCO")]
        ORD_Erp_Export_GOLD_DOMAIN_TRANSCO = 1,

        /// <summary>
        /// Mapping for target calculation export.
        /// </summary>
        [DynamicMappingApp(nameof(AppEnum.Hub))]
        [Description("ORD_Erp_Export_Target_Calculation")]
        ORD_Erp_Export_Target_Calculation = 2,

        /// <summary>
        /// Mapping for warehouse data export.
        /// </summary>
        [DynamicMappingApp(nameof(AppEnum.Hub))]
        [Description("ORD_Erp_Export_WAREHOUSE_DATA")]
        ORD_Erp_Export_WAREHOUSE_DATA = 3,

        /// <summary>
        /// Mapping for Axiodis site export.
        /// </summary>
        [DynamicMappingApp(nameof(AppEnum.Hub))]
        [Description("Wh_Axiodis_Site")]
        ORD_Erp_Export_Wh_Axiodis_Site = 4,

        /// <summary>
        /// Mapping for ACTION_KIND_TRANSCO export.
        /// </summary>
        [DynamicMappingApp(nameof(AppEnum.Hub))]
        [Description("ORD_Erp_Export_ACTION_KIND_TRANSCO")]
        ORD_Erp_Export_ACTION_KIND_TRANSCO_AXIO = 5,

        /// <summary>
        /// Mapping for POSTOU filter settings export.
        /// </summary>
        [DynamicMappingApp(nameof(AppEnum.Hub))]
        [Description("ORD_Erp_Export_POSTOU_FILTER_SETTINGS")]
        ORD_Erp_Export_POSTOU_FILTER_SETTINGS = 6,

        /// <summary>
        /// Mapping linking legacy store identifier.
        /// </summary>
        [DynamicMappingApp(nameof(AppEnum.Hub))]
        [Description("SaptranscoStore_Link_for_Legacy_Store_ID")]
        ORD_Erp_Export_SaptranscoStore_Link_for_Legacy_Store_ID = 7,

        /// <summary>
        /// Mapping linking warehouse and SAP identifier.
        /// </summary>
        [DynamicMappingApp(nameof(AppEnum.Hub))]
        [Description("Warehouse_Link_for_sap_ID")]
        ORD_Erp_Export_Warehouse_Link_for_sap_ID = 8,

        /// <summary>
        /// Mapping for TMS warehouse link for WH_SAPD export.
        /// </summary>
        [DynamicMappingApp(nameof(AppEnum.Hub))]
        [Description("Warehouse_Link_for_sap_ID")]
        ORD_Erp_Export_Tms_warehouse_link_for_WH_SAPD = 9
    }
}