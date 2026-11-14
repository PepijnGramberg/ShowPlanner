using System.Drawing;

namespace Theaterverdeling
{
  // The icon of the app, which every window shows in its title bar and on the taskbar.
  public static class AppIcon
  {
    public static Icon Value { get; } = Load();

    private static Icon Load()
    {
      using var stream = typeof(AppIcon).Assembly.GetManifestResourceStream("Theaterverdeling.ico");
      return new Icon(stream);
    }
  }
}
