using System;
using System.Drawing;
using Xunit;
using static Theaterverdeling.Tests.TestHalls;

namespace Theaterverdeling.Tests
{
  public class PagePreviewTests
  {
    [Fact]
    public void Preview_image_has_the_shape_of_the_printed_page()
    {
      using var image = PagePreview.Render(Showing(), "Kikker is jarig", 2);

      Assert.Equal(new Size(1190, 1684), image.Size);
    }

    [Fact]
    public void Preview_image_is_a_white_page()
    {
      using var image = PagePreview.Render(Showing(), "Kikker is jarig", 1);

      Assert.Equal(Color.White.ToArgb(), image.GetPixel(2, 2).ToArgb());
    }

    [Fact]
    public void Preview_image_shows_the_seats_of_a_group_in_its_colour()
    {
      using var image = PagePreview.Render(Showing(Group("A", 2)), "Kikker is jarig", 1);

      Assert.True(Contains(image, Zaal.GroupColor(0)));
    }

    [Fact]
    public void Preview_image_of_a_showing_without_groups_has_no_group_colour()
    {
      using var image = PagePreview.Render(Showing(), "Kikker is jarig", 1);

      Assert.False(Contains(image, Zaal.GroupColor(0)));
    }

    private static Voorstelling Showing(params Group[] groups)
    {
      var date = new DateTime(2026, 3, 14);
      var show = new Voorstelling(date.AddHours(10), Hall(Row(1, 4)), date);
      show.Groups.AddRange(groups);
      show.Assignseats();
      return show;
    }

    private static bool Contains(Bitmap image, Color colour)
    {
      for (var y = 0; y < image.Height; y++)
      {
        for (var x = 0; x < image.Width; x++)
        {
          if (image.GetPixel(x, y).ToArgb() == colour.ToArgb())
          {
            return true;
          }
        }
      }
      return false;
    }
  }
}
