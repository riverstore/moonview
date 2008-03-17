using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace MoonView
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MoonViewForm(args));
        }
    }
}
