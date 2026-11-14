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
  public class UnsavedPlanTests
  {
    private static readonly DateTime Day = new DateTime(2026, 3, 14);
    private static readonly DateTime HalfPastTen = new DateTime(2026, 3, 14, 10, 30, 0);

    [Fact]
    public void Reset_asks_first_when_the_plan_is_not_saved_and_keeps_it_on_no()
    {
      OnUiThread(() =>
      {
        using var main = MainWithAGroup();
        var questions = Answer(main, false);

        Find<Button>(main, "ResetButton").PerformClick();

        Assert.Equal(new[] { "Het plan is gewijzigd en nog niet opgeslagen. Toch alles wissen?" }, questions);
        Assert.Single(Find<ListView>(main, "Groupviewer").Items);
      });
    }

    [Fact]
    public void Reset_empties_the_plan_on_yes()
    {
      OnUiThread(() =>
      {
        using var main = MainWithAGroup();
        Answer(main, true);

        Find<Button>(main, "ResetButton").PerformClick();

        Assert.Empty(Find<ListView>(main, "Groupviewer").Items);
        Assert.Empty(Find<ListView>(main, "Voorstellingviewer").Items);
      });
    }

    [Fact]
    public void Reset_does_not_ask_when_the_plan_is_saved()
    {
      OnUiThread(() =>
      {
        using var main = MainWithAGroup();
        var questions = Answer(main, false);
        main.SavePlan(NewPlanPath());

        Find<Button>(main, "ResetButton").PerformClick();

        Assert.Empty(questions);
        Assert.Empty(Find<ListView>(main, "Groupviewer").Items);
      });
    }

    [Fact]
    public void Plan_that_was_just_loaded_counts_as_saved()
    {
      OnUiThread(() =>
      {
        using var main = MainWithAGroup();
        var path = NewPlanPath();
        main.SavePlan(path);
        var questions = Answer(main, false);

        main.LoadPlan(path);
        main.Close();

        Assert.Empty(questions);
        Assert.False(main.Visible);
      });
    }

    [Fact]
    public void Closing_the_window_asks_first_when_the_plan_is_not_saved()
    {
      OnUiThread(() =>
      {
        using var main = MainWithAGroup();
        var questions = Answer(main, false);

        main.Close();

        Assert.Equal(new[] { "Het plan is gewijzigd en nog niet opgeslagen. Toch afsluiten?" }, questions);
        Assert.True(main.Visible);
      });
    }

    [Fact]
    public void Changing_the_show_name_counts_as_a_change()
    {
      OnUiThread(() =>
      {
        using var main = MainWithAGroup();
        main.SavePlan(NewPlanPath());
        var questions = Answer(main, false);

        Find<TextBox>(main, "voorstellingnaambox").Text = "Kikker in de kou";
        main.Close();

        Assert.Single(questions);
      });
    }

    [Fact]
    public void Pinning_a_group_in_the_preview_counts_as_a_change()
    {
      OnUiThread(() =>
      {
        using var main = MainWithAGroup();
        main.SavePlan(NewPlanPath());
        var questions = Answer(main, false);
        Find<Button>(main, "Dividebutton").PerformClick();
        var preview = Application.OpenForms.OfType<PreviewForm>().Single();

        Find<ListBox>(preview, "Grouplist").SelectedIndex = 0;
        Find<Button>(preview, "Pinbutton").PerformClick();
        main.Close();

        Assert.Single(questions);
      });
    }

    private static Theaterverdeling MainWithAGroup()
    {
      var main = new Theaterverdeling();
      main.Show();
      AddShowing(main, Day, HalfPastTen);
      AddGroup(main, "De Regenboog", 28);
      return main;
    }
    [Fact]
    public void Closing_the_window_asks_first_when_a_hall_in_the_editor_is_not_saved()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();
        var editor = new EditorForm();
        editor.Show();
        var questions = new List<string>();
        editor.Confirm = question =>
        {
          questions.Add(question);
          return false;
        };
        Find<TextBox>(editor, "Hallname").Text = "Nieuwe zaal";

        main.Close();

        Assert.Equal(new[] { "De wijzigingen in deze zaal zijn niet opgeslagen. Toch sluiten?" }, questions);
        Assert.True(main.Visible);
        Assert.True(editor.Visible);
      });
    }

    [Fact]
    public void Closing_the_window_closes_a_hall_editor_without_changes()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();
        var editor = new EditorForm();
        editor.Show();

        main.Close();

        Assert.False(main.Visible);
        Assert.True(editor.IsDisposed);
      });
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

    private static string NewPlanPath()
    {
      var folder = Path.Combine(AppContext.BaseDirectory, "testplans");
      Directory.CreateDirectory(folder);
      return Path.Combine(folder, Guid.NewGuid() + ".json");
    }
  }
}
