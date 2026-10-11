# ADR-0002: DI コンテナに VContainer を使う

- 状態: 採用
- 日付: 2026-10-12

## 背景

[ADR-0001](0001-layered-architecture-with-asmdef.md) で層を分けたため、インターフェースと実装を結びつけて各クラスに渡す仕組みが必要になりました。Unity では、シーンの読み込み・破棄に合わせてオブジェクトの寿命を管理する必要もあります。

## 決定

[VContainer](https://github.com/hadashiA/VContainer)（1.19.0）を使います。

- ルートの `LifetimeScope` を `VContainerSettings` に設定し、シーンごとの `LifetimeScope` をその子にする（[dependency-injection.md](../dependency-injection.md)）。
- Presenter は `RegisterEntryPoint` で登録し、MonoBehaviour にしない（[ADR-0004](0004-plain-csharp-presenters.md)）。

## 検討した選択肢

| 選択肢 | 採用しなかった理由 |
|---|---|
| Zenject / Extenject | 機能は豊富だが、メンテナンスが停滞している。解決時の処理が重く、GC の発生も多い |
| DI コンテナを使わず、手で組み立てる（Pure DI） | 小規模なら成立するが、シーンごとのスコープや寿命管理、`Update` の呼び出しを自前で書くことになる |
| Service Locator / static のシングルトン | 依存がコンストラクタに現れず、テストで差し替えにくい |

VContainer を選んだ理由は次のとおりです。

- 解決が速く、GC の発生が少ない。
- `LifetimeScope` の親子関係で、シーンの寿命に合わせたスコープを自然に表現できる。
- `IStartable` / `ITickable` / `IDisposable` で、普通の C# クラスを Unity のライフサイクルに参加させられる。
- 継続的にメンテナンスされている。

## 結果

**良い点**

- 依存がすべてコンストラクタに現れ、テストでは `new` で組み立てられる。
- シーンを抜けると、そのスコープの Presenter が自動で `Dispose()` される。

**注意点**

- DI の設定ミス（登録漏れなど）は実行するまで分からない。`AppLifetimeScopeTests` で EditMode のうちに検査している。
- `LifetimeScope.Build()` は Play モードを前提にしているので、テストでは `Configure` をリフレクションで呼んで素の `ContainerBuilder` で組み立てている。
- Presentation と Bootstrap が VContainer に依存する。Domain・UseCase・Infrastructure は依存しない。
