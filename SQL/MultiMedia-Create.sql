IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[Albums]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	DROP TABLE [dbo].[Albums]
GO
CREATE TABLE [dbo].[Albums]
(
  [A_ID] [int] IDENTITY(1,1) NOT NULL,
  [A_Name] [varchar](32) NOT NULL,
  [A_Lp] [int] NOT NULL,
 CONSTRAINT [PK_Albums] PRIMARY KEY CLUSTERED
(
  [A_ID] ASC
)
)
ON [PRIMARY]
GO

CREATE UNIQUE NONCLUSTERED INDEX [NDX_A_ByName] ON [dbo].[Albums]
(
  [A_Name] ASC
)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name='FK_I2V' AND xtype='F')
  ALTER TABLE [dbo].[Items] DROP CONSTRAINT [FK_I2V];
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[Views]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
  DROP TABLE [dbo].[Views]
GO
CREATE TABLE [dbo].[Views]
(
  [V_ID] [int] IDENTITY(1,1) NOT NULL,
  [V_Name] [varchar](64) NOT NULL,
  [V_IsHidden] [tinyint] DEFAULT(0),
 CONSTRAINT [PK_Views] PRIMARY KEY CLUSTERED
(
  [V_ID] ASC
)
)
ON [PRIMARY]
GO

CREATE UNIQUE NONCLUSTERED INDEX [NDX_V_ByName] ON [dbo].[Views]
(
  [V_Name] ASC,
  [V_ID] ASC
)
GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[ViewsLinks]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
  DROP TABLE [dbo].[ViewsLinks]
GO
CREATE TABLE [dbo].[ViewsLinks]
(
  [VL_ID] [int] IDENTITY(1,1) NOT NULL,
  [VL_ParentAID] [int] NOT NULL,
  [VL_ParentVID] [int] NULL,
  [VL_VID] [int] NOT NULL,
  [VL_Name] [varchar](64) NULL,
  [VL_Lp] [int] NOT NULL
 CONSTRAINT [PK_ViewsLinks] PRIMARY KEY CLUSTERED
(
  [VL_ID] ASC
)
)
ON [PRIMARY]
GO

CREATE UNIQUE NONCLUSTERED INDEX [NDX_VL_ByParent] ON [dbo].[ViewsLinks]
(
  [VL_ParentAID] ASC,
  [VL_ParentVID] ASC,
  [VL_VID] ASC
)
GO

CREATE UNIQUE NONCLUSTERED INDEX [NDX_VL_ByLp] ON [dbo].[ViewsLinks]
(
  [VL_ParentAID] ASC,
  [VL_ParentVID] ASC,
  [VL_Lp] ASC,
  [VL_VID] ASC
)
GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[Items]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
  DROP TABLE [dbo].[Items]
GO
CREATE TABLE [dbo].[Items]
(
  [I_ID] [int] IDENTITY(1,1) NOT NULL,
  [I_VID] [int] NOT NULL,
  [I_FileSpec] [varchar](255) NOT NULL,
  [I_FileName] [varchar](255) NOT NULL,
  [I_Lp] [int] NOT NULL,
  [I_Quality] [smallint] DEFAULT(0),
  [I_IsArt] [tinyint] DEFAULT(0),
  [I_IsHidden] [tinyint] DEFAULT(0),
  [I_IsImportant] [tinyint] DEFAULT(0),
CONSTRAINT [PK_Items] PRIMARY KEY CLUSTERED
(
  [I_ID] ASC
)
)
ON [PRIMARY]
GO

CREATE UNIQUE NONCLUSTERED INDEX [NDX_I_ByLp] ON [dbo].[Items]
(
  [I_VID] ASC,
  [I_Lp] ASC,
  [I_ID] ASC
)
GO

ALTER TABLE [dbo].[Items] ADD CONSTRAINT [FK_I2V]
FOREIGN KEY
(
  [I_VID]
)
REFERENCES [dbo].[Views]
(
  [V_ID]
)
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Items] CHECK CONSTRAINT [FK_I2V]
GO


