using System;
using System.Linq;
using System.Windows.Forms;
using Xunit;
using static Theaterverdeling.Tests.FormDriver;
using static Theaterverdeling.Tests.TestHalls;

namespace Theaterverdeling.Tests
{
  // Form tests share the list of open windows, so they must not run side by side.
  [Collection("Forms")]
  public class GroupNameTests
  {
    private const string TooLong = "De groepsnaam is te lang: hoogstens 25 tekens, anders past hij niet in de PDF.";
    private static readonly DateTime Day = new DateTime(2026, 3, 14);

    [Fact]
    public void Group_name_of_25_characters_is_accepted()
    {
      Assert.Null(PlanWithAShowing().CheckGroup(new string('a', 25), 4));
    }

    [Fact]
    public void Group_name_of_26_characters_is_refused()
    {
      Assert.Equal(TooLong, PlanWithAShowing().CheckGroup(new string('a', 26), 4));
    }

    [Fact]
    public void Spaces_around_the_name_do_not_count()
    {
      Assert.Null(PlanWithAShowing().CheckGroup("  " + new string('a', 25) + "  ", 4));
    }

    [Fact]
    public void Grid_refuses_the_same_names_as_the_dialog()
    {
      var entries = new[]
      {
        new GroupEntry { Name = new string('a', 25), People = "4" },
        new GroupEntry { Name = new string('b', 26), People = "4" }
      };

      Assert.Equal(new[] { null, TooLong }, PlanWithAShowing().CheckNewGroups(entries));
    }

    [Fact]
    public void Group_dialog_takes_no_more_characters_than_the_limit()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();
        AddShowing(main, Day, Day.AddHours(10));

        Find<Button>(main, "Addgroupbutton").PerformClick();

        var dialog = Application.OpenForms.OfType<Addgroupform>().Single();
        Assert.Equal(Plan.MaxGroupNameLength, Find<TextBox>(dialog, "Groepnaam").MaxLength);
      });
    }

    private static Plan PlanWithAShowing()
    {
      var plan = new Plan();
      plan.Shows.Add(new Voorstelling(DateTime.Today.AddHours(10), Hall(Row(1, 10)), Day));
      return plan;
    }
  }
}
