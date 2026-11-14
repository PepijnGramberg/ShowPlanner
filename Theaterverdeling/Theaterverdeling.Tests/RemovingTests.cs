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
  public class RemovingTests
  {
    private static readonly DateTime Day = new DateTime(2026, 3, 14);
    private static readonly DateTime HalfPastTen = new DateTime(2026, 3, 14, 10, 30, 0);

    [Fact]
    public void Removing_a_group_asks_first_and_keeps_it_on_no()
    {
      OnUiThread(() =>
      {
        using var main = MainWithTwoGroups();
        var questions = Answer(main, false);
        Find<ListView>(main, "Groupviewer").Items[0].Selected = true;

        Find<Button>(main, "RemoveGroupbut").PerformClick();

        Assert.Equal(new[] { "De groep \"De Regenboog\" verwijderen?" }, questions);
        Assert.Equal(2, Find<ListView>(main, "Groupviewer").Items.Count);
      });
    }

    [Fact]
    public void Group_is_removed_on_yes()
    {
      OnUiThread(() =>
      {
        using var main = MainWithTwoGroups();
        Answer(main, true);
        Find<ListView>(main, "Groupviewer").Items[0].Selected = true;

        Find<Button>(main, "RemoveGroupbut").PerformClick();

        Assert.Equal(1, Find<ListView>(main, "Groupviewer").Items.Count);
      });
    }

    [Fact]
    public void Removing_several_groups_asks_once()
    {
      OnUiThread(() =>
      {
        using var main = MainWithTwoGroups();
        var questions = Answer(main, true);
        Find<ListView>(main, "Groupviewer").Items[0].Selected = true;
        Find<ListView>(main, "Groupviewer").Items[1].Selected = true;

        Find<Button>(main, "RemoveGroupbut").PerformClick();

        Assert.Equal(new[] { "2 groepen verwijderen?" }, questions);
        Assert.Empty(Find<ListView>(main, "Groupviewer").Items);
      });
    }

    [Fact]
    public void Removing_a_showing_asks_first_and_keeps_it_on_no()
    {
      OnUiThread(() =>
      {
        using var main = MainWithTwoGroups();
        var questions = Answer(main, false);
        Find<ListView>(main, "Voorstellingviewer").Items[0].Selected = true;

        Find<Button>(main, "Removeshowbutton").PerformClick();

        Assert.Equal(new[] { "De voorstelling van 14-03-2026 10:30 verwijderen?" }, questions);
        Assert.Single(Find<ListView>(main, "Voorstellingviewer").Items);
      });
    }

    [Fact]
    public void Showing_is_removed_on_yes()
    {
      OnUiThread(() =>
      {
        using var main = MainWithTwoGroups();
        Answer(main, true);
        Find<ListView>(main, "Voorstellingviewer").Items[0].Selected = true;

        Find<Button>(main, "Removeshowbutton").PerformClick();

        Assert.Empty(Find<ListView>(main, "Voorstellingviewer").Items);
      });
    }

    [Fact]
    public void Remove_buttons_ask_nothing_without_a_selection()
    {
      OnUiThread(() =>
      {
        using var main = MainWithTwoGroups();
        var questions = Answer(main, true);

        Find<Button>(main, "RemoveGroupbut").PerformClick();
        Find<Button>(main, "Removeshowbutton").PerformClick();

        Assert.Empty(questions);
        Assert.Equal(2, Find<ListView>(main, "Groupviewer").Items.Count);
      });
    }

    private static Theaterverdeling MainWithTwoGroups()
    {
      var main = new Theaterverdeling();
      main.Show();
      AddShowing(main, Day, HalfPastTen);
      AddGroup(main, "De Regenboog", 28);
      AddGroup(main, "Het Kompas", 31);
      return main;
    }

    // Answers every question of the window and returns the list the questions are added to.
    private static List<string> Answer(Theaterverdeling main, bool answer)
    {
      var questions = new List<string>();
      main.Confirm = question =>
      {
        questions.Add(question);
        return answer;
      };
      return questions;
    }
  }
}
