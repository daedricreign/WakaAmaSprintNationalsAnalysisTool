﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WakaAmaSprintNationalsAnalysisTool
{
    class Program
    {
        
        static void Main(string[] args)
        {
            Dictionary<int, Team> teamDictionary = new Dictionary<int, Team>();

            //---------------Pick directory---------------
            //Get list of directorys
            IEnumerable<string> DirectoryList = SearchCurrentDirectory();

            //Is there no folders
            if (DirectoryList.Count() <= 0)
            {
                Console.WriteLine("Error no folders to pick!");
                Console.Read();
                return;
            }

            //Is there one folder
            if (DirectoryList.Count() == 1)
            {
                Console.WriteLine("Auto picking folder " + DirectoryList.ElementAt(0));
                ReadDirectory(DirectoryList.ElementAt(0), teamDictionary);
            }
            else
            {
                //List them for the user
                int i = 0;
                foreach (var item in DirectoryList)
                {
                    Console.WriteLine(i + " " + item);
                    i++;
                }
                //Get the user input
                int pick = ReadInt("Pick directory number : ", i, 0);
                //Open all csv file in the directory
                ReadDirectory(DirectoryList.ElementAt(pick), teamDictionary);
            }

            //---------------Show data---------------
            //Got thow sorted data
            using (StreamWriter outputFile = new StreamWriter(Directory.GetCurrentDirectory() + @"\output.csv"))
            {
                foreach (KeyValuePair<int, Team> entry in SortTeamDictionary( teamDictionary ))
                {
                    Console.WriteLine(entry.Value.Name.ToString().PadRight(30) + ":" + entry.Value.Scores);
                    outputFile.WriteLine(entry.Value.Name.ToString() + "," + entry.Value.Scores);
                }
            }

            //Stop the console application from exiting immediately
            Console.Read();
        }

        /// <summary>
        /// Read the contents of a file and then write into the database
        /// </summary>
        /// <param name="Directory"></param>
        static void ReadFile(string Directory , Dictionary<int, Team> teamDictionary)
        {
            using (StreamReader sr = File.OpenText(Directory))
            {
                sr.ReadLine();
                string currentLine = "";
                int lineIndex = 1;
                //Read untill the end of the file
                while ((currentLine = sr.ReadLine()) != null)
                {
                    //Is this line valid
                    string lineValid = "";

                    //Split line by ','
                    string[] currentLineArray = currentLine.Split(',');

                    //Extract data
                    int ID;
                    if (!Int32.TryParse(currentLineArray[1], out ID)) lineValid = "Invalid ID at ";
                    string Name;
                    Name = currentLineArray[3];
                    int Place;
                    if (!Int32.TryParse(currentLineArray[0], out Place)) lineValid = "Invalid Score at ";

                    //Make the temp team class
                    Team newTeam = new Team();
                    newTeam.ID = ID;
                    newTeam.Name = Name;

                    //Add the team  to the databace
                    if (lineValid == "") AddPlacing(newTeam, Place, teamDictionary);
                    if (!(lineValid == "")) Console.WriteLine(lineValid + (lineIndex + 1));

                    //Move on to next line
                    lineIndex++;
                }
            }
        }

        /// <summary>
        /// Add or update the score of a team
        /// </summary>
        /// <param name="newTeam"></param>
        /// <param name="Place"></param>
        static void AddPlacing(Team newTeam, int Place , Dictionary<int, Team> teamDictionary)
        {
            //Add them to dictionary if needed
            if (!teamDictionary.ContainsKey(newTeam.ID)) {
                teamDictionary.Add(newTeam.ID, newTeam);
                teamDictionary[newTeam.ID].Scores = 0;
            }
            //Give them the score
            teamDictionary[newTeam.ID].Scores += GetScore(Place);
        }

        /// <summary>
        /// Returns a IOrderedEnumerable that is sorted by score
        /// </summary>
        /// <returns></returns>
        static IOrderedEnumerable<KeyValuePair<int, Team>> SortTeamDictionary(Dictionary<int, Team> teamDictionary)
        {
            //Sort the Dictionary and return the result
            return teamDictionary.OrderByDescending(x => x.Value.Scores);
        }

        /// <summary>
        /// List all sub sirectory in the current directory
        /// </summary>
        /// <returns></returns>
        static IEnumerable<string> SearchCurrentDirectory()
        {
            //List all directories in the folder were the exe is ran
            return Directory.EnumerateDirectories(Directory.GetCurrentDirectory());
        }

        /// <summary>
        /// Add all CSV data to teamDictionary in a Directory
        /// </summary>
        /// <param name="DirectoryStr"></param>
        static void ReadDirectory(string DirectoryStr , Dictionary<int, Team> teamDictionary)
        {
            //List all files
            IEnumerable<string> temp = Directory.EnumerateFiles(DirectoryStr);

            //Go thow all files
            foreach (var item in temp)
            {
                //Ignore all filles that are not CSV
                if (item.EndsWith(".lif") && item.Contains("Final"))
                    ReadFile(item, teamDictionary);
            }
        }

        /// <summary>
        /// Reads an int in from Console
        /// </summary>
        /// <param name="Question"></param>
        /// <param name="Max"></param>
        /// <param name="Min"></param>
        /// <returns></returns>
        static int ReadInt(string Question , int Max = Int32.MaxValue, int Min = Int32.MinValue)
        {
            int input;
            while (true){
                try
                {
                    Console.WriteLine(Question);
                    input = Int32.Parse(Console.ReadLine());
                    if ( input >= Min && input < Max )
                        return input;
                }
                catch (System.FormatException){}
                Console.WriteLine("Invalid input please enter an interger between " + Min + " and " + (Max - 1));
            }
        }

        /// <summary>
        /// Get a Score from a Place
        /// </summary>
        /// <param name="Place"></param>
        static int GetScore(int Place)
        {
            //Less then 1
            if (Place <= 0) return 0;
            //1 to 8
            if (Place <= 8) return 9 - Place;
            //Bigger then 8
            return 1;
        }
    }
}
