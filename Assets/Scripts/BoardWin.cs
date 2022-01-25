using System;
using UnityEngine;

public class BoardWin : MonoBehaviour
{
    // Event when slot have been clicked
    public event Action<string, int> OnEditPlayerSlotClickedEvent;

    // Editor variable
    public Swiper Board;

    // Private
    int SelectedSlot = -1;

    // Initializing after parent widget
    public void Initialize()
    {
        Board.Initialize();
        Board.OnAnySlotClickedEvent += OnSlotClicked;
    }

    // Binded to swiper any slot clicked event
    public void OnSlotClicked(int index, string nickname, int score, bool right)
    {
        if (right)
        {
            OnEditPlayerSlotClickedEvent.Invoke(nickname, score);
        }
        else
        {
            SelectedSlot = index;
        }
    }

    // Add player to the leaderboard
    public void AddPlayer(string nickname, int score)
    {
        PlayerScoreInfo info = new PlayerScoreInfo();
        info.nickname = nickname;
        info.score = score;
        CurrentGame.game.leaderboard.Add(info);
        CurrentGame.SortRecord(CurrentGame.game.leaderboard.Count-1);
        Board.UpdateSlots();
    }

    // Binded to remove button
    public void RemoveBtnClicked()
    {
        if (SelectedSlot > -1)
        {
            Slot slot = Board.GetSlot(SelectedSlot).GetComponent<Slot>();
            string nickname = slot.info.nickname;
            for (int i = 0; i < CurrentGame.game.leaderboard.Count; i++)
            {
                if (CurrentGame.game.leaderboard[i].nickname == nickname)
                {
                    CurrentGame.game.leaderboard.RemoveAt(i);
                    break;
                }
            }
            Board.UpdateSlots();
            SelectedSlot = -1;
        }
    }

    // Edit player score
    public void EditPlayer(string nickname, int score)
    {
        int i;
        for (i = 0; i < CurrentGame.game.leaderboard.Count; i++)
        {
            if (CurrentGame.game.leaderboard[i].nickname == nickname)
            {
                PlayerScoreInfo info = new PlayerScoreInfo();
                info.nickname = nickname;
                info.score = score;
                CurrentGame.game.leaderboard[i] = info;
                break;
            }
        }
        CurrentGame.SortRecord(i);
        Board.UpdateSlots();
    }
}
