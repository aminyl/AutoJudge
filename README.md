## テストケースを自動実行

TextBox左:テストケース  
TextBox右:正解  
右上の■を右クリック→モード切替  
■:ウィンドウが最前面になるたびにそのタブ内全てをジャッジ  
更新マーク:マークをクリックすればそのタブ内全てをジャッジ  
ごみ箱マーク:そのタブ内のテストケースを削除  
背景画像変更可能  

### 設定ファイルについて  
実行ファイルと同じ場所に"settings.txt"という名前で作成  
例：  
[code file path] // デフォルトは".\\"  
.\ruby\  
[test case path] // デフォルトは"TestCases.txt"  
out\TestCases.txt  
[background image info] // 背景画像のファイル名と背景色, なければ読まない  
BackgroundImage_sized.png  
1  
0.9  
1  
0.5  
1
