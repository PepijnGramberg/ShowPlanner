using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Xunit;
using static Theaterverdeling.Tests.FormDriver;

namespace Theaterverdeling.Tests
{
  // Form tests share the list of open windows, so they must not run side by side.
  [Collection("Forms")]
  public class DialogTests
  {
    private static readonly DateTime Day = new DateTime(2026, 3, 14);
    private static readonly DateTime HalfPastTen = new DateTime(2026, 3, 14, 10, 30, 0);

    [Fact]
    public void Group_with_a_name_that_is_already_used_is_refused_in_the_dialog()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();
        AddShowing(main, Day, HalfPastTen);
        AddGroup(main, "De Regenboog", 28);

        AddGroup(main, "De Regenboog", 20);

        var dialog = Application.OpenForms.OfType<Addgroupform>().Single();
        Assert.Equal("Er is al een groep met de naam \"De Regenboog\".", Find<Label>(dialog, "Errorlabel").Text);
        Assert.Equal(new[] { "De Regenboog 28" }, Texts(main, "Groupviewer"));
      });
    }

    [Fact]
    public void Group_without_a_name_is_refused_in_the_dialog()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();
        AddShowing(main, Day, HalfPastTen);

        AddGroup(main, "", 20);

        var dialog = Application.OpenForms.OfType<Addgroupform>().Single();
        Assert.Equal("Vul een groepsnaam in.", Find<Label>(dialog, "Errorlabel").Text);
        Assert.Empty(Texts(main, "Groupviewer"));
      });
    }

    [Fact]
    public void Group_without_a_preferred_date_is_added()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();
        AddShowing(main, Day, HalfPastTen);

        Find<Button>(main, "Addgroupbutton").PerformClick();
        var dialog = Application.OpenForms.OfType<Addgroupform>().Single();
        Find<TextBox>(dialog, "Groepnaam").Text = "De Regenboog";
        Find<NumericUpDown>(dialog, "Groepsgrootte").Value = 28;
        Find<Button>(dialog, "Addgroupbutton").PerformClick();

        Assert.Equal(new[] { "De Regenboog 28" }, Texts(main, "Groupviewer"));
      });
    }

    [Fact]
    public void Second_showing_at_the_same_moment_is_refused_in_the_dialog()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();
        AddShowing(main, Day, HalfPastTen);

        AddShowing(main, Day, HalfPastTen);

        var dialog = Application.OpenForms.OfType<Addshowform>().Single();
        Assert.Equal("Er is al een voorstelling op dat moment.", Find<Label>(dialog, "Errorlabel").Text);
        Assert.Equal(new[] { "14-03-2026 10:30" }, Texts(main, "Voorstellingviewer"));
      });
    }

    [Fact]
    public void Editing_a_group_opens_the_dialog_with_its_details()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();
        main.LoadPlan(SavedPlan(new Group("De Regenboog", 28, true, true, new List<DateTime> { Day })));

        Select(main, "Groupviewer", 0);
        Find<Button>(main, "Editgroupbutton").PerformClick();

        var dialog = Application.OpenForms.OfType<Addgroupform>().Single();
        Assert.Equal("De Regenboog", Find<TextBox>(dialog, "Groepnaam").Text);
        Assert.Equal(28, Find<NumericUpDown>(dialog, "Groepsgrootte").Value);
        Assert.True(Find<CheckBox>(dialog, "Firstrowcheck").Checked);
        Assert.True(Find<CheckBox>(dialog, "Morningcheck").Checked);
        Assert.Equal(new[] { Day }, Find<CheckedListBox>(dialog, "Datumvoorkeurenbox").CheckedItems.Cast<DateTime>());
      });
    }

    [Fact]
    public void Changed_group_replaces_the_original()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();
        AddShowing(main, Day, HalfPastTen);
        AddGroup(main, "De Regenboog", 28);

        Select(main, "Groupviewer", 0);
        Find<Button>(main, "Editgroupbutton").PerformClick();
        var dialog = Application.OpenForms.OfType<Addgroupform>().Single();
        Find<NumericUpDown>(dialog, "Groepsgrootte").Value = 30;
        Find<Button>(dialog, "Addgroupbutton").PerformClick();

        Assert.Equal(new[] { "De Regenboog 30" }, Texts(main, "Groupviewer"));
        Assert.Equal("30 personen, 550 stoelen", Find<Label>(main, "Seatlabel").Text);
      });
    }

    [Fact]
    public void Editing_a_showing_opens_the_dialog_with_its_details()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();
        AddShowing(main, Day, HalfPastTen, "Balkon");

        Select(main, "Voorstellingviewer", 0);
        Find<Button>(main, "Editshowbutton").PerformClick();

        var dialog = Application.OpenForms.OfType<Addshowform>().Single();
        Assert.Equal(Day, Find<DateTimePicker>(dialog, "Datumkeuze").Value.Date);
        Assert.Equal(HalfPastTen.TimeOfDay, Find<DateTimePicker>(dialog, "Time").Value.TimeOfDay);
        Assert.Equal(new[] { "Balkon" }, Find<CheckedListBox>(dialog, "Sectionsoff").CheckedItems.Cast<string>());
      });
    }

    [Fact]
    public void Changed_showing_replaces_the_original()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();
        AddShowing(main, Day, HalfPastTen);

        Select(main, "Voorstellingviewer", 0);
        Find<Button>(main, "Editshowbutton").PerformClick();
        var dialog = Application.OpenForms.OfType<Addshowform>().Single();
        var sections = Find<CheckedListBox>(dialog, "Sectionsoff");
        sections.SetItemChecked(sections.Items.IndexOf("Balkon"), true);
        Find<Button>(dialog, "Addshow").PerformClick();

        Assert.Equal(new[] { "14-03-2026 10:30 (Balkon uit)" }, Texts(main, "Voorstellingviewer"));
        Assert.Equal("0 personen, 363 stoelen", Find<Label>(main, "Seatlabel").Text);
      });
    }

    [Fact]
    public void Double_click_or_Enter_on_a_group_opens_it_for_editing()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();
        AddShowing(main, Day, HalfPastTen);
        AddGroup(main, "De Regenboog", 28);

        Select(main, "Groupviewer", 0);
        Activate(main, "Groupviewer");

        var dialog = Application.OpenForms.OfType<Addgroupform>().Single();
        Assert.Equal("De Regenboog", Find<TextBox>(dialog, "Groepnaam").Text);
      });
    }

    [Fact]
    public void Double_click_or_Enter_on_a_showing_opens_it_for_editing()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();
        AddShowing(main, Day, HalfPastTen);

        Select(main, "Voorstellingviewer", 0);
        Activate(main, "Voorstellingviewer");

        var dialog = Application.OpenForms.OfType<Addshowform>().Single();
        Assert.Equal(Day, Find<DateTimePicker>(dialog, "Datumkeuze").Value.Date);
      });
    }

    [Fact]
    public void Group_dialog_lists_the_showing_dates_as_short_dates()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();
        AddShowing(main, Day, HalfPastTen);

        Find<Button>(main, "Addgroupbutton").PerformClick();

        var dialog = Application.OpenForms.OfType<Addgroupform>().Single();
        var dates = Find<CheckedListBox>(dialog, "Datumvoorkeurenbox");
        Assert.Equal("14-03-2026", dates.GetItemText(dates.Items[0]));
      });
    }

    [Fact]
    public void New_showing_starts_at_ten_o_clock_without_seconds()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();

        Find<Button>(main, "Addshow").PerformClick();

        var time = Find<DateTimePicker>(Application.OpenForms.OfType<Addshowform>().Single(), "Time");
        Assert.Equal(new TimeSpan(10, 0, 0), time.Value.TimeOfDay);
        Assert.Equal("10:00", time.Text);
      });
    }

    [Fact]
    public void Main_window_waits_while_a_dialog_is_open()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();

        Find<Button>(main, "Addshow").PerformClick();

        var dialog = Application.OpenForms.OfType<Addshowform>().Single();
        Assert.False(IsWindowEnabled(main.Handle));
        Assert.Same(main, dialog.Owner);

        Find<Button>(dialog, "Cancelbutton").PerformClick();

        Assert.True(IsWindowEnabled(main.Handle));
      });
    }

    [Fact]
    public void Main_window_waits_while_the_group_dialogs_are_open()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();
        AddShowing(main, Day, HalfPastTen);

        Find<Button>(main, "Addgroupbutton").PerformClick();
        Assert.False(IsWindowEnabled(main.Handle));
        Application.OpenForms.OfType<Addgroupform>().Single().Close();
        Assert.True(IsWindowEnabled(main.Handle));

        Find<Button>(main, "Bulkgroupbutton").PerformClick();
        Assert.False(IsWindowEnabled(main.Handle));
        Application.OpenForms.OfType<Bulkgroupform>().Single().Close();
        Assert.True(IsWindowEnabled(main.Handle));
      });
    }

    [Fact]
    public void Group_dialog_says_what_the_wishes_mean()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();
        AddShowing(main, Day, HalfPastTen);

        Find<Button>(main, "Addgroupbutton").PerformClick();

        var dialog = Application.OpenForms.OfType<Addgroupform>().Single();
        Assert.Equal("Aantal personen", Find<Label>(dialog, "label3").Text);
        Assert.Equal("Wil vooraan zitten", Find<CheckBox>(dialog, "Firstrowcheck").Text);
        Assert.Equal("Alleen een ochtendvoorstelling (voor 12:00)", Find<CheckBox>(dialog, "Morningcheck").Text);
        Assert.Equal("Voorkeur voor datum", Find<Label>(dialog, "label1").Text);
        Assert.Equal("Niets aangevinkt: elke datum is goed.", Find<Label>(dialog, "Datehint").Text);
      });
    }

    [Fact]
    public void Showing_dialog_says_what_ticking_a_section_does()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();

        Find<Button>(main, "Addshow").PerformClick();

        var dialog = Application.OpenForms.OfType<Addshowform>().Single();
        Assert.Equal("Niet gebruiken in deze voorstelling", Find<Label>(dialog, "Sectionslabel").Text);
      });
    }

    [Fact]
    public void Enter_adds_and_Esc_cancels_in_the_group_dialog_and_typing_starts_in_the_name()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();
        AddShowing(main, Day, HalfPastTen);

        Find<Button>(main, "Addgroupbutton").PerformClick();

        var dialog = Application.OpenForms.OfType<Addgroupform>().Single();
        Assert.Same(Find<Button>(dialog, "Addgroupbutton"), dialog.AcceptButton);
        Assert.Same(Find<Button>(dialog, "Cancelbutton"), dialog.CancelButton);
        Assert.Same(Find<TextBox>(dialog, "Groepnaam"), dialog.ActiveControl);
        Assert.True(Find<TextBox>(dialog, "Groepnaam").TabIndex < Find<NumericUpDown>(dialog, "Groepsgrootte").TabIndex);
        Assert.True(Find<NumericUpDown>(dialog, "Groepsgrootte").TabIndex < Find<Button>(dialog, "Addgroupbutton").TabIndex);
      });
    }

    [Fact]
    public void Enter_adds_and_Esc_cancels_in_the_showing_dialog()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();

        Find<Button>(main, "Addshow").PerformClick();

        var dialog = Application.OpenForms.OfType<Addshowform>().Single();
        Assert.Same(Find<Button>(dialog, "Addshow"), dialog.AcceptButton);
        Assert.Same(Find<Button>(dialog, "Cancelbutton"), dialog.CancelButton);
        Assert.True(Find<DateTimePicker>(dialog, "Datumkeuze").TabIndex < Find<Button>(dialog, "Addshow").TabIndex);
      });
    }

    [Fact]
    public void New_showing_starts_with_the_date_hall_and_unused_sections_of_the_last_one()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();
        AddShowing(main, Day, HalfPastTen, "Balkon");

        Find<Button>(main, "Addshow").PerformClick();

        var dialog = Application.OpenForms.OfType<Addshowform>().Single();
        Assert.Equal(Day, Find<DateTimePicker>(dialog, "Datumkeuze").Value.Date);
        Assert.Equal("Lampegiet", Find<ComboBox>(dialog, "Hallchoice").SelectedItem);
        Assert.Equal(new[] { "Balkon" }, Find<CheckedListBox>(dialog, "Sectionsoff").CheckedItems.Cast<string>());
        // The time is the one thing that differs, so it is not copied.
        Assert.Equal(new TimeSpan(10, 0, 0), Find<DateTimePicker>(dialog, "Time").Value.TimeOfDay);
      });
    }

    [Fact]
    public void First_showing_of_a_plan_starts_today_with_every_section_in_use()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();

        Find<Button>(main, "Addshow").PerformClick();

        var dialog = Application.OpenForms.OfType<Addshowform>().Single();
        Assert.Equal(DateTime.Today, Find<DateTimePicker>(dialog, "Datumkeuze").Value.Date);
        Assert.Empty(Find<CheckedListBox>(dialog, "Sectionsoff").CheckedItems);
      });
    }

    [Fact]
    public void Edit_buttons_do_nothing_without_a_selection()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();
        AddShowing(main, Day, HalfPastTen);
        AddGroup(main, "De Regenboog", 28);

        Find<Button>(main, "Editgroupbutton").PerformClick();
        Find<Button>(main, "Editshowbutton").PerformClick();

        Assert.Empty(Application.OpenForms.OfType<Addgroupform>());
        Assert.Empty(Application.OpenForms.OfType<Addshowform>());
      });
    }

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool IsWindowEnabled(IntPtr window);

    // What the list does on a double-click or on Enter.
    private static void Activate(Control main, string listName)
    {
      var list = Find<ListView>(main, listName);
      typeof(ListView).GetMethod("OnItemActivate", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(list, new object[] { EventArgs.Empty });
    }

    private static void Select(Control main, string listName, int index)
    {
      Find<ListView>(main, listName).Items[index].Selected = true;
    }

    private static List<string> Texts(Control main, string listName)
    {
      return Find<ListView>(main, listName).Items.Cast<ListViewItem>()
        .Select(item => string.Join(" ", item.SubItems.Cast<ListViewItem.ListViewSubItem>().Select(cell => cell.Text).Where(text => text != "")))
        .ToList();
    }

    private static string SavedPlan(Group group)
    {
      var plan = new Plan();
      plan.Shows.Add(new Voorstelling(HalfPastTen, Zaal.Load("Lampegiet"), Day));
      plan.Groups.Add(group);
      var folder = Path.Combine(AppContext.BaseDirectory, "testplans");
      Directory.CreateDirectory(folder);
      var path = Path.Combine(folder, Guid.NewGuid() + ".json");
      plan.Save(path);
      return path;
    }
  }
}
