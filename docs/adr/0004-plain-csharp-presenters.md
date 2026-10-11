# ADR-0004: Presenter を MonoBehaviour にしない

- 状態: 採用
- 日付: 2026-10-12

## 背景

Unity では、画面のロジックをボタンやテキストと同じ MonoBehaviour に書くのが一般的です。しかしそうすると、ロジックのテストにシーンや GameObject が必要になり、依存もシリアライズされたフィールドや `FindObjectOfType` で取ることになりがちです。

## 決定

画面を View と Presenter に分けます。

| | 種類 | 役割 |
|---|---|---|
| View | MonoBehaviour | シーン上のボタンやテキストへの参照を持ち、表示を更新するメソッドを公開する。ロジックは持たない |
| Presenter | 普通の C# クラス | UseCase を呼び、結果を View に反映する。VContainer の `RegisterEntryPoint` で登録する |

Presenter は `IStartable` / `ITickable` / `IDisposable` で Unity のライフサイクルに参加します。

## 検討した選択肢

| 選択肢 | 採用しなかった理由 |
|---|---|
| Presenter を MonoBehaviour にする | コンストラクタで依存を受け取れず、テストに GameObject の生成が必要になる |
| MVVM + データバインディング（UI Toolkit など） | uGUI を使う前提では仕組みが大きくなりすぎる |
| R3（Reactive Extensions）で View と Presenter をつなぐ | 今の規模ではイベントの購読・解除で十分。依存ライブラリを増やす理由が弱い |

## 結果

**良い点**

- Presenter のテストは `new` して `Start()` や `Tick()` を呼ぶだけで書ける。
- 依存がコンストラクタに並ぶので、その画面が何を使っているか一目で分かる。
- シーンを抜けると VContainer が `Dispose()` を呼ぶので、イベントの購読解除を確実に行える。

**注意点**

- View のテストでは、ボタンやテキストの参照を `SerializedObject` で設定する手間がかかる（`TestUi` にまとめている）。
- `ITickable` の呼び出し順は VContainer が管理するので、MonoBehaviour の `Update` との順序に依存する処理は書かないようにする。
