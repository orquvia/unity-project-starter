using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using Orquvia.Starter.Presentation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Orquvia.Starter.Tests.PlayMode
{
    // 実際のシーン・DI・設定アセットを使い、Title → Game → Result → Title を通しで確認する。
    public sealed class GameFlowE2ETests
    {
        // 各待機の上限（実時間）
        private const float TimeoutSeconds = 10f;

        // 制限時間（既定 20 秒）を短縮するための時間倍率
        private const float GameTimeScale = 20f;

        private const int Hits = 3;

        private float _originalTimeScale;

        [SetUp]
        public void SetUp()
        {
            _originalTimeScale = Time.timeScale;
        }

        [TearDown]
        public void TearDown()
        {
            Time.timeScale = _originalTimeScale;
        }

        [UnityTest]
        public IEnumerator PlayThrough_TitleGameResultTitle()
        {
            yield return SceneManager.LoadSceneAsync("Title");
            yield return null; // TitlePresenter.Start を待つ

            Click(FindSingle<TitleView>().StartButton);

            yield return WaitForScene("Game");
            yield return WaitUntil(ScoreIsShown, "GamePresenter の開始");

            var target = FindSingle<GameView>().TargetButton;
            for (var i = 0; i < Hits; i++)
            {
                Click(target);
            }

            var gameScore = FindText("Score").text;
            Assert.That(gameScore, Is.Not.EqualTo("Score: 0"), "ヒットがスコアに反映されていません");

            Time.timeScale = GameTimeScale;
            yield return WaitForScene("Result");
            Time.timeScale = _originalTimeScale;
            yield return WaitUntil(ScoreIsShown, "ResultPresenter の開始");

            Assert.That(FindText("Score").text, Is.EqualTo(gameScore));

            Click(FindSingle<ResultView>().TitleButton);

            yield return WaitForScene("Title");
        }

        // onClick.Invoke() と違い、Button の interactable や有効状態の判定を通す
        private static void Click(Button button)
        {
            ExecuteEvents.Execute(
                button.gameObject,
                new PointerEventData(EventSystem.current),
                ExecuteEvents.pointerClickHandler);
        }

        // 遷移直後は前のシーンのオブジェクトが残っていることがあるため、アクティブなシーンに限定して探す
        private static T FindSingle<T>()
            where T : Component
        {
            var found = FindInActiveScene<T>();
            Assert.That(found, Has.Length.EqualTo(1), $"{typeof(T).Name} がちょうど1つ見つかりません");
            return found[0];
        }

        private static Text FindText(string objectName)
        {
            return FindInActiveScene<Text>().FirstOrDefault(text => text.name == objectName);
        }

        private static bool ScoreIsShown()
        {
            // シーン上の初期表記は "SCORE: " なので、"Score: " に変われば Presenter の Start が済んでいる
            var score = FindText("Score");
            return score != null && score.text.StartsWith("Score: ", StringComparison.Ordinal);
        }

        private static T[] FindInActiveScene<T>()
            where T : Component
        {
            var activeScene = SceneManager.GetActiveScene();
            return Object.FindObjectsByType<T>(FindObjectsSortMode.None)
                .Where(component => component.gameObject.scene == activeScene)
                .ToArray();
        }

        private static IEnumerator WaitForScene(string sceneName)
        {
            return WaitUntil(
                () => SceneManager.GetActiveScene().name == sceneName,
                $"{sceneName} シーンへの遷移");
        }

        private static IEnumerator WaitUntil(Func<bool> condition, string description)
        {
            var deadline = Time.realtimeSinceStartup + TimeoutSeconds;

            while (!condition())
            {
                if (Time.realtimeSinceStartup > deadline)
                {
                    Assert.Fail($"タイムアウトしました: {description}");
                }

                yield return null;
            }
        }
    }
}
