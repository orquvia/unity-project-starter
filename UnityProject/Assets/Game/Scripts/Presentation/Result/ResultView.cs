using UnityEngine;
using UnityEngine.UI;

namespace Orquvia.Starter.Presentation
{
    public sealed class ResultView : MonoBehaviour
    {
        [SerializeField] private Button titleButton;
        [SerializeField] private Text scoreText;

        public Button TitleButton => titleButton;

        public void SetScore(int score)
        {
            scoreText.text = $"Score: {score}";
        }
    }
}
