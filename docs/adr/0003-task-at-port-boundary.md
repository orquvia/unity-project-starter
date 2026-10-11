# ADR-0003: Port の非同期メソッドは UniTask ではなく Task を返す

- 状態: 採用
- 日付: 2026-10-12

## 背景

シーン遷移を抽象化する `ISceneNavigator` は UseCase 層に置いています（理由は [architecture.md](../architecture.md#port-の置き場所)）。プロジェクト全体では UniTask を使っていますが、UseCase 層は `noEngineReferences: true` で、外部ライブラリにも依存させない方針です。

## 決定

UseCase 層に置く Port の非同期メソッドは、標準の `Task` を返します。

```csharp
Task LoadAsync(SceneId sceneId, CancellationToken ct);
```

UniTask を使うのは、Port を実装する Infrastructure と、Port を呼ぶ Presentation だけにします。

## 検討した選択肢

| 選択肢 | 採用しなかった理由 |
|---|---|
| Port で `UniTask` を返す | UseCase 層が UniTask に依存する。UniTask は Unity 向けのライブラリなので、Unity に依存しない層という前提が崩れる |
| `ValueTask` を返す | 割り当てを減らせるが、`await` は1回しかできないなどの制約があり、シーン遷移のように頻度の低い処理では利点が小さい |
| コールバックで完了を通知する | キャンセルや例外の扱いが煩雑になる |

## 結果

**良い点**

- UseCase 層が標準ライブラリだけで完結する。
- テストの Fake は `TaskCompletionSource` で完了のタイミングを自由に制御できる。

**注意点**

- `Task` は呼び出しのたびに割り当てが発生する。シーン遷移は頻度が低いので問題にならないが、**毎フレーム呼ぶような Port を追加するときはこの判断を見直す**。
- Infrastructure では UniTask と `Task` の変換を意識する必要がある（`async Task` メソッドの中で UniTask を `await` している）。
