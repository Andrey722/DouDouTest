using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Widget class of swipable area. Has only 3 tabs.
// Changes not visible tabs when swiped.
public class Swiper : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // Event when any slot have been clicked
    public event Action<int, string, int, bool> OnAnySlotClickedEvent;

    // Editor prefabs variables
    public GameObject slidePrefab;
    public GameObject SlotPrefab;

    // Private
    List<GameObject> slides;
    RectTransform ownTransform;
    // Mouse positions
    float firstX; // first touch X
    float x1; // previous X
    float x2; // current X
    float move = 0; // move direction
    float targetX; // target X position for left tab
    bool moveEnded = false; // mouse up. Sets true on MouseUp and false on the final move of tabs
    bool pressed = false; // mouse is down bool for Update func.
    bool moveCancelled = false; // move has been cancelled (did not moved more than 50% of swiper widget width)

    public int moveBackSpeed = 2500;
    public int moveForwardSpeed = 5000;

    // Slots & pages
    List<Slot> SlotsInBoard;
    List<Slot> DeadSlots;
    int CurrentPage = 1;
    int PageSize = 4;
    int PagesCount; // calculates automatically

    // Calculating pages count by leaderboard records count
    public void UpdatePagesCount()
    {
        PagesCount = (int)Mathf.Ceil((CurrentGame.game.leaderboard.Count + 0.0f) / PageSize);
    }

    // Initializing after parent widget
    public void Initialize()
    {
        slides = new List<GameObject>();
        SlotsInBoard = new List<Slot>();
        DeadSlots = new List<Slot>();
        ownTransform = gameObject.GetComponent<RectTransform>();
        for (int i = 0; i < 3; i++)
        {
            GameObject slide = Instantiate(slidePrefab, gameObject.transform);
            RectTransform r = slide.GetComponent<RectTransform>();
            slides.Add(slide);
            r.anchoredPosition = new Vector2((i - 1) * ownTransform.rect.width, r.anchoredPosition.y);
        }
        UpdatePagesCount();
        UpdateSlots();
    }

    // MonoBehaviour mouse down event
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            x1 = Input.mousePosition.x;
            firstX = x1;
            pressed = true;
        }
    }

    // MonoBehaviour mouse up event
    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (Mathf.Abs(-ownTransform.rect.width - slides[0].GetComponent<RectTransform>().anchoredPosition.x) > ownTransform.rect.width / 2)
            {
                if (firstX - x2 > 0)
                {
                    targetX = -ownTransform.rect.width * 2;
                    move = -moveBackSpeed;
                }
                else if (firstX - x2 < 0)
                {
                    targetX = 0;
                    move = moveBackSpeed;
                }
                moveCancelled = false;
            }
            else
            {
                targetX = -ownTransform.rect.width;
                move = firstX - x2 > 0 ? moveForwardSpeed : -moveForwardSpeed;
                moveCancelled = true;
            }
            moveEnded = true;
            pressed = false;
        }
    }

    // Update tab positions every frame
    void Update()
    {
        // mouse pressed
        if (pressed)
        {
            x2 = Input.mousePosition.x;

            move = (x2 - x1);
            x1 = x2;

            if ((CurrentPage == 1 && (x2-firstX) > 0) || (CurrentPage == PagesCount && (x2 - firstX) < 0))
            {
                move = move / (Mathf.Abs(firstX - x2) % Screen.width) * 5;
            }

            for (int i = 0; i < slides.Count; i++)
            {
                GameObject slide = slides[i];
                RectTransform r = slide.GetComponent<RectTransform>();
                r.anchoredPosition = new Vector2(r.anchoredPosition.x + move, r.anchoredPosition.y);
            }
        }
        // ending tab moves
        else if (moveEnded)
        {
            for (int i = 0; i < slides.Count; i++)
            {
                GameObject slide = slides[i];
                RectTransform r = slide.GetComponent<RectTransform>();
                r.anchoredPosition = new Vector2(r.anchoredPosition.x + move * Time.deltaTime, r.anchoredPosition.y);
            }

            if (Mathf.Abs(targetX - slides[0].GetComponent<RectTransform>().anchoredPosition.x) <= moveForwardSpeed * Time.deltaTime)
            {

                for (int i = 0; i < slides.Count; i++)
                {
                    GameObject slide = slides[i];
                    RectTransform r = slide.GetComponent<RectTransform>();
                    r.anchoredPosition = new Vector2(targetX + ownTransform.rect.width * i, r.anchoredPosition.y);
                }
                moveEnded = false;

                if (!moveCancelled)
                {
                    if (move > 0)
                    {
                        GameObject slide = slides[2];
                        slides.RemoveAt(2);
                        slides.Insert(0, slide);
                        RectTransform r = slide.GetComponent<RectTransform>();
                        r.anchoredPosition = new Vector2(-ownTransform.rect.width, r.anchoredPosition.y);
                        CurrentPage--;
                    }
                    else if (move < 0)
                    {
                        GameObject slide = slides[0];
                        slides.RemoveAt(0);
                        slides.Add(slide);
                        RectTransform r = slide.GetComponent<RectTransform>();
                        r.anchoredPosition = new Vector2(ownTransform.rect.width, r.anchoredPosition.y);
                        CurrentPage++;
                    }
                }

                move = 0;
                UpdateSlots();
            }
        }
    }

    // Update Slots in visible and neighbour parts of table
    public void UpdateSlots()
    {
        UpdatePagesCount();
        int i = (CurrentPage == 1 ? CurrentPage : CurrentPage - 1) * PageSize - PageSize;
        int childIndex;
        int j = CurrentPage == 1 ? 1 : 0;
        for (childIndex = 0; i < CurrentGame.game.leaderboard.Count && i < (CurrentPage+1) * PageSize && j < 3; i++, childIndex++)
        {
            PlayerScoreInfo info = CurrentGame.game.leaderboard[i];
            if (SlotsInBoard.Count > i)
            {
                SlotsInBoard[i].Initialize(i + 1, info, childIndex);
                SlotsInBoard[i].gameObject.transform.SetParent(slides[j].transform);
            }
            else if (DeadSlots.Count > 0)
            {
                DeadSlots[0].gameObject.SetActive(true);
                DeadSlots[0].gameObject.transform.SetParent(slides[j].transform);
                DeadSlots[0].gameObject.transform.SetSiblingIndex(i);
                DeadSlots[0].Initialize(i + 1, info, childIndex);
                SlotsInBoard.Add(DeadSlots[0]);
                DeadSlots.RemoveAt(0);
            }
            else
            {
                GameObject obj = Instantiate(SlotPrefab, slides[j].transform);
                obj.transform.SetSiblingIndex(i);
                SlotsInBoard.Add(obj.GetComponent<Slot>());
                SlotsInBoard[SlotsInBoard.Count - 1].Initialize(i + 1, info, childIndex);
                SlotsInBoard[SlotsInBoard.Count - 1].OnSlotClickedEvent += OnSlotClicked;
            }

            if (i % PageSize == 3)
            {

                childIndex = 0;
                j++;
            }
        }
        while (i < SlotsInBoard.Count)
        {
            SlotsInBoard[i].gameObject.SetActive(false);
            DeadSlots.Add(SlotsInBoard[i]);
            SlotsInBoard.RemoveAt(i);
        }
    }

    // Function to get slot by index. Only in visible part.
    public Transform GetSlot(int index)
    {
        return slides[1].transform.GetChild(index);
    }

    // Binded to slot click event and Invoking for BoardWin
    void OnSlotClicked(int index, string nickname, int score, bool right)
    {
        OnAnySlotClickedEvent.Invoke(index, nickname, score, right);
    }
}
