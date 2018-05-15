USE Dependencies
GO

IF OBJECT_ID('projectdependency', 'U') IS NOT NULL
  DROP TABLE projectdependency
GO

CREATE TABLE projectdependency
(
	Id int NOT NULL,
    ProjectFromId int NOT NULL,
    ProjectToId int NOT NULL,
    Version nvarchar(50) NULL,
    TargetFramework nvarchar(50) NULL,
    Type nvarchar(50) NULL
)  ON [PRIMARY]
GO

ALTER TABLE projectdependency ADD CONSTRAINT
	PK_projectdependency PRIMARY KEY CLUSTERED 
	(
		Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
