# Durable Function Sample

* [Durable Functions とは](https://learn.microsoft.com/ja-jp/azure/azure-functions/durable/durable-functions-overview?tabs=in-process%2Cnodejs-v3%2Cv1-model&pivots=csharp)

## 環境

* .NET 8.0.407
* Azure Functions Core Tools 4.1.0
* Azurite 3.35.0 ※Visual Studio Code拡張

## ローカルでのデバッグ実行

### 事前準備

* local.settings.json を作成

``` json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "Activity3Condition:Enabled3_1": true,
    "Activity3Condition:Count3_1": 2,
    "Activity3Condition:Enabled3_2": true,
    "Activity3Condition:Count3_2": 3,
    "Activity3Condition:Enabled3_3": true,
    "Activity3Condition:Count3_3": 5
  },
  "ConnectionStrings": {
    "BlobStorage": "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;"
  }
}
```

* （任意）作業環境に合わせて各種設定ファイルを調整
  * sqlserver.env
  * docker-compose.yaml
  * local.settings.json

* SQL Serverの起動

``` bash
docker compose up -d
```
<!-- 
* （初回のみ）DBに接続し、DDL.sqlを実行（任意のクライアントツールで接続、実行）

``` bash
# sqlcmd.exe で実行する場合 
sqlcmd -S localhost -U sa -P sqlpass123! -i ./DDL.sql
``` -->

* （初回のみ）dotnet-ef によるデータベース作成

```
dotnet ef migrations add InitialCreate
dotnet ef database update -- 'Data Source=tcp:localhost, 1433;Initial Catalog=dfuncdb;User ID=sa;Password=sqlpass123!;TrustServerCertificate=True'
```

https://learn.microsoft.com/ja-jp/ef/core/get-started/overview/first-app?tabs=netcore-cli
https://learn.microsoft.com/ja-jp/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli

* （初回のみ）SQL Serverの接続用シークレット作成

``` bash
dotnet user-secrets set 'ConnectionStrings:DurableFunctionsSampleDb' 'Data Source=tcp:localhost, 1433;Initial Catalog=dfuncdb;User ID=sa;Password=sqlpass123!;TrustServerCertificate=True'
```

* Azuriteの起動 - コマンドパレットを起動し、`Azurite: Start` を実行

### デバッグ実行

* VSCodeの場合、実行とデバッグ（Ctrl+Shift+D）より、`Attach to .NET Functions` を起動
* 任意のブラウザで以下のURLにアクセス

``` url
http://localhost:7071/api/start/{fileName}
```

## Azure環境での実行

### 事前準備

TODO: Azure環境での準備手順

### 実行

TODO: Azure環境での実行手順

## プログラム仕様

### 概要

* HTTPトリガーによって起動する Durable Functions 実装
* Durable Functionsにおける特徴的な機能を実行する
  * 複数の処理を同時実行（ファンイン/ファンアウト）
  * DB、Blobストレージへのアクセス検証用機能
  * ログ出力

### 処理フロー

``` mermaid
graph TD;
    Start[開始] --> Activity1[アクティビティ関数1];
    Activity1 --> Activity2[アクティビティ関数2];
    Activity1 -- エラー --> Error[エラー]
    Activity2 --> Activity3-1Condition{3-1開始要否};
    Activity2 -- エラー --> Error[エラー]
    Activity3-1Condition -- はい--> Activity3-1[アクティビティ関数3-1];
    Activity3-1Condition -- いいえ--> Activity3Check;
    Activity3-1 --> Activity3Check;
    Activity2 --> Activity3-2Condition{3-2開始要否};
    Activity3-2Condition -- はい --> Activity3-2[アクティビティ関数3-2];
    Activity3-2Condition -- いいえ --> Activity3Check;
    Activity3-2 --> Activity3Check;
    Activity2 --> Activity3-3Condition{3-3開始要否};
    Activity3-3Condition -- はい --> Activity3-3[アクティビティ関数3-3];
    Activity3-3Condition -- いいえ --> Activity3Check;
    Activity3-3 --> Activity3Check{関数3結果チェック};
    Activity3Check --> Activity4[アクティビティ関数4]
    Activity3Check -- エラー --> Error[エラー]
    Activity4 --> End[正常終了];
    Activity4 -- エラー --> Error[エラー]

```

* アクティビティ関数1（SqlClientの検証）
  * ファイル名、実行日時を記録
* アクティビティ関数2（IConfigurationの検証）
  * アクティビティ関数3の実行条件を環境設定から設定
* アクティビティ関数3 環境変数の値に応じて、実行要否、待機時間を決定
  * アクティビティ関数3-1 ～ アクティビティ関数3-3 でそれぞれ指定された時間待機を実行
  * 待機している間、1秒ごとにログを出力
* アクティビティ関数4
  * [https://example.com](https://example.com) へのアクセス（HttpClientの検証）
  * ファイルのアップロード(BlobServiceClientの検証)
    * アクセスしたURLのコンテンツをファイル内容として記録
    * URLで指定したファイル名でアップロード

## 基本概念

### 実行モード

* 1 つは Functions ホスト ランタイムと同じプロセス内 ("インプロセス") で実行、もう 1 つは分離ワーカー プロセス内で実行する
* [分離ワーカー モデルの利点](https://learn.microsoft.com/ja-jp/azure/azure-functions/dotnet-isolated-process-guide?tabs=ihostapplicationbuilder%2Cwindows#benefits-of-the-isolated-worker-model)
  * 関数は独立したプロセスの中で実行される
  * DI（Dependency Injection）を利用するためには、分離ワーカーモデルである必要がある（インプロセスだと不可）
* 現状は分離ワーカーモデルで実装するのが推奨

### 持続的オーケストレーション

<!--TODO: 持続的オーケストレーションについてポイントをまとめる-->
* [持続的オーケストレーション](https://learn.microsoft.com/ja-jp/azure/azure-functions/durable/durable-functions-orchestrations?source=recommendations&tabs=csharp-inproc)

### 構成

* [構成](https://learn.microsoft.com/ja-jp/azure/azure-functions/dotnet-isolated-process-guide?tabs=ihostapplicationbuilder%2Cwindows#configuration)

* [Durable Functions ストレージ プロバイダー](https://learn.microsoft.com/ja-jp/azure/azure-functions/durable/durable-functions-storage-providers)
* [クイック スタート: MSSQL ストレージ プロバイダーを使用する Durable Functions アプリを作成する](https://learn.microsoft.com/ja-jp/azure/azure-functions/durable/quickstart-mssql)
  * Durable Functions はアプリケーションの状態、チェックポイント、再起動をDBで管理（デフォルトはBlobストレージ）
  * [複数のストレージプロバイダー](https://learn.microsoft.com/ja-jp/azure/azure-functions/durable/durable-functions-storage-providers)がサポートされている

### DB, Storageの接続

* [Azure Functions での接続の管理](https://learn.microsoft.com/ja-jp/azure/azure-functions/manage-connections?tabs=csharp)
  * 従量課金プランで多くの関数が同時に実行されている場合、利用可能な接続が不足する可能性がある。
  * 必要以上に多くの接続を使用しないように関数をコーディングする必要がある。
  * 関数コードでは、SQL リレーショナル データベースに接続するために、.NET Framework Data Provider for SQL Server (SqlClient) を使用できる

* [Visual Studio Code を使用して Azure Functions を Azure SQL Database に接続する](https://learn.microsoft.com/ja-jp/azure/azure-functions/functions-add-output-binding-azure-sql-vs-code?pivots=programming-language-csharp)

### エラー処理

* (Durable Functions のエラー処理 (Azure Functions))[https://learn.microsoft.com/ja-jp/azure/azure-functions/durable/durable-functions-error-handling?tabs=csharp-inproc]
  * ※注意：リトライの実装などが、おそらくインプロセスモードでのサンプルになっている。プロセス分離モード（`TaskOrchestrationContext`での実装）で対応するメソッドは、[こちら](https://learn.microsoft.com/ja-jp/azure/azure-functions/durable/durable-functions-dotnet-isolated-overview#public-api-changes)を参照
  * アクティビティ関数でスローされた例外はオーケストレーター関数に戻され、`FunctionFailedException` としてスローされる

### ファンイン/ファンアウト

* [Durable Functions のファンアウト/ファンイン シナリオ - クラウド バックアップの例](https://learn.microsoft.com/ja-jp/azure/azure-functions/durable/durable-functions-cloud-backup?tabs=csharp)
  * "ファンアウト/ファンイン" は、複数の関数を同時に実行した後、その結果に対して集計を行うパターン

## テスト実装

* [Durable Functions 単体テスト (C# Isolated)](https://learn.microsoft.com/ja-jp/azure/azure-functions/durable/durable-functions-unit-testing-dotnet-isolated)
* [実稼働データベース システムを使用しないテスト](https://learn.microsoft.com/ja-jp/ef/core/testing/testing-without-the-database)


## その他

* [Durable Functions のバインド (Azure Functions)](https://learn.microsoft.com/ja-jp/azure/azure-functions/durable/durable-functions-bindings?tabs=python-v2%2Cin-process%2C2x-durable-functions&pivots=programming-language-csharp)

* [Visual Studio Code で Azure Functions のデバッグ終了後に再デバッグ出来ない問題](https://zenn.dev/microsoft/articles/azure-functions-vscode-debugbug)
  * 問題の原因は func.exe が終了していないことで起きている
  * タスクマネージャーから func という名前のプロセスを探して終了させることで再度デバッグ実行が出来る
  * この作業を自動化する方法が該当の [issue](https://github.com/microsoft/vscode-azurefunctions/issues/4416) で EvilConsultant さんから提案されている

* [SQLServerをdocker-composeで使いたい！](https://qiita.com/y-yoshizawa/items/4535c06eaa0245a6cd0d)

https://learn.microsoft.com/ja-jp/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli

## 参考資料

* [クイック スタート: C# Durable Functions アプリを作成する](https://learn.microsoft.com/ja-jp/azure/azure-functions/durable/durable-functions-isolated-create-first-csharp?pivots=code-editor-vscode)
* [.NET Isolated 版の Durable Functions を使った開発を始める](https://blog.shibayan.jp/entry/20250106/1736147619)
* [Durable Functions の型と機能](https://learn.microsoft.com/ja-jp/azure/azure-functions/durable/durable-functions-types-features-overview)
* [Durable Functions での関数チェーン - Hello シーケンス サンプル](https://learn.microsoft.com/ja-jp/azure/azure-functions/durable/durable-functions-sequence?tabs=csharp)
* [Core Tools を使用してローカルで Azure Functions を開発する](https://learn.microsoft.com/ja-jp/azure/azure-functions/functions-run-local?tabs=windows%2Cisolated-process%2Cnode-v4%2Cpython-v2%2Chttp-trigger%2Ccontainer-apps&pivots=programming-language-csharp)
* [Azure Functions の分離ワーカー モデルでユーザー シークレットを使う方法](https://zenn.dev/microsoft/articles/isolated-functions-user-secret)
