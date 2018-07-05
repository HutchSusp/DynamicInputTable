using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace DynamicTable
{
    [ToolboxData("<{0}:DynamicTextBoxTable runat=server></{0}:DynamicTextBoxTable>")]
    public class DynamicTable : Table
    {
        [Bindable(true)]
        [Category("Appearance")]
        [Localizable(true)]
        public int Columns { get; set; }

        [Bindable(true)]
        [Category("Appearance")]
        [Localizable(true)]
        public int InitialRows { get; set; }

        [Bindable(false)]
        [Category("Appearance")]
        protected int CurrentRows { get; set; }

        [Bindable(true)]
        [Category("Appearance")]
        [Localizable(true)]
        public string HeaderRow { get; set; }

        [Bindable(true)]
        [Category("Appearance")]
        [Localizable(true)]
        public string ColumnFormat { get; set; }

        [Bindable(true)]
        [Category("Appeaerance")]
        [Localizable(true)]
        public Color RowColor { get; set; }

        [Bindable(true)]
        [Category("Appearance")]
        [Localizable(true)]
        public Color RowAlternatingColor { get; set; }

        [Bindable(true)]
        [Category("Appearance")]
        [Localizable(true)]
        public Color HeaderRowColor { get; set; }

        protected override object SaveViewState() {
            ViewState["Columns_DynamicTable_" + ID] = Columns;
            ViewState["InitialRows_DynamicTable_" + ID] = InitialRows;
            ViewState["CurrentRows_DynamicTable_" + ID] = CurrentRows;
            ViewState["HeaderRow_DynamicTable_" + ID] = HeaderRow;
            ViewState["ColumnFormat_DynamicTable_" + ID] = ColumnFormat;
            ViewState["RowColor_DynamicTable_" + ID] = RowColor;
            ViewState["RowAlternatingColor_DynamicTable_" + ID] = RowAlternatingColor;
            ViewState["HeaderRowColor_DynamicTable_" + ID] = HeaderRowColor;
            ViewState["CellInput_DynamicTable_" + ID] = ExportCellInput();

            return base.SaveViewState();
        }

        protected override void LoadViewState(object savedState) {
            base.LoadViewState(savedState);

            Columns = (int)ViewState["Columns_DynamicTable_" + ID];
            InitialRows = (int)ViewState["InitialRows_DynamicTable_" + ID];
            CurrentRows = (int)ViewState["CurrentRows_DynamicTable_" + ID];
            HeaderRow = ViewState["HeaderRow_DynamicTable_" + ID] as string;
            ColumnFormat = ViewState["ColumnFormat_DynamicTable_" + ID] as string;
            RowColor = (Color)ViewState["RowColor_DynamicTable_" + ID];
            RowAlternatingColor = (Color)ViewState["RowAlternatingColor_DynamicTable_" + ID];
            HeaderRowColor = (Color)ViewState["HeaderRowColor_DynamicTable_" + ID];

            GenerateHeaderRow();
            InsertRow((CurrentRows == 0 ? InitialRows : CurrentRows));
            ImportCellInput(ViewState["CellInput_DynamicTable_" + ID] as string);
        }

        protected string ExportCellInput() {
            StringBuilder export = new StringBuilder();
            for (int r = 0; r == CurrentRows; r++) {
                for (int c = 0; c == Columns; c++)
                    export.Append((FindControl("Box_" + r + "_" + c) as TextBox).Text.Replace("$", "&dollar&").Replace("@", "&at&") + "$");
                if (export.Length > 0)
                    export.Length--;
                export.Append("@");
            }
            return export.ToString();
        }

        protected void ImportCellInput(string input) {
            string[] rows = input.Split('@');
            try {
                for (int r = 0; r < CurrentRows; r++) {
                    string[] cells = rows[r].Split('$');
                    for (int c = 0; c < Columns; c++)
                        (FindControl("Box_" + r + "_" + c) as TextBox).Text = cells[c].Replace("&dollar&", "$").Replace("&at&", "@");
                }
            }
            catch { }
        }

        public void InsertRow(int numrows = 1) {
            if (numrows < 0)
                throw new Exception("Invalid number of rows to be generated");
            else if (ColumnFormat.Split(';').Count() != Columns && !string.IsNullOrEmpty(ColumnFormat))
                throw new Exception("Invalid ColumnFormat for the number of Columns");
            else {
                for (int i = 0; i < numrows; i++) {
                    int rowindex = Rows.Count - 1;
                    TableRow row = new TableRow();
                    for (int j = 0; j < Columns; j++)
                        row.Cells.Add(GenerateCell(rowindex, j));
                    row.Style.Add(HtmlTextWriterStyle.BackgroundColor, (rowindex % 2 == 0 ? RowColor.ToString() : RowAlternatingColor.ToString()));
                    Rows.Add(row);
                }
                CurrentRows = Rows.Count - 1;
                DataBind();
            }
        }

        // Returns 2D array in the form of [row, column]
        // This method skips the header row
        public string[,] GetTableInput() {
            string[,] input = new string[Rows.Count, Columns];
            for (int r = 1; r < Rows.Count; r++)
                for (int c = 0; c < Columns; c++)
                    input[r, c] = (FindControl("Box_" + r + "_" + c) as TextBox).Text;
            return input;
        }

        protected TextBoxMode FormatTextBox(string formatstring, int columnindex) {
            switch (formatstring) {
                case "Color":
                    return TextBoxMode.Color;
                case "Date":
                    return TextBoxMode.Date;
                case "DateTime":
                    return TextBoxMode.DateTime;
                case "DateTimeLocal":
                    return TextBoxMode.DateTimeLocal;
                case "Email":
                    return TextBoxMode.Email;
                case "Month":
                    return TextBoxMode.Month;
                case "MultiLine":
                    return TextBoxMode.MultiLine;
                case "Number":
                    return TextBoxMode.Number;
                case "Password":
                    return TextBoxMode.Password;
                case "Phone":
                    return TextBoxMode.Phone;
                case "Range":
                    return TextBoxMode.Range;
                case "Search":
                    return TextBoxMode.Search;
                case "SingleLine":
                    return TextBoxMode.SingleLine;
                case "Time":
                    return TextBoxMode.Time;
                case "Url":
                    return TextBoxMode.Url;
                case "Week":
                    return TextBoxMode.Week;
                default:
                    throw new Exception("Invalid ColumnFormat for column " + columnindex);
            }
        }

        protected TableCell GenerateCell(int rowindex, int columnindex) {
            TableCell cell = new TableCell();

            cell.Controls.Add(new TextBox {
                ID = "Box_" + rowindex + "_" + columnindex,
                TextMode = FormatTextBox(ColumnFormat.Split(';')[columnindex], columnindex)
            });

            return cell;
        }

        protected TableHeaderCell GenerateHeaderCell(string headerText) {
            TableHeaderCell cell = new TableHeaderCell();

            cell.Controls.Add(new Label {
                Text = headerText
            });

            return cell;
        }

        protected void GenerateHeaderRow() {
            string[] headerarr = HeaderRow.Split(';');
            if (headerarr.Count() != Columns && !string.IsNullOrEmpty(HeaderRow))
                throw new Exception("Invalid Header Row for ammount of Columns");
            else {
                TableHeaderRow row = new TableHeaderRow();
                for (int i = 0; i < Columns; i++)
                    row.Cells.Add(GenerateHeaderCell(headerarr[i]));
                row.Style.Add(HtmlTextWriterStyle.BackgroundColor, HeaderRowColor.ToString());
                Rows.Add(row);
                DataBind();
            }
        }

        public void ResetTable() {
            Rows.Clear();
            CurrentRows = 0;
            GenerateHeaderRow();
            InsertRow(InitialRows);
        }
    }
}
