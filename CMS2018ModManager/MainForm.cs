using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;    //For streamReader, and directory info
using System.IO.Compression;    //For Zip file stuff

namespace CMS2018ModManager
{
    public partial class MainForm : Form
    {
        private string ModManVersion = "0.1";       //Version constant for ModManager
        private string GameVersion = "1.2.0";         //Version constant for the game

        //Class object for class that does the acutal mod managing stuff    //here so it's scope is within the form object  //should move the config stuff out at somepoint
        ConfigFile ModManConfig;

        public MainForm()
        {
            //Setup the form controls
            InitializeComponent();

            //Setup the class that does the acutal mod managing stuff
            ModManConfig = new ConfigFile();

            //Populate the Available Cars list box
            PopulateCarsAvailableList();
        }

        //stuff to do when the form is loaded
        private void MainForm_Load(object sender, EventArgs e)
        {
            //Called from within InitializeComponent()
            //A good place to load data after the form is loaded

            //load the config file, contains saved game dir locations
            ModManConfig.ReadConfigFile();

            //Save Games Tab
            //Get the list of profiles
            PopulateProfileComboBox();
        }

        #region Menu strip
        //Misc Menu Items

        //Handle a call to exit
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Close this all down
            Application.Exit();
        }

        //Handle a click on the about menu item
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Car Mechanic Simulator 2015 Mod Manager Version " + ModManVersion + "\nVery much a work in progress\nDesigned for " + GameVersion + "\n\nThanks to all that have helped\n\nBy Blue Icarian Wings");
        }

        //Save Menu Items

        //Handle the menu request to backup the save dir
        private void backupSavesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Prompt the user to see if they are sure
            DialogResult PromptResult = MessageBox.Show("This will backup all the save profiles overwriting the ones in the save backup folder\nAre you sure?", "Backup Game Saves", MessageBoxButtons.YesNo);

            if (PromptResult == DialogResult.Yes)
            {
                ModManConfig.DirectoryCopy(ModManConfig.GetSavedGamesDir(), ModManConfig.GetSavedGamesDirBkUp(), true);
            }
        }

        //Handle the menu request to restore the saves dir
        private void restoreSavesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Prompt the user to see if they are sure
            DialogResult PromptResult = MessageBox.Show("This will overwrite all the save profiles with the ones in the save backup folder\nAre you sure?", "Restore Game Saves", MessageBoxButtons.YesNo);

            if (PromptResult == DialogResult.Yes)
            {
                ModManConfig.DirectoryCopy(ModManConfig.GetSavedGamesDirBkUp(), ModManConfig.GetSavedGamesDir(), true);
            }
        }

        //Handle set saves source click from menu strip
        private void setSavesDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Open up a folder browser
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            //Get the result and check the dialog was ok'd
            DialogResult result = fbd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                //Retrieve and set the selected dir
                ModManConfig.SetSavedGamesDir(fbd.SelectedPath);
                //Update the config file
                ModManConfig.SaveConfigFile();
            }
        }

        //Handle set saves backup click from menu strip
        private void setSavesBackupDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Open up a folder browser
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            //Get the result and check the dialog was ok'd
            DialogResult result = fbd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                //Retrieve and set the selected dir
                ModManConfig.SetSavedGamesDirBkUp(fbd.SelectedPath);
                //Update the config file
                ModManConfig.SaveConfigFile();
            }
        }

        //Car Config Menu Items

        //Handle set car data source click from menu strip
        private void setCarConfigDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Open up a folder browser
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            //Get the result and check the dialog was ok'd
            DialogResult result = fbd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                //Retrieve and set the selected dir
                ModManConfig.SetCarsDataDir(fbd.SelectedPath);
                //Update the config file
                ModManConfig.SaveConfigFile();
            }
        }
        #endregion

        #region Save File - General
        //Populate the profile list combo box
        private void PopulateProfileComboBox()
        {
            //Get directories
            DirectoryInfo di = new DirectoryInfo(ModManConfig.GetSavedGamesDir());
            //Counter for profiles
            int ProCount = 0;

            foreach (System.IO.DirectoryInfo Folder in di.GetDirectories())
            {
                string temp = Folder.ToString();
                if (!temp.Equals("Unity"))  //Skip over the Unity folder
                {
                    //Add folder to the combo box lists
                    SGETProfilecomboBox.Items.Add(Folder);
                    //Increment the profiles counter
                    ProCount++;
                }
            }

            //Set the found profiles counter
            SGTProfilesFoundlabel.Text = ProCount + " Profiles Found";
        }
        #endregion

        #region Save File - Global file
        //Resets the Save Data Global tab GUI elements
        private void ClearOutSaveDataTabsGlobal()
        {
            //Fill out the GUI

            //Global
            SGETGMoneynumericUpDown.Value = 0;
            SGETGLevelnumericUpDown.Value = 0;
            SGETGXPnumericUpDown.Value = 0;


            SGETGPartsRepairednumericUpDown.Value = 0;
            SGETGMoneyIncomePartsnumericUpDown.Value = 0;
            SGETGMoneyIncomeCarsnumericUpDown.Value = 0;
            SGETGCarsSoldnumericUpDown.Value = 0;
            SGETGJobsCompletednumericUpDown.Value = 0;
            SGETGCarsOwnednumericUpDown.Value = 0;
            SGETGMoneyIncomenumericUpDown.Value = 0;
            SGETGPartsUnmountednumericUpDown.Value = 0;
            SGETGBoltsUndonenumericUpDown.Value = 0;
            SGETBankLoannumericUpDown.Value = 0; 
        }

        //Load the global save file
        private void SGEGobalFileLoad()
        {
            //Check the combo box text isn't blank
            if (SGETProfilecomboBox.Text != "")
            {
                SaveGameDataGlobal LocalGrab = new SaveGameDataGlobal();        //Create a local to get save data
                //Check if the file exists
                if (LocalGrab.LoadGlobalSaveFile(ModManConfig.GetSavedGamesDir() + "\\" + SGETProfilecomboBox.Text))     //Load the save file
                {
                    //Fill out the GUI
                    //SGETGPartsRepairednumericUpDown.Value = LocalGrab._Stats_PartsRepaired;
                    //SGETGMoneyIncomePartsnumericUpDown.Value = LocalGrab._Stats_MoneyIncomeParts;
                    //SGETGMoneyIncomeCarsnumericUpDown.Value = LocalGrab._Stats_MoneyIncomeCars;
                    //SGETGCarsSoldnumericUpDown.Value = LocalGrab._Stats_CarsSold;
                    //SGETGJobsCompletednumericUpDown.Value = LocalGrab._Stats_JobsCompletted;
                    //SGETGCarsOwnednumericUpDown.Value = LocalGrab._Stats_CarsOwned;
                    //SGETGMoneyIncomenumericUpDown.Value = LocalGrab._Stats_MoneyIncome;
                    //SGETGPartsUnmountednumericUpDown.Value = LocalGrab._Stats_PartsUnmounted;
                    //SGETGBoltsUndonenumericUpDown.Value = LocalGrab._Stats_Bolts;
                    //SGETBankLoannumericUpDown.Value = LocalGrab._bankLoan;
                    
                    SGETGMoneynumericUpDown.Value = LocalGrab._money;
                    SGETGLevelnumericUpDown.Value = LocalGrab._level;
                    SGETGXPnumericUpDown.Value = LocalGrab._xp;
                }
            }
        }

        //Save the global save file
        private void SGETGSavebutton_Click(object sender, EventArgs e)
        {
            //Create a local to hold save data
            SaveGameDataGlobal LocalSave = new SaveGameDataGlobal();

            //Fill out the data object
            LocalSave._xp = (int)SGETGXPnumericUpDown.Value;
            LocalSave._money = (int)SGETGMoneynumericUpDown.Value;
            LocalSave._level = (int)SGETGLevelnumericUpDown.Value;


            //LocalSave._Stats_PartsRepaired = (int)SGETGPartsRepairednumericUpDown.Value;
            //LocalSave._Stats_MoneyIncomeParts = (int)SGETGMoneyIncomePartsnumericUpDown.Value;
            //LocalSave._Stats_MoneyIncomeCars = (int)SGETGMoneyIncomeCarsnumericUpDown.Value;
            //LocalSave._Stats_CarsSold = (int)SGETGCarsSoldnumericUpDown.Value;
            //LocalSave._Stats_JobsCompletted = (int)SGETGJobsCompletednumericUpDown.Value;
            //LocalSave._Stats_CarsOwned = (int)SGETGCarsOwnednumericUpDown.Value;
            //LocalSave._Stats_MoneyIncome = (int)SGETGMoneyIncomenumericUpDown.Value;
            //LocalSave._Stats_PartsUnmounted = (int)SGETGPartsUnmountednumericUpDown.Value;
            //LocalSave._Stats_Bolts = (int)SGETGBoltsUndonenumericUpDown.Value;
            //LocalSave._bankLoan = (int)SGETBankLoannumericUpDown.Value;

            //Save the file
            LocalSave.WriteGlobalSaveFile(ModManConfig.GetSavedGamesDir() + "\\" + SGETProfilecomboBox.Text);
        }

        //Handles a change in the selected save profile combo box
        private void SGETProfilecomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Global file
            ClearOutSaveDataTabsGlobal(); //Clear out the GUI
            SGEGobalFileLoad();     //Load the Global tab data
        }

        #endregion

        #region Car List

        //Populate the Available Cars list box
        private void PopulateCarsAvailableList()
        {
            //Empty out current contents
            CCMTAvailableCarslistBox.Items.Clear();

            //Get directories
            DirectoryInfo di = new DirectoryInfo(ModManConfig.GetCarsDataDir());
            //Counter
            int Count = 0;

            foreach (System.IO.DirectoryInfo Folder in di.GetDirectories())
            {
                //Add folder to the combo box lists
                CCMTAvailableCarslistBox.Items.Add(Folder);
                //Increment the profiles counter
                Count++;
            }
        }
        
        //Handles a car being selected
        private void CCMTAvailableCarslistBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Call the fuction to fill out the configs listbox
            SummariseCarConfigFiles(CCMTAvailableCarslistBox.SelectedItem.ToString());
        }

        //Summarise the config text files
        private void SummariseCarConfigFiles(string CarConfigFolder)
        {
            //Clear out the existing list
            CCMTConfigslistBox.Items.Clear();
            //Clear out the version name text box
            CCMTCarVersionNametextBox.Text = "";
            //Clear out the body config textbox
            CCMTBodyConfigPresenttextBox.Text = "";
            CCMTBodyConfigPresenttextBox.BackColor = CCMTCarVersionNametextBox.BackColor;   //Hacky but resets it to the orginal colour

            //Assemble the directory filepath
            string Filepath = ModManConfig.GetCarsDataDir() + "\\" + CarConfigFolder;
            //Get all text files
            string[] Txtfiles = Directory.GetFiles(Filepath, "*.txt");

            //Set the car name
            String TempFilename = Filepath + "\\name.txt";
            if (File.Exists(TempFilename))  //Check if the config file exists
            {
                //create a streamReader to accses the config file
                StreamReader reader = new StreamReader(TempFilename);
                //string list (an array) to hold file output
                List<string> list = new List<string>();
                //string to hold a single line
                string line;

                //Read a line from the file
                line = reader.ReadLine();
                //Set the text box name
                CCMTNametextBox.Text = line;

                //we are finished with the reader so close and bin it
                reader.Close();
                reader.Dispose();
            }

            //Fill out the config files

            //Get all files with config, but body in them.
            List<string> CarConfigFiles = new List<string>();

            foreach (string line in Txtfiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(line);   //Get the filename without txt extension
                if ((fileName.Contains("config")) && (!(fileName.Contains("body"))))
                {
                    CarConfigFiles.Add(fileName);
                    CCMTConfigslistBox.Items.Add(fileName);
                }
            }
        }

        //Handles a car config being selected
        private void CCMTConfigslistBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Assemble the directory filepath
            string Filepath = ModManConfig.GetCarsDataDir() + "\\" + CCMTAvailableCarslistBox.SelectedItem.ToString();

            //Set the car config filename
            String TempFilename = Filepath + "\\" + CCMTConfigslistBox.SelectedItem.ToString() + ".txt";
            if (File.Exists(TempFilename))  //Check if the config file exists
            {
                //create a streamReader to accses the config file
                StreamReader reader = new StreamReader(TempFilename);
                //string to hold a single line
                string line;

                //loop through all of file a line at a time
                while (true)
                {
                    //Read a line from the file
                    line = reader.ReadLine();
                    //check if line is null
                    if (line == null)
                    {
                        break;  //exit loop if an empty line
                    }
                    else
                    {
                        if(line.Contains("carVersionName"))
                        {
                            //Breakout the part we want
                            int j = line.IndexOf('=');      //Find the end of label string
                                                            //Grab the bit after the '=' and remove the leading and trailing spaces
                            CCMTCarVersionNametextBox.Text = line.Substring(j + 1, line.Length - (j + 1)).Trim(' ');
                        }
                    }
                }

                //we are finished with the reader so close and bin it
                reader.Close();
                reader.Dispose();

                //Setup the body config file
                //CCMTBodyConfigPresenttextBox
                TempFilename = Filepath + "\\body" + CCMTConfigslistBox.SelectedItem.ToString() + ".txt";
                if (File.Exists(TempFilename))  //Check if the config file exists
                {
                    //Set the body config file textbox
                    CCMTBodyConfigPresenttextBox.Text = "body" + CCMTConfigslistBox.SelectedItem.ToString();
                    CCMTBodyConfigPresenttextBox.BackColor = CCMTCarVersionNametextBox.BackColor;   //Hacky but resets it to the orginal colour
                }
                else
                {
                    //No file found
                    CCMTBodyConfigPresenttextBox.Text = "None present";
                    CCMTBodyConfigPresenttextBox.BackColor = Color.Red;
                }
            }
        }

        //Handles a call to add a new car config file from a file
        private void CCMTAddCarConfigfromfilebutton_Click(object sender, EventArgs e)
        {
            //Prompt the user to see if they are sure
            DialogResult PromptResult = MessageBox.Show("This will load a new config file into the currently selected car directory\n\n" +
                                                        "The config file will be placed at the end of the list, so do not worry about filename numbers\n" +
                                                        "If a matching body config file is present it will be used\n" +
                                                        /* "Text or zip files can be used\n" + */
                                                        "\nAre you sure?", "Load Car Config File", MessageBoxButtons.YesNo);

            if (PromptResult == DialogResult.Yes)
            {
                //Open up a file browser
                OpenFileDialog ofd = new OpenFileDialog();
                // Set filter options and filter index.
                //ofd.Filter = "All Config Files (*.txt, *.zip)|*.txt;*.zip";
                ofd.Filter = "All Config Files (*.txt)|*.txt";  //Disable zip stuff until I have the brain power to process their contents
                ofd.FilterIndex = 1;

                // Show the dialog and get result.
                DialogResult result = ofd.ShowDialog();
                if (result == DialogResult.OK) // Test result.
                {
                    string fileresult = ofd.FileName;
                    string fileextension = Path.GetExtension(fileresult);
                    if (fileextension == ".txt")
                    {
                        //Do text file stuff

                        //Get the new file name
                        string temp = CCMTConfigslistBox.Items[CCMTConfigslistBox.Items.Count-1].ToString();
                        string Dest = ModManConfig.GetCarsDataDir() + "\\" + CCMTAvailableCarslistBox.SelectedItem;

                        //Does it contain a number?
                        if (temp.Any(char.IsDigit))
                        {
                            //Contains a number
                            //We need to figure out what that number is
                            int n;  //Variable to hold the file number
                            int.TryParse(new string(temp.Where(a => Char.IsDigit(a)).ToArray()), out n);    //Get the file number
                            n++;    //Inc it to the next one along

                            //Assemble the destination filename
                            Dest += "\\config" + n.ToString() + ".txt";
                        }
                        else
                        {
                            //Does not contain a number
                            //Only a single config file exists so the installed car config fill will be config1

                            //Assemble the destination filename
                            Dest += "\\config1.txt";
                        }
                        //Copy the file, /Do not overwrite the destination file if it already exists
                        System.IO.File.Copy(fileresult, Dest, false);
                    }
                    else    //Do Zip file stuff
                    {
                        string zipPath = fileresult;    //The source zip file
                        string extractPath = ModManConfig.GetCarsDataDir() + "\\" + CCMTAvailableCarslistBox.SelectedItem + "\\Unzip folder";   //Folder to extract too

                        ZipFile.ExtractToDirectory(zipPath, extractPath);
                        //Now we need to process the contents of the zip file
                    }
                    //Call the fuction to fill out the configs listbox
                    SummariseCarConfigFiles(CCMTAvailableCarslistBox.SelectedItem.ToString());
                }
            }
        }

        //Dialog box with a large text input field for pasting in a car data file as text
        public static DialogResult CarConfigTextInputBox(ref string OutputValue)
        {
            //Setup object
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = "Car Config File as text";
            label.Text = "Please copy and paste the Car Config File text into the box below";
            textBox.Text = OutputValue;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 10, 372, 13);
            textBox.Multiline = true;
            textBox.ScrollBars = ScrollBars.Vertical;
            textBox.SetBounds(12, 36, 672, 820);
            buttonOk.SetBounds(105, 872, 75, 23);
            buttonCancel.SetBounds(186, 872, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(696, 907);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(600, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();

            //Get text
            OutputValue = textBox.Text;
            return dialogResult;
        }

        //Handles a call to new car config file from a text input
        private void CCMTAddCarConfigfromtextbutton_Click(object sender, EventArgs e)
        {
            //Prompt the user to see if they are sure
            DialogResult PromptResult = MessageBox.Show("This will create a new car config file using the supplied text\nAre you sure?", "Load Car Data Text", MessageBoxButtons.YesNo);

            if (PromptResult == DialogResult.Yes)
            {
                //Local to Hold the return
                string CarDataText = "";
                //Call the dialog to get the result
                if (CarConfigTextInputBox(ref CarDataText) == DialogResult.OK)
                {
                    //If we returned with an OK

                    //Get the new file name
                    string temp = CCMTConfigslistBox.Items[CCMTConfigslistBox.Items.Count - 1].ToString();
                    int n = 1;  //Config file number, default of 1
                    //Does it contain a number?
                    if (temp.Any(char.IsDigit))
                    {
                        //Contains a number
                        //We need to figure out what that number is
                        int.TryParse(new string(temp.Where(a => Char.IsDigit(a)).ToArray()), out n);    //Get the file number
                        n++;    //Inc it to the next one along
                    }
                    //Else Does not contain a number so default of 1 applies

                    //Save text to the car config file
                    //Assemble path name for the car config file
                    string Dest = ModManConfig.GetCarsDataDir() + "\\" + CCMTAvailableCarslistBox.SelectedItem + "\\config" + n.ToString() + ".txt";
                    //Create a local file writer
                    StreamWriter writer = new StreamWriter(Dest);
                    writer.WriteLine(CarDataText);
                    //we are finished with the writer so close and bin it
                    writer.Close();
                    writer.Dispose();

                    //Call the fuction to fill out the configs listbox
                    SummariseCarConfigFiles(CCMTAvailableCarslistBox.SelectedItem.ToString());
                }
            }
        }

        //Handles a call to create a new car config file
        private void CCMTCreateNewCarConfigbutton_Click(object sender, EventArgs e)
        {
            //Get the index of the selected car data file
            int Index = CCMTConfigslistBox.SelectedIndex;
            //Check if a line has been selected
            if (Index > -1)
            {
                //Prompt the user to see if they are sure
                DialogResult PromptResult = MessageBox.Show("This will create a new car config and body config file for the selected car\n" +
                                                            "The files will be a copy of the selected cars config files\n" +
                                                            "\nAre you sure?", "Delete Car body Config File", MessageBoxButtons.YesNo);

                if (PromptResult == DialogResult.Yes)
                {
                    //Get the new file name
                    string temp = CCMTConfigslistBox.Items[CCMTConfigslistBox.Items.Count - 1].ToString();
                    int n = 1;  //Config file number, default of 1
                    //Does it contain a number?
                    if (temp.Any(char.IsDigit))
                    {
                        //Contains a number
                        //We need to figure out what that number is
                        int.TryParse(new string(temp.Where(a => Char.IsDigit(a)).ToArray()), out n);    //Get the file number
                        n++;    //Inc it to the next one along
                    }
                    //Else Does not contain a number so default of 1 applies

                    //Assemble path name for the car config file
                    string Dest = ModManConfig.GetCarsDataDir() + "\\" + CCMTAvailableCarslistBox.SelectedItem + "\\config" + n.ToString() + ".txt";
                    //Assemble path name for the car config file
                    string Source = ModManConfig.GetCarsDataDir() + "\\" + CCMTAvailableCarslistBox.SelectedItem + "\\" + CCMTConfigslistBox.SelectedItem + ".txt";
                    //Copy the file, /Do not overwrite the destination file if it already exists
                    System.IO.File.Copy(Source, Dest, false);

                    //Assemble path name for the car body config file
                    Dest = ModManConfig.GetCarsDataDir() + "\\" + CCMTAvailableCarslistBox.SelectedItem + "\\bodyconfig" + n.ToString() + ".txt";
                    //Assemble path name for the car body config file
                    Source = ModManConfig.GetCarsDataDir() + "\\" + CCMTAvailableCarslistBox.SelectedItem + "\\body" + CCMTConfigslistBox.SelectedItem + ".txt";
                    //Copy the file, /Do not overwrite the destination file if it already exists
                    System.IO.File.Copy(Source, Dest, false);
                }
                //Call the fuction to fill out the configs listbox
                SummariseCarConfigFiles(CCMTAvailableCarslistBox.SelectedItem.ToString());
            }
        }

        //Handles a call to remove a car config file
        private void CCMTDeleteCarConfigbutton_Click(object sender, EventArgs e)
        {
            //Get the index of the selected car data file
            int Index = CCMTConfigslistBox.SelectedIndex;
            //Check if a line has been selected
            if (Index > -1)
            {
                //Prompt the user to see if they are sure
                DialogResult PromptResult = MessageBox.Show("This will delete the selected car config file and\n matching body config file and cannot be undone\n" +
                                                            "\nAre you sure?", "Delete Car Config Files", MessageBoxButtons.YesNo);

                if (PromptResult == DialogResult.Yes)
                {
                    //Assemble path name for the car config file
                    string Target = ModManConfig.GetCarsDataDir() + "\\" + CCMTAvailableCarslistBox.SelectedItem + "\\" + CCMTConfigslistBox.SelectedItem + ".txt";
                    if (File.Exists(Target))  //Check if the config file exists
                    {
                        System.IO.File.Delete(Target);
                    }
                    //Assemble path name for the car config file
                    Target = ModManConfig.GetCarsDataDir() + "\\" + CCMTAvailableCarslistBox.SelectedItem + "\\" + CCMTConfigslistBox.SelectedItem + ".txt";
                    if (File.Exists(Target))  //Check if the config file exists
                    {
                        System.IO.File.Delete(Target);
                    }
                    //Assemble path name for the car body config file
                    Target = ModManConfig.GetCarsDataDir() + "\\" + CCMTAvailableCarslistBox.SelectedItem + "\\body" + CCMTConfigslistBox.SelectedItem + ".txt";
                    if (File.Exists(Target))  //Check if the config file exists
                    {
                        System.IO.File.Delete(Target);
                    }
                }

                //Need to reorder the remaining files

                //Call the fuction to fill out the configs listbox
                SummariseCarConfigFiles(CCMTAvailableCarslistBox.SelectedItem.ToString());
            }
        }

        //Handles a call to add a missing body config file
        private void CCMTAddMissingBodyConfigbutton_Click(object sender, EventArgs e)
        {
            //Get the index of the selected car data file
            int Index = CCMTConfigslistBox.SelectedIndex;
            //Check if a line has been selected
            if (Index > -1)
            {
                //Prompt the user to see if they are sure
                DialogResult PromptResult = MessageBox.Show("This will create a car body config file for the selected car config file\n" +
                                                            "The body config file will be a copy of the first body config file\n" + 
                                                            "\nAre you sure?", "Add Missing Car body Config File", MessageBoxButtons.YesNo);

                if (PromptResult == DialogResult.Yes)
                {
                    //Assemble path name for the car body config file
                    string Dest = ModManConfig.GetCarsDataDir() + "\\" + CCMTAvailableCarslistBox.SelectedItem + "\\body" + CCMTConfigslistBox.SelectedItem + ".txt";
                    //Assemble path name for the car body config file
                    string Source = ModManConfig.GetCarsDataDir() + "\\" + CCMTAvailableCarslistBox.SelectedItem + "\\bodyconfig.txt";
                    //Copy the file, /Do not overwrite the destination file if it already exists
                    System.IO.File.Copy(Source, Dest, false);
                }
                //Call the fuction to fill out the configs listbox
                SummariseCarConfigFiles(CCMTAvailableCarslistBox.SelectedItem.ToString());
            }
        }

        //Handles a call to remove a car body config
        private void CCMTDeleteBodyConfigbutton_Click(object sender, EventArgs e)
        {
            //Get the index of the selected car data file
            int Index = CCMTConfigslistBox.SelectedIndex;
            //Check if a line has been selected
            if (Index > -1)
            {
                //Prompt the user to see if they are sure
                DialogResult PromptResult = MessageBox.Show("This will delete the selected car body config file and cannot be undone\n" +
                                                            "\nAre you sure?", "Delete Car body Config File", MessageBoxButtons.YesNo);

                if (PromptResult == DialogResult.Yes)
                {
                    //Assemble path name for the car body config file
                    string Target = ModManConfig.GetCarsDataDir() + "\\" + CCMTAvailableCarslistBox.SelectedItem + "\\body" + CCMTConfigslistBox.SelectedItem + ".txt";
                    if (File.Exists(Target))  //Check if the config file exists
                    {
                        System.IO.File.Delete(Target);
                    }
                }
                //Call the fuction to fill out the configs listbox
                SummariseCarConfigFiles(CCMTAvailableCarslistBox.SelectedItem.ToString());
            }
        }
        
        #endregion Car List
    }
}
