using UnityEngine;

public class LeaderboardController : MonoBehaviour
{
    // Editor variables
    [SerializeField] BoardWin boardWin;
    [SerializeField] PlayerScoreWin playerScoreWin;

    // Private
    string editNickname = "";

    void Start()
    {
        boardWin.gameObject.SetActive(true);
        playerScoreWin.gameObject.SetActive(false);

        CurrentGame.LoadGame();

        boardWin.Initialize();
        boardWin.OnEditPlayerSlotClickedEvent += EditBtnClicked;
        playerScoreWin.OnAcceptBtnClickedEvent += PlayerScoreWinResult;
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
    public void PlayerScoreWinResult(string nickname, int score)
    {
        if (editNickname == "")
        {
            foreach (PlayerScoreInfo info in CurrentGame.game.leaderboard)
            {
                if (info.nickname.Equals(nickname))
                {
                    playerScoreWin.SetNicknameError();
                    return;
                }
            }
            boardWin.AddPlayer(nickname, score);
            playerScoreWin.gameObject.SetActive(false);
            boardWin.gameObject.SetActive(true);
        }
        else
        {
            boardWin.EditPlayer(editNickname, score);
            playerScoreWin.gameObject.SetActive(false);
            boardWin.gameObject.SetActive(true);
            editNickname = "";
        }
    }
}
