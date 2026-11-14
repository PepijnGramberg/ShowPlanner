using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Xunit;
using static Theaterverdeling.Tests.FormDriver;

namespace Theaterverdeling.Tests
{
  // Form tests share the list of open windows, so they must not run side by side.
  [Collection("Forms")]
  public class BulkGroupTests
  {
    private static readonly DateTime Saturday = new DateTime(2026, 3, 14);
    private static readonly DateTime Sunday = new DateTime(2026, 3, 15);

    [Fact]
    public void Grid_has_a_tick_column_for_every_showing_date()
    {
      OnUiThread(() =>
      {
        using var main = MainWithTwoShowings();
        var grid = OpenGrid(main);

        Assert.Equal(
          new[] { "Naam", "Aantal", "Vooraan", "Ochtend", "14-03-2026", "15-03-2026", "Melding" },
          grid.Columns.Cast<DataGridViewColumn>().Select(column => column.HeaderText));
      });
    }

    [Fact]
    public void Pasted_rows_appear_in_the_grid()
    {
      OnUiThread(() =>
      {
        using var main = MainWithTwoShowings();
        var grid = OpenGrid(main);

        Dialog().PasteText("De Regenboog\t28\nHet Kompas\t31");

        Assert.Equal(new[] { "De Regenboog", "Het Kompas" }, Column(grid, "Naam"));
        Assert.Equal(new[] { "28", "31" }, Column(grid, "Aantal"));
      });
    }

    [Fact]
    public void Adding_puts_the_groups_in_the_plan_and_closes_the_dialog()
    {
      OnUiThread(() =>
      {
        using var main = MainWithTwoShowings();
        OpenGrid(main);
        Dialog().PasteText("De Regenboog\t28\nHet Kompas\t31");

        Find<Button>(Dialog(), "Addbutton").PerformClick();

        Assert.Equal(new[] { "De Regenboog 28", "Het Kompas 31" }, Texts(main, "Groupviewer"));
        Assert.Equal("59/1100 stoelen", Find<Label>(main, "Seatlabel").Text);
        Assert.Empty(Application.OpenForms.OfType<Bulkgroupform>());
      });
    }

    [Fact]
    public void Typed_row_is_added_like_a_pasted_one()
    {
      OnUiThread(() =>
      {
        using var main = MainWithTwoShowings();
        var grid = OpenGrid(main);
        grid.Rows.Add("De Vlieger", "24");

        Find<Button>(Dialog(), "Addbutton").PerformClick();

        Assert.Equal(new[] { "De Vlieger 24" }, Texts(main, "Groupviewer"));
      });
    }

    [Fact]
    public void Row_with_a_problem_stays_in_the_grid_with_its_message()
    {
      OnUiThread(() =>
      {
        using var main = MainWithTwoShowings();
        var grid = OpenGrid(main);
        Dialog().PasteText("De Regenboog\t28\nHet Kompas\tdertig");

        Find<Button>(Dialog(), "Addbutton").PerformClick();

        Assert.Equal(new[] { "De Regenboog 28" }, Texts(main, "Groupviewer"));
        Assert.Equal(new[] { "Het Kompas" }, Column(grid, "Naam"));
        Assert.Equal(new[] { "Vul bij aantal een heel getal in." }, Column(grid, "Melding"));
      });
    }

    [Fact]
    public void Ticks_in_the_grid_become_the_wishes_of_the_group()
    {
      OnUiThread(() =>
      {
        using var main = MainWithTwoShowings();
        var grid = OpenGrid(main);
        Dialog().PasteText("De Regenboog\t28");
        grid.Rows[0].Cells["Vooraan"].Value = true;
        grid.Rows[0].Cells["Ochtend"].Value = true;
        grid.Rows[0].Cells[5].Value = true;

        Find<Button>(Dialog(), "Addbutton").PerformClick();

        var group = SavedPlan(main).Groups.Single();
        Assert.True(group.Firstrows);
        Assert.True(group.Morning);
        Assert.Equal(new[] { Sunday }, group.Datum);
      });
    }

    [Fact]
    public void Adding_groups_closes_the_preview_because_it_is_out_of_date()
    {
      OnUiThread(() =>
      {
        using var main = MainWithTwoShowings();
        Find<Button>(main, "Dividebutton").PerformClick();
        OpenGrid(main);
        Dialog().PasteText("De Regenboog\t28");

        Find<Button>(Dialog(), "Addbutton").PerformClick();

        Assert.Empty(Application.OpenForms.OfType<PreviewForm>());
      });
    }

    private static Theaterverdeling MainWithTwoShowings()
    {
      var main = new Theaterverdeling();
      main.Show();
      AddShowing(main, Sunday, Sunday.AddHours(13));
      AddShowing(main, Saturday, Saturday.AddHours(10).AddMinutes(30));
      return main;
    }

    private static DataGridView OpenGrid(Form main)
    {
      Find<Button>(main, "Bulkgroupbutton").PerformClick();
      return Find<DataGridView>(Dialog(), "Groupgrid");
    }

    private static Bulkgroupform Dialog()
    {
      return Application.OpenForms.OfType<Bulkgroupform>().Single();
    }

    // The values of one column, without the empty row at the bottom where a new row is typed.
    private static List<string> Column(DataGridView grid, string name)
    {
      return grid.Rows.Cast<DataGridViewRow>()
        .Where(row => !row.IsNewRow)
        .Select(row => Convert.ToString(row.Cells[name].Value))
        .ToList();
    }

    private static List<string> Texts(Control main, string listName)
    {
      return Find<ListView>(main, listName).Items.Cast<ListViewItem>().Select(item => item.Text).ToList();
    }

    private static Plan SavedPlan(Theaterverdeling main)
    {
      var folder = Path.Combine(AppContext.BaseDirectory, "testplans");
      Directory.CreateDirectory(folder);
      var path = Path.Combine(folder, Guid.NewGuid() + ".json");
      main.SavePlan(path);
      return Plan.Load(path);
    }
  }
}
