# ADR（Architecture Decision Records）

設計上の判断を、背景・検討した選択肢・結果と一緒に1件1ファイルで記録しています。結論だけでなく「なぜ他の選択肢を選ばなかったか」を残すことで、前提が変わったときに判断を見直せるようにしています。

| # | タイトル | 状態 |
|---|---|---|
| [0001](0001-layered-architecture-with-asmdef.md) | 層構造と asmdef で依存の向きを強制する | 採用 |
| [0002](0002-use-vcontainer.md) | DI コンテナに VContainer を使う | 採用 |
| [0003](0003-task-at-port-boundary.md) | Port の非同期メソッドは UniTask ではなく Task を返す | 採用 |
| [0004](0004-plain-csharp-presenters.md) | Presenter を MonoBehaviour にしない | 採用 |
| [0005](0005-keep-game-state-in-root-scope.md) | シーンをまたぐ状態はルートスコープの Singleton で持つ | 採用 |
| [0006](0006-restrict-domain-mutation-to-usecase.md) | Domain の状態を変更できるのを UseCase だけにする | 採用 |

## 書き方

新しい ADR は次の連番で作り、以下の見出しで書きます。判断を覆すときは元の ADR を書き換えず、新しい ADR を作って元の状態を「〜で置き換え」にします。

```markdown
# ADR-XXXX: タイトル

- 状態: 提案 / 採用 / 置き換え（ADR-YYYY）
- 日付: YYYY-MM-DD

## 背景
## 決定
## 検討した選択肢
## 結果（良い点 / 注意点）
```
