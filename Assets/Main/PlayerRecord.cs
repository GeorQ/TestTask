using UnityEngine;
using TMPro;


public class PlayerRecord : MonoBehaviour
{
    [SerializeField] private TMP_Text _nicknameText;
    [SerializeField] private TMP_Text _colorText;
    [SerializeField] private TMP_Text _scoreText;


    public void Initialize(GamePlayerInfo gamePlayerInfo)
    {
        UpdateDisplayInfo(gamePlayerInfo);
    }

    public void UpdateDisplayInfo(GamePlayerInfo gamePlayerInfo)
    {
        _nicknameText.text = gamePlayerInfo.playerName;
        _colorText.text = gamePlayerInfo.playerColor.ToString();
        _scoreText.text = gamePlayerInfo.score.ToString();
    }
}