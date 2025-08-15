create database dfuncdb
GO

USE dfuncdb;
GO

create schema dfunc
GO

-- TODO: テーブル作成
CREATE SEQUENCE dfunc.Seq_ProcessStart
    AS BIGINT
    START WITH 1
    INCREMENT BY 1;

-- テーブル作成
CREATE TABLE dfunc.ProcessStartLog (
    Id BIGINT NOT NULL PRIMARY KEY,       -- シーケンス番号
    FileName NVARCHAR(max),               -- ファイル名
    StartTime DATETIME2 NOT NULL          -- 開始日時
);
GO


-- ※データベース削除用
-- USE master;
-- GO;

-- drop database dfuncdb;
--