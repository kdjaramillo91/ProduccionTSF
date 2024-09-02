using DevExpress.Web;
using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using TreeListColumn = DevExpress.Web.ASPxTreeList.TreeListColumn;
using TreeListEditMode = DevExpress.Web.ASPxTreeList.TreeListEditMode;
using TreeListExpandCollapseAction = DevExpress.Web.ASPxTreeList.TreeListExpandCollapseAction;
using TreeListToolbarCommand = DevExpress.Web.ASPxTreeList.TreeListToolbarCommand;

namespace DXPANACEASOFT.Operations
{
	public static class DevExpressSettingsExtensions
	{
		#region Configuración de tablas de datos

		public static void SetDefaultQuerySettings(this GridViewSettings gridView)
		{
			SetDefaultCommonSettings(gridView);

			var settings = gridView.Settings;
			settings.ShowFilterRow = true;
			settings.ShowFilterRowMenu = true;
			settings.AutoFilterCondition = AutoFilterCondition.Contains;

			gridView.SettingsSearchPanel.Visible = true;
			gridView.Styles.SearchPanel.CssClass = "searchPanel";
		}
		public static void SetDefaultEditSettings(this GridViewSettings gridView)
		{
			SetDefaultCommonSettings(gridView);

			var settings = gridView.Settings;
			settings.ShowFilterRow = false;
			settings.ShowFilterRowMenu = false;

			gridView.SettingsSearchPanel.Visible = false;
		}

		private static void SetDefaultCommonSettings(GridViewSettings gridView)
		{
			gridView.Width = Unit.Percentage(100);

			var settings = gridView.Settings;
			settings.ShowTitlePanel = false;
			settings.ShowGroupPanel = false;

			var behavior = gridView.SettingsBehavior;
			behavior.AllowSelectByRowClick = true;
			behavior.AllowEllipsisInText = true;
			behavior.AllowDragDrop = false;
			behavior.ConfirmDelete = true;
			behavior.EnableRowHotTrack = true;

			var header = gridView.Styles.Header;
			header.BackColor = System.Drawing.ColorTranslator.FromHtml("#F39C12");
			header.Font.Bold = true;
			header.ForeColor = System.Drawing.Color.White;

			var pager = gridView.SettingsPager;
			pager.Visible = true;
			pager.AlwaysShowPager = true;
			pager.Summary.EmptyText = "No hay datos para mostrar";

			var loading = gridView.SettingsLoadingPanel;
			loading.Mode = GridViewLoadingPanelMode.ShowAsPopup;
			loading.Text = "Cargando...";

			var styles = gridView.Styles;
			styles.AlternatingRow.Enabled = DevExpress.Utils.DefaultBoolean.True;
			styles.AlternatingRow.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffffff");
			styles.Row.BackColor = System.Drawing.ColorTranslator.FromHtml("#f5f3f4");

			styles.EditFormDisplayRow.BackColor = System.Drawing.ColorTranslator.FromHtml("#F6F6F7");
			styles.EditForm.BackColor = System.Drawing.ColorTranslator.FromHtml("#F6F6F7");

			var adaptivity = gridView.SettingsAdaptivity;
			adaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
			adaptivity.AdaptiveColumnPosition = GridViewAdaptiveColumnPosition.Right;
			adaptivity.AdaptiveDetailColumnCount = 1;
			adaptivity.AllowOnlyOneAdaptiveDetailExpanded = false;

			SetDefaultCommonSettings(adaptivity.AdaptiveDetailLayoutProperties);

			var editing = gridView.SettingsEditing;
			editing.Mode = GridViewEditingMode.EditFormAndDisplayRow;
			editing.NewItemRowPosition = GridViewNewItemRowPosition.Bottom;
			editing.ShowModelErrorsForEditors = false;
			editing.UseFormLayout = true;
		}

