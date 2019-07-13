using System;
using System.Collections.Generic;
using System.Linq;

namespace Teotihuacan.GumRuntimes.Menus
{
    public partial class PauseMenuRuntime
    {
        public event EventHandler QuitClicked;
        public event EventHandler ResumeClicked;

        partial void CustomInitialize()
        {
            this.AztecMenuButton_Quit.FormsControl.Click += (not, used) => QuitClicked?.Invoke(this, null);
            this.AztecMenuButton_Resume.FormsControl.Click += (not, used) => ResumeClicked?.Invoke(this, null);
        }
    }
}
