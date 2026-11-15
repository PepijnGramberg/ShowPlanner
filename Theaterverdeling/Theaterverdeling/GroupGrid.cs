using System.Windows.Forms;

namespace Theaterverdeling
{
  // The grid in which many groups are typed. Enter on the size of a group goes to the name of the next row,
  // so groups can be typed one after the other: name, Tab, size, Enter.
  public class GroupGrid : DataGridView
  {
    public const string NameColumn = "Naam";
    public const string PeopleColumn = "Aantal";

    // Enter while a cell is being typed in.
    protected override bool ProcessDialogKey(Keys keyData)
    {
      return (keyData == Keys.Enter && ToNextName()) || base.ProcessDialogKey(keyData);
    }

    // Enter on a cell that is not being typed in.
    protected override bool ProcessDataGridViewKey(KeyEventArgs e)
    {
      return (e.KeyData == Keys.Enter && ToNextName()) || base.ProcessDataGridViewKey(e);
    }

    // Goes from the size of a group to the name of the next row. False when the cursor is somewhere else
    // or there is no next row; Enter then does what it does in any grid.
    private bool ToNextName()
    {
      if (CurrentCell == null || Columns[CurrentCell.ColumnIndex].Name != PeopleColumn || !EndEdit())
      {
        return false;
      }
      var next = CurrentCell.RowIndex + 1;
      if (next >= Rows.Count)
      {
        return false;
      }
      CurrentCell = Rows[next].Cells[NameColumn];
      return true;
    }
  }
}
