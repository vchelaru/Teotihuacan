using System;
using System.Collections.Generic;
using System.Linq;
using Teotihuacan.Managers;

namespace Teotihuacan.GumRuntimes.Menus
{
    public partial class PauseMenuRuntime
    {
        public event EventHandler QuitClicked;
        public event EventHandler ResumeClicked;
        public event EventHandler ClearDataClicked;

        partial void CustomInitialize()
        {
            this.AztecMenuButton_Quit.FormsControl.Click += (not, used) => QuitClicked?.Invoke(this, null);
            this.AztecMenuButton_Resume.FormsControl.Click += (not, used) => ResumeClicked?.Invoke(this, null);
            this.OptionsButtonInstance.FormsControl.Click += HandleOptionsClicked;
            this.OptionsBackButton.FormsControl.Click += HandleOptionsBackClicked;
            this.ClearDataButton.FormsControl.Click += ClearDataButtonClicked;
        }

        private void HandleOptionsClicked(object sender, EventArgs e)
        {
            OptionsButtons.Visible = true;
            MainButtons.Visible = false;
        }

        private void HandleOptionsBackClicked(object sender, EventArgs e)
        {
            OptionsButtons.Visible = false;
            MainButtons.Visible = true;
        }

        private void ClearDataButtonClicked(object sender, EventArgs e)
        {

            ClearDataClicked(this, null);
        }
    }
}