		public static void SetDataExportSettings(this GridViewSettings gridView, string reportHeader, string filename, bool landscape = true)
		{
			var export = gridView.SettingsExport;

			export.EnableClientSideExportAPI = true;
			export.ExcelExportMode = DevExpress.Export.ExportType.WYSIWYG;

			export.FileName = $"{filename}_{DateTime.Now:yyyyMMdd HHmm}";
			export.Landscape = landscape;

			export.PaperKind = System.Drawing.Printing.PaperKind.A4;

			var titleStyle = export.Styles.Title;
			titleStyle.Font.Name = "Verdana";
			titleStyle.Font.Size = FontUnit.Point(11);
			titleStyle.HorizontalAlign = HorizontalAlign.Left;

			var headerStyle = export.Styles.Header;
			headerStyle.Font.Name = "Verdana";
			headerStyle.Font.Size = FontUnit.Point(10);

			var defaultStyle = export.Styles.Default;
			defaultStyle.Font.Name = "Tahoma";
			defaultStyle.Font.Size = FontUnit.Point(10);

			MVCxGridViewToolbarItem prepareToolbarButton(GridViewToolbarCommand command)
			{
				return new MVCxGridViewToolbarItem()
				{
					Name = Convert.ToString(command),
					ClientVisible = false,
					Command = command,
				};
			}

			gridView.Toolbars.Add(toolbar =>
			{
				toolbar.Items.Add(prepareToolbarButton(GridViewToolbarCommand.ExportToXlsx));
				toolbar.Items.Add(prepareToolbarButton(GridViewToolbarCommand.ExportToDocx));
				toolbar.Items.Add(prepareToolbarButton(GridViewToolbarCommand.ExportToPdf));
			});

			gridView.StylesToolbar.Style.CssClass = "hidden";

			// Se exportarán todas las columnas, inclusive las ocultas...
			gridView.SettingsExport.BeforeExport = (s, e) =>
			{
				if (s is MVCxGridView gridViewExport)
				{
					gridViewExport.Settings.ShowTitlePanel = true;
					gridViewExport.SettingsText.Title = reportHeader;

					var index = 0;
					foreach (GridViewColumn column in gridViewExport.Columns)
					{
						column.Visible = true;
						column.VisibleIndex = index++;
					}
				}
			};
		}

		public static void EnableWrapForAllColumns(this GridViewSettings gridView)
		{
			foreach (GridViewColumn column in gridView.Columns)
			{
				column.HeaderStyle.Wrap = DevExpress.Utils.DefaultBoolean.True;
				column.CellStyle.Wrap = DevExpress.Utils.DefaultBoolean.True;
			}
		}

		public static void SetClientJSProperties(this GridViewSettings gridView,
			string operationMessageType, string operationMessage,
			Dictionary<string, object> extraProperties = null)
		{
			gridView.CustomJSProperties =
				(s, e) => GridViewClientJSPropertiesHandler((s as MVCxGridView), e, operationMessageType, operationMessage, extraProperties);
		}

		private static void GridViewClientJSPropertiesHandler(
			MVCxGridView gridView, ASPxGridViewClientJSPropertiesEventArgs eventArgs,
			string operationMessageType, string operationMessage,
			Dictionary<string, object> extraProperties)
		{
			if (gridView != null)
			{
				eventArgs.Properties["cpVisibleRowCount"] = gridView.VisibleRowCount;
				eventArgs.Properties["cpFilteredRowCountWithoutPage"] = GetFilteredRowCountWithoutPage(gridView);

				if (gridView.IsEditing)
				{
					eventArgs.Properties["cpEditingRowKey"] = gridView
						.GetRowValues(gridView.EditingRowVisibleIndex, gridView.KeyFieldName) ?? "";
				}
				else
				{
					eventArgs.Properties["cpEditingRowKey"] = "";
				}

				eventArgs.Properties["cpOperationMessageType"] = operationMessageType;
				eventArgs.Properties["cpOperationMessage"] = operationMessage;

				if (extraProperties != null)
				{
					foreach (var keyValue in extraProperties)
					{
						eventArgs.Properties.Add("cp" + keyValue.Key, keyValue.Value);
					}
				}
			}
		}

		private static int GetFilteredRowCountWithoutPage(MVCxGridView grid)
		{
			var selectedRowsOnPage = 0;

			foreach (var key in grid.GetCurrentPageRowValues(grid.KeyFieldName))
			{
				if (grid.Selection.IsRowSelectedByKey(key))
				{
					selectedRowsOnPage++;
				}
			}

			return grid.Selection.FilteredCount - selectedRowsOnPage;
		}


		public static void PrepareEditLayoutItems(this MVCxGridView gridView,
			Action<GridViewColumnLayoutItem, bool> prepareEditLayoutItem)
		{
			if (gridView.IsEditing)
			{
				PrepareEditLayoutItems(
					gridView.EditFormLayoutProperties.Items,
					gridView.IsNewRowEditing,
					prepareEditLayoutItem);
			}
		}

		public static void PrepareEditLayoutItem(this MVCxGridView gridView, string columnName,
			Action<GridViewColumnLayoutItem, bool> prepareEditLayoutItem)
		{
			if (gridView.IsEditing)
			{
				var layoutItem = gridView.EditFormLayoutProperties.FindColumnItem(columnName);

				if (layoutItem is GridViewColumnLayoutItem)
				{
					var isNewRow = gridView.IsNewRowEditing;
					prepareEditLayoutItem((GridViewColumnLayoutItem)layoutItem, isNewRow);
				}
			}
		}

		private static void PrepareEditLayoutItems(
			GridViewLayoutItemCollection layoutItems, bool isNewRow,
			Action<GridViewColumnLayoutItem, bool> prepareEditLayoutItem)
		{
			foreach (LayoutItemBase layoutItem in layoutItems)
			{
				if (layoutItem is GridViewColumnLayoutItem)
				{
					prepareEditLayoutItem((GridViewColumnLayoutItem)layoutItem, isNewRow);
				}
				else if (layoutItem is GridViewLayoutGroup)
				{
					PrepareEditLayoutItems(((GridViewLayoutGroup)layoutItem).Items, isNewRow, prepareEditLayoutItem);
				}
			}
		}


		public static void AddCommand(this MVCxGridViewColumnCollection gridColumns,
			Action<GridViewCommandColumn> method)
		{
			var commandColumn = new GridViewCommandColumn();
			method.Invoke(commandColumn);
			gridColumns.Add(commandColumn);
		}
		public static void AddCustomButton(this GridViewCommandColumnCustomButtonCollection customButtons,
			Action<GridViewCommandColumnCustomButton> method)
		{
			var customButton = new GridViewCommandColumnCustomButton();
			method.Invoke(customButton);
			customButtons.Add(customButton);
		}

		#endregion

		#region Configuración de control tabulador

		public static void SetDefaultSettings(this PageControlSettings pageControl)
		{
			pageControl.Width = Unit.Percentage(100);
			pageControl.Styles.Tab.Width = Unit.Percentage(100);
			pageControl.Styles.ActiveTab.Width = Unit.Percentage(100);

			pageControl.EnableTabScrolling = true;
			pageControl.TabAlign = TabAlign.Justify;
		}

		public static void SetDefaultSettings(this TabPage tabPage)
		{
			tabPage.TabStyle.Width = Unit.Percentage(100);

			tabPage.TabImage.Width = Unit.Pixel(16);
			tabPage.TabImage.Height = Unit.Pixel(16);

			tabPage.TabImage.Align = ImageAlign.Right;
			tabPage.TabImage.Url = "~/Content/image/noimage.png";

			tabPage.ActiveTabImage.Align = ImageAlign.Right;
			tabPage.ActiveTabImage.Url = "~/Content/image/noimage.png";
		}

		public static void SetClientJSProperties(this PageControlSettings pageControl,
			string operationMessageType, string operationMessage,
			Dictionary<string, object> extraProperties = null)
		{
			pageControl.CustomJSProperties =
				(s, e) => EditorControlClientJSPropertiesHandler(e, operationMessageType, operationMessage, extraProperties);
		}

		#endregion

		#region Configuración de control de formulario

		public static void SetDefaultSettingsForSimple<T>(this FormLayoutSettings<T> formLayout)
		{
			SetDefaultCommonSettings(formLayout);

			var a = formLayout.SettingsAdaptivity;
			a.SwitchToSingleColumnAtWindowInnerWidth = 840;
			a.AdaptivityMode = FormLayoutAdaptivityMode.SingleColumnWindowLimit;
		}

		public static void SetDefaultSettingsForSimpleWide<T>(this FormLayoutSettings<T> formLayout)
		{
			SetDefaultCommonSettings(formLayout);

			var a = formLayout.SettingsAdaptivity;
			a.SwitchToSingleColumnAtWindowInnerWidth = 1080;
			a.AdaptivityMode = FormLayoutAdaptivityMode.SingleColumnWindowLimit;
		}

		private static void SetDefaultCommonSettings<T>(FormLayoutSettings<T> formLayout)
		{
			formLayout.Width = Unit.Percentage(100);
			formLayout.AlignItemCaptionsInAllGroups = true;
			formLayout.UseDefaultPaddings = true;
			formLayout.RequiredMarkDisplayMode = RequiredMarkMode.None;

			var itemStyle = formLayout.Styles.LayoutItem;
			itemStyle.Caption.CssClass = "dxeBase_Metropolis";
			itemStyle.CaptionCell.Paddings.PaddingRight = Unit.Pixel(25);
		}

		public static void SetDefaultSettings(this LayoutGroup group)
		{
			group.ShowCaption = DevExpress.Utils.DefaultBoolean.False;
			group.UseDefaultPaddings = true;
			group.GroupBoxDecoration = GroupBoxDecoration.None;
		}

		public static void SetDefaultSettingsForQuery(this LayoutGroup group)
		{
			group.ShowCaption = DevExpress.Utils.DefaultBoolean.True;
			group.UseDefaultPaddings = true;
			group.GroupBoxDecoration = GroupBoxDecoration.HeadingLine;
		}

