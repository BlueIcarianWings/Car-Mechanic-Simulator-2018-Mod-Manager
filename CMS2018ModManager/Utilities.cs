using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;    //For streamReader, and directory info

namespace CMS2018ModManager
{
    public static class Utilities
    {
        //Gets the number from a config filename
        public static int GetNumberFromFilename(string filename)
        {
            int n = 0;  //Config file number, default of 1
            if (filename.Any(char.IsDigit))     //Does it contain a number?
            {
                //Contains a number
                //We need to figure out what that number is
                int.TryParse(new string(filename.Where(a => Char.IsDigit(a)).ToArray()), out n);    //Get the file number
                n++;    //Inc it to the next one along
            }
            //Else Does not contain a number so default of 0 applies

            return n;
        }

        //Determines if a folder or file is in an array, and returns it's index
        public static int ArrayContainsString(string[] StringList, string Search)
        {
            int index = -1;     //Return value, set to -1 as a default 'not found' value
            int counter = 0;    //Array counter

            foreach (string line in StringList)     //Loop through the array
            {
                if (line.Contains(Search))           //Look in line
                {
                    index = counter;                //Fill out return
                    break;                          //Exit the loop
                }
                counter++;
            }

            return index;
        }

        //Determines if a folder contains required folders or file
        public static bool[] RequiredFolderFileCheck(string[] StringList, string Location, bool Folder)
        {
            //StringList - Contains the items to look for
            //Location the folder to look in
            //Folder - True -> Looking for folders - False -> Looking for files
            //ReturnList - Contains flags indicate if a wanted folder / file was found

            string[] LocationList;            //Array to hold the list of items found in the search directory
            bool[] ReturnList = new bool[StringList.Length];  //Need to fill this out with -1's to the same size as StringList
            for (int i = 0; i < StringList.Length; i++)
            {
                ReturnList[i] = false;     //False is the not found value
            }


            //Get the files or folders in the current directory
            if (Folder)
            {
                LocationList = Directory.GetDirectories(Location);
                //LocationList = LocationList1;
            }
            else
            {
                LocationList = Directory.GetFiles(Location);
                //LocationList = LocationList2;
            }

            int LocationCounter = 0;    //Array counter
            int ListCounter = 0;        //Array counter

            //We will now search every result found in the folder, for every item in the list we are looking for
            foreach (string line in LocationList)     //Loop through the array of items found in the search location
            {
                foreach (string SearchList in StringList)   //Loop throught the list of items we are searching for
                {
                    if (line.Contains(SearchList))          //Look in line
                    {
                        ReturnList[ListCounter] = true; //Fill out return
                        break;                              //Exit the inner loop
                    }
                    ListCounter++;
                }
                ListCounter = 0;    //Reset before reuse in inner loop
                LocationCounter++;
            }

            return ReturnList;
        }

        //Determines if an array contains at least one true
        public static bool ArrayContainsTrue(bool[] CheckArr)
        {
            //Setup the return value
            bool ReturnValue = false;

            foreach (bool check in CheckArr)
            {
                //
                if (check)
                {
                    ReturnValue = true;
                    break;
                }
            }

            return ReturnValue;
        }

    }
}
