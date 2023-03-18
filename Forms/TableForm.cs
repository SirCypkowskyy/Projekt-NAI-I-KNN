using System.Drawing;
using System.Windows.Forms;
namespace NAI.Projekt.KNN_ConsoleApp_s24759.Forms;

public class TableForm : Form
{
    public List<string> TableHeaders { get; set; }
    public List<List<string>> TableRows { get; set; }
    public DataGridView WindowDataGrid { get; set; }

    public TableForm(IEnumerable<string> tableHeaders, IEnumerable<List<string>> tableRows)
    {
        TableHeaders = tableHeaders.ToList();
        TableRows = tableRows.ToList();

        WindowDataGrid = new DataGridView() {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            AllowUserToResizeColumns = false,
            MultiSelect = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
            ColumnHeadersVisible = true,
            RowHeadersVisible = false,
            RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing,
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
            CellBorderStyle = DataGridViewCellBorderStyle.Single,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            Font = new Font("Consolas", 10, FontStyle.Regular),
        };
        
        
        WindowDataGrid.ColumnCount = TableHeaders.Count;
        WindowDataGrid.RowCount = TableRows.Count;
        for (var i = 0; i < TableHeaders.Count; i++)
        {
            WindowDataGrid.Columns[i].DefaultCellStyle.ForeColor = Color.RebeccaPurple;
            WindowDataGrid.Columns[i].Name = TableHeaders[i];
        }
        
        var lastAddedDecidingAttribute = "";
        var colorsIndex = -1;
        var decidingAttributeColors = new[] {
            Color.Coral,
            Color.Crimson,
            Color.DarkCyan,
            Color.DarkGoldenrod,
            Color.DarkGreen,
            Color.DarkKhaki,
            Color.DarkRed,
            Color.DarkSalmon,
            Color.DarkSeaGreen,
        };

        var vectorValuesColors = new[] {
            Color.Brown,
            Color.CadetBlue,
            Color.DarkMagenta,
            Color.DarkOliveGreen,
            Color.DarkOrange,
            Color.DarkOrchid,
            Color.DarkBlue,
            Color.Fuchsia
        };
        
        for (var i = 0; i < TableRows.Count; i++)
        {
            if(lastAddedDecidingAttribute != TableRows[i][TableRows[i].Count - 1])
            {
                lastAddedDecidingAttribute = TableRows[i][TableRows[i].Count - 1];
                colorsIndex++;
            }
            WindowDataGrid.Rows[i].Cells[TableRows[i].Count - 1].Style.ForeColor = decidingAttributeColors[colorsIndex];
            for (var j = 0; j < TableRows[i].Count; j++)
            {
                if(j != TableRows[i].Count - 1)
                    WindowDataGrid.Rows[i].Cells[j].Style.ForeColor = vectorValuesColors[j];
                
                WindowDataGrid.Rows[i].Cells[j].Value = TableRows[i][j];
            }
        }
        
        Controls.Add(WindowDataGrid);
        
        var exitButton = new Button() {
            Text = "Zamknij",
            Dock = DockStyle.Bottom,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            AutoEllipsis = true,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Consolas", 10, FontStyle.Regular),
        };
        exitButton.Click += (sender, args) => Close();
        Controls.Add(exitButton);
    }
    
    

}