		public static EmptyLayoutItem AddCustomEmptyItem(this LayoutItemCollection items)
		{
			var emptyLayoutItem = new EmptyLayoutItem()
			{
				CssClass = "empty-form-item",
			};

			items.Add(emptyLayoutItem);
			return emptyLayoutItem;
		}
		public static void AddCustomEmptyItem(this LayoutItemCollection items,
			Action<EmptyLayoutItem> method)
		{
			var emptyLayoutItem = new EmptyLayoutItem()
			{
				CssClass = "empty-form-item",
			};

			method.Invoke(emptyLayoutItem);
			items.Add(emptyLayoutItem);
		}


		public static void SetDefaultSettingsForSimple(this FormLayoutProperties formLayout)
		{
			SetDefaultCommonSettings(formLayout);

			var a = formLayout.SettingsAdaptivity;
			a.SwitchToSingleColumnAtWindowInnerWidth = 840;
			a.AdaptivityMode = FormLayoutAdaptivityMode.SingleColumnWindowLimit;
		}

		public static void SetDefaultSettingsForSimpleWide(this FormLayoutProperties formLayout)
		{
			SetDefaultCommonSettings(formLayout);

			var a = formLayout.SettingsAdaptivity;
			a.SwitchToSingleColumnAtWindowInnerWidth = 1080;
			a.AdaptivityMode = FormLayoutAdaptivityMode.SingleColumnWindowLimit;
		}

		private static void SetDefaultCommonSettings(FormLayoutProperties formLayout)
		{
			formLayout.AlignItemCaptionsInAllGroups = true;
			formLayout.UseDefaultPaddings = true;
			formLayout.RequiredMarkDisplayMode = RequiredMarkMode.None;

			var itemStyle = formLayout.Styles.LayoutItem;
			itemStyle.Caption.CssClass = "dxeBase_Metropolis";
			itemStyle.CaptionCell.Paddings.PaddingRight = Unit.Pixel(25);
		}

		#endregion

		#region Configuración de control de árbol

		public static void SetDefaultQuerySettings(this TreeListSettings treeList)
		{
			SetDefaultCommonSettings(treeList);

			var settings = treeList.Settings;
			settings.ShowFilterRow = true;
			settings.ShowFilterRowMenu = true;
			settings.AutoFilterCondition = AutoFilterCondition.Contains;

			treeList.SettingsSearchPanel.Visible = true;
			treeList.Styles.SearchPanel.CssClass = "searchPanel";
		}

		public static void SetDefaultEditSettings(this TreeListSettings treeList)
		{
			SetDefaultCommonSettings(treeList);

			var settings = treeList.Settings;
			settings.ShowFilterRow = false;
			settings.ShowFilterRowMenu = false;

			treeList.SettingsSearchPanel.Visible = false;
		}

		private static void SetDefaultCommonSettings(TreeListSettings treeList)
		{
			treeList.Width = Unit.Percentage(100);

			var settings = treeList.Settings;
			settings.GridLines = GridLines.Both;
			settings.ShowTreeLines = true;

			var behavior = treeList.SettingsBehavior;
			behavior.AllowEllipsisInText = true;
			behavior.AllowFocusedNode = true;
			behavior.AllowDragDrop = false;
			behavior.AutoExpandAllNodes = true;
			behavior.ExpandCollapseAction = TreeListExpandCollapseAction.NodeDblClick;

			var header = treeList.Styles.Header;
			header.BackColor = System.Drawing.Color.FromArgb(255, 255, 191, 102);
			header.Font.Bold = true;

			var pager = treeList.SettingsPager;
			pager.Visible = true;
			pager.Mode = DevExpress.Web.ASPxTreeList.TreeListPagerMode.ShowPager;
			pager.AlwaysShowPager = true;
			pager.Summary.EmptyText = "No hay datos para mostrar";

			var loading = treeList.SettingsLoadingPanel;
			loading.Text = "Cargando...";

			var styles = treeList.Styles;
			styles.AlternatingNode.Enabled = DevExpress.Utils.DefaultBoolean.True;
			styles.AlternatingNode.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
			styles.Node.BackColor = System.Drawing.ColorTranslator.FromHtml("#f5f3f4");

			var editing = treeList.SettingsEditing;
			editing.Mode = TreeListEditMode.EditFormAndDisplayNode;
			editing.ShowModelErrorsForEditors = false;
			editing.ConfirmDelete = true;

			var selection = treeList.SettingsSelection;
			selection.Enabled = false;
			selection.AllowSelectAll = false;
			selection.Recursive = false;

			treeList.Styles.FilterBar.Border.BorderStyle = BorderStyle.None;
			treeList.Styles.SearchPanel.Border.BorderStyle = BorderStyle.None;
			treeList.Styles.Header.Border.BorderStyle = BorderStyle.None;
			treeList.ControlStyle.Border.BorderStyle = BorderStyle.Solid;
		}

