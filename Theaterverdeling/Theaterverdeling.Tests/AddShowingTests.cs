using System;
using System.Globalization;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Windows.Forms;
using Xunit;

namespace Theaterverdeling.Tests
{
  public class AddShowingTests
  {
    [Fact]
    public void Adding_a_showing_adds_the_seats_of_the_hall_to_the_seat_count()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();

        AddShowing(main, new DateTime(2026, 3, 14), new DateTime(2026, 3, 14, 10, 30, 0));

        Assert.Equal("0/550 stoelen", Find<Label>(main, "Seatlabel").Text);
      });
    }

    [Fact]
    public void Adding_a_showing_lists_it_with_its_date_and_time()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();

        AddShowing(main, new DateTime(2026, 3, 14), new DateTime(2026, 3, 14, 10, 30, 0));

        var listed = Find<ListView>(main, "Voorstellingviewer").Items.Cast<ListViewItem>().Select(item => item.Text);
        Assert.Equal(new[] { "14-03-2026 10:30" }, listed);
      });
    }

    private static void AddShowing(Form main, DateTime date, DateTime time)
    {
      Find<Button>(main, "Addshow").PerformClick();
      var dialog = Application.OpenForms.OfType<Addshowform>().Single();
      Find<DateTimePicker>(dialog, "Datumkeuze").Value = date;
      Find<DateTimePicker>(dialog, "Time").Value = time;
      Find<Button>(dialog, "Addshow").PerformClick();
    }

    private static T Find<T>(Control parent, string name) where T : Control
    {
      return (T)parent.Controls.Find(name, true).Single();
    }

    // WinForms needs an STA thread. The culture is pinned so date and time texts are predictable.
    private static void OnUiThread(Action test)
    {
      ExceptionDispatchInfo failure = null;
      var thread = new Thread(() =>
      {
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("nl-NL");
        try
        {
          test();
        }
        catch (Exception ex)
        {
          failure = ExceptionDispatchInfo.Capture(ex);
        }
        finally
        {
          foreach (var form in Application.OpenForms.Cast<Form>().ToList())
          {
            form.Dispose();
          }
        }
      });
      thread.SetApartmentState(ApartmentState.STA);
      thread.Start();
      thread.Join();
      failure?.Throw();
    }
  }
}
