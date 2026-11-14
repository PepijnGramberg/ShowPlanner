using System.Linq;
using Xunit;
using static Theaterverdeling.Tests.TestHalls;

namespace Theaterverdeling.Tests
{
  public class HallDraftTests
  {
    [Fact]
    public void Draft_of_a_hall_lists_its_rows_as_text()
    {
      var draft = HallDraft.FromHall(Hall(Row(1, 6, true, 1, 2, 6), Walkway, SectionRow(2, 8, "Balkon")));

      Assert.Equal(new[] { false, true, false }, draft.Rows.Select(row => row.IsWalkway));
      Assert.Equal("1", draft.Rows[0].RowNr);
      Assert.Equal("6", draft.Rows[0].SeatCount);
      Assert.Equal("1-2, 6", draft.Rows[0].Unusable);
      Assert.Equal("Balkon", draft.Rows[2].Section);
    }

    [Fact]
    public void Draft_turns_back_into_the_same_hall()
    {
      var draft = HallDraft.FromHall(Zaal.Load("Lampegiet"));

      var hall = draft.ToHall();

      Assert.Equal(550, hall.Seats);
      Assert.Equal(new[] { "Rij 1 en 2", "Balkon" }, hall.Sections);
      Assert.Equal(Zaal.Load("Lampegiet").ToJson(), hall.ToJson());
    }

    [Fact]
    public void Hall_written_by_the_editor_is_read_back_the_same()
    {
      var hall = HallDraft.FromHall(Hall(Row(1, 6, true, 1, 6), Walkway, SectionRow(2, 8, "Balkon"), Row(3, 4, false))).ToHall();

      var reloaded = Zaal.FromJson(hall.ToJson());

      Assert.Equal(hall.ToJson(), reloaded.ToJson());
      Assert.Equal(12, reloaded.Seats);
      Assert.IsType<WalkWay>(reloaded.Rows[1]);
    }

    [Fact]
    public void Row_without_in_use_in_the_file_is_in_use()
    {
      var hall = Zaal.FromJson("{\"Rows\":[{\"Type\":\"ChairRow\",\"RowNr\":1,\"SeatCount\":5}]}");

      Assert.Equal(5, hall.Seats);
    }

    [Theory]
    [InlineData("1, 2, 22-23", "1-2, 22-23")]
    [InlineData("3;1 2", "1-3")]
    [InlineData("", "")]
    public void Unusable_seats_are_read_as_numbers_and_ranges(string typed, string expected)
    {
      var row = new RowDraft { Unusable = typed };

      Assert.True(row.TryGetUnusable(out var seats));
      Assert.Equal(expected, RowDraft.Format(seats));
    }

    [Theory]
    [InlineData("a")]
    [InlineData("0")]
    [InlineData("5-3")]
    [InlineData("1-2-3")]
    [InlineData("-4")]
    public void Unusable_seats_that_are_no_seat_numbers_are_refused(string typed)
    {
      Assert.False(new RowDraft { Unusable = typed }.TryGetUnusable(out _));
    }

    [Fact]
    public void Toggling_a_seat_makes_it_unusable_and_usable_again()
    {
      var row = new RowDraft { SeatCount = "10", Unusable = "1-2" };

      row.ToggleSeat(3);
      Assert.Equal("1-3", row.Unusable);

      row.ToggleSeat(2);
      Assert.Equal("1, 3", row.Unusable);
    }

    [Fact]
    public void New_row_gets_the_next_number_and_the_size_and_section_of_the_row_before_it()
    {
      var draft = HallDraft.FromHall(Hall(Row(1, 6), SectionRow(2, 8, "Balkon")));

      var index = draft.AddRow(1);

      Assert.Equal(2, index);
      Assert.Equal("3", draft.Rows[2].RowNr);
      Assert.Equal("8", draft.Rows[2].SeatCount);
      Assert.Equal("Balkon", draft.Rows[2].Section);
    }

