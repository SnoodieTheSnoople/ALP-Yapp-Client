using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using YappPrototype1.Chat_Forms;

namespace YappPrototype1
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var loginForm = new LoginForm();
            Application.Run(loginForm);

            if (loginForm.Authenticated)
            {
                Application.Run(new ChatWindow(loginForm.Username, loginForm.Email));
            }
        }
    }
}
