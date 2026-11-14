using System;
using System.Globalization;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Windows.Forms;

namespace Theaterverdeling.Tests
{
  // Drives the real forms the way a user does: find a control by name, fill it, click it.
  internal static class FormDriver
  {
    // WinForms needs an STA thread. The culture is pinned so date and time texts are predictable.
    public static void OnUiThread(Action test)
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

    public static T Find<T>(Control parent, string name) where T : Control
    {
      return (T)parent.Controls.Find(name, true).Single();
    }

    public static void AddShowing(Form main, DateTime date, DateTime time, params string[] switchedOff)
    {
      Find<Button>(main, "Addshow").PerformClick();
      var dialog = Application.OpenForms.OfType<Addshowform>().Single();
      Find<DateTimePicker>(dialog, "Datumkeuze").Value = date;
      Find<DateTimePicker>(dialog, "Time").Value = time;
      // A new showing starts with the sections of the last one, so every section is set here.
      var sections = Find<CheckedListBox>(dialog, "Sectionsoff");
      for (var i = 0; i < sections.Items.Count; i++)
      {
        sections.SetItemChecked(i, switchedOff.Contains((string)sections.Items[i]));
      }
      Find<Button>(dialog, "Addshow").PerformClick();
    }

    // Adds a group that prefers every showing date. Needs at least one showing, like the app itself.
    public static void AddGroup(Form main, string name, int people)
    {
      Find<Button>(main, "Addgroupbutton").PerformClick();
      var dialog = Application.OpenForms.OfType<Addgroupform>().Single();
      Find<TextBox>(dialog, "Groepnaam").Text = name;
      Find<NumericUpDown>(dialog, "Groepsgrootte").Value = people;
      var dates = Find<CheckedListBox>(dialog, "Datumvoorkeurenbox");
      for (var i = 0; i < dates.Items.Count; i++)
      {
        dates.SetItemChecked(i, true);
      }
      Find<Button>(dialog, "Addgroupbutton").PerformClick();
    }
  }
}
