# Lily

Google Cloud Speech APIを利用したリアルタイム音声認識を、JavaとC#のプロセス間通信を利用してWPFで動かそうというリポジトリです。<br>
成功するかどうかは分かりません。<br>
<b>開発はdevelopブランチで進めています</b>。適宜ブランチの切り替えをお願いします。

## 開発環境

### Java
- IDE: NetBeans 8.2
- Project Type: Maven

### C#
- IDE: Visual Studio 2017 Community
- Project Type: WPF

## セットアップ

### Javaプログラムだけを実行

- リポジトリからファイルを落とします
- 「JavaSpeechServer」フォルダをNetBeansなどお好みのIDEで開きます
- pom.xmlを確認し、足りないパッケージをmavenから落とします
- 「JavaSpeechServer」内に「credential」フォルダを作ります
- Cloud Speech API（※not Speech API）と関連付けられたGoogle Cloud Platformプロジェクトのサーバーキーを作成し、その時に落としたjsonファイルをcredentialフォルダにコピーします。ファイルの名前は「SpeechTest-d2316e38bee3.json」に変更します（あるいはプログラム内のコードを修正します）
- コンパイルします
- 実行します（NetBeaansでプロジェクトを開いた場合は、プロジェクト設定から実行時オプションを手動で削除してください）
- 2017/4/8時点では、自動でプログラムが終了しないようになっています。音声認識を楽しんだら、NetBeansの右下にある停止ボタンなどからプログラムの実行を止めます

#### 実行時に「Jetty ALPN/NPN has not been properly configured.」が出た時は

- 「JavaSpeechServer」内に「jars」フォルダを作ります
- https://repo.maven.apache.org/maven2/org/mortbay/jetty/alpn/alpn-boot/ から最新verのjarを落として、jarsフォルダにコピーします
- https://repo.maven.apache.org/maven2/org/mortbay/jetty/alpn/jetty-alpn-agent/ から最新verのjarを落として、jarsフォルダにコピーします
- 実行時にjava.exeに与えるコマンドラインオプションに、以下を追加します
```
-Xbootclasspath/p:jars\alpn-boot-8.1.9.v20160720.jar
-javaagent:jars\jetty-alpn-agent-2.0.6.jar
```

### C#プログラムを実行

C#は、Javaのプログラムを呼び出します。<br>
以上の手順を参考に、Javaプログラムをビルドして、「target」フォルダ内に「JavaSpeechServer-1.0-SNAPSHOT-jar-with-dependencies.jar」という名前のファイルを作ります。<br>
NetBeansでは、このプロセスが自動的に実行されることを確認しています。もしNetBeans以外をお使いの場合は、適切な場所に適切な名前のファイルがあるか確認してください。

C#プログラムは、「実行時にJetty ALPN/NPN has not～～が出た時は」の対策が完了していることを前提としています。<br>
Javaプロジェクト内の「jars」フォルダ内に、jarファイルをコピーする必要があります。<br>
なお、java.exe起動時のオプションは、C#プログラム内ですでに設定されていますので不要です。

Visual Studioで、DebugまたはReleaseでプログラムを実行します。
