using Xunit;

namespace Theaterverdeling.Tests
{
  public class HallLoadingTests
  {
    [Fact]
    public void Hall_loaded_by_name_knows_its_name()
    {
      var hall = Zaal.Load("Lampegiet");

      Assert.Equal("Lampegiet", hall.Name);
    }

    [Fact]
    public void Lampegiet_has_550_usable_seats_in_the_hall_file_shipped_with_the_app()
    {
      var hall = Zaal.Load("Lampegiet");

      Assert.Equal(550, hall.Seats);
    }
  }
}