		public static void EnableWrapForAllColumns(this TreeListSettings treeList)
		{
			foreach (TreeListColumn column in treeList.Columns)
			{
				column.HeaderStyle.Wrap = DevExpress.Utils.DefaultBoolean.True;
			}
		}

		public static void SetClientJSProperties(this TreeListSettings treeList,
			string operationMessageType, string operationMessage,
			Dictionary<string, object> extraProperties = null)
		{
			treeList.CustomJSProperties =
				(s, e) => EditorControlClientJSPropertiesHandler(e, operationMessageType, operationMessage, extraProperties);
		}

		#endregion

		#region Configuración de controles editores varios

		public static void SetDefaultSettings(this ComboBoxSettings comboBox)
		{
			comboBox.Width = Unit.Percentage(100);

			var p = comboBox.Properties;
			p.DropDownStyle = DropDownStyle.DropDownList;
			p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
			p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;
			p.IncrementalFilteringDelay = 250;

			//comboBox.CustomFiltering = (sender, args) =>
			//{
			//	args.CustomHighlighting = args.Filter;
			//};

			comboBox.ItemTextCellPrepared = (sender, e) =>
			{
				if (e.Column != null)
				{
					var cellValue = Convert.ToString(e.Item.GetFieldValue(e.Column.FieldName));
					e.TextCell.ToolTip = HttpUtility.HtmlDecode(cellValue);
				}
				else
				{
					e.TextCell.ToolTip = HttpUtility.HtmlDecode(e.Item.Text);
				}
			};
		}
		public static void SetDefaultSettings(this ComboBoxProperties comboBox)
		{
			comboBox.Width = Unit.Percentage(100);

			comboBox.DropDownStyle = DropDownStyle.DropDownList;
			comboBox.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
			comboBox.IncrementalFilteringMode = IncrementalFilteringMode.Contains;
			comboBox.IncrementalFilteringDelay = 250;

			//comboBox.CustomFiltering += (sender, args) =>
			//{
			//	args.CustomHighlighting = args.Filter;
			//};
		}

		public static void SetDefaultSettings(this TokenBoxSettings tokenBox)
		{
			tokenBox.Width = Unit.Percentage(100);

			var p = tokenBox.Properties;
			p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
			p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;
			p.IncrementalFilteringDelay = 250;
			p.ValueSeparator = '|';

			tokenBox.ItemTextCellPrepared = (sender, e) =>
			{
				if (e.Column != null)
				{
					var cellValue = Convert.ToString(e.Item.GetFieldValue(e.Column.FieldName));
					e.TextCell.ToolTip = HttpUtility.HtmlDecode(cellValue);
				}
				else
				{
					e.TextCell.ToolTip = HttpUtility.HtmlDecode(e.Item.Text);
				}
			};
		}
		public static void SetDefaultSettings(this TokenBoxProperties tokenBox)
		{
			tokenBox.Width = Unit.Percentage(100);

			tokenBox.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
			tokenBox.IncrementalFilteringMode = IncrementalFilteringMode.Contains;
			tokenBox.IncrementalFilteringDelay = 250;
		}
		public static void SetDefaultSettingsForInteger(this SpinEditSettings spinEdit)
		{
			spinEdit.Width = Unit.Percentage(100);
			spinEdit.ControlStyle.HorizontalAlign = HorizontalAlign.Right;

			var p = spinEdit.Properties;
			p.SpinButtons.ShowIncrementButtons = false;
			p.SpinButtons.ShowLargeIncrementButtons = false;
			p.MinValue = 0.00m;
			p.MaxValue = 100000000.00m;
			p.NumberType = SpinEditNumberType.Integer;
			p.NumberFormat = SpinEditNumberFormat.Number;
			p.DisplayFormatString = GlobalUtils.IntegerFormat;
			p.DecimalPlaces = 0;
			p.DisplayFormatInEditMode = true;
		}
		public static void SetDefaultSettingsForInteger(this SpinEditProperties spinEdit)
		{
			spinEdit.Width = Unit.Percentage(100);
			spinEdit.Style.HorizontalAlign = HorizontalAlign.Right;

			spinEdit.SpinButtons.ShowIncrementButtons = false;
			spinEdit.SpinButtons.ShowLargeIncrementButtons = false;
			spinEdit.MinValue = 0.00m;
			spinEdit.MaxValue = 100000000.00m;
			spinEdit.NumberType = SpinEditNumberType.Integer;
			spinEdit.NumberFormat = SpinEditNumberFormat.Number;
			spinEdit.DisplayFormatString = GlobalUtils.IntegerFormat;
			spinEdit.DecimalPlaces = 0;
			spinEdit.DisplayFormatInEditMode = true;
		}

