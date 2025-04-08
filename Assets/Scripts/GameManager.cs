using UnityEngine;

public class CardManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform handArea;

    void Start()
    {
        GenerateCards(7); // Tạo 7 thẻ bài mặc định
    }

    void GenerateCards(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject card = Instantiate(cardPrefab, handArea);
            card.transform.position += new Vector3(i * 100, 0, 0); // Xếp ngang các thẻ bài
        }
    }
}