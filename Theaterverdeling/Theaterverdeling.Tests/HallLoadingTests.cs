using Xunit;

namespace Theaterverdeling.Tests
{
  public class HallLoadingTests
  {
    [Fact]
    public void Lampegiet_loads_all_558_seats_from_the_hall_file_shipped_with_the_app()
    {
      var hall = Lampegiet.Load();

      Assert.Equal(558, hall.Seats);
    }
  }
}