		public static void SetDefaultSettingsForMoney(this SpinEditSettings spinEdit)
		{
			spinEdit.Width = Unit.Percentage(100);
			spinEdit.ControlStyle.HorizontalAlign = HorizontalAlign.Right;

			var p = spinEdit.Properties;
			p.SpinButtons.ShowIncrementButtons = false;
			p.SpinButtons.ShowLargeIncrementButtons = false;
			p.MinValue = 0.00m;
			p.MaxValue = 1000000000.00m;
			p.NumberType = SpinEditNumberType.Float;
			p.NumberFormat = SpinEditNumberFormat.Currency;
			p.DisplayFormatString = GlobalUtils.CurrencyFormat;
			p.DecimalPlaces = 2;
			p.DisplayFormatInEditMode = true;
		}
		public static void SetDefaultSettingsForMoney(this SpinEditProperties spinEdit)
		{
			spinEdit.Width = Unit.Percentage(100);
			spinEdit.Style.HorizontalAlign = HorizontalAlign.Right;

			spinEdit.SpinButtons.ShowIncrementButtons = false;
			spinEdit.SpinButtons.ShowLargeIncrementButtons = false;
			spinEdit.MinValue = 0.00m;
			spinEdit.MaxValue = 1000000000.00m;
			spinEdit.NumberType = SpinEditNumberType.Float;
			spinEdit.NumberFormat = SpinEditNumberFormat.Currency;
			spinEdit.DisplayFormatString = GlobalUtils.CurrencyFormat;
			spinEdit.DecimalPlaces = 2;
			spinEdit.DisplayFormatInEditMode = true;
		}

		public static void SetDefaultSettingsForDecimal(this SpinEditSettings spinEdit)
		{
			spinEdit.Width = Unit.Percentage(100);
			spinEdit.ControlStyle.HorizontalAlign = HorizontalAlign.Right;

			var p = spinEdit.Properties;
			p.SpinButtons.ShowIncrementButtons = false;
			p.SpinButtons.ShowLargeIncrementButtons = false;
			p.MinValue = 0.00m;
			p.MaxValue = 1000000000.00m;
			p.NumberType = SpinEditNumberType.Float;
			p.NumberFormat = SpinEditNumberFormat.Number;
			p.DisplayFormatString = GlobalUtils.Decimal4Format;
			p.DecimalPlaces = 4;
			p.DisplayFormatInEditMode = true;
		}
		public static void SetDefaultSettingsForDecimal(this SpinEditProperties spinEdit)
		{
			spinEdit.Width = Unit.Percentage(100);
			spinEdit.Style.HorizontalAlign = HorizontalAlign.Right;

			spinEdit.SpinButtons.ShowIncrementButtons = false;
			spinEdit.SpinButtons.ShowLargeIncrementButtons = false;
			spinEdit.MinValue = 0.00m;
			spinEdit.MaxValue = 1000000000.00m;
			spinEdit.NumberType = SpinEditNumberType.Float;
			spinEdit.NumberFormat = SpinEditNumberFormat.Number;
			spinEdit.DisplayFormatString = GlobalUtils.Decimal4Format;
			spinEdit.DecimalPlaces = 4;
			spinEdit.DisplayFormatInEditMode = true;
		}

		public static void SetDefaultSettingsForPercentage(this SpinEditSettings spinEdit)
		{
			spinEdit.Width = Unit.Percentage(100);
			spinEdit.ControlStyle.HorizontalAlign = HorizontalAlign.Right;

			var p = spinEdit.Properties;
			p.SpinButtons.ShowIncrementButtons = false;
			p.SpinButtons.ShowLargeIncrementButtons = false;
			p.MinValue = 0.00m;
			p.MaxValue = 100.00m;
			p.NumberType = SpinEditNumberType.Float;
			p.NumberFormat = SpinEditNumberFormat.Number;
			p.DisplayFormatString = GlobalUtils.DecimalFormat;
			p.DecimalPlaces = 2;
			p.DisplayFormatInEditMode = true;
		}
		public static void SetDefaultSettingsForPercentage(this SpinEditProperties spinEdit)
		{
			spinEdit.Width = Unit.Percentage(100);
			spinEdit.Style.HorizontalAlign = HorizontalAlign.Right;

			spinEdit.SpinButtons.ShowIncrementButtons = false;
			spinEdit.SpinButtons.ShowLargeIncrementButtons = false;
			spinEdit.MinValue = 0.00m;
			spinEdit.MaxValue = 100.00m;
			spinEdit.NumberType = SpinEditNumberType.Float;
			spinEdit.NumberFormat = SpinEditNumberFormat.Number;
			spinEdit.DisplayFormatString = GlobalUtils.DecimalFormat;
			spinEdit.DecimalPlaces = 2;
			spinEdit.DisplayFormatInEditMode = true;
		}