IF EXISTS (SELECT * FROM sysobjects Where name = 'RenumViewItemsLP' And xtype = 'P')
	DROP PROCEDURE [dbo].[RenumViewItemsLP]
GO
CREATE PROCEDURE [dbo].[RenumViewItemsLP] @ViewID INT
AS
BEGIN
  SET NOCOUNT ON

  DECLARE @I_ID INT, @Lp INT

  SELECT * INTO #ViewItems FROM dbo.Items WHERE I_VID = @ViewID ORDER BY I_Lp, I_FileName

  DECLARE c CURSOR LOCAL FAST_FORWARD READ_ONLY FOR
    SELECT I_ID FROM #ViewItems
  OPEN c
  SET @Lp = 1
  WHILE 1=1
  BEGIN
    FETCH NEXT FROM c INTO @I_ID
    IF @@FETCH_STATUS <> 0 BREAK
    UPDATE dbo.Items SET I_Lp = @Lp WHERE I_ID = @I_ID
    SET @Lp = @Lp + 1
  END
  CLOSE c
  DEALLOCATE c

  DROP TABLE #ViewItems

  SET NOCOUNT OFF
END
GO

IF EXISTS (SELECT * FROM sysobjects Where name = 'RenumViewItemsMMDD' And xtype = 'P')
	DROP PROCEDURE [dbo].[RenumViewItemsMMDD]
GO
CREATE PROCEDURE [dbo].[RenumViewItemsMMDD] @ViewID INT
AS
BEGIN
  SET NOCOUNT ON

  DECLARE @I_ID INT, @Lp INT

  SELECT * INTO #ViewItems FROM dbo.Items WHERE I_VID = @ViewID ORDER BY SUBSTRING(I_FileName, 6, 5)

  DECLARE c CURSOR LOCAL FAST_FORWARD READ_ONLY FOR
    SELECT I_ID FROM #ViewItems
  OPEN c
  SET @Lp = 1
  WHILE 1=1
  BEGIN
    FETCH NEXT FROM c INTO @I_ID
    IF @@FETCH_STATUS <> 0 BREAK
    UPDATE dbo.Items SET I_Lp = @Lp WHERE I_ID = @I_ID
    SET @Lp = @Lp + 1
  END
  CLOSE c
  DEALLOCATE c

  DROP TABLE #ViewItems

  SET NOCOUNT OFF
END
GO

IF EXISTS (SELECT * FROM sysobjects Where name = 'RenumViewItemsFNAME' And xtype = 'P')
	DROP PROCEDURE [dbo].[RenumViewItemsFNAME]
GO
CREATE PROCEDURE [dbo].[RenumViewItemsFNAME] @ViewID INT
AS
BEGIN
  SET NOCOUNT ON

  DECLARE @I_ID INT, @Lp INT

  SELECT * INTO #ViewItems FROM dbo.Items WHERE I_VID = @ViewID ORDER BY I_FileName

  DECLARE c CURSOR LOCAL FAST_FORWARD READ_ONLY FOR
    SELECT I_ID FROM #ViewItems
  OPEN c
  SET @Lp = 1
  WHILE 1=1
  BEGIN
    FETCH NEXT FROM c INTO @I_ID
    IF @@FETCH_STATUS <> 0 BREAK
    UPDATE dbo.Items SET I_Lp = @Lp WHERE I_ID = @I_ID
    SET @Lp = @Lp + 1
  END
  CLOSE c
  DEALLOCATE c

  DROP TABLE #ViewItems

  SET NOCOUNT OFF
END
GO


IF EXISTS (SELECT * FROM sysobjects Where name = 'RenumAlbums' And xtype = 'P')
  DROP PROCEDURE [dbo].[RenumAlbums]
