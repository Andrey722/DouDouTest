using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Slot : MonoBehaviour, IPointerClickHandler
{
    // Event for slot clicked LMB or RMB
    // <index, nickname, score, isRMB>
    public event Action<int, string, int, bool> OnSlotClickedEvent;

    // Editor variables
    public Text PlaceText;
    public Text NicknameText;
    public Text ScoreText;

    // Private
    int Index;
    public PlayerScoreInfo info { private set; get; }

    // Initializing after parent widget
    public void Initialize(int place, PlayerScoreInfo nInfo, int index)
    {
        info = nInfo;
        Index = index;
        PlaceText.text = place.ToString();
        NicknameText.text = info.nickname;
        ScoreText.text = info.score.ToString();
    }

    // Click event from IPointerClickHandler
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnSlotClickedEvent.Invoke(Index, info.nickname, int.Parse(ScoreText.text), true);
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnSlotClickedEvent.Invoke(Index, info.nickname, int.Parse(ScoreText.text), false);
        }
    }
}
