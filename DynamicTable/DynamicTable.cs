using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DynamicTable
{
    [ToolboxData("<{0}:DynamicTextBoxTable runat=server></{0}:DynamicTextBoxTable>")]
    public class DynamicTable : Table
    {
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(5)]
        [Localizable(true)]
        public int Columns { get; set; } = 5;

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(5)]
        [Localizable(true)]
        public int InitialRows { get; set; } = 5;

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("Column 1;Column 2;Column 3;Column 4;Column 5")]
        [Localizable(true)]
        public string HeaderRow { get; set; } = "Column 1;Column 2;Column 3;Column 4;Column 5";

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("SingleLine;SingleLine;SingleLine;SingleLine;SingleLine")]
        [Localizable(true)]
        public string ColumnFormat { get; set; } = "SingleLine;SingleLine;SingleLine;SingleLine;SingleLine";

        [Bindable(true)]
        [Category("Appeaerance")]
        [DefaultValue(typeof(Color), "0xFFFFFF")]
        [Localizable(true)]
        public Color RowColor { get; set; } = Color.White;

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "0xFFFFFF")]
        [Localizable(true)]
        public Color RowAlternatingColor { get; set; } = Color.White;

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "0xFFFFFF")]
        [Localizable(true)]
        public Color HeaderRowColor { get; set; } = Color.White;

        public void InsertRow(int numrows = 1) {
            if (numrows < 1)
                throw new Exception("Invalid number of rows to be generated");
            else if (ColumnFormat.Split(';').Count() != Columns)
                throw new Exception("Invalid ColumnFormat for the number of Columns");
            else {
                for (int i = 0; i < numrows; i++) {
                    int rowindex = Rows.Count - 1;
                    TableRow row = new TableRow();
                    for (int j = 0; j < Columns; j++)
                        row.Cells.Add(GenerateCell(i, j));
                    row.Style.Add(HtmlTextWriterStyle.BackgroundColor, (rowindex % 2 == 0 ? RowColor.ToString() : RowAlternatingColor.ToString()));
                    Rows.Add(row);
                }
                DataBind();
            }
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
            if (headerarr.Count() != Columns)
                throw new Exception("");
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

        public DynamicTable() {
            GenerateHeaderRow();
            InsertRow(InitialRows);
        }
    }
}
