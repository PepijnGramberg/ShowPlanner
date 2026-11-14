using System;
using System.Windows.Forms;

namespace Theaterverdeling
{
  static class Program
  {
    // The font of every window. The windows are laid out for the default font and scale along with this one.
    public static readonly System.Drawing.Font AppFont = new System.Drawing.Font("Segoe UI", 10F);

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      // One font for every window, a size larger than the Windows default so it reads easily.
      Application.SetDefaultFont(AppFont);
      Application.Run(new Theaterverdeling());
    }
  }
}
