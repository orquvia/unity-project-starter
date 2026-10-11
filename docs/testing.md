# テスト戦略と CI

## テストの種類

| 種類 | モード | 対象 | 確認すること |
|---|---|---|---|
| アーキテクチャ | EditMode | asmdef の依存関係、Domain の公開範囲 | 層の依存ルールが崩れていないか |
| 単体 | EditMode | Domain, Presenter, `GameFlow`, Infrastructure | ロジックと画面の振る舞い |
| 構成 | EditMode | `AppLifetimeScope` と設定アセット | DI の登録と、アセット同士のつながり |
| E2E | PlayMode | 実際のシーン・DI・設定アセット | Title → Game → Result → Title が通しで動くか |

ほとんどを高速な EditMode で確認し、実際のシーンでしか確認できないこと（シーン遷移、`LifetimeScope` の親子関係、Inspector での参照設定）だけを PlayMode の E2E で確認する構成です。

## アーキテクチャテスト

| テスト | 内容 |
|---|---|
| [`LayerDependencyTests`](../UnityProject/Assets/Game/Tests/EditMode/Architecture/LayerDependencyTests.cs) | ビルド後のアセンブリが参照しているアセンブリを `Assembly.GetReferencedAssemblies()` で調べ、許可されていない依存がないか検査する |
| [`DomainEncapsulationTests`](../UnityProject/Assets/Game/Tests/EditMode/Architecture/DomainEncapsulationTests.cs) | `GameSession` に public な変更メソッドがないこと、Domain の `InternalsVisibleTo` が UseCase とテストだけであることを検査する |

依存ルールはコードレビューで見落としやすく、崩れても動作には影響しないので気づきにくい種類の問題です。テストにしておくと、CI で機械的に止められます。詳しくは [アーキテクチャ](architecture.md#ルールをテストで守る) を参照してください。

## 単体テスト

### Fake は手書きにする

モックライブラリは使わず、`Tests/EditMode/Fakes/` に手書きの Fake を置いています。

| Fake | 用途 |
|---|---|
| `FakeSceneNavigator` | 要求されたシーンを記録する。`Handler` を差し替えて、完了の遅延・失敗・キャンセルを再現する |
| `FakeGameLogger` | 出力されたログをレベルごとに記録する |
| `FakeTimeProvider` | `DeltaTime` をテストから設定する |

インターフェースが小さいので手書きでも短く済み、外部パッケージを増やさずに済みます。

### Presenter のテスト

Presenter は普通の C# クラスなので、`new` で組み立てて `Start()` / `Tick()` / `Dispose()` を直接呼びます。View は `GameObject` に `AddComponent` し、`SerializedObject` でボタンやテキストへの参照を設定しています（`TestUi`）。

時間は `FakeTimeProvider` で進めるので、「時間切れになったら Result への遷移を1回だけ要求する」といったテストも待ち時間なしで書けます。

### 非同期処理のテスト

`GameFlowTests` では `SynchronizationContext` を外し、`await` の続きを同期的に実行させています。詳しくは [非同期処理とキャンセル](async-and-cancellation.md#テスト) を参照してください。

### internal のテスト

Infrastructure の `UnitySceneNavigator.ToScenePath` は `internal` で、`InternalsVisibleTo` でテストにだけ公開しています。すべての `SceneId` が、存在し Build Settings で有効になっているシーンを指しているかを、`EditorBuildSettings` と照らし合わせて確認しています。シーンの追加や移動で遷移先が壊れても、実行前に気づけます。

## 構成テスト

[`AppLifetimeScopeTests`](../UnityProject/Assets/Game/Tests/EditMode/Bootstrap/AppLifetimeScopeTests.cs) で、実際の `AppLifetimeScope.prefab` の DI 設定を EditMode で検証しています。内容は [DI とスコープ設計](dependency-injection.md#di-構成のテスト) を参照してください。

## E2E テスト

[`GameFlowE2ETests`](../UnityProject/Assets/Game/Tests/PlayMode/GameFlowE2ETests.cs) は実際の Title シーンから始めて、ボタンを押しながら一周します。PlayMode のテストアセンブリは Presentation と uGUI しか参照せず、外から画面を操作する立場でテストしています。

| 工夫 | 理由 |
|---|---|
| `ExecuteEvents.Execute` でクリックする | `onClick.Invoke()` と違い、`interactable` や有効状態の判定を通るので、実際のクリックに近い |
| `Time.timeScale = 20` で時間を早める | 20秒の制限時間を待たずに Result まで進める |
| 待機の上限を実時間（`realtimeSinceStartup`）で判定する | `timeScale` を変えても上限が縮まない。CI の初回はシーンの読み込みやシェーダーのコンパイルが遅いので余裕を持たせている |
| アクティブなシーンのオブジェクトだけを探す | 遷移直後は前のシーンのオブジェクトが残っていることがあるため |
| `TearDown` で `timeScale` を元に戻す | 失敗しても後のテストに影響を残さない |

## CI

[`.github/workflows/unity-tests.yml`](../.github/workflows/unity-tests.yml) で、pull request と main への push のたびにテストを実行します。

| 設定 | 内容 |
|---|---|
| `game-ci/unity-test-runner@v4` | Unity のバージョンは `ProjectVersion.txt` から自動で決まる |
| `matrix: [editmode, playmode]` | 2種類を並列に実行し、`fail-fast: false` で一方が失敗しても他方は最後まで実行する |
| `actions/cache`（`Library/`） | 2回目以降のアセットインポートを短縮する |
| `concurrency` | 同じブランチに新しい push があったら、実行中のものを止める |
| `checks: write` | テスト結果を PR のチェックとして表示する |
| `upload-artifact` | 失敗したときもテスト結果を保存する |

### 注意点

- **ライセンス**：Unity Personal ライセンスを secrets（`UNITY_LICENSE` / `UNITY_EMAIL` / `UNITY_PASSWORD`）で渡しています。公開リポジトリなので、fork からの pull request には secrets が渡らず、ライセンス認証で失敗します。
- **ローカルでの PlayMode テスト**：Test Runner が `ProjectSettings/ProjectSettings.asset` を書き換えます。この差分はコミットしないでください。
