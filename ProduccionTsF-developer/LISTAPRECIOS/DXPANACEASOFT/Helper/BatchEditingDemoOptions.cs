using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.UI.WebControls;

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
}