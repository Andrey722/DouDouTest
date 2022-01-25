using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreWin : MonoBehaviour
{
    // Editor variables
    public Text TitleText;
    public Button AcceptBtn;
    public Text AcceptBtnText;
    public InputField NameInput;
    public InputField ScoreInput;

    // Initializing after parent widget
    public void Initialize(bool edit, string name = "", int score = 0)
    {
        if (edit)
        {
            NameInput.text = name;
            ScoreInput.text = score.ToString();
            NameInput.DeactivateInputField();
            AcceptBtnText.text = "EDIT";
            TitleText.text = "EDIT PLAYER";
        }
        else
        {
            NameInput.text = "";
            ScoreInput.text = "";
            NameInput.ActivateInputField();
            AcceptBtnText.text = "ADD";
            TitleText.text = "ADD PLAYER";
        }
        NameInput.textComponent.color = Color.black;
    }

    // Set nickname input text RED
    // It used when nickname already have been used in the leaderboard
    public void SetNicknameError()
    {
        NameInput.textComponent.color = Color.red;
    }
}