GO
CREATE PROCEDURE [dbo].[RenumAlbums]
AS
BEGIN
  SET NOCOUNT ON

  DECLARE @A_ID INT, @newID INT, @A_Lp INT, @Lp INT
  DECLARE @A_Name VARCHAR(255)

  SELECT * INTO #Albums FROM dbo.Albums ORDER BY A_Lp, A_Name

  DECLARE c CURSOR LOCAL FAST_FORWARD READ_ONLY FOR
    SELECT A_ID, A_Name FROM #Albums
  OPEN c
  SET @Lp = 1
  WHILE 1=1
  BEGIN
    FETCH NEXT FROM c INTO @A_ID, @A_Name
    IF @@FETCH_STATUS <> 0 BREAK
    SET @newID = @Lp
    SET IDENTITY_INSERT dbo.Albums ON
    INSERT INTO dbo.Albums (A_ID, A_Name, A_Lp) VALUES(-@newID, '_' + @A_Name, @Lp)
    UPDATE dbo.ViewsLinks SET VL_ParentAID = -@newID WHERE VL_ParentAID = @A_ID
    SET IDENTITY_INSERT dbo.Albums OFF
    SET @Lp = @Lp + 1
  END
  CLOSE c
  DEALLOCATE c
  DROP TABLE #Albums

  DELETE dbo.Albums WHERE A_ID > 0

  SELECT * INTO #Albums2 FROM dbo.Albums ORDER BY A_Lp, A_Name

  DECLARE c CURSOR LOCAL FAST_FORWARD READ_ONLY FOR
    SELECT A_ID, A_Name, A_Lp FROM #Albums2
  OPEN c
  WHILE 1=1
  BEGIN
    FETCH NEXT FROM c INTO @A_ID, @A_Name, @A_Lp
    IF @@FETCH_STATUS <> 0 BREAK
    SET IDENTITY_INSERT dbo.Albums ON
    INSERT INTO dbo.Albums (A_ID, A_Name, A_Lp) VALUES(-@A_ID, SUBSTRING(@A_Name, 2, LEN(@A_Name)-1), @A_Lp)
    SET IDENTITY_INSERT dbo.Albums OFF
  END
  CLOSE c
  DEALLOCATE c
  DROP TABLE #Albums2

  UPDATE dbo.ViewsLinks SET VL_ParentAID = -VL_ParentAID WHERE VL_ParentAID < 0

  DELETE dbo.Albums WHERE A_ID < 0

  DECLARE @NextID INT
  SELECT @NextID = MAX(A_ID) FROM dbo.Albums
	DBCC CHECKIDENT('Albums', RESEED, @NextID)

  SET NOCOUNT OFF
END
GO


IF EXISTS (SELECT * FROM sysobjects Where name = 'RenumViews' And xtype = 'P')
  DROP PROCEDURE [dbo].[RenumViews]
