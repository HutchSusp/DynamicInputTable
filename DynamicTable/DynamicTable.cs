using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DynamicTable
{
    [ToolboxData("<{0}:DynamicTextBoxTable runat=server></{0}:DynamicTextBoxTable>")]
    public class DynamicTable : Table
    {
        [Bindable(true)]
        [Category("Appearance")]
        [Localizable(true)]
        public int Columns {
            get {
                int? c = (int?)ViewState["Columns_" + ID];
                return c ?? 0;
            }
            set {
                ViewState["Columns_" + ID] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [Localizable(true)]
        public int InitialRows {
            get {
                int? ir = (int?)ViewState["InitialRows_" + ID];
                return ir ?? 0;
            }
            set {
                ViewState["InitialRows_" + ID] = value;
            }
        }

        [Bindable(false)]
        [Category("Appearance")]
        protected int CurrentRows {
            get {
                int? cr = (int?)ViewState["CurrentRows_" + ID];
                return cr ?? InitialRows;
            }
            set {
                ViewState["CurrentRows_" + ID] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [Localizable(true)]
        public string HeaderRow {
            get {
                string hr = (string)ViewState["HeaderRow_" + ID];
                return hr ?? string.Empty;
            }
            set {
                ViewState["HeaderRow_" + ID] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [Localizable(true)]
        public string ColumnFormat {
            get {
                string cf = (string)ViewState["ColumnFormat_" + ID];
                return cf ?? string.Empty;
            }
            set {
                ViewState["ColumnFormat_" + ID] = value;
            }
        }

        [Bindable(true)]
        [Category("Appeaerance")]
        [Localizable(true)]
        public Color RowColor {
            get {
                Color? rc = (Color?)ViewState["RowColor_" + ID];
                return rc ?? Color.White;
            }
            set {
                ViewState["RowColor_" + ID] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [Localizable(true)]
        public Color RowAlternatingColor {
            get {
                Color? arc = (Color?)ViewState["RowAlternatingColor_" + ID];
                return arc ?? Color.White;
            }
            set {
                ViewState["RowAlternatingColor_" + ID] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [Localizable(true)]
        public Color HeaderRowColor {
            get {
                Color? hrc = (Color?)ViewState["HeaderRowColor_" + ID];
                return hrc ?? Color.White;
            }
            set {
                ViewState["HeaderRowColor_" + ID] = value;
            }
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
                CurrentRows += numrows;
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
            GenerateHeaderRow();
            InsertRow(InitialRows);
        }

        public override void Dispose() {
            base.Dispose();
        }

        public DynamicTable() : base() {
            GenerateHeaderRow();
            InsertRow(CurrentRows);
        }

        
    }
}
