using UnityEngine;

public class LeaderboardController : MonoBehaviour
{
    // Editor variables
    public BoardWin boardWin;
    public PlayerScoreWin playerScoreWin;

    // Private
    string editNickname = "";

    void Start()
    {
        boardWin.gameObject.SetActive(true);
        playerScoreWin.gameObject.SetActive(false);

        CurrentGame.LoadGame();

        boardWin.Initialize();
        boardWin.OnEditPlayerSlotClickedEvent += EditBtnClicked;
    }

    // Save game when quit
    void OnApplicationQuit()
    {
        CurrentGame.SaveGame();
    }

    // Binded to edit button in BoardWin
    public void EditBtnClicked(string nickname, int score)
    {
        playerScoreWin.Initialize(true, nickname, score);
        playerScoreWin.gameObject.SetActive(true);
        boardWin.gameObject.SetActive(false);
        editNickname = nickname;
    }

    // Binded to add button in BoardWin
    public void AddBtnClicked()
    {
        playerScoreWin.Initialize(false);
        playerScoreWin.gameObject.SetActive(true);
        boardWin.gameObject.SetActive(false);
    }

    // Binded to close button in PlayerScoreWin
    public void PlayerScoreWinClosed()
    {
        playerScoreWin.gameObject.SetActive(false);
        boardWin.gameObject.SetActive(true);
        editNickname = "";
    }

    // Binded to accept button in PlayerScoreWin
    public void PlayerScoreWinResult()
    {
        if (editNickname == "")
        {
            if (playerScoreWin.NameInput.text == "" || playerScoreWin.ScoreInput.text == "") {
                playerScoreWin.SetNicknameError();
                return;
            }
            foreach (PlayerScoreInfo info in CurrentGame.game.leaderboard)
            {
                if (info.nickname.Equals(playerScoreWin.NameInput.text))
                {
                    playerScoreWin.SetNicknameError();
                    return;
                }
            }
            boardWin.AddPlayer(playerScoreWin.NameInput.text, int.Parse(playerScoreWin.ScoreInput.text));
            playerScoreWin.gameObject.SetActive(false);
            boardWin.gameObject.SetActive(true);
        }
        else
        {
            boardWin.EditPlayer(editNickname, int.Parse(playerScoreWin.ScoreInput.text));
            playerScoreWin.gameObject.SetActive(false);
            boardWin.gameObject.SetActive(true);
            editNickname = "";
        }
    }
}
