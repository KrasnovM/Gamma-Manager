using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Gamma_Manager
{
    public partial class Window : Form
    {
        System.Globalization.CultureInfo customCulture;
        IniFile iniFile;

        List<Display.DisplayInfo> displays = new List<Display.DisplayInfo>();
        int numDisplay = 0;
        Display.DisplayInfo currDisplay;

        List<ToolStripComboBox> toolMonitors = new List<ToolStripComboBox>();
        ToolStripComboBox toolMonitor;

        bool disableChangeFunc = false;

        bool allColors = true;
        bool redColor = false;
        bool greenColor = false;
        bool blueColor = false;

        private void clearColors()
        {
            buttonAllColors.Font = new Font(buttonAllColors.Font.Name, buttonAllColors.Font.Size, FontStyle.Regular);
            buttonRed.Font = new Font(buttonRed.Font.Name, buttonRed.Font.Size, FontStyle.Regular);
            buttonGreen.Font = new Font(buttonGreen.Font.Name, buttonGreen.Font.Size, FontStyle.Regular);
            buttonBlue.Font = new Font(buttonBlue.Font.Name, buttonBlue.Font.Size, FontStyle.Regular);

            allColors = false;
            redColor = false;
            greenColor = false;
            blueColor = false;
        }

        private void initPresets()
        {
            comboBoxPresets.Items.Clear();
            comboBoxPresets.Text = string.Empty;
            string[] presets = iniFile.GetSections();
            if (presets != null)
            {
                for (int i = 0; i < presets.Length; i++)
                {
                    if (iniFile.Read("monitor", presets[i]) == currDisplay.displayName)
                    {
                        comboBoxPresets.Items.Add(presets[i]);
                    }
                }
            }
        }

        private void initTrayMenu()
        {
            contextMenu.Items.Clear();

            ToolStripMenuItem toolSetting = new ToolStripMenuItem("Settings", null, toolSettings_Click);
            contextMenu.Items.Add(toolSetting);

            ToolStripSeparator toolStripSeparator1 = new ToolStripSeparator();
            contextMenu.Items.Add(toolStripSeparator1);

            for (int i = 0; i < displays.Count; i++)
            {
                toolMonitor = new ToolStripComboBox(displays[i].displayName);
                toolMonitor.DropDownStyle = ComboBoxStyle.DropDownList;

                toolMonitor.Items.Add(displays[i].displayName + ":");
                toolMonitor.Text = displays[i].displayName + ":";

                toolMonitor.SelectedIndexChanged += new EventHandler(comboBoxToolMonitor_TextChanged);

                string[] presets = iniFile.GetSections();
                if (presets != null)
                {
                    for (int j = 0; j < presets.Length; j++)
                    {
                        if (iniFile.Read("monitor", presets[j]) == displays[i].displayName)
                        {
                            //preset.name = presets[j].Substring(presets[j].IndexOf(")") + 1);
                            toolMonitor.Items.Add(presets[j]);
                        }
                    }
                }
                toolMonitors.Add(toolMonitor);
                contextMenu.Items.Add(toolMonitor);
            }
            ToolStripSeparator toolStripSeparator2 = new ToolStripSeparator();
            contextMenu.Items.Add(toolStripSeparator2);
            ToolStripMenuItem toolExit = new ToolStripMenuItem("Exit", null, toolExit_Click);
            contextMenu.Items.Add(toolExit);
        }

        private void fillInfo(Display.DisplayInfo currDisplay)
        {
            disableChangeFunc = true;

            textBoxGamma.Text = ((currDisplay.rGamma + currDisplay.gGamma + currDisplay.bGamma) / 3f).ToString("0.00");
            textBoxContrast.Text = ((currDisplay.rContrast + currDisplay.gContrast + currDisplay.bContrast) / 3f).ToString("0.00");
            textBoxBrightness.Text = ((currDisplay.rBright + currDisplay.gBright + currDisplay.bBright) / 3f).ToString("0.00");

            trackBarGamma.Value = (int)(((currDisplay.rGamma + currDisplay.gGamma + currDisplay.bGamma) / 3f) * 100f);
            trackBarContrast.Value = (int)(((currDisplay.rContrast + currDisplay.gContrast + currDisplay.bContrast) / 3f) * 100f);
            trackBarBrightness.Value = (int)(((currDisplay.rBright + currDisplay.gBright + currDisplay.bBright) / 3f) * 100f);

            if (currDisplay.isExternal)
            {
                labelMonitorContrastUp.Visible = true;
                labelMonitorContrastDown.Visible = true;
                trackBarMonitorContrast.Visible = true;
                textBoxMonitorContrast.Visible = true;

                textBoxMonitorBrightness.Text = ExternalMonitor.GetBrightness(currDisplay.PhysicalHandle).ToString();
                trackBarMonitorBrightness.Value = ExternalMonitor.GetBrightness(currDisplay.PhysicalHandle);

                textBoxMonitorContrast.Text = ExternalMonitor.GetContrast(currDisplay.PhysicalHandle).ToString();
                trackBarMonitorContrast.Value = ExternalMonitor.GetContrast(currDisplay.PhysicalHandle);
            }
            else
            {
                labelMonitorContrastUp.Visible = false;
                labelMonitorContrastDown.Visible = false;
                trackBarMonitorContrast.Visible = false;
                textBoxMonitorContrast.Visible = false;

                textBoxMonitorBrightness.Text = InternalMonitor.GetBrightness().ToString();
                trackBarMonitorBrightness.Value = InternalMonitor.GetBrightness();
            }
            disableChangeFunc = false;
        }
        private void Window_Load(object sender, EventArgs e)
        {
            int screenWidth = Screen.PrimaryScreen.Bounds.Size.Width;
            int windowWidth = Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Size.Height;
            int windowHeight = Height;
            int tmp = Screen.PrimaryScreen.Bounds.Height;
            int TaskBarHeight = tmp - Screen.PrimaryScreen.WorkingArea.Height;

            //dpi
            /*int PSH = SystemParameters.PrimaryScreenHeight;
            int PSBH = Screen.PrimaryScreen.Bounds.Height;
            double ratio = PSH / PSBH;
            int TaskBarHeight = PSBH - Screen.PrimaryScreen.WorkingArea.Height;
            TaskBarHeight *= ratio;*/

            Location = new Point(screenWidth - windowWidth, screenHeight - (windowHeight + TaskBarHeight));
        }

        public Window()
        {
            InitializeComponent();
            customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ",";

            iniFile = new IniFile("GammaManager.ini");

            buttonAllColors.Font = new Font(buttonAllColors.Font.Name, buttonAllColors.Font.Size, FontStyle.Bold);


            displays = Display.QueryDisplayDevices();
            displays.Reverse();
            for (int i = 0; i < displays.Count; i++)
            {
                displays[i].numDisplay = i;
                comboBoxMonitors.Items.Add(i+1+") "+ displays[i].displayName);
            }
            currDisplay = displays[numDisplay];
            comboBoxMonitors.SelectedIndex = numDisplay;

            fillInfo(currDisplay);

            initPresets();

            initTrayMenu();
            notifyIcon.ContextMenuStrip = contextMenu;
        }

        private void trackBarGamma_ValueChanged(object sender, EventArgs e)
        {
            comboBoxPresets.Text = string.Empty;

            if (!disableChangeFunc)
            {
                textBoxGamma.Text = ((float)trackBarGamma.Value / 100f).ToString("0.00");

                if (allColors)
                {
                    currDisplay.rGamma = (float)trackBarGamma.Value / 100f;
                    currDisplay.gGamma = (float)trackBarGamma.Value / 100f;
                    currDisplay.bGamma = (float)trackBarGamma.Value / 100f;
                    Gamma.SetGammaRamp(currDisplay.displayLink,
                        Gamma.CreateGammaRamp(currDisplay.rGamma, currDisplay.gGamma, currDisplay.bGamma, currDisplay.rContrast,
                        currDisplay.gContrast, currDisplay.bContrast, currDisplay.rBright, currDisplay.gBright, currDisplay.bBright));
                    goto EndColors;
                }

                if (redColor)
                {
                    currDisplay.rGamma = (float)trackBarGamma.Value / 100f;
                    Gamma.SetGammaRamp(currDisplay.displayLink,
                        Gamma.CreateGammaRamp(currDisplay.rGamma, currDisplay.gGamma, currDisplay.bGamma, currDisplay.rContrast,
                        currDisplay.gContrast, currDisplay.bContrast, currDisplay.rBright, currDisplay.gBright, currDisplay.bBright));
                    goto EndColors;
                }

                if (greenColor)
                {
                    currDisplay.gGamma = (float)trackBarGamma.Value / 100f;
                    Gamma.SetGammaRamp(currDisplay.displayLink,
                        Gamma.CreateGammaRamp(currDisplay.rGamma, currDisplay.gGamma, currDisplay.bGamma, currDisplay.rContrast,
                        currDisplay.gContrast, currDisplay.bContrast, currDisplay.rBright, currDisplay.gBright, currDisplay.bBright));
                    goto EndColors;
                }

                if (blueColor)
                {
                    currDisplay.bGamma = (float)trackBarGamma.Value / 100f;
                    Gamma.SetGammaRamp(currDisplay.displayLink,
                        Gamma.CreateGammaRamp(currDisplay.rGamma, currDisplay.gGamma, currDisplay.bGamma, currDisplay.rContrast,
                        currDisplay.gContrast, currDisplay.bContrast, currDisplay.rBright, currDisplay.gBright, currDisplay.bBright));
                }

            EndColors:
                return;

            }
        }
            

        private void trackBarContrast_ValueChanged(object sender, EventArgs e)
        {
            comboBoxPresets.Text = string.Empty;

            if (!disableChangeFunc)
            {
                textBoxContrast.Text = ((float)trackBarContrast.Value / 100f).ToString("0.00");

                if (allColors)
                {
                    currDisplay.rContrast = (float)trackBarContrast.Value / 100f;
                    currDisplay.gContrast = (float)trackBarContrast.Value / 100f;
                    currDisplay.bContrast = (float)trackBarContrast.Value / 100f;
                    Gamma.SetGammaRamp(currDisplay.displayLink,
                        Gamma.CreateGammaRamp(currDisplay.rGamma, currDisplay.gGamma, currDisplay.bGamma, currDisplay.rContrast,
                        currDisplay.gContrast, currDisplay.bContrast, currDisplay.rBright, currDisplay.gBright, currDisplay.bBright));
                    goto EndColors;
                }

                if (redColor)
                {
                    currDisplay.rContrast = (float)trackBarContrast.Value / 100f;
                    Gamma.SetGammaRamp(currDisplay.displayLink,
                        Gamma.CreateGammaRamp(currDisplay.rGamma, currDisplay.gGamma, currDisplay.bGamma, currDisplay.rContrast,
                        currDisplay.gContrast, currDisplay.bContrast, currDisplay.rBright, currDisplay.gBright, currDisplay.bBright));
                    goto EndColors;
                }

                if (greenColor)
                {
                    currDisplay.gContrast = (float)trackBarContrast.Value / 100f;
                    Gamma.SetGammaRamp(currDisplay.displayLink,
                        Gamma.CreateGammaRamp(currDisplay.rGamma, currDisplay.gGamma, currDisplay.bGamma, currDisplay.rContrast,
                        currDisplay.gContrast, currDisplay.bContrast, currDisplay.rBright, currDisplay.gBright, currDisplay.bBright));
                    goto EndColors;
                }

                if (blueColor)
                {
                    currDisplay.bContrast = (float)trackBarContrast.Value / 100f;
                    Gamma.SetGammaRamp(currDisplay.displayLink,
                        Gamma.CreateGammaRamp(currDisplay.rGamma, currDisplay.gGamma, currDisplay.bGamma, currDisplay.rContrast,
                        currDisplay.gContrast, currDisplay.bContrast, currDisplay.rBright, currDisplay.gBright, currDisplay.bBright));
                }

            EndColors:
                return;
            }
        }

        private void trackBarBrightness_ValueChanged(object sender, EventArgs e)
        {
            comboBoxPresets.Text = string.Empty;

            if (!disableChangeFunc)
            {
                textBoxBrightness.Text = ((float)trackBarBrightness.Value / 100f).ToString("0.00");

                if (allColors)
                {
                    currDisplay.rBright = (float)trackBarBrightness.Value / 100f;
                    currDisplay.gBright = (float)trackBarBrightness.Value / 100f;
                    currDisplay.bBright = (float)trackBarBrightness.Value / 100f;
                    Gamma.SetGammaRamp(currDisplay.displayLink,
                        Gamma.CreateGammaRamp(currDisplay.rGamma, currDisplay.gGamma, currDisplay.bGamma, currDisplay.rContrast,
                        currDisplay.gContrast, currDisplay.bContrast, currDisplay.rBright, currDisplay.gBright, currDisplay.bBright));
                    goto EndColors;
                }

                if (redColor)
                {
                    currDisplay.rBright = (float)trackBarBrightness.Value / 100f;
                    Gamma.SetGammaRamp(currDisplay.displayLink,
                        Gamma.CreateGammaRamp(currDisplay.rGamma, currDisplay.gGamma, currDisplay.bGamma, currDisplay.rContrast,
                        currDisplay.gContrast, currDisplay.bContrast, currDisplay.rBright, currDisplay.gBright, currDisplay.bBright));
                    goto EndColors;
                }

                if (greenColor)
                {
                    currDisplay.gBright = (float)trackBarBrightness.Value / 100f;
                    Gamma.SetGammaRamp(currDisplay.displayLink,
                        Gamma.CreateGammaRamp(currDisplay.rGamma, currDisplay.gGamma, currDisplay.bGamma, currDisplay.rContrast,
                        currDisplay.gContrast, currDisplay.bContrast, currDisplay.rBright, currDisplay.gBright, currDisplay.bBright));
                    goto EndColors;
                }

                if (blueColor)
                {
                    currDisplay.bBright = (float)trackBarBrightness.Value / 100f;
                    Gamma.SetGammaRamp(currDisplay.displayLink,
                        Gamma.CreateGammaRamp(currDisplay.rGamma, currDisplay.gGamma, currDisplay.bGamma, currDisplay.rContrast,
                        currDisplay.gContrast, currDisplay.bContrast, currDisplay.rBright, currDisplay.gBright, currDisplay.bBright));
                }

            EndColors:
                return;
            }
        }

        private void trackBarMonitorBrightness_ValueChanged(object sender, EventArgs e)
        {
            comboBoxPresets.Text = string.Empty;

            if (!disableChangeFunc)
            {
                textBoxMonitorBrightness.Text = trackBarMonitorBrightness.Value.ToString();

                currDisplay.monitorBrightness = trackBarMonitorBrightness.Value;

                if (currDisplay.isExternal)
                {
                    ExternalMonitor.SetBrightness(currDisplay.PhysicalHandle, (uint)trackBarMonitorBrightness.Value);
                }
                else
                {
                    InternalMonitor.SetBrightness((byte)trackBarMonitorBrightness.Value);
                }
            }
        }

        private void trackBarMonitorContrast_ValueChanged(object sender, EventArgs e)
        {
            comboBoxPresets.Text = string.Empty;

            if (!disableChangeFunc)
            {
                textBoxMonitorContrast.Text = trackBarMonitorContrast.Value.ToString();

                currDisplay.monitorContrast = trackBarMonitorContrast.Value;

                ExternalMonitor.SetContrast(currDisplay.PhysicalHandle, (uint)trackBarMonitorContrast.Value);
            }
        }

        private void buttonAllColors_Click(object sender, EventArgs e)
        {
            disableChangeFunc = true;
            clearColors();
            allColors = true;

            textBoxGamma.Text = ((currDisplay.rGamma + currDisplay.gGamma + currDisplay.bGamma) / 3f).ToString("0.00");
            textBoxContrast.Text = ((currDisplay.rContrast + currDisplay.gContrast + currDisplay.bContrast) / 3f).ToString("0.00");
            textBoxBrightness.Text = ((currDisplay.rBright + currDisplay.gBright + currDisplay.bBright) / 3f).ToString("0.00");

            trackBarGamma.Value = (int)(((currDisplay.rGamma + currDisplay.gGamma + currDisplay.bGamma) / 3f) * 100f);
            trackBarContrast.Value = (int)(((currDisplay.rContrast + currDisplay.gContrast + currDisplay.bContrast) / 3f) * 100f);
            trackBarBrightness.Value = (int)(((currDisplay.rBright + currDisplay.gBright + currDisplay.bBright) / 3f) * 100f);

            buttonAllColors.Font = new Font(buttonAllColors.Font.Name, buttonAllColors.Font.Size, FontStyle.Bold);
            disableChangeFunc = false;
        }

        private void buttonRed_Click(object sender, EventArgs e)
        {
            disableChangeFunc = true;
            clearColors();
            redColor = true;

            textBoxGamma.Text = currDisplay.rGamma.ToString("0.00");
            textBoxContrast.Text = currDisplay.rContrast.ToString("0.00");
            textBoxBrightness.Text = currDisplay.rBright.ToString("0.00");

            trackBarGamma.Value = (int)(currDisplay.rGamma * 100f);
            trackBarContrast.Value = (int)(currDisplay.rContrast * 100f);
            trackBarBrightness.Value = (int)(currDisplay.rBright * 100f);

            buttonRed.Font = new Font(buttonRed.Font.Name, buttonRed.Font.Size, FontStyle.Bold);
            disableChangeFunc = false;
        }

        private void buttonGreen_Click(object sender, EventArgs e)
        {
            disableChangeFunc = true;
            clearColors();
            greenColor = true;

            textBoxGamma.Text = currDisplay.gGamma.ToString("0.00");
            textBoxContrast.Text = currDisplay.gContrast.ToString("0.00");
            textBoxBrightness.Text = currDisplay.gBright.ToString("0.00");

            trackBarGamma.Value = (int)(currDisplay.gGamma * 100f);
            trackBarContrast.Value = (int)(currDisplay.gContrast * 100f);
            trackBarBrightness.Value = (int)(currDisplay.gBright * 100f);

            buttonGreen.Font = new Font(buttonGreen.Font.Name, buttonGreen.Font.Size, FontStyle.Bold);
            disableChangeFunc = false;
        }

        private void buttonBlue_Click(object sender, EventArgs e)
        {
            disableChangeFunc = true;
            clearColors();
            blueColor = true;

            textBoxGamma.Text = currDisplay.bGamma.ToString("0.00");
            textBoxContrast.Text = currDisplay.bContrast.ToString("0.00");
            textBoxBrightness.Text = currDisplay.bBright.ToString("0.00");

            trackBarGamma.Value = (int)(currDisplay.bGamma * 100f);
            trackBarContrast.Value = (int)(currDisplay.bContrast * 100f);
            trackBarBrightness.Value = (int)(currDisplay.bBright * 100f);

            buttonBlue.Font = new Font(buttonBlue.Font.Name, buttonBlue.Font.Size, FontStyle.Bold);
            disableChangeFunc = false;
        }

        private void checkBoxExContrast_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxExContrast.Checked)
            {
                trackBarContrast.Maximum = 10000;
            } else
            {
                trackBarContrast.Maximum = 300;
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            string tmp = comboBoxPresets.Text;
            iniFile.Write("monitor", currDisplay.displayName, currDisplay.displayName+": "+ comboBoxPresets.Text);
            iniFile.Write("rGamma", currDisplay.rGamma.ToString(), currDisplay.displayName + ": " + comboBoxPresets.Text);
            iniFile.Write("gGamma", currDisplay.gGamma.ToString(), currDisplay.displayName + ": " + comboBoxPresets.Text);
            iniFile.Write("bGamma", currDisplay.bGamma.ToString(), currDisplay.displayName + ": " + comboBoxPresets.Text);
            iniFile.Write("rContrast", currDisplay.rContrast.ToString(), currDisplay.displayName + ": " + comboBoxPresets.Text);
            iniFile.Write("gContrast", currDisplay.gContrast.ToString(), currDisplay.displayName + ": " + comboBoxPresets.Text);
            iniFile.Write("bContrast", currDisplay.bContrast.ToString(), currDisplay.displayName + ": " + comboBoxPresets.Text);
            iniFile.Write("rBright", currDisplay.rBright.ToString(), currDisplay.displayName + ": " + comboBoxPresets.Text);
            iniFile.Write("gBright", currDisplay.gBright.ToString(), currDisplay.displayName + ": " + comboBoxPresets.Text);
            iniFile.Write("bBright", currDisplay.bBright.ToString(), currDisplay.displayName + ": " + comboBoxPresets.Text);
            iniFile.Write("monitorBrightness", currDisplay.monitorBrightness.ToString(), currDisplay.displayName + ": " + comboBoxPresets.Text);
            iniFile.Write("monitorContrast", currDisplay.monitorContrast.ToString(), currDisplay.displayName + ": " + comboBoxPresets.Text);

            initPresets();
            comboBoxPresets.Text = currDisplay.displayName + ": " + tmp;

            initTrayMenu();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            iniFile.DeleteSection(comboBoxPresets.Text);

            initPresets();
            initTrayMenu();
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            comboBoxPresets.Text = string.Empty;

            trackBarGamma.Value = 100;
            trackBarContrast.Value = 100;
            trackBarBrightness.Value = 0;

            currDisplay.rGamma = 1;
            currDisplay.gGamma = 1;
            currDisplay.bGamma = 1;
            currDisplay.rContrast = 1;
            currDisplay.gContrast = 1;
            currDisplay.bContrast = 1;
            currDisplay.rBright = 0;
            currDisplay.gBright = 0;
            currDisplay.bBright = 0;

            
            if (currDisplay.isExternal)
            {

                trackBarMonitorBrightness.Value = 100;

                trackBarMonitorContrast.Value = 50;
            } else
            {
                trackBarMonitorBrightness.Value = 100;
            }
            

            Gamma.SetGammaRamp(displays[numDisplay].displayLink, Gamma.CreateGammaRamp(1, 1, 1, 1, 1, 1, 0, 0, 0));
        }

        private void buttonHide_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void comboBoxMonitors_SelectedIndexChanged(object sender, EventArgs e)
        {
            string num = comboBoxMonitors.SelectedItem.ToString();

            num = num.Substring(0, num.IndexOf(")"));
            numDisplay = Int32.Parse(num)-1;

            currDisplay = displays[numDisplay];
            fillInfo(currDisplay);
            
            initPresets();
        }

        private void buttonForward_Click(object sender, EventArgs e)
        {
            if (numDisplay + 1 <= displays.Count-1)
            {
                comboBoxMonitors.SelectedIndex = numDisplay + 1;
            } else
            {
                comboBoxMonitors.SelectedIndex = 0;
            }
        }

        private void comboBoxPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            string test = comboBoxPresets.Text;
            currDisplay.rGamma = float.Parse(iniFile.Read("rGamma", comboBoxPresets.Text), customCulture);
            currDisplay.gGamma = float.Parse(iniFile.Read("gGamma", comboBoxPresets.Text), customCulture);
            currDisplay.bGamma = float.Parse(iniFile.Read("bGamma", comboBoxPresets.Text), customCulture);
            currDisplay.rContrast = float.Parse(iniFile.Read("rContrast", comboBoxPresets.Text), customCulture);
            currDisplay.gContrast = float.Parse(iniFile.Read("gContrast", comboBoxPresets.Text), customCulture);
            currDisplay.bContrast = float.Parse(iniFile.Read("bContrast", comboBoxPresets.Text), customCulture);
            currDisplay.rBright = float.Parse(iniFile.Read("rBright", comboBoxPresets.Text), customCulture);
            currDisplay.gBright = float.Parse(iniFile.Read("gBright", comboBoxPresets.Text), customCulture);
            currDisplay.bBright = float.Parse(iniFile.Read("bBright", comboBoxPresets.Text), customCulture);
            currDisplay.monitorBrightness = int.Parse(iniFile.Read("monitorBrightness", comboBoxPresets.Text));
            currDisplay.monitorContrast = int.Parse(iniFile.Read("monitorContrast", comboBoxPresets.Text));

            fillInfo(currDisplay);

            Gamma.SetGammaRamp(currDisplay.displayLink,
                Gamma.CreateGammaRamp(currDisplay.rGamma, currDisplay.gGamma, currDisplay.bGamma, 
                currDisplay.rContrast,currDisplay.gContrast, currDisplay.bContrast, currDisplay.rBright, currDisplay.gBright, 
                currDisplay.bBright));

            if (currDisplay.isExternal)
            {
                trackBarMonitorBrightness.Value = currDisplay.monitorBrightness;
                trackBarMonitorContrast.Value = currDisplay.monitorContrast;
            }
            else
            {
                trackBarMonitorBrightness.Value = currDisplay.monitorBrightness;
            }
        }

        //tray
        private void Window_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void toolSettings_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void toolExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void comboBoxToolMonitor_TextChanged(object sender, EventArgs e)
        {
            if (!disableChangeFunc)
            {
                string monitor = sender.ToString().Substring(0, sender.ToString().IndexOf(":"));
                /*string comName = toolMonitor.Items[i].ToString().Substring
                        (0, toolMonitor.Items[i].ToString().IndexOf(":"));*/

                int tmp = 0;

                disableChangeFunc = true;
                for (int i = 0; i < displays.Count; i++)
                {
                    if (monitor.Equals(displays[i].displayName))
                    {
                        tmp = i;
                    }
                    else
                    {
                        toolMonitor = toolMonitors[i];
                        toolMonitor.SelectedIndex = 0;
                    }
                }
                disableChangeFunc = false;

                toolMonitor = toolMonitors[tmp];

                if (toolMonitor.SelectedIndex != 0)
                {

                    for (int i = 0; i < displays.Count; i++)
                    {
                        if (displays[i].displayName.Equals(toolMonitor.Items[0].ToString().Substring(0, toolMonitor.Items[0].ToString().IndexOf(":"))))
                        {
                            comboBoxMonitors.Text = i + 1 + ") " + displays[i].displayName;

                            numDisplay = i;
                            currDisplay.numDisplay = numDisplay;
                            currDisplay.displayLink = displays[i].displayLink;
                            currDisplay.isExternal = displays[i].isExternal;
                            break;
                        }
                    }

                    currDisplay.displayName = toolMonitor.Items[0].ToString().Substring(0, toolMonitor.Items[0].ToString().IndexOf(":"));

                    currDisplay.rGamma = float.Parse(iniFile.Read("rGamma", toolMonitor.Text), customCulture);
                    currDisplay.gGamma = float.Parse(iniFile.Read("gGamma", toolMonitor.Text), customCulture);
                    currDisplay.bGamma = float.Parse(iniFile.Read("bGamma", toolMonitor.Text), customCulture);
                    currDisplay.rContrast = float.Parse(iniFile.Read("rContrast", toolMonitor.Text), customCulture);
                    currDisplay.gContrast = float.Parse(iniFile.Read("gContrast", toolMonitor.Text), customCulture);
                    currDisplay.bContrast = float.Parse(iniFile.Read("bContrast", toolMonitor.Text), customCulture);
                    currDisplay.rBright = float.Parse(iniFile.Read("rBright", toolMonitor.Text), customCulture);
                    currDisplay.gBright = float.Parse(iniFile.Read("gBright", toolMonitor.Text), customCulture);
                    currDisplay.bBright = float.Parse(iniFile.Read("bBright", toolMonitor.Text), customCulture);
                    currDisplay.monitorBrightness = int.Parse(iniFile.Read("monitorBrightness", toolMonitor.Text));
                    currDisplay.monitorContrast = int.Parse(iniFile.Read("monitorContrast", toolMonitor.Text));

                    fillInfo(currDisplay);

                    Gamma.SetGammaRamp(currDisplay.displayLink,
                        Gamma.CreateGammaRamp(currDisplay.rGamma, currDisplay.gGamma, currDisplay.bGamma,
                        currDisplay.rContrast, currDisplay.gContrast, currDisplay.bContrast, currDisplay.rBright, currDisplay.gBright,
                        currDisplay.bBright));

                    if (currDisplay.isExternal)
                    {
                        trackBarMonitorBrightness.Value = currDisplay.monitorBrightness;
                        trackBarMonitorContrast.Value = currDisplay.monitorContrast;
                    }
                    else
                    {
                        trackBarMonitorBrightness.Value = currDisplay.monitorBrightness;
                    }
                    comboBoxPresets.Text = toolMonitor.Text;
                }
            }
        }

        //destroy focuses on buttons, trackbars, comboboxes, text, checkbox
    }
}
