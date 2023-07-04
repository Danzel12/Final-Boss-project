using System;
using System.Collections.Generic;
using System.IO;

class Final_Boss_project
{
    public static void Main()

    {
        bool playAgain = true;

        Console.WriteLine("Tower of Hanoi");

        while (playAgain)
        {
            int numberOfDisks = ReadDifficultyLevel();

            // Create towers
            char[] towerNames = { 'A', 'B', 'C' };
            Stack<int>[] towerStacks = new Stack<int>[3];
            
            for (int i = 0; i < 3; i++)
            {
                towerStacks[i] = new Stack<int>();
            }

            for (int i = numberOfDisks; i > 0; i--)
            {
                towerStacks[0].Push(i);
            }

            List<string> moveHistory = new List<string>();

            while (towerStacks[2].Count < numberOfDisks)
            {
                Console.Clear();
                DisplayTowers(towerStacks, towerNames);

                Console.WriteLine("Enter your move (e.g., AB to move disk from A to B):");
                string move = Console.ReadLine().ToUpper();

                if (move.Length != 2 || !IsValidTower(move[0]) || !IsValidTower(move[1]))
                {
                    Console.WriteLine("Invalid move. Please try again.");
                    continue;
                }


                int sourceIndex = move[0] - 'A';
                int destinationIndex = move[1] - 'A';

                if (towerStacks[sourceIndex].Count == 0 || (towerStacks[destinationIndex].Count > 0 &&
                    towerStacks[sourceIndex].Peek() > towerStacks[destinationIndex].Peek()))
                {
                    Console.WriteLine("Invalid move. Please try again.");
                    continue;
                }

                int disk = towerStacks[sourceIndex].Pop();
                towerStacks[destinationIndex].Push(disk);

                moveHistory.Add($"Move disk from tower {sourceIndex + 1} to {destinationIndex + 1}");
            }

            Console.Clear();
            DisplayTowers(towerStacks, towerNames);
            Console.WriteLine("Congratulations! You solved the Tower of Hanoi.");

            Console.WriteLine("Do you want to play again? (Y/N)");
            string playAgainInput = Console.ReadLine().ToUpper();

            playAgain = playAgainInput == "Y";

            if (playAgain)
            {
                SaveMoveHistory(moveHistory);
                moveHistory.Clear();
            }

            Console.WriteLine();
        }

        Console.WriteLine("Thank you for playing Tower of Hanoi!");
        Console.ReadLine();
    }

    static int ReadDifficultyLevel()
    {
        string setupFilePath = "setup.ini"; // Assuming the setup.ini file is in the same directory as the executable

        if (!File.Exists(setupFilePath))
        {
            Console.WriteLine("setup.ini file not found. Defaulting to 3 disks.");
            return 3;
        }

        try
        {
            using (StreamReader reader = new StreamReader(setupFilePath))
            {
                string setupFileContent = reader.ReadLine();
                int difficultyLevel;

                if (int.TryParse(setupFileContent, out difficultyLevel))
                {
                    switch (difficultyLevel)
                    {
                        case 1:
                            return 3;
                        case 2:
                            return 5;
                        case 3:
                            return 7;
                        default:
                            Console.WriteLine("Invalid difficulty level in setup.ini file. Defaulting to 3 disks.");
                            return 3;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid difficulty level in setup.ini file. Defaulting to 3 disks.");
                    return 3;
                }
            }
        }
        catch (Exception)
        {
            Console.WriteLine("Error reading setup.ini file. Defaulting to 3 disks.");
            return 3;
        }
    }

    static bool IsValidTower(char tower)
    {
        return tower >= 'A' && tower <= 'C';
    }

    static void DisplayTowers(Stack<int>[] towerStacks, char[] towerNames)
    {
        int maxHeight = 0;
        for (int i = 0; i < towerStacks.Length; i++)
        {
            if (towerStacks[i].Count > maxHeight)
                maxHeight = towerStacks[i].Count;
        }

        for (int level = 0; level < maxHeight; level++)
        {
            for (int i = 0; i < towerStacks.Length; i++)
            {
                string diskRepresentation = GetDiskRepresentation(towerStacks[i], level);
                Console.Write("{0,-10}", diskRepresentation);
            }
            Console.WriteLine();
        }

        for (int i = 0; i < towerStacks.Length; i++)
        {
            Console.Write("{0,-10}", towerNames[i]);
        }
        Console.WriteLine();
    }

    static string GetDiskRepresentation(Stack<int> towerStack, int level)
    {
        if (level < towerStack.Count)
        {
            int diskSize = towerStack.ToArray()[level];
            return new string('=', diskSize);
        }
        else
        {
            return "|";
        }
    }

    static void SaveMoveHistory(List<string> moveHistory)
    {
        string debugFolderPath = "move history"; // Specify the folder path where you want to save the move history file
        int sessionNumber = GetNextSessionNumber(debugFolderPath);
        string historyFilePath = Path.Combine(debugFolderPath, $"move_history{sessionNumber}.txt");

        try
        {
            using (StreamWriter writer = new StreamWriter(historyFilePath))
            {
                foreach (string move in moveHistory)
                {
                    writer.WriteLine(move);
                }
            }

            Console.WriteLine($"Move history saved to {historyFilePath}");
        }
        catch (Exception)
        {
            Console.WriteLine("Error saving move history.");
        }
    }

    static int GetNextSessionNumber(string debugFolderPath)
    {
        int sessionNumber = 1;
        string[] existingFiles = Directory.GetFiles(debugFolderPath, "move_history*.txt");
        foreach (string file in existingFiles)
        {
            if (int.TryParse(Path.GetFileNameWithoutExtension(file).Replace("move_history", ""), out int fileNumber))
            {
                if (fileNumber >= sessionNumber)
                {
                    sessionNumber = fileNumber + 1;
                }
            }
        }
        return sessionNumber;
    }
}