using Xunit;

namespace Theaterverdeling.Tests
{
  public class HallLoadingTests
  {
    [Fact]
    public void Lampegiet_has_550_usable_seats_in_the_hall_file_shipped_with_the_app()
    {
      var hall = Lampegiet.Load();

      Assert.Equal(550, hall.Seats);
    }
  }
}
