# ADR-0005: シーンをまたぐ状態はルートスコープの Singleton で持つ

- 状態: 採用
- 日付: 2026-10-12

## 背景

Game シーンで稼いだスコアを Result シーンで表示する必要があります。Unity ではシーンを切り替えると、そのシーンのオブジェクトはすべて破棄されます。

## 決定

ゲームの状態を持つ `GameSession` を、ルートの `AppLifetimeScope` に Singleton として登録します。シーン遷移を担う `GameFlow` も同じくルートに置きます。

```csharp
builder.Register<GameSession>(Lifetime.Singleton)
    .AsSelf()
    .As<IReadOnlyGameSession>();
```

各シーンの Presenter は、親スコープから同じインスタンスを受け取ります。

## 検討した選択肢

| 選択肢 | 採用しなかった理由 |
|---|---|
| static 変数で持つ | どこからでも書き換えられ、テストの間で状態が残る |
| `DontDestroyOnLoad` の MonoBehaviour で持つ | 取得に `FindObjectOfType` などが必要になり、依存が見えなくなる。テストに GameObject が必要になる |
| 遷移時に引数として次のシーンに渡す | VContainer の `LifetimeScope.EnqueueParent` などで実現できるが、遷移のたびに受け渡しのコードが必要になる。今の規模では過剰 |
| PlayerPrefs やファイルに書き出す | 保存が目的ではなく、メモリ上で受け渡せば足りる |

## 結果

**良い点**

- Result は `IReadOnlyGameSession` を受け取るだけで、Game で確定したスコアを読める。
- 状態の持ち主が DI の登録で明示される。

**注意点**

- `GameSession` と `IReadOnlyGameSession` が別のインスタンスになると、Result がスコア 0 の別のセッションを読んでしまう。`AppLifetimeScopeTests` で同じインスタンスであることを検査している。
- アプリ全体で1つの状態なので、複数のゲームを同時に進めるような要件には対応できない。そうなった場合は、ゲームごとのスコープを作る設計に見直す。
