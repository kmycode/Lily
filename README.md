# Lily

Google Cloud Speech APIを利用したリアルタイム音声認識を、JavaとC#のプロセス間通信を利用してWPFで動かそうというリポジトリです。<br>
成功するかどうかは分かりません。<br>
開発はdevelopブランチで進めています。適宜ブランチの切り替えをお願いします。

## 開発環境

### Java
- IDE: NetBeans 8.2
- Project Type: Maven

### C#
- IDE: Visual Studio 2017 Community
- Project Type: WPF

## セットアップ

### Java

- リポジトリからファイルを落とします
- 「JavaSpeechServer」フォルダをNetBeansなどお好みのIDEで開きます
- pom.xmlを確認し、足りないパッケージをmavenから落とします
- 「JavaSpeechServer」内に「credential」フォルダを作ります
- Cloud Speech API（※not Speech API）と関連付けられたGoogle Cloud Platformプロジェクトのサーバーキーを作成し、その時に落としたjsonファイルをcredentialフォルダにコピーします。ファイルの名前は「SpeechTest-d2316e38bee3.json」に変更します（あるいはプログラム内のコードを修正します）
- コンパイルします
- 実行します（NetBeaansプロジェクトでプロジェクトを開いた場合は、実行時オプションを手動で削除してください）
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
