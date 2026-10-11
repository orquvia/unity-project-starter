# ADR-0001: 層構造と asmdef で依存の向きを強制する

- 状態: 採用
- 日付: 2026-10-12

## 背景

Unity のプロジェクトは、何もしなければ全コードが `Assembly-CSharp` という1つのアセンブリにまとまり、どのクラスからどのクラスでも参照できます。最初は問題なくても、機能が増えるにつれてゲームのルールが MonoBehaviour に入り込み、テストのために Play モードが必要になり、1か所の変更が広い範囲に影響するようになります。

このリポジトリはチームの開発の土台になるので、**構造が崩れにくいこと**を最優先にしたいと考えました。

## 決定

コードを Domain / UseCase / Presentation / Infrastructure / Bootstrap の5層に分け、それぞれを asmdef にします。

- 依存は内側（Domain）に向かう向きだけを許す。Presentation と Infrastructure は互いを参照しない。
- Domain と UseCase は `noEngineReferences: true` にし、UnityEngine を参照できなくする。
- すべての asmdef を `autoReferenced: false` にする。
- 依存ルールが崩れていないことを `LayerDependencyTests` で、ビルド後のアセンブリの参照から検査する。

詳細は [architecture.md](../architecture.md) を参照してください。

## 検討した選択肢

| 選択肢 | 採用しなかった理由 |
|---|---|
| asmdef を分けず、フォルダと名前空間だけで層を分ける | ルールを破ってもコンパイルが通るので、レビューでしか守れない |
| Runtime / Editor / Tests 程度の粗い分割にする | ロジックと Unity 依存のコードが同じアセンブリに残り、純粋な C# としてテストできない |
| 機能（画面）単位で asmdef を分ける | 規模が小さいうちは分割が細かすぎる。層での分割と組み合わせるのは機能が増えてからでよい |

## 結果

**良い点**

- 依存ルールの違反がコンパイルエラーになる。
- Domain と UseCase が Unity に依存しないので、EditMode テストが速く、ロジックを Unity なしで検証できる。
- 変更したアセンブリとそれに依存するものだけが再コンパイルされる。

**注意点**

- 小さな機能でも複数の層にまたがってファイルが増える。
- asmdef の参照は Inspector から簡単に追加できるので、asmdef だけでは「ルールを緩める変更」を防げない。そのため `LayerDependencyTests` で二重に守っている。
- `Presentation` と `Infrastructure` の両方から使うインターフェースは UseCase 層に置く必要がある（[architecture.md](../architecture.md#port-の置き場所)）。
