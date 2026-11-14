using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Xunit;
using static Theaterverdeling.Tests.FormDriver;

namespace Theaterverdeling.Tests
{
  // Form tests share the list of open windows, so they must not run side by side.
  [Collection("Forms")]
  public class HallEditorTests
  {
    [Fact]
    public void Halls_button_opens_the_editor_with_a_new_hall()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();

        Find<Button>(main, "Hallsbutton").PerformClick();

        var editor = Application.OpenForms.OfType<EditorForm>().Single();
        Assert.Equal(0, Find<ComboBox>(editor, "Hallpicker").SelectedIndex);
        Assert.Contains("Lampegiet", Find<ComboBox>(editor, "Hallpicker").Items.Cast<string>());
        Assert.Empty(Grid(editor).Rows);
      });
    }

    [Fact]
    public void Opening_a_hall_shows_its_rows_and_its_seats()
    {
      OnUiThread(() =>
      {
        var editor = Editor();

        editor.OpenHall("Lampegiet");

        var grid = Grid(editor);
        Assert.Equal(20, grid.Rows.Count);
        Assert.Equal("Lampegiet", Find<TextBox>(editor, "Hallname").Text);
        Assert.Equal(new[] { "Rij", "1", "23", "Rij 1 en 2", "1-2, 22-23" }, Cells(grid, 0).Take(5));
        Assert.Equal("Looppad", Cells(grid, 13)[0]);
        Assert.Equal("558 stoelen, 550 te gebruiken", Find<Label>(editor, "Countlabel").Text);
        Assert.Equal(550, Map(editor).Hall.Seats);
      });
    }

    [Fact]
    public void Added_rows_and_walkway_appear_in_the_grid_and_on_the_map()
    {
      OnUiThread(() =>
      {
        var editor = Editor();

        Find<Button>(editor, "Addrowbutton").PerformClick();
        Find<Button>(editor, "Addwalkwaybutton").PerformClick();
        Find<Button>(editor, "Addrowbutton").PerformClick();

        var grid = Grid(editor);
        Assert.Equal(new[] { "Rij", "Looppad", "Rij" }, Enumerable.Range(0, 3).Select(row => Cells(grid, row)[0]));
        Assert.Equal("2", Cells(grid, 2)[1]);
        Assert.Equal(20, Map(editor).Hall.Seats);
        Assert.Equal(2, Map(editor).SelectedRow);
      });
    }

    [Fact]
    public void Typing_a_seat_count_changes_the_map()
    {
      OnUiThread(() =>
      {
        var editor = Editor();
        Find<Button>(editor, "Addrowbutton").PerformClick();

        Grid(editor).Rows[0].Cells["Stoelen"].Value = "14";

        Assert.Equal(14, Map(editor).Hall.Seats);
        Assert.Equal("14 stoelen, 14 te gebruiken", Find<Label>(editor, "Countlabel").Text);
      });
    }

    [Fact]
    public void Typing_something_wrong_shows_a_message_on_that_line()
    {
      OnUiThread(() =>
      {
        var editor = Editor();
        Find<Button>(editor, "Addrowbutton").PerformClick();

        Grid(editor).Rows[0].Cells["Stoelen"].Value = "veel";

        Assert.Equal("Vul bij stoelen een heel getal van 1 tot 100 in.", Cells(Grid(editor), 0)[5]);
      });
    }

    [Fact]
    public void Clicking_a_seat_on_the_map_switches_it_off_and_on_again()
    {
      OnUiThread(() =>
      {
        var editor = Editor();
        editor.OpenHall("Lampegiet");
        var map = Map(editor);

        map.ClickAt(Centre(map.SeatBounds(2, 0)));

        Assert.Equal("1", Cells(Grid(editor), 2)[4]);
        Assert.Equal(549, map.Hall.Seats);
        Assert.Equal(2, map.SelectedRow);
        Assert.Equal(2, Grid(editor).CurrentCell.RowIndex);

        map.ClickAt(Centre(map.SeatBounds(2, 0)));

        Assert.Equal("", Cells(Grid(editor), 2)[4]);
        Assert.Equal(550, map.Hall.Seats);
      });
    }

    [Fact]
    public void Clicking_next_to_the_seats_chooses_the_row_without_changing_it()
    {
      OnUiThread(() =>
      {
        var editor = Editor();
        editor.OpenHall("Lampegiet");
        var map = Map(editor);
        var firstSeat = map.SeatBounds(5, 0);

        map.ClickAt(new Point(firstSeat.Left - firstSeat.Width / 2, firstSeat.Top + firstSeat.Height / 2));

        Assert.Equal(5, Grid(editor).CurrentCell.RowIndex);
        Assert.Equal(550, map.Hall.Seats);
      });
    }

    [Fact]
    public void Choosing_a_line_in_the_grid_marks_the_row_on_the_map()
    {
      OnUiThread(() =>
      {
        var editor = Editor();
        editor.OpenHall("Lampegiet");

        Grid(editor).CurrentCell = Grid(editor).Rows[7].Cells["Stoelen"];

        Assert.Equal(7, Map(editor).SelectedRow);
      });
    }

    [Fact]
    public void Removing_and_moving_work_on_the_chosen_line()
    {
      OnUiThread(() =>
      {
        var editor = Editor();
        Find<Button>(editor, "Addrowbutton").PerformClick();
        Find<Button>(editor, "Addrowbutton").PerformClick();
        Find<Button>(editor, "Addrowbutton").PerformClick();
        var grid = Grid(editor);

        Find<Button>(editor, "Upbutton").PerformClick();
        Assert.Equal(new[] { "1", "3", "2" }, Enumerable.Range(0, 3).Select(row => Cells(grid, row)[1]));
        Assert.Equal(1, grid.CurrentCell.RowIndex);

        Find<Button>(editor, "Removerowbutton").PerformClick();
        Assert.Equal(new[] { "1", "2" }, Enumerable.Range(0, 2).Select(row => Cells(grid, row)[1]));
      });
    }

    [Fact]
    public void Saved_hall_can_be_loaded_by_its_name()
    {
      OnUiThread(() =>
      {
        var name = TemporaryHallName();
        try
        {
          var editor = Editor();
          Find<Button>(editor, "Addrowbutton").PerformClick();
          Find<Button>(editor, "Addrowbutton").PerformClick();
          var map = Map(editor);
          map.ClickAt(Centre(map.SeatBounds(0, 9)));
          Grid(editor).Rows[1].Cells["Deel"].Value = "Balkon";
          Find<TextBox>(editor, "Hallname").Text = name;

          Find<Button>(editor, "Savebutton").PerformClick();

          Assert.Equal("Opgeslagen.", Find<Label>(editor, "Statuslabel").Text);
          Assert.Equal(name, Find<ComboBox>(editor, "Hallpicker").SelectedItem);
          var hall = Zaal.Load(name);
          Assert.Equal(19, hall.Seats);
          Assert.Equal(new[] { "Balkon" }, hall.Sections);
        }
        finally
        {
          File.Delete(Path.Combine(Zaal.HallsFolder, name + ".json"));
        }
      });
    }

    [Fact]
    public void Hall_with_a_problem_is_not_saved()
    {
      OnUiThread(() =>
      {
        var name = TemporaryHallName();
        var editor = Editor();
        Find<Button>(editor, "Addrowbutton").PerformClick();
        Grid(editor).Rows[0].Cells["Rij"].Value = "";
        Find<TextBox>(editor, "Hallname").Text = name;

        Assert.False(editor.SaveHall());

        Assert.Equal("Los eerst de meldingen bij de rijen op.", Find<Label>(editor, "Statuslabel").Text);
        Assert.False(Zaal.Exists(name));
      });
    }

    [Fact]
    public void Saving_under_the_name_of_another_hall_asks_first()
    {
      OnUiThread(() =>
      {
        var editor = Editor();
        var asked = "";
        editor.Confirm = question =>
        {
          asked = question;
          return false;
        };
        Find<Button>(editor, "Addrowbutton").PerformClick();
        Find<TextBox>(editor, "Hallname").Text = "Lampegiet";

        Assert.False(editor.SaveHall());

        Assert.Equal("Er is al een zaal \"Lampegiet\". Overschrijven?", asked);
        Assert.Equal(550, Zaal.Load("Lampegiet").Seats);
      });
    }

    [Fact]
    public void Opening_another_hall_with_unsaved_changes_asks_first()
    {
      OnUiThread(() =>
      {
        var editor = Editor();
        Find<Button>(editor, "Addrowbutton").PerformClick();
        editor.Confirm = question => false;

        editor.OpenHall("Lampegiet");

        Assert.Equal(0, Find<ComboBox>(editor, "Hallpicker").SelectedIndex);
        Assert.Single(Grid(editor).Rows.Cast<DataGridViewRow>());
      });
    }

    [Fact]
    public void Map_shows_seats_that_are_not_used_in_grey()
    {
      OnUiThread(() =>
      {
        var editor = Editor();
        editor.OpenHall("Lampegiet");
        var map = Map(editor);
        using var image = new Bitmap(map.Width, map.Height);

        map.DrawToBitmap(image, new Rectangle(Point.Empty, map.Size));

        var unused = map.SeatBounds(0, 0);
        var used = map.SeatBounds(0, 5);
        Assert.Equal(Zaal.ClosedSeatColor.ToArgb(), image.GetPixel(unused.Left + 3, unused.Top + 3).ToArgb());
        Assert.Equal(SeatMapControl.SelectedRowColor.ToArgb(), image.GetPixel(used.Left + 3, used.Top + 3).ToArgb());
      });
    }

    [Fact]
    public void Map_is_still_drawn_while_a_line_has_a_problem()
    {
      OnUiThread(() =>
      {
        var editor = Editor();
        editor.OpenHall("Lampegiet");
        Grid(editor).Rows[4].Cells["Stoelen"].Value = "veel";
        var map = Map(editor);
        using var image = new Bitmap(map.Width, map.Height);

        map.DrawToBitmap(image, new Rectangle(Point.Empty, map.Size));

        var unused = map.SeatBounds(0, 0);
        Assert.Equal(Zaal.ClosedSeatColor.ToArgb(), image.GetPixel(unused.Left + 3, unused.Top + 3).ToArgb());
      });
    }

    private static EditorForm Editor()
    {
      var editor = new EditorForm { Confirm = question => true };
      editor.Show();
      return editor;
    }

    private static DataGridView Grid(Form editor)
    {
      return Find<DataGridView>(editor, "Rowsgrid");
    }

    private static SeatMapControl Map(Form editor)
    {
      return Find<SeatMapControl>(editor, "Seatmap");
    }

    private static string[] Cells(DataGridView grid, int row)
    {
      return grid.Rows[row].Cells.Cast<DataGridViewCell>().Select(cell => Convert.ToString(cell.Value)).ToArray();
    }

    private static string TemporaryHallName()
    {
      return "Testzaal " + Guid.NewGuid().ToString("N");
    }

    private static Point Centre(Rectangle rect)
    {
      return new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
    }
  }
}
