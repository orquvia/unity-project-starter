# CLAUDE.md

Unity 6 のスタータープロジェクト。Unity プロジェクト本体は `UnityProject/` 配下にある。概要は [README.md](README.md) を参照。

## 設計ルール

コードを変更する前に、関係する [docs/](docs/) と [ADR](docs/adr/README.md) を読むこと。特に次のルールは崩さない。

- 層は Domain / UseCase / Presentation / Infrastructure / Bootstrap。依存の向きは asmdef で強制されている。Domain と UseCase に UnityEngine への参照を足さない（[ADR-0001](docs/adr/0001-layered-architecture-with-asmdef.md)）
- ゲーム状態を変更できるのは UseCase だけ。`GameSession` の変更メソッドは `internal` にしておき、`InternalsVisibleTo` の対象を増やさない（[ADR-0006](docs/adr/0006-restrict-domain-mutation-to-usecase.md)）
- Port の非同期メソッドは UniTask ではなく `Task` を返す（[ADR-0003](docs/adr/0003-task-at-port-boundary.md)）
- Presenter は MonoBehaviour にせず、普通の C# クラスとしてシーンの `LifetimeScope` に登録する（[ADR-0004](docs/adr/0004-plain-csharp-presenters.md)）
- シーンをまたぐ状態はルートスコープ（`AppLifetimeScope`）に置く（[ADR-0005](docs/adr/0005-keep-game-state-in-root-scope.md)）
- テストでは、モックライブラリを使わずに `Tests/EditMode/Fakes/` の手書き Fake を使う

設計判断を変えるときは既存の ADR を書き換えず、新しい ADR を追加して古い ADR の状態を「置き換え」にする。

## 作業上の注意

- `UnityProject/` 配下にファイルを追加・移動したら、Unity が生成する `.meta` も一緒にコミットする
- ローカルで PlayMode テストを実行すると `ProjectSettings/ProjectSettings.asset` が書き換わる。この差分はコミットしない
- テストは Unity の Test Runner（EditMode / PlayMode）で実行する。CI は GitHub Actions + GameCI（[testing.md](docs/testing.md#ci)）
