using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Theaterverdeling.Tests.TestHalls;

namespace Theaterverdeling.Tests
{
  // Rows of the bulk input grid, before they become groups.
  public class GroupEntryTests
  {
    private static readonly DateTime Monday = new DateTime(2026, 3, 16);

    [Fact]
    public void Rows_copied_from_a_spreadsheet_give_a_name_and_a_size_each()
    {
      var entries = GroupEntry.FromPastedText("De Regenboog\t28\r\nHet Kompas\t31\r\n");

      Assert.Equal(new[] { "De Regenboog", "Het Kompas" }, entries.Select(entry => entry.Name));
      Assert.Equal(new[] { "28", "31" }, entries.Select(entry => entry.People));
    }

    [Fact]
    public void Semicolons_separate_name_and_size_too()
    {
      var entries = GroupEntry.FromPastedText("De Regenboog;28\nHet Kompas;31");

      Assert.Equal(new[] { "De Regenboog", "Het Kompas" }, entries.Select(entry => entry.Name));
      Assert.Equal(new[] { "28", "31" }, entries.Select(entry => entry.People));
    }

    [Fact]
    public void Empty_lines_are_skipped_and_cells_are_trimmed()
    {
      var entries = GroupEntry.FromPastedText("\n  De Regenboog \t 28 \n\n\t\n");

      Assert.Equal(new[] { "De Regenboog" }, entries.Select(entry => entry.Name));
      Assert.Equal(new[] { "28" }, entries.Select(entry => entry.People));
    }

    [Fact]
    public void Header_row_is_skipped()
    {
      var entries = GroupEntry.FromPastedText("Naam\tAantal\nDe Regenboog\t28");

      Assert.Equal(new[] { "De Regenboog" }, entries.Select(entry => entry.Name));
    }

    [Fact]
    public void Names_without_a_size_are_all_kept_and_a_number_in_a_name_stays_in_the_name()
    {
      var entries = GroupEntry.FromPastedText("De Vlieger groep 5\nHet Kompas");

      Assert.Equal(new[] { "De Vlieger groep 5", "Het Kompas" }, entries.Select(entry => entry.Name));
      Assert.Equal(new[] { "", "" }, entries.Select(entry => entry.People));
    }

    [Fact]
    public void Size_that_is_not_a_number_on_a_later_row_is_kept_as_it_was_typed()
    {
      var entries = GroupEntry.FromPastedText("De Regenboog\t28\nHet Kompas\tdertig");

      Assert.Equal(new[] { "28", "dertig" }, entries.Select(entry => entry.People));
    }

    [Fact]
    public void Entry_becomes_a_group_with_its_wishes()
    {
      var entry = new GroupEntry { Name = " De Regenboog ", People = "28", FrontRows = true, Morning = true, Dates = { Monday } };

      var group = entry.ToGroup();

      Assert.Equal("De Regenboog", group.Name);
      Assert.Equal(28, group.Personen);
      Assert.True(group.Firstrows);
      Assert.True(group.Morning);
      Assert.Equal(new[] { Monday }, group.Datum);
    }

    [Fact]
    public void Rows_that_meet_all_rules_have_no_message()
    {
      var plan = PlanWithRoomFor(40);

      var messages = plan.CheckNewGroups(new[] { Entry("De Regenboog", "28"), Entry("Het Kompas", "31") });

      Assert.Equal(new string[] { null, null }, messages);
    }

    [Theory]
    [InlineData("")]
    [InlineData("dertig")]
    [InlineData("0")]
    [InlineData("-3")]
    [InlineData("2,5")]
    public void Row_needs_a_whole_number_of_people(string people)
    {
      var plan = PlanWithRoomFor(40);

      var messages = plan.CheckNewGroups(new[] { Entry("De Regenboog", people) });

      Assert.Equal(new[] { "Vul bij aantal een heel getal in." }, messages);
    }

    [Fact]
    public void Row_is_checked_with_the_rules_of_a_single_group()
    {
      var plan = PlanWithRoomFor(40);
      plan.Groups.Add(Group("De Regenboog", 28));

      var messages = plan.CheckNewGroups(new[] { Entry("", "20"), Entry("de regenboog", "20"), Entry("Groot", "41") });

      Assert.Equal(new[]
      {
        "Vul een groepsnaam in.",
        "Er is al een groep met de naam \"de regenboog\".",
        "De groep past in geen enkele voorstelling. De grootste heeft 40 stoelen."
      }, messages);
    }

    [Fact]
    public void Name_that_is_twice_in_the_rows_is_refused_the_second_time()
    {
      var plan = PlanWithRoomFor(40);

      var messages = plan.CheckNewGroups(new[] { Entry("De Regenboog", "28"), Entry("Het Kompas", "31"), Entry("de regenboog", "20") });

      Assert.Equal(new[] { null, null, "Er is al een groep met de naam \"de regenboog\"." }, messages);
    }

    private static GroupEntry Entry(string name, string people)
    {
      return new GroupEntry { Name = name, People = people };
    }

    private static Plan PlanWithRoomFor(int seats)
    {
      var plan = new Plan();
      plan.Shows.Add(new Voorstelling(Monday.AddHours(10), Hall(Row(1, seats)), Monday));
      return plan;
    }
  }
}
