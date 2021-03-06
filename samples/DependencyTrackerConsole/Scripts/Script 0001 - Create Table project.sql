﻿USE Dependencies
GO

IF OBJECT_ID('project', 'U') IS NOT NULL
  DROP TABLE project
GO

CREATE TABLE project
(
	Id int NOT NULL,
	Name nvarchar(150) NOT NULL
)  ON [PRIMARY]
GO

ALTER TABLE project ADD CONSTRAINT
	PK_project PRIMARY KEY CLUSTERED 
	(
		Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
