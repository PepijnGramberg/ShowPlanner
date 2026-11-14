using System.Linq;
using Xunit;
using static Theaterverdeling.Tests.TestHalls;

namespace Theaterverdeling.Tests
{
  public class SectionTests
  {
    [Fact]
    public void Hall_lists_its_sections_once_in_row_order()
    {
      var hall = Hall(SectionRow(1, 4, "Voor"), SectionRow(2, 4, "Voor"), Row(3, 4), SectionRow(4, 4, "Balkon"));

      Assert.Equal(new[] { "Voor", "Balkon" }, hall.Sections);
    }

    [Fact]
    public void Switched_off_section_does_not_count_as_capacity()
    {
      var hall = Hall(Row(1, 4), SectionRow(2, 6, "Balkon"));

      hall.SwitchOff("Balkon");

      Assert.Equal(4, hall.Seats);
    }

    [Fact]
    public void Switched_off_section_gets_no_groups()
    {
      var hall = Hall(SectionRow(1, 4, "Voor"), Row(2, 4));
      var a = Group("A", 2);
      hall.SwitchOff("Voor");

      Seat(hall, a);

      Assert.Equal("rij 2: 3-4", Describe(a));
    }

    [Fact]
    public void Hall_tells_which_sections_are_switched_off()
    {
      var hall = Hall(SectionRow(1, 4, "Voor"), Row(2, 4), SectionRow(3, 4, "Balkon"));

      hall.SwitchOff("Balkon");

      Assert.Equal(new[] { "Balkon" }, hall.SwitchedOffSections);
    }

    [Fact]
    public void Sections_survive_saving_and_loading_the_hall()
    {
      var hall = Hall(SectionRow(1, 4, "Voor"), Row(2, 4));

      var reloaded = Zaal.FromJson<Zaal>(hall.ToJson());

      Assert.Equal(new[] { "Voor" }, reloaded.Sections);
    }

    [Fact]
    public void Lampegiet_can_switch_off_the_front_rows_and_the_balcony()
    {
      var hall = Zaal.Load("Lampegiet");

      Assert.Equal(new[] { "Rij 1 en 2", "Balkon" }, hall.Sections);
    }

    [Fact]
    public void Lampegiet_without_front_rows_and_balcony_has_324_seats()
    {
      var hall = Zaal.Load("Lampegiet");

      hall.SwitchOff("Rij 1 en 2");
      hall.SwitchOff("Balkon");

      Assert.Equal(324, hall.Seats);
    }
  }
}
