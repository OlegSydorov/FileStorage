create database FileStorage


CREATE TABLE Users (UserId INT IDENTITY PRIMARY KEY, 
					Login VARCHAR(40),
					Password VARCHAR (10),
					Registration DateTime,
					Status VARCHAR(10),
					Files INT,
					Bytes BIGINT)

SELECT*FROM Users
DELETE Users
DROP TABLE Users