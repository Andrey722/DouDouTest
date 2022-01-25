using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

// Struct for leaderboard slot info
[System.Serializable]
public struct PlayerScoreInfo
{
    public string nickname;
    public int score;
}

// Static class to save to the file
[System.Serializable]
public class CurrentGame
{
    // Currently loaded game
    public static CurrentGame game { get; private set; }

    // Create empty game
    public static CurrentGame CreateGame()
    {
        game = new CurrentGame();

        game.leaderboard = new List<PlayerScoreInfo>();

        return game;
    }

    // Load info, given from file. Save to the current game var
    public static void LoadGame()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/saves"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saves");
        }

        if (File.Exists(Application.persistentDataPath + $"/saves/main.save"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream f = File.Open(Application.persistentDataPath + $"/saves/main.save", FileMode.Open);
            game = (CurrentGame)bf.Deserialize(f);
            f.Close();
        }
        else
        {
            CreateGame();
        }
    }

    // Save current game to the file
    public static void SaveGame()
    {
        FileStream file = File.Open(Application.persistentDataPath + $"/saves/main.save", FileMode.OpenOrCreate);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, game);
        file.Close();
    }

    // Find a place in leaderboard for record on position
    public static void SortRecord(int recIndex)
    {
        PlayerScoreInfo info = game.leaderboard[recIndex];
        game.leaderboard.RemoveAt(recIndex);
        int whereToPlace = -1;
        for(int i = 0; i < game.leaderboard.Count; i++)
        {
            if (game.leaderboard[i].score < info.score)
            {
                whereToPlace = i;
                break;
            }
        }
        if (whereToPlace != -1)
        {
            game.leaderboard.Insert(whereToPlace, info);
        }
        else
        {
            game.leaderboard.Add(info);
        }
    }

    // INFO TO SAVE
    public List<PlayerScoreInfo> leaderboard;
}