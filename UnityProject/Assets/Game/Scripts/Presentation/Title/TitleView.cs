using UnityEngine;
using UnityEngine.UI;

namespace Orquvia.Starter.Presentation
{
    public sealed class TitleView : MonoBehaviour
    {
        [SerializeField] private Button startButton;

        public Button StartButton => startButton;
    }
}
