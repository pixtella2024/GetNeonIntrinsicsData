# GetNeonIntrinsicsData

C# でのスクレイピングのサンプルコードである。  
Arm の開発者用サイトをスクレイピングして [Intrinsic 関数の一覧](https://developer.arm.com/architectures/instruction-sets/intrinsics/) から Neon の Intrinsic 関数のみを取得し、JSON ファイルに出力する。  
Selenium を使用しており Chrome がインストールされている環境で使用することを前提としている。


## 注意点

一部の処理を Python で作成しており、下記のファイルを Pyinstaller 等で exe 化する必要がある。  

- GetNeonIntrinsicsData¥robots_txt_parser¥robots_txt_parser.py

生成した exe ファイル `robots_txt_parser.exe` は C# の exe ファイル`GetNeonIntrinsicsData.exe` と同じディレクトリに移動させる。