GO
CREATE PROCEDURE [dbo].[RenumViews]
AS
BEGIN
  SET NOCOUNT ON

  DECLARE @V_ID INT, @newID INT, @V_IsHidden INT
  DECLARE @V_Name VARCHAR(255)

  SELECT * INTO #Views FROM dbo.Views ORDER BY V_ID

  DECLARE c CURSOR LOCAL FAST_FORWARD READ_ONLY FOR
    SELECT V_ID, V_Name, V_IsHidden FROM #Views
  OPEN c
  SET @newID = 1
  WHILE 1=1
  BEGIN
    FETCH NEXT FROM c INTO @V_ID, @V_Name, @V_IsHidden
    IF @@FETCH_STATUS <> 0 BREAK
    SET IDENTITY_INSERT dbo.Views ON
    INSERT INTO dbo.Views (V_ID, V_Name, V_IsHidden) VALUES(-@newID, '_' + @V_Name, @V_IsHidden)
    UPDATE dbo.ViewsLinks SET VL_ParentVID = -@newID WHERE VL_ParentVID = @V_ID
    UPDATE dbo.ViewsLinks SET VL_VID = -@newID WHERE VL_VID = @V_ID
    UPDATE dbo.Items SET I_VID = -@newID WHERE I_VID = @V_ID
    SET IDENTITY_INSERT dbo.Views OFF
    SET @newID = @newID + 1
  END
  CLOSE c
  DEALLOCATE c
  DROP TABLE #Views

  DELETE dbo.Views WHERE V_ID > 0

  SELECT * INTO #Views2 FROM dbo.Views ORDER BY V_ID DESC

  DECLARE c CURSOR LOCAL FAST_FORWARD READ_ONLY FOR
    SELECT V_ID, V_Name, V_IsHidden FROM #Views2
  OPEN c
  WHILE 1=1
  BEGIN
    FETCH NEXT FROM c INTO @V_ID, @V_Name, @V_IsHidden
    IF @@FETCH_STATUS <> 0 BREAK
    SET IDENTITY_INSERT dbo.Views ON
    INSERT INTO dbo.Views (V_ID, V_Name, V_IsHidden) VALUES(-@V_ID, SUBSTRING(@V_Name, 2, LEN(@V_Name)-1), @V_IsHidden)
    SET IDENTITY_INSERT dbo.Views OFF
  END
  CLOSE c
  DEALLOCATE c
  DROP TABLE #Views2

  UPDATE dbo.ViewsLinks SET VL_ParentVID = -VL_ParentVID WHERE VL_ParentVID < 0
  UPDATE dbo.ViewsLinks SET VL_VID = -VL_VID WHERE VL_VID < 0
  UPDATE dbo.Items SET I_VID = -I_VID WHERE I_VID < 0

  DELETE dbo.Views WHERE V_ID < 0

  DECLARE @NextID INT
  SELECT @NextID = MAX(V_ID) FROM dbo.Views
  DBCC CHECKIDENT('Views', RESEED, @NextID)

  SET NOCOUNT OFF
END
GO

IF EXISTS (SELECT * FROM sysobjects Where name = 'ReLpSubViews' And xtype = 'P')
  DROP PROCEDURE [dbo].[ReLpSubViews]
GO

CREATE PROCEDURE [dbo].[ReLpSubViews] @AID INT, @VID INT
AS
BEGIN
  SET NOCOUNT ON

  SELECT * INTO #SubLinks FROM dbo.ViewsLinks WHERE VL_ParentAID = @AID AND VL_ParentVID = @VID ORDER BY VL_Lp

	DECLARE @VL_ID INT, @Lp INT
  DECLARE c CURSOR LOCAL FAST_FORWARD READ_ONLY FOR
    SELECT VL_ID FROM #SubLinks
  OPEN c
  SET @Lp = 1
  WHILE 1=1
  BEGIN
    FETCH NEXT FROM c INTO @VL_ID
    IF @@FETCH_STATUS <> 0 BREAK
    UPDATE dbo.ViewsLinks SET VL_Lp = @Lp WHERE VL_ID = @VL_ID
    SET @Lp = @Lp + 1
  END
  CLOSE c
  DEALLOCATE c

  DROP TABLE #SubLinks

  SET NOCOUNT OFF
END
GO

IF EXISTS (SELECT * FROM sysobjects Where name = 'ReLpItems' And xtype = 'P')
  DROP PROCEDURE [dbo].[ReLpItems]
GO

CREATE PROCEDURE [dbo].[ReLpItems] @VID INT
AS
BEGIN
  SET NOCOUNT ON

  SELECT * INTO #Items FROM dbo.Items WHERE I_VID = @VID ORDER BY I_Lp

	DECLARE @I_ID INT, @Lp INT
  DECLARE c CURSOR LOCAL FAST_FORWARD READ_ONLY FOR
    SELECT I_ID FROM #Items
  OPEN c
  SET @Lp = 1
  WHILE 1=1
  BEGIN
    FETCH NEXT FROM c INTO @I_ID
    IF @@FETCH_STATUS <> 0 BREAK
    UPDATE dbo.Items SET I_Lp = @Lp WHERE I_ID = @I_ID
    SET @Lp = @Lp + 1
  END
  CLOSE c
  DEALLOCATE c

  DROP TABLE #Items

  SET NOCOUNT OFF
END
GO
