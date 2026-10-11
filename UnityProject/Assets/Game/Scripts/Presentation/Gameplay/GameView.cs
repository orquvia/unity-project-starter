using System;
using UnityEngine;
using UnityEngine.UI;

namespace Orquvia.Starter.Presentation
{
    public sealed class GameView : MonoBehaviour
    {
        [SerializeField] private Button targetButton;
        [SerializeField] private Text scoreText;
        [SerializeField] private Text timeText;

        public Button TargetButton => targetButton;

        public void SetScore(int score) => scoreText.text = $"Score: {score}";

        public void SetTime(float seconds) => timeText.text = $"Time: {Mathf.CeilToInt(seconds)}";

        public void SetInteractable(bool value) => targetButton.interactable = value;
    }
}
