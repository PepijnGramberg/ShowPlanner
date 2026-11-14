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
    public void Buttons_for_the_plan_file_say_that_they_are_about_the_plan()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();

        Assert.Equal("Plan opslaan", Find<Button>(main, "SaveButton").Text);
        Assert.Equal("Plan laden", Find<Button>(main, "LoadButton").Text);
        Assert.Equal("Nieuw plan", Find<Button>(main, "ResetButton").Text);
      });
    }

    [Fact]
    public void Lists_grow_with_the_window_and_the_buttons_stay_next_to_them()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();
        var groups = Find<ListView>(main, "Groupviewer");
        var showings = Find<ListView>(main, "Voorstellingviewer");
        var add = Find<Button>(main, "Addgroupbutton");
        var divide = Find<Button>(main, "Dividebutton");
        var before = (groups.Size, showings.Height, Gap: add.Left - groups.Right, Below: divide.Top - groups.Bottom, Name: groups.Columns[0].Width);

        main.Size = new System.Drawing.Size(main.Width + 200, main.Height + 150);

        Assert.Equal(before.Size.Width + 200, groups.Width);
        Assert.Equal(before.Size.Height + 150, groups.Height);
        Assert.Equal(before.Height + 150, showings.Height);
        Assert.Equal(before.Gap, add.Left - groups.Right);
        Assert.Equal(before.Below, divide.Top - groups.Bottom);
        Assert.Equal(before.Name + 200, groups.Columns[0].Width);
      });
    }

    [Fact]
    public void Halls_button_is_in_the_top_corner_away_from_the_plan_buttons()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();
        var halls = Find<Button>(main, "Hallsbutton");

        Assert.Equal("Zalen beheren", halls.Text);
        Assert.True(halls.Bottom < Find<ListView>(main, "Groupviewer").Top);
        Assert.True(halls.Left > Find<TextBox>(main, "voorstellingnaambox").Right);
        Assert.True(halls.Bottom < Find<Button>(main, "SaveButton").Top);
      });
    }

    [Fact]
    public void Every_window_shows_the_icon_of_the_app()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();
        AddShowing(main, new DateTime(2026, 3, 14), new DateTime(2026, 3, 14, 10, 30, 0));
        AddGroup(main, "De Regenboog", 28);
        Find<Button>(main, "Dividebutton").PerformClick();
        Find<Button>(main, "Hallsbutton").PerformClick();
        using var showing = new Addshowform(new Plan());
        using var group = new Addgroupform(new Plan());
        using var groups = new Bulkgroupform(new Plan());

        var windows = Application.OpenForms.Cast<Form>().Concat(new Form[] { showing, group, groups }).ToList();

        Assert.Equal(6, windows.Count);
        Assert.All(windows, window => Assert.Same(AppIcon.Value, window.Icon));
      });
    }

    [Fact]
    public void Column_headers_of_the_group_list_are_not_cut_off()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();
        var groups = Find<ListView>(main, "Groupviewer");

        Assert.All(
          groups.Columns.Cast<ColumnHeader>(),
          column => Assert.True(column.Width >= TextRenderer.MeasureText(column.Text, groups.Font).Width, column.Text));
      });
    }

    [Fact]
    public void Windows_fit_on_the_screen_of_a_small_laptop()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();
        AddShowing(main, new DateTime(2026, 3, 14), new DateTime(2026, 3, 14, 10, 30, 0));
        AddGroup(main, "De Regenboog", 28);
        Find<Button>(main, "Dividebutton").PerformClick();
        Find<Button>(main, "Hallsbutton").PerformClick();

        // A laptop screen of 1366 by 768 has about 728 pixels above the taskbar. The app runs with a
        // font that is a size larger than the one in this test, which makes its windows 17/15 as high.
        Assert.True(main.Height * 17 / 15 <= 728, $"The main window is {main.Height} pixels high.");
        Assert.True(main.MinimumSize.Height <= main.Height);
        // The windows with a seat map take the whole screen, whatever its size.
        Assert.Equal(FormWindowState.Maximized, Application.OpenForms.OfType<PreviewForm>().Single().WindowState);
        Assert.Equal(FormWindowState.Maximized, Application.OpenForms.OfType<EditorForm>().Single().WindowState);
        Assert.True(Application.OpenForms.OfType<PreviewForm>().Single().MinimumSize.Height * 17 / 15 <= 728);
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
