using System;
using System.Linq;
using System.Windows.Forms;
using Xunit;
using static Theaterverdeling.Tests.FormDriver;

namespace Theaterverdeling.Tests
{
  // Form tests share the list of open windows, so they must not run side by side.
  [Collection("Forms")]
  public class AddShowingTests
  {
    [Fact]
    public void Adding_a_showing_adds_the_seats_of_the_hall_to_the_seat_count()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();

        AddShowing(main, new DateTime(2026, 3, 14), new DateTime(2026, 3, 14, 10, 30, 0));

        Assert.Equal("0/550 stoelen", Find<Label>(main, "Seatlabel").Text);
      });
    }

    [Fact]
    public void Adding_a_showing_lists_it_with_its_date_and_time()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();

        AddShowing(main, new DateTime(2026, 3, 14), new DateTime(2026, 3, 14, 10, 30, 0));

        var listed = Find<ListView>(main, "Voorstellingviewer").Items.Cast<ListViewItem>().Select(item => item.Text);
        Assert.Equal(new[] { "14-03-2026 10:30" }, listed);
      });
    }

    [Fact]
    public void Add_showing_dialog_offers_the_sections_of_the_hall_to_switch_off()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();
        Find<Button>(main, "Addshow").PerformClick();
        var dialog = Application.OpenForms.OfType<Addshowform>().Single();

        var offered = Find<CheckedListBox>(dialog, "Sectionsoff").Items.Cast<string>();

        Assert.Equal(new[] { "Rij 1 en 2", "Balkon" }, offered);
      });
    }

    [Fact]
    public void Showing_added_without_the_balcony_has_fewer_seats()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();

        AddShowing(main, new DateTime(2026, 3, 14), new DateTime(2026, 3, 14, 10, 30, 0), "Balkon");

        Assert.Equal("0/363 stoelen", Find<Label>(main, "Seatlabel").Text);
      });
    }

    [Fact]
    public void Showing_added_without_the_balcony_says_so_in_the_list()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();

        AddShowing(main, new DateTime(2026, 3, 14), new DateTime(2026, 3, 14, 10, 30, 0), "Balkon");

        var list = Find<ListView>(main, "Voorstellingviewer");
        var item = list.Items.Cast<ListViewItem>().Single();
        Assert.Equal("14-03-2026 10:30 (Balkon uit)", item.Text);
        // The list is narrow, so the full text is also the tooltip.
        Assert.True(list.ShowItemToolTips);
        Assert.Equal(item.Text, item.ToolTipText);
      });
    }

    [Fact]
    public void Reset_empties_the_seat_count()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();
        AddShowing(main, new DateTime(2026, 3, 14), new DateTime(2026, 3, 14, 10, 30, 0));

        main.Confirm = question => true;
        Find<Button>(main, "ResetButton").PerformClick();

        Assert.Equal("0/0 stoelen", Find<Label>(main, "Seatlabel").Text);
      });
    }

    [Fact]
    public void Front_rows_are_not_switched_off_for_the_whole_plan_anymore()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();

        Assert.Empty(main.Controls.Find("Frontrowsdisable", true));
      });
    }
  }
}