		public static void SetDefaultQuerySettings(this DateEditSettings dateEdit)
		{
			dateEdit.Width = Unit.Percentage(100);

			var p = dateEdit.Properties;
			p.DisplayFormatInEditMode = true;
			p.DisplayFormatString = GlobalUtils.DateFormat;
			p.EditFormat = EditFormat.Custom;
			p.EditFormatString = GlobalUtils.DateFormat;

			p.CalendarProperties.Columns = 1;

			p.ClientSideEvents.GotFocus = "function(s, e) { s.ShowDropDown(); }"; // DropDown on focus()
		}
		public static void SetDefaultQuerySettings(this DateEditProperties dateEdit)
		{
			dateEdit.Width = Unit.Percentage(100);

			dateEdit.DisplayFormatInEditMode = true;
			dateEdit.DisplayFormatString = GlobalUtils.DateFormat;
			dateEdit.EditFormat = EditFormat.Custom;
			dateEdit.EditFormatString = GlobalUtils.DateFormat;

			dateEdit.CalendarProperties.Columns = 1;

			dateEdit.ClientSideEvents.GotFocus = "function(s, e) { s.ShowDropDown(); }"; // DropDown on focus()
		}

		public static void SetDefaultReadOnlySettings(this DateEditSettings dateEdit)
		{
			dateEdit.Width = Unit.Percentage(100);
			dateEdit.ReadOnly = true;

			var p = dateEdit.Properties;
			p.DisplayFormatInEditMode = true;
			p.DisplayFormatString = GlobalUtils.DateFormat;
			p.EditFormat = EditFormat.Custom;
			p.EditFormatString = GlobalUtils.DateFormat;

			p.DropDownButton.Visible = false;
		}
		public static void SetDefaultReadOnlySettings(this DateEditProperties dateEdit)
		{
			dateEdit.Width = Unit.Percentage(100);

			dateEdit.DisplayFormatInEditMode = true;
			dateEdit.DisplayFormatString = GlobalUtils.DateFormat;
			dateEdit.EditFormat = EditFormat.Custom;
			dateEdit.EditFormatString = GlobalUtils.DateFormat;

			dateEdit.DropDownButton.Visible = false;
		}

		public static void SetDefaultQuerySettingsForDateTime(this DateEditSettings dateEdit)
		{
			dateEdit.Width = Unit.Percentage(100);

			var p = dateEdit.Properties;
			p.DisplayFormatInEditMode = true;
			p.DisplayFormatString = GlobalUtils.DateTimeFormat;
			p.EditFormat = EditFormat.Custom;
			p.EditFormatString = GlobalUtils.DateTimeFormat;

			p.CalendarProperties.Columns = 1;

			p.TimeSectionProperties.Visible = true;
			p.TimeSectionProperties.Adaptive = true;
			p.TimeSectionProperties.TimeEditProperties.EditFormat = EditFormat.Custom;
			p.TimeSectionProperties.TimeEditProperties.EditFormatString = GlobalUtils.TimeFormat;

			p.ClientSideEvents.GotFocus = "function(s, e) { s.ShowDropDown(); }"; // DropDown on focus()
		}
		public static void SetDefaultQuerySettingsForDateTime(this DateEditProperties dateEdit)
		{
			dateEdit.Width = Unit.Percentage(100);

			dateEdit.DisplayFormatInEditMode = true;
			dateEdit.DisplayFormatString = GlobalUtils.DateTimeFormat;
			dateEdit.EditFormat = EditFormat.Custom;
			dateEdit.EditFormatString = GlobalUtils.DateTimeFormat;

			dateEdit.CalendarProperties.Columns = 1;

			dateEdit.TimeSectionProperties.Visible = true;
			dateEdit.TimeSectionProperties.Adaptive = true;
			dateEdit.TimeSectionProperties.TimeEditProperties.EditFormat = EditFormat.Custom;
			dateEdit.TimeSectionProperties.TimeEditProperties.EditFormatString = GlobalUtils.TimeFormat;

			dateEdit.ClientSideEvents.GotFocus = "function(s, e) { s.ShowDropDown(); }"; // DropDown on focus()
		}
		public static void SetDefaultReadOnlySettingsForDateTime(this DateEditSettings dateEdit)
		{
			dateEdit.Width = Unit.Percentage(100);
			dateEdit.ReadOnly = true;

			var p = dateEdit.Properties;
			p.DisplayFormatInEditMode = true;
			p.DisplayFormatString = GlobalUtils.DateTimeFormat;
			p.EditFormat = EditFormat.DateTime;
			p.EditFormatString = GlobalUtils.DateTimeFormat;

			p.DropDownButton.Visible = false;
		}

