using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public TMP_Text nameText;
    public Image avatarImage;
    public TMP_Text cardCountText;

    public void Setup(string playerName, Sprite avatar)
    {
        nameText.text = playerName;
        avatarImage.sprite = avatar;
        UpdateCardCount(0);
    }

    public void UpdateCardCount(int count)
    {
        cardCountText.text = "Cards: " + count;
    }
}
