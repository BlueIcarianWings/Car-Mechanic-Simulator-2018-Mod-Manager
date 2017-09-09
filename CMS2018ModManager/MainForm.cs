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
using System.Threading;

namespace CMS2018ModManager
{
    public partial class MainForm : Form
    {
        private string ModManVersion = "0.2.1";     //Version constant for ModManager
        private string GameVersion = "1.2.7";       //Version constant for the game
        //Lists to hold lists of mods   //Should probably move this stuff out into it's own class with the instal mod functions
        List<string> CarsModList = new List<string>();       //Holds the list of cars
        List<string> DialsModList = new List<string>();      //Holds the list of dials
        //Currently running off the GUI
        //Image ImageObject = null;

        //Class object for class that does the acutal mod managing stuff    //here so it's scope is within the form object  //should move the config stuff out at somepoint
        ConfigFile ModManConfig;

        public MainForm()
        {
            //Setup the form controls
            InitializeComponent();

            //Setup the class that does the acutal mod managing stuff
            ModManConfig = new ConfigFile();
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

            //Populate the Available Cars list box
            PopulateCarsAvailableList();

            //Populate the mod cars list GUI listbox
            PopulateInstalledModCarsList();

            //Populate the mod cars list GUI listbox
            PopulateInstalledModDialsList();
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

        //Handles a car being selected
        private void CCMTAvailableCarslistBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Get the car folder name
            string CarConfigFolder = CCMTAvailableCarslistBox.SelectedItem.ToString();
            //Call the fuction to fill out the configs listbox
            SummariseCarConfigFiles(CarConfigFolder);

            //Empty out the car picture box
            CCMTCarPicturepictureBox.Image = null;
            //Set the picture
            //Assemble the image filepath
            string ImageFilepath = ModManConfig.GetCarsDataDir() + "\\" + CarConfigFolder + "\\PartThumb\\car_" + CarConfigFolder + "-car_" + CarConfigFolder + ".png";
            if (File.Exists(ImageFilepath))
            {
                //Create the image
                Image ImageObject = Image.FromFile(ImageFilepath);
                //Fill out the picture box
                CCMTCarPicturepictureBox.Image = ImageObject;
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
            DialogResult PromptResult = MessageBox.Show("This will load a new car config file into the currently selected car directory\n\n" +
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

                        //Get the new file name
                        int n = Utilities.GetNumberFromFilename(CCMTConfigslistBox.Items[CCMTConfigslistBox.Items.Count - 1].ToString());
                        //We need to up it by 1 to get the next number
                        n++;
                        //Assemble the destination filename
                        Dest += "\\config" + n.ToString() + ".txt";

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
        
        //Handles a call to create a new car config file from a text input
        private void CCMTAddCarConfigfromtextbutton_Click(object sender, EventArgs e)
        {
            //Prompt the user to see if they are sure
            DialogResult PromptResult = MessageBox.Show("This will create a new car config file using the supplied text\nAre you sure?", "Load car config text", MessageBoxButtons.YesNo);

            if (PromptResult == DialogResult.Yes)
            {
                //Local to Hold the return
                string CarDataText = "";
                //Call the dialog to get the result
                if (CarConfigTextInputBox(ref CarDataText) == DialogResult.OK)
                {
                    //If we returned with an OK

                    //Get the new file name
                    int n = Utilities.GetNumberFromFilename(CCMTConfigslistBox.Items[CCMTConfigslistBox.Items.Count - 1].ToString());
                    //We need to up it by 1 to get the next number
                    n++;

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
                    int n = Utilities.GetNumberFromFilename(CCMTConfigslistBox.Items[CCMTConfigslistBox.Items.Count - 1].ToString());
                    //We need to up it by 1 to get the next number
                    n++;

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

        //Handles a call to create a new body config file   //NOT USED
        private void CCMTCreateNewBodyConfigbutton_Click(object sender, EventArgs e)
        {
            //Here as it matches the car config GUI buttons and methods
            //However I can't think of a need as if a new car is being setup
            //the create car config button will also create the body config file
            
            //So I've hid the button for now
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

        //Handles a call to create a new body config file
        private void CCMTAddBodyConfigFromFilebutton_Click(object sender, EventArgs e)
        {
            //Prompt the user to see if they are sure
            DialogResult PromptResult = MessageBox.Show("This will load a new body config file into the currently selected car directory,\n" +
                                                        "for currently selected car config\n" +
                                                        "If a body config files exists it will be overwritten\n" +
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
                        string temp = CCMTConfigslistBox.SelectedItem.ToString();
                        string Dest = ModManConfig.GetCarsDataDir() + "\\" + CCMTAvailableCarslistBox.SelectedItem;

                        //Get the config file number
                        int n = Utilities.GetNumberFromFilename(temp);
                        //Assemble the destination filename
                        Dest += "\\bodyconfig" + n.ToString() + ".txt";

                        //Copy the file, /Will overwrite the destination file if it already exists
                        System.IO.File.Copy(fileresult, Dest, true);
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

        //Handles a call to create a new body config file from a text input
        private void CCMTAddBodyConfigFromTextbutton_Click(object sender, EventArgs e)
        {
            //Prompt the user to see if they are sure
            DialogResult PromptResult = MessageBox.Show("This will load a new body config file into the currently selected car directory,\n" +
                                                        "for currently selected car config\n" +
                                                        "If a body config files exists it will be overwritten\n" +
                                                        /* "Text or zip files can be used\n" + */
                                                        "\nAre you sure?", "Load Car Config File", MessageBoxButtons.YesNo);

            if (PromptResult == DialogResult.Yes)
            {
                //Local to Hold the return
                string CarDataText = "";
                //Call the dialog to get the result
                if (CarConfigTextInputBox(ref CarDataText) == DialogResult.OK)
                {
                    //If we returned with an OK

                    //Get the new file name
                    string temp = CCMTConfigslistBox.SelectedItem.ToString();
                    string Dest = ModManConfig.GetCarsDataDir() + "\\" + CCMTAvailableCarslistBox.SelectedItem;

                    //Get the config file number
                    int n = Utilities.GetNumberFromFilename(temp);
                    //Assemble the destination filename
                    Dest += "\\bodyconfig" + n.ToString() + ".txt";

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

                    //Check if the file does not aready exist
                    if (!File.Exists(Dest))
                    {
                        //Copy the file, /Do not overwrite the destination file if it already exists
                        System.IO.File.Copy(Source, Dest, false);

                        //Call the fuction to fill out the configs listbox
                        SummariseCarConfigFiles(CCMTAvailableCarslistBox.SelectedItem.ToString());
                    }
                }
            }
        }
        #endregion Car List

        #region Install Mod Cars

        //Reads the mod cars list file and populates the GUI listbox
        private void PopulateInstalledModCarsList()
        {
            //Assemble the file path to the mod car list file
            string ModCarsList = ModManConfig.GetCarsDataDir() + "\\ModCarsList.txt";
            //Check if the file exists
            if (File.Exists(ModCarsList))
            {
                //Open and read the mod cars list
                //create a streamReader to accses the config file
                StreamReader reader = new StreamReader(ModCarsList);
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
                    else if(line.StartsWith("["))   //Header and notes row start with [
                    {
                        //Header line nothing to do
                    }
                    else
                    {
                        //Add the mod car to the list
                        //if (line != "") //I've messed something up and it's adding blank lines somewhere  //Think it was duplication in copy/pasting the this function for the dial side
                        //{
                            CarsModList.Add(line);
                        //}
                    }
                }

                //we are finished with the reader so close and bin it
                reader.Close();
                reader.Dispose();
            }
            //No else as it(the mod file list) will be created when the first car is added

            //Update the GUI
            UpdateModCarsGUI();
        }

        //Handles a car mod being selected
        private void IMCModCarsInstalledlistBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Get the car folder name
            string CarConfigFolder = IMCModCarsInstalledlistBox.SelectedItem.ToString();

            //Empty out the car picture box
            IMCCarPicturepictureBox.Image = null;
            //Set the picture
            //Assemble the image filepath
            string ImageFilepath = ModManConfig.GetCarsDataDir() + "\\" + CarConfigFolder + "\\PartThumb\\car_" + CarConfigFolder + "-car_" + CarConfigFolder + ".png";
            if (File.Exists(ImageFilepath))
            {
                //Create the image
                Image ImageObject = Image.FromFile(ImageFilepath);
                //Fill out the picture box
                IMCCarPicturepictureBox.Image = ImageObject;
            }
        }

        //Writes the mod cars list file
        private void WriteModCarsListFile()
        {

            try         //I think some people have problems with permissions, this will help 'skip-over' the file creation step
            {
                string Dest = ModManConfig.GetCarsDataDir() + "\\ModCarsList.txt";
                using (StreamWriter writer = new StreamWriter(Dest))
                {
                    foreach (string Line in CarsModList)
                    {
                        if (CarsModList.Count > 1)
                        {
                            writer.WriteLine("\n" + Line);
                        }
                        else
                        {
                            writer.WriteLine(Line);
                        }
                    }
                }
            }
            catch (Exception)
            {
                //Explain to the user
                MessageBox.Show("There was a problem writing the Mod Cars List file.\nThis is probably access permissons related?\nThis may affect other file writes.\n", "File creation problem");
            }
        }

        //Populate Mod Cars GUI listbox
        private void UpdateModCarsGUI()
        {
            //Clear out the existing contents
            IMCModCarsInstalledlistBox.Items.Clear();

            //Fill out the GUI from the list of mod cars
            foreach (string ModCar in CarsModList)
            {
                IMCModCarsInstalledlistBox.Items.Add(ModCar);
            }

            //Update the counter label
            IMCInstalledModCarsCountlabel.Text = CarsModList.Count() + " Mod Cars installed";
        }

        //Installs a car into the selected directory
        private void InstallModCar(string Source, string Target)
        {
            //string Dest = ModManConfig.GetCarsDataDir() + "\\" + Path.GetFileName(Source);
            string Dest = ModManConfig.GetCarsDataDir() + "\\" + Target;
            //First check the top level files
            string[] FileWantedList = { "\\config.txt", "\\bodyconfig.txt", "\\name.txt", "\\parts.txt"};  //Items to work through the .cms needs different handling

            foreach (string Entry in FileWantedList)
            {
                string FileFrom = Source + Entry;
                String FileTo = Dest + Entry;
                if (File.Exists(FileFrom))
                {
                    //Need to create the directory if it doesn't exist
                    System.IO.Directory.CreateDirectory(Dest);
                    System.IO.File.Copy(FileFrom, FileTo, true);
                }
            }

            //Special handling for the .cms file
            string[] FileList = Directory.GetFiles(Source);   //Get a list of the files in the source dir
            foreach(string Entry in FileList)
            {
                string Extension = Path.GetExtension(Entry);
                if(Extension == ".cms")
                {
                    String FileTo = Dest + "\\" + Path.GetFileName(Entry);
                    System.IO.File.Copy(Entry, FileTo, true);
                    break;
                }
            }

            string[] FolderWantedList = { "\\Liveries", "\\PartThumb", "\\RustMaps" };
            foreach (string Entry in FolderWantedList)
            {
                string FileFrom = Source + Entry;
                String FileTo = Dest + Entry;
                if (Directory.Exists(FileFrom))
                {
                    ModManConfig.DirectoryCopy(FileFrom, FileTo, false);
                }
            }

            //Need to update the list of mods cars
            CarsModList.Add(Target);
            //Sort the list alphabetically
            CarsModList.Sort();
            //Update the GUI
            UpdateModCarsGUI();
            //Write the updated file;
            WriteModCarsListFile();
        }

        //Handles a call to install a mod car
        private void IMCInstallNewModCarbutton_Click(object sender, EventArgs e)
        {
            //Will need to unzip the zip file to a temp folder, inspect it, and copy over as needed

            //Prompt the user to see if they are sure
            DialogResult PromptResult = MessageBox.Show("This will install a new mod car (and dial if present) from a zip file\n\n" +
                                                        "\nAre you sure?", "Install New Mod Car", MessageBoxButtons.YesNo);

            if (PromptResult == DialogResult.Yes)
            {
                //Open up a file browser
                OpenFileDialog ofd = new OpenFileDialog();
                // Set filter options and filter index.
                ofd.Filter = "All Config Files (*.zip)|*.zip";  //Limit to zip files
                ofd.FilterIndex = 1;

                // Show the dialog and get result.
                DialogResult result = ofd.ShowDialog();
                if (result == DialogResult.OK) // Test result.
                {
                    string fileresult = ofd.FileName;

                    //Do Zip file stuff
                    string zipPath = fileresult;         //The source zip file
                    string extractPath = ModManConfig.GetCarsDataDir() + "\\Unzip folder";     //Folder to extract too

                    //Check is the "Unzip folder" already exists and delete it if it does
                    if (Directory.Exists(extractPath))
                    {
                        Directory.Delete(extractPath, true);
                    }

                    ZipFile.ExtractToDirectory(zipPath, extractPath);       //Extract the zip file
                                                                            //Now we need to process the contents of the zip file

                    //check for files and folders
                    //if a single folder then inspect within it
                    //when files are found, look for the required files
                    //when we have found the right contents, make a note of the folder and check if it's already installed
                    //if so check with the user before removing the old one and installing the new one
                    //install if all is ok

                    //Set where we start the search
                    string CurrentSearchDir = extractPath;
                    //Get the files and folders in the current directory
                    string[] DirList = Directory.GetDirectories(CurrentSearchDir);
                    string[] FileList = Directory.GetFiles(CurrentSearchDir);
                    //Loop controller
                    bool KeepSearching = true;
                    //Strings to hold folder we think contain what we are searching for
                    string CarModDir = null;
                    string DialModDir = null;
                    //Are we looking for a dial
                    bool DialSearch = false;

                    //Look for car mod folder, keep an eye out for the dials too
                    while (KeepSearching)
                    {
                        if ((DirList.Length == 1) && (FileList.Length == 0))
                        {
                            //Contains a single folder and no files so move into it
                            //Update the dir we are searching in
                            CurrentSearchDir = DirList[0];
                            //Clear out the existing search and check for files and folders again
                            DirList = null;
                            FileList = null;
                            DirList = Directory.GetDirectories(CurrentSearchDir);
                            FileList = Directory.GetFiles(CurrentSearchDir);
                        }
                        else if ((DirList.Length > 1) && (FileList.Length == 0))
                        {
                            //Contains 2 or more folders
                            //This could be a "Cars" and "Dials" folder setup

                            //Look for cars
                            int Index = Utilities.ArrayContainsString(DirList, "Cars");
                            if (Index > -1)
                            {
                                CarModDir = DirList[Index];     //Save the possible car mod dir
                                CurrentSearchDir = CarModDir;   //Update the search path
                            }
                            //Look for dials
                            Index = Utilities.ArrayContainsString(DirList, "Dials");
                            if (Index > -1)
                            {
                                DialModDir = DirList[Index];     //Save the possible dial mod dir
                            }

                            //Clear out the existing search and check for files and folders again
                            DirList = null;
                            FileList = null;
                            DirList = Directory.GetDirectories(CurrentSearchDir);
                            FileList = Directory.GetFiles(CurrentSearchDir);
                        }
                        else if ((DirList.Length > 0) && (FileList.Length >= 3))
                        {
                            //Contains at least the three files need at least more than than the single dir needed

                            //We may reach this point with having filled out the CarModDir (if this wasn't a Car and Dials folder)
                            //Fill it out witht the current if not already filled out
                            CarModDir = CurrentSearchDir;   //Update the search path

                            //Are we looking for Car or Dial files?
                            if (!DialSearch)     //If Not currently in Dial search mode (ie looking at cars)
                            {
                                //Investigate the folder found to contain files (check for minimum folders and files)
                                //Folder check
                                string[] FolderWantedList = { "PartThumb" };    //Optional "Liveries" "RustMaps"
                                bool[] FolderCheckList = Utilities.RequiredFolderFileCheck(FolderWantedList, CarModDir, true);
                                //File check
                                string[] FileWantedList = { "\\config.txt", "\\name.txt", ".cms" };     //Optional "bodyconfig.txt", "parts.txt"
                                bool[] FileCheckList = Utilities.RequiredFolderFileCheck(FileWantedList, CarModDir, false);

                                //Check if any of the required bits came back false
                                if ((Utilities.ArrayContainsTrue(FolderCheckList)) && (Utilities.ArrayContainsTrue(FileCheckList)))
                                {
                                    //We have a folder with enough folders / files in that we think it's a car so install it
                                    string Target = Path.GetFileName(CarModDir);

                                    //For cars directly in the zip we need decide what name to put the folder in
                                    if (CarModDir == extractPath)
                                    {
                                        //Car mod contents is directly in the zip file
                                        //This means we need to use the zip file name as the folder name
                                        Target = Path.GetFileNameWithoutExtension(fileresult);
                                    }

                                    InstallModCar(CarModDir, Target);
                                }

                                //------------------------------
                                //Should we also look for a dial?
                                if (DialModDir != null)
                                {
                                    //We saved a possible dial mod dir, lets go and look at it
                                    CurrentSearchDir = DialModDir;
                                    DialSearch = true;

                                    //Clear out the existing search and check for files and folders again
                                    DirList = null;
                                    FileList = null;
                                    DirList = Directory.GetDirectories(CurrentSearchDir);
                                    FileList = Directory.GetFiles(CurrentSearchDir);
                                }
                                else
                                {
                                    //No dial to look for car found time to exit
                                    KeepSearching = false;
                                }
                                //------------------------------
                            }
                            //else    //Else we must be looking for a dial so process it's contents
                            //{
                                //Dials contain no subfolderso will never get in here
                            //}                            
                        }
                        else if((DirList.Length == 0) && (FileList.Length == 3))    //Dial folder contains no sub folders
                        {
                            //Investigate the folder found to contain files (check for minimum folders and files)
                            if (DialSearch)
                            {
                                //We may reach this point with having filled out the CarModDir (if this wasn't a Car and Dials folder)
                                //Fill it out witht eh current if not already filled out
                                CarModDir = CurrentSearchDir;   //Update the search path

                                //File check
                                string[] FileWantedList = { "\\config.txt", ".png" };
                                bool[] FileCheckList = Utilities.RequiredFolderFileCheck(FileWantedList, CarModDir, false);

                                //Check if any of the required bits came back false
                                if (Utilities.ArrayContainsTrue(FileCheckList))
                                {
                                    //We have a folder with enough files in that we think it's a dial so install it
                                    string Target = Path.GetFileName(CarModDir);
                                    InstallModDial(CarModDir, Target);
                                }
                            }
                            //No dial to look for car found time to exit
                            KeepSearching = false;
                        }
                        else
                        {
                            //No files or folder present, time to exit
                            KeepSearching = false;
                        }
                    }

                    //Delete the unzip folder
                    System.IO.Directory.Delete(extractPath, true);
                }
            }
        }

        //Handles a call to delete a mod car
        private void IMCRemoveSelectedCarModbutton_Click(object sender, EventArgs e)
        {
            //Get the index of the selected car data file
            int Index = IMCModCarsInstalledlistBox.SelectedIndex;
            //Check if a line has been selected
            if (Index > -1)
            {
                //Prompt the user to see if they are sure
                DialogResult PromptResult = MessageBox.Show("This will delete the selected car mod and cannot be undone\n" +
                                                            "\nAre you sure?", "Delete Car Mod", MessageBoxButtons.YesNo);

                if (PromptResult == DialogResult.Yes)
                {
                    //Need to unload  the picture to release it before we can delete it
                    //Empty out the car picture box
                    if (IMCCarPicturepictureBox.Image != null)
                    {
                        IMCCarPicturepictureBox.Image.Dispose();
                    }
                    IMCCarPicturepictureBox.Image = null;

                    //Get the folder path to remove
                    string DeletePath = ModManConfig.GetCarsDataDir() + "\\" + IMCModCarsInstalledlistBox.SelectedItem.ToString();

                    //I HATE doing this however it (the software) seems to need to keep the image file open while it's displaying it in the GUI
                    //but it won't wait for the image to released before it tries to delete the image file.
                    Thread.Sleep(40);    //In milliseconds

                    //Delete the selected car mod directory
                    System.IO.Directory.Delete(DeletePath, true);

                    //Remove car from list of car mods
                    CarsModList.RemoveAt(CarsModList.IndexOf(IMCModCarsInstalledlistBox.SelectedItem.ToString()));
                    //Update the Mod Cars list file
                    WriteModCarsListFile();
                    //Update the GUI
                    UpdateModCarsGUI();
                }
            }
        }

        #endregion

        #region Install Mod Dials

        //Reads the mod cars list file and populates the GUI
        private void PopulateInstalledModDialsList()
        {
            //Assemble the file path to the mod car list file
            string ModDialsList = ModManConfig.GetDialsDataDir() + "\\ModDialsList.txt";
            //Check if the file exists
            if (File.Exists(ModDialsList))
            {
                //Open and read the mod cars list
                //create a streamReader to accses the config file
                StreamReader reader = new StreamReader(ModDialsList);
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
                    else if (line.StartsWith("["))   //Header and notes row start with [
                    {
                        //Header line nothing to do
                    }
                    else
                    {
                        //Add the mod car to the list
                        DialsModList.Add(line);
                    }
                }

                //we are finished with the reader so close and bin it
                reader.Close();
                reader.Dispose();
            }
            //No else as it(the mod file list) will be created when the first car is added

            //Update the GUI
            UpdateModDialsGUI();
        }

        //Writes the mod dials list file
        private void WriteModDialsListFile()
        {

            try         //I think some people have problems with permissions, this will help 'skip-over' the file creation step
            {
                string Dest = ModManConfig.GetDialsDataDir() + "\\ModDialsList.txt";
                using (StreamWriter writer = new StreamWriter(Dest))
                {
                    foreach (string Line in CarsModList)
                    {
                        if (DialsModList.Count > 1)
                        {
                            writer.WriteLine("\n" + Line);
                        }
                        else
                        {
                            writer.WriteLine(Line);
                        }
                    }
                }
            }
            catch (Exception)
            {
                //Explain to the user
                MessageBox.Show("There was a problem writing the Mod Dials List file.\nThis is probably access permissons related?\nThis may affect other file writes.\n", "File creation problem");
            }
        }

        //Populate Mod Cars GUI listbox
        private void UpdateModDialsGUI()
        {
            //Clear out the existing contents
            IMDModDialsInstalledlistBox.Items.Clear();

            //Fill out the GUI from the list of mod cars
            foreach (string ModDial in DialsModList)
            {
                IMDModDialsInstalledlistBox.Items.Add(ModDial);
            }

            //Update the counter label
            IMDInstalledModDialsCountlabel.Text = CarsModList.Count() + " Mod Dials installed";
        }

        //Installs a car into the selected directory    //needs a recheck
        private void InstallModDial(string Source, string Target)
        {
            //string Dest = ModManConfig.GetDialsDataDir() + Path.GetFileName(Source);
            string Dest = ModManConfig.GetDialsDataDir() + Target;
            //First check the top level files
            string[] FileWantedList = { "\\config.txt" };  //Items to work through the .cms needs different handling

            //Need to create the directory if it doesn't exist
            System.IO.Directory.CreateDirectory(Dest);

            foreach (string Entry in FileWantedList)
            {
                string FileFrom = Source + Entry;
                String FileTo = Dest + Entry;
                if (File.Exists(FileFrom))
                {
                    //Need to create the directory if it doesn't exist
                    System.IO.Directory.CreateDirectory(Dest);
                    System.IO.File.Copy(FileFrom, FileTo, true);
                }
            }

            //Special handling for the .png files
            //There are two png files required and they listed in the config.txt file
            //For now a simply copy over all .png files
            string[] FileList = Directory.GetFiles(Source);   //Get a list of the files in the source dir
            foreach (string Entry in FileList)
            {
                string Extension = Path.GetExtension(Entry);
                if (Extension == ".png")
                {
                    String FileTo = Dest + "\\" + Path.GetFileName(Entry);
                    System.IO.File.Copy(Entry, FileTo, true);
                }
            }
            //Need to update the list of mods dials
            DialsModList.Add(Target);
            //Sort the list alphabetically
            DialsModList.Sort();
            //Update the GUI
            UpdateModDialsGUI();
            //Write the updated file;
            WriteModDialsListFile();
        }

        //Handles a dial mod being selected
        private void IMDModDialsInstalledlistBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Nothing to do here until I display the dial images
        }

        //Handles a call to install a mod dial
        private void IMDInstallNewModDialbutton_Click(object sender, EventArgs e)
        {
            //A LOT OF DUPLICATION with the car mod side, should really refactor this into a single function
            //Will need to unzip the zip file to a temp folder, inspect it, and copy over as needed

            //Prompt the user to see if they are sure
            DialogResult PromptResult = MessageBox.Show("This will install a new mod dial from a zip file\n\n" +
                                                        "\nAre you sure?", "Install New Mod Dial", MessageBoxButtons.YesNo);

            if (PromptResult == DialogResult.Yes)
            {
                //Open up a file browser
                OpenFileDialog ofd = new OpenFileDialog();
                // Set filter options and filter index.
                ofd.Filter = "All Config Files (*.zip)|*.zip";  //Limit to zip files
                ofd.FilterIndex = 1;

                // Show the dialog and get result.
                DialogResult result = ofd.ShowDialog();
                if (result == DialogResult.OK) // Test result.
                {
                    string fileresult = ofd.FileName;

                    //Do Zip file stuff
                    string zipPath = fileresult;         //The source zip file
                    string extractPath = ModManConfig.GetDialsDataDir() + "\\Unzip folder";     //Folder to extract too

                    //Check is the "Unzip folder" already exists and delete it if it does
                    if (Directory.Exists(extractPath))
                    {
                        Directory.Delete(extractPath, true);
                    }

                    ZipFile.ExtractToDirectory(zipPath, extractPath);       //Extract the zip file
                                                                            //Now we need to process the contents of the zip file

                    //check for files and folders
                    //if a single folder then inspect within it
                    //when files are found, look for the required files
                    //when we have found the right contents, make a note of the folder and check if it's already installed
                    //if so check with the user before removing the old one and installing the new one
                    //install if all is ok

                    //Set where we start the search
                    string CurrentSearchDir = extractPath;
                    //Get the files and folders in the current directory
                    string[] DirList = Directory.GetDirectories(CurrentSearchDir);
                    string[] FileList = Directory.GetFiles(CurrentSearchDir);
                    //Loop controller
                    bool KeepSearching = true;
                    //Strings to hold folder we think contain what we are searching for
                    string DialModDir = null;

                    //Look for car mod folder, keep an eye out for the dials too
                    while (KeepSearching)
                    {
                        if ((DirList.Length == 1) && (FileList.Length == 0))
                        {
                            //Contains a single folder and no files so move into it
                            //Update the dir we are searching in
                            CurrentSearchDir = DirList[0];
                            //Clear out the existing search and check for files and folders again
                            DirList = null;
                            FileList = null;
                            DirList = Directory.GetDirectories(CurrentSearchDir);
                            FileList = Directory.GetFiles(CurrentSearchDir);
                        }
                        else if ((DirList.Length == 0) && (FileList.Length == 3))    //Dial folder contains no sub folders
                        {
                            //Investigate the folder found to contain files (check for minimum folders and files)
                            //We may reach this point with having filled out the CarModDir (if this wasn't a Car and Dials folder)
                            //Fill it out witht eh current if not already filled out
                            DialModDir = CurrentSearchDir;   //Update the search path

                            //File check
                            string[] FileWantedList = { "\\config.txt", ".png" };
                            bool[] FileCheckList = Utilities.RequiredFolderFileCheck(FileWantedList, DialModDir, false);

                            //Check if any of the required bits came back false
                            if (Utilities.ArrayContainsTrue(FileCheckList))
                            {
                                //We have a folder with enough files in that we think it's a dial so install it
                                string Target = Path.GetFileName(DialModDir);

                                //For cars directly in the zip we need decide what name to put the folder in
                                if (DialModDir == extractPath)
                                {
                                    //Car mod contents is directly in the zip file
                                    //This means we need to use the zip file name as the folder name
                                    Target = Path.GetFileNameWithoutExtension(fileresult);
                                }

                                InstallModDial(DialModDir, Target);
                            }

                            //Dial found time to exit
                            KeepSearching = false;
                        }
                        else
                        {
                            //No files or folder present, time to exit
                            KeepSearching = false;
                        }
                    }

                    //Delete the unzip folder
                    System.IO.Directory.Delete(extractPath, true);
                }
            }
        }

        //Handles a call to delete a mod dial
        private void IMDRemoveSelectedDialModbutton_Click(object sender, EventArgs e)
        {
            //Get the index of the selected car data file
            int Index = IMDModDialsInstalledlistBox.SelectedIndex;
            //Check if a line has been selected
            if (Index > -1)
            {
                //Prompt the user to see if they are sure
                DialogResult PromptResult = MessageBox.Show("This will delete the selected dial mod and cannot be undone\n" +
                                                            "\nAre you sure?", "Delete Dial Mod", MessageBoxButtons.YesNo);

                if (PromptResult == DialogResult.Yes)
                {
                    //Need to unload  the picture to release it before we can delete it //Picture stuff is N/A at the moment
                    //Empty out the car picture box
                    //IMCCarPicturepictureBox.Image.Dispose();
                    //IMCCarPicturepictureBox.Image = null;

                    //Get the folder path to remove
                    string DeletePath = ModManConfig.GetDialsDataDir() + "\\" + IMDModDialsInstalledlistBox.SelectedItem.ToString();

                    //I HATE doing this however it (the software) seems to need to keep the image file open while it's displaying it in the GUI
                    //but it won't wait for the image to released before it tries to delete the image file.
                    //Thread.Sleep(40);    //In milliseconds

                    //Delete the selected car mod directory
                    System.IO.Directory.Delete(DeletePath, true);

                    //Remove car from list of car mods
                    DialsModList.RemoveAt(DialsModList.IndexOf(IMDModDialsInstalledlistBox.SelectedItem.ToString()));
                    //Update the Mod Cars list file
                    WriteModDialsListFile();
                    //Update the GUI
                    UpdateModDialsGUI();
                }
            }
        }
        #endregion
    }
}