    [Fact]
    public void First_row_of_an_empty_hall_is_row_1_with_ten_seats()
    {
      var draft = new HallDraft();

      draft.AddRow(-1);

      Assert.Equal("1", draft.Rows[0].RowNr);
      Assert.Equal("10", draft.Rows[0].SeatCount);
    }

    [Fact]
    public void Walkway_is_added_below_the_chosen_line()
    {
      var draft = HallDraft.FromHall(Hall(Row(1, 6), Row(2, 6)));

      draft.AddWalkway(0);

      Assert.Equal(new[] { false, true, false }, draft.Rows.Select(row => row.IsWalkway));
    }

    [Fact]
    public void Line_moves_one_place_and_stops_at_the_ends()
    {
      var draft = HallDraft.FromHall(Hall(Row(1, 6), Row(2, 6)));

      Assert.Equal(1, draft.Move(0, 1));
      Assert.Equal(new[] { "2", "1" }, draft.Rows.Select(row => row.RowNr));
      Assert.Equal(1, draft.Move(1, 1));
    }

    [Fact]
    public void Every_line_with_a_problem_gets_its_message()
    {
      var draft = new HallDraft();
      draft.Rows.Add(new RowDraft { RowNr = "1", SeatCount = "6" });
      draft.Rows.Add(new RowDraft { RowNr = "x", SeatCount = "6" });
      draft.Rows.Add(new RowDraft { RowNr = "1", SeatCount = "6" });
      draft.Rows.Add(new RowDraft { RowNr = "2", SeatCount = "0" });
      draft.Rows.Add(new RowDraft { RowNr = "3", SeatCount = "6", Unusable = "een" });
      draft.Rows.Add(new RowDraft { RowNr = "4", SeatCount = "6", Unusable = "6-7" });
      draft.Rows.Add(new RowDraft { IsWalkway = true });

      Assert.Equal(new[]
      {
        null,
        "Vul bij rij een heel getal in.",
        "Rij 1 komt al eerder voor.",
        "Vul bij stoelen een heel getal van 1 tot 100 in.",
        "Vul bij niet gebruikt stoelnummers in, zoals 1, 2 of 22-23.",
        "Stoel 7 bestaat niet in deze rij.",
        null
      }, draft.Check());
    }

    [Theory]
    [InlineData("", "Vul een naam voor de zaal in.")]
    [InlineData("Zaal 1/2", "De naam van de zaal mag geen tekens als \\ / : * ? \" < > | bevatten en niet op een punt eindigen.")]
    [InlineData("Kleine zaal", null)]
    public void Hall_needs_a_name_that_can_be_a_file_name(string name, string expected)
    {
      var draft = HallDraft.FromHall(Hall(Row(1, 6)));

      Assert.Equal(expected, draft.CheckHall(name));
    }

    [Fact]
    public void Hall_needs_a_row_of_chairs()
    {
      var draft = new HallDraft();
      draft.AddWalkway(-1);

      Assert.Equal("Voeg minstens één rij met stoelen toe.", draft.CheckHall("Kleine zaal"));
    }

    [Fact]
    public void Hall_with_a_problem_in_a_line_cannot_be_saved()
    {
      var draft = new HallDraft();
      draft.Rows.Add(new RowDraft { RowNr = "1", SeatCount = "veel" });

      Assert.Equal("Los eerst de meldingen bij de rijen op.", draft.CheckHall("Kleine zaal"));
    }

    [Fact]
    public void Hall_that_is_half_typed_can_still_be_shown()
    {
      var draft = new HallDraft();
      draft.Rows.Add(new RowDraft { RowNr = "1", SeatCount = "6", Unusable = "5-9" });
      draft.Rows.Add(new RowDraft { RowNr = "", SeatCount = "veel" });

      var hall = draft.ToHall();

      Assert.Equal(4, hall.Seats);
      Assert.Equal(0, hall.Rows[1].TotalSeatCount);
    }
  }
}
