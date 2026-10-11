# ADR-0006: Domain の状態を変更できるのを UseCase だけにする

- 状態: 採用
- 日付: 2026-10-12

## 背景

`GameSession` の状態は UseCase を通してだけ変更し、Presenter は読み取り専用の `IReadOnlyGameSession` を使う、という方針にしています（[architecture.md](../architecture.md#usecase-を分ける理由)）。

しかし Presentation は Domain を参照しているので、Presenter のコンストラクタで `GameSession` を直接受け取れば、UseCase を通さずに状態を変更できてしまいます。DI の登録も `AsSelf()` なので解決できます。つまりこの方針は、**取り決めとしてしか守られていませんでした**。

## 決定

`GameSession` の変更メソッド（`Start`, `TryHitTarget`, `Advance`, `Reset`）を `internal` にし、Domain の `InternalsVisibleTo` で公開する先を UseCase とテストのアセンブリだけにします。

```csharp
// Domain/AssemblyInfo.cs
[assembly: InternalsVisibleTo("Orquvia.Starter.UseCase")]
[assembly: InternalsVisibleTo("Orquvia.Starter.Tests.EditMode")]
```

あわせて `DomainEncapsulationTests` で次の2点を検査します。

- `GameSession` に public な変更メソッドがないこと
- `InternalsVisibleTo` の公開先が上の2つだけであること

## 検討した選択肢

| 選択肢 | 採用しなかった理由 |
|---|---|
| 取り決めのままにして、ドキュメントに制約として書く | レビューで見落とせば崩れる。このリポジトリの「構造をコンパイラで守る」という方針と合わない |
| Presentation の asmdef から Domain への参照を外す | Presenter は `IReadOnlyGameSession` や `GameStatus` を使うので、Domain は参照する必要がある |
| 読み取り用の型を UseCase 層に移し、Presentation は UseCase だけを参照する | 読み取り用の型を UseCase に複製する必要があり、今の規模では手間に見合わない |
| `GameSession` を UseCase 層に移す | ゲームのルールが Domain から外れてしまう |

## 結果

**良い点**

- Presentation で `GameSession` を受け取っても、状態を変更しようとするとコンパイルエラーになる。
- 「状態を変える処理は UseCase にある」ことが保証されるので、変更の影響範囲を追いやすい。

**注意点**

- `InternalsVisibleTo` はアセンブリ名の文字列で指定するので、asmdef の名前を変えると UseCase からも呼べなくなる（コンパイルエラーで気づける）。
- テストのアセンブリにも internal を公開している。テストから Domain の状態を直接作れる利便性を優先した。
- Domain に新しい状態クラスを追加するときも、同じく変更メソッドを `internal` にする必要がある。現在の `DomainEncapsulationTests` は `GameSession` だけを検査しているので、クラスが増えたら検査対象を広げる。