		public static void SetClientJSProperties(this EditorSettings editor,
			string operationMessageType, string operationMessage,
			Dictionary<string, object> extraProperties = null)
		{
			editor.CustomJSProperties =
				(s, e) => EditorControlClientJSPropertiesHandler(e, operationMessageType, operationMessage, extraProperties);
		}

		public static void SetClientJSProperties(this PopupMenuSettings editor,
			string operationMessageType, string operationMessage,
			Dictionary<string, object> extraProperties = null)
		{
			editor.CustomJSProperties =
				(s, e) => EditorControlClientJSPropertiesHandler(e, operationMessageType, operationMessage, extraProperties);
		}

		public static void SetClientJSProperties(this ButtonSettings editor,
			string operationMessageType, string operationMessage,
			Dictionary<string, object> extraProperties = null)
		{
			editor.CustomJSProperties =
				(s, e) => EditorControlClientJSPropertiesHandler(e, operationMessageType, operationMessage, extraProperties);
		}

		public static void SetClientJSProperties(this CollapsiblePanelSettings panel,
			string operationMessageType, string operationMessage,
			Dictionary<string, object> extraProperties = null)
		{
			panel.CustomJSProperties =
				(s, e) => EditorControlClientJSPropertiesHandler(e, operationMessageType, operationMessage, extraProperties);
		}

		public static void SetClientJSProperties(this UploadControlSettings uploadControl,
			string operationMessageType, string operationMessage,
			Dictionary<string, object> extraProperties = null)
		{
			uploadControl.CustomJSProperties =
				(s, e) => EditorControlClientJSPropertiesHandler(e, operationMessageType, operationMessage, extraProperties);
		}

		private static void EditorControlClientJSPropertiesHandler(
			CustomJSPropertiesEventArgs eventArgs,
			string operationMessageType, string operationMessage,
			Dictionary<string, object> extraProperties)
		{
			eventArgs.Properties["cpOperationMessageType"] = operationMessageType;
			eventArgs.Properties["cpOperationMessage"] = operationMessage;

			if (extraProperties != null)
			{
				foreach (var keyValue in extraProperties)
				{
					eventArgs.Properties.Add("cp" + keyValue.Key, keyValue.Value);
				}
			}
		}

		public static void SetDefaultSettings(this PopupControlSettings popup)
		{
			popup.AllowDragging = true;
			popup.ShowOnPageLoad = false;
			popup.CloseOnEscape = true;
			popup.Modal = true;
			popup.PopupAction = PopupAction.None;
			popup.CloseAction = CloseAction.CloseButton;
			popup.MinWidth = Unit.Pixel(400);
			popup.MinHeight = Unit.Pixel(100);
			popup.PopupHorizontalAlign = PopupHorizontalAlign.WindowCenter;
			popup.PopupVerticalAlign = PopupVerticalAlign.WindowCenter;
			popup.HeaderText = "Sistema de Empacadoras - Panaceasoft";
		}

		public static void SetQueryDataDefaultSettings(this PopupControlSettings popup)
		{
			popup.AllowDragging = true;
			popup.ShowOnPageLoad = false;
			popup.CloseOnEscape = true;
			popup.Modal = true;
			popup.PopupAction = PopupAction.None;
			popup.CloseAction = CloseAction.CloseButton;
			popup.MinWidth = Unit.Pixel(1000);
			popup.MinHeight = Unit.Pixel(300);
			popup.PopupHorizontalAlign = PopupHorizontalAlign.WindowCenter;
			popup.PopupVerticalAlign = PopupVerticalAlign.WindowCenter;
		}

		public static void SetDefaultQuerySettings(this RadioButtonListSettings radioButtonSettings)
		{
			radioButtonSettings.Width = Unit.Percentage(100);
			radioButtonSettings.SelectedIndex = 0;

			var p = radioButtonSettings.Properties;
			p.RepeatColumns = 1;
			p.RepeatDirection = RepeatDirection.Vertical;
		}

		public static void SetDefaultDetailEditSettings(this RadioButtonListSettings radioButtonSettings)
		{
			radioButtonSettings.Width = Unit.Percentage(100);

			var p = radioButtonSettings.Properties;
			p.RepeatColumns = 2;
			p.RepeatDirection = RepeatDirection.Horizontal;
		}

		#endregion
	}
}