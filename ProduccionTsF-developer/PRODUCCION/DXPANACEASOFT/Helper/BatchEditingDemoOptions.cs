using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.UI.WebControls;
using DevExpress.Web.Mvc;
using DevExpress.Utils;
using System.Web.Mvc;

namespace DXPANACEASOFT.Helper
{
    public class BatchEditingDemoOptions
    {
        public BatchEditingDemoOptions()
        {
            EditMode = GridViewBatchEditMode.Cell;
            StartEditAction = GridViewBatchStartEditAction.FocusedCellClick;
            HighlightDeletedRows = true;
        }

        public GridViewBatchEditMode EditMode { get; set; }
        public GridViewBatchStartEditAction StartEditAction { get; set; }
        public bool HighlightDeletedRows { get; set; }

        public void Assign(BatchEditingDemoOptions source)
        {
            EditMode = source.EditMode;
            StartEditAction = source.StartEditAction;
            HighlightDeletedRows = source.HighlightDeletedRows;
        }
    }
    public class GridViewFeaturesHelper
    {
        public static void SetupGlobalGridViewBehavior(GridViewSettings settings)
        {
            settings.EnablePagingGestures = AutoBoolean.False;
            settings.SettingsPager.EnableAdaptivity = true;
            settings.Styles.Header.Wrap = DefaultBoolean.True;
            settings.Styles.GroupPanel.CssClass = "GridNoWrapGroupPanel";
        }
        public static MvcHtmlString GetHeadPartialResources()
        {
            return new MvcHtmlString(GetGridNoWrapGroupPanelCssStyle());
        }
        public static string GetGridNoWrapGroupPanelCssStyle()
        {
            return "\r\n<style>.GridNoWrapGroupPanel td.dx-wrap { white-space: nowrap !important; }</style>\r\n";
        }
    }

}