using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Xunit;
using static Theaterverdeling.Tests.FormDriver;

namespace Theaterverdeling.Tests
{
  // Form tests share the list of open windows, so they must not run side by side.
  [Collection("Forms")]
  public class MainWindowTests
  {
    [Fact]
    public void Show_is_called_a_title_and_a_showing_a_voorstelling_with_date_and_time()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();

        Assert.Equal("Titel van de voorstelling", Find<Label>(main, "label1").Text);
        Assert.Equal("1. Voorstellingen (datum en tijd)", Find<Label>(main, "label2").Text);
        Assert.Equal("Voorstelling toevoegen", Find<Button>(main, "Addshow").Text);
      });
    }

    [Fact]
    public void Steps_are_numbered_and_the_last_one_stands_out_and_names_the_pdf()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();

        Assert.StartsWith("1. ", Find<Label>(main, "label2").Text);
        Assert.Equal("2. Groepen", Find<Label>(main, "label3").Text);
        Assert.Equal("3. Indeling", Find<Label>(main, "Steplabel3").Text);
        Assert.True(Find<Button>(main, "Dividebutton").Font.Bold);
        Assert.False(Find<Button>(main, "ResetButton").Font.Bold);
        Assert.Contains("PDF", Find<Label>(main, "Stephint").Text);
      });
    }

    [Fact]
    public void Groups_are_listed_in_columns_with_their_wishes_in_words()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();
        AddShowing(main, new DateTime(2026, 3, 14), new DateTime(2026, 3, 14, 10, 30, 0));
        Find<Button>(main, "Addgroupbutton").PerformClick();
        var dialog = Application.OpenForms.OfType<Addgroupform>().Single();
        Find<TextBox>(dialog, "Groepnaam").Text = "De Regenboog";
        Find<NumericUpDown>(dialog, "Groepsgrootte").Value = 28;
        Find<CheckBox>(dialog, "Morningcheck").Checked = true;
        Find<Button>(dialog, "Addgroupbutton").PerformClick();

        var groups = Find<ListView>(main, "Groupviewer");
        Assert.Equal(new[] { "Naam", "Aantal", "Vooraan", "Ochtend" }, groups.Columns.Cast<ColumnHeader>().Select(column => column.Text));
        Assert.Equal(
          new[] { "De Regenboog", "28", "", "ja" },
          groups.Items[0].SubItems.Cast<ListViewItem.ListViewSubItem>().Select(cell => cell.Text));
      });
    }

    [Fact]
    public void Seat_counter_is_in_words_and_red_when_there_are_too_few_seats()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();
        AddShowing(main, new DateTime(2026, 3, 14), new DateTime(2026, 3, 14, 10, 30, 0));
        var counter = Find<Label>(main, "Seatlabel");

        AddGroup(main, "De Regenboog", 300);
        Assert.Equal("300 personen, 550 stoelen", counter.Text);
        Assert.Equal(System.Drawing.SystemColors.ControlText, counter.ForeColor);

        AddGroup(main, "Het Kompas", 300);
        Assert.Equal("600 personen, 550 stoelen", counter.Text);
        Assert.Equal(System.Drawing.Color.Firebrick, counter.ForeColor);
      });
    }

    [Fact]
    public void Adding_a_group_without_a_showing_says_what_a_showing_is()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();
        var told = new List<string>();
        main.Tell = told.Add;

        Find<Button>(main, "Addgroupbutton").PerformClick();
        Find<Button>(main, "Bulkgroupbutton").PerformClick();

        Assert.Equal(2, told.Count);
        Assert.All(told, text => Assert.Equal("Voeg eerst een voorstelling toe: de datum en de tijd waarop gespeeld wordt.", text));
        Assert.Empty(Application.OpenForms.OfType<Addgroupform>());
      });
    }
  }
}
