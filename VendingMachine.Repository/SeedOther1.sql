

/**** Testing Account ****/

-- disable all constraints
EXEC sp_msforeachtable "ALTER TABLE ? NOCHECK CONSTRAINT all";

-- ADD IF exists drop table
Create Table tableIds
(	Id int,
	TableName varchar(75)
)

DECLARE @newEventId int;
DECLARE @newAccountId int;

-- Populate Events Table --
Insert Into Events (Name, Description, IsActive) VALUES ('Tony''s Great Testing Event','I''m so awesome.',1);
select @newEventId = SCOPE_IDENTITY()



-- Populate MachineTwitterAccounts Table --
Insert Into MachineTwitterAccounts 
	(
		MachineName
		,MachineDescription
		,ScreenName
		,Password
		,ConsumerKey
		,ConsumerSecret
		,AccessToken
		,AccessTokenSecret
		,HashTag
		,CodeReplyMessage
		,IsActive
		,EventId
	) 
VALUES 
	(
		'TonyTesting'
		,'Testing Account/Machine for Tony'
		,'BluewaterTonyG'
		,'11111'
		,'11111'
		,'11111'
		,'11111-11111'
		,'11111'
		,'bwtTonytest'
		,'Your Tony special code is {0}.'
		,1
		,@newEventId
	) 
select @newAccountId = SCOPE_IDENTITY()


Insert Into ReplyTweets (Tweet, EventId, IsActive) VALUES('Special Code Response Alpha is {0}.', @newEventId, 1);
Insert Into ReplyTweets (Tweet, EventId, IsActive) VALUES('Bravo Special Code {0}. Thanks', @newEventId, 1);
Insert Into ReplyTweets (Tweet, EventId, IsActive) VALUES('{0} is your Charlie Code.', @newEventId, 1);
Insert Into ReplyTweets (Tweet, EventId, IsActive) VALUES('Delta Code is {0}.', @newEventId, 1);
Insert Into ReplyTweets (Tweet, EventId, IsActive) VALUES('Echo Code {0}', @newEventId, 1);



/*
Declare @newAccountId  int;
Declare @newEventId  int;
select @newAccountId = 2
select @newEventId = 2

--Delete from Codes where DenormalizedEventId > 1;
*/

-- Populate Codes Table --
Insert Into Codes (CodeValue,Description,IsActive,DenormalizedEventId,MachineTwitterAccountId) Values ('A10001','',1,@newEventId,@newAccountId);
Insert Into Codes (CodeValue,Description,IsActive,DenormalizedEventId,MachineTwitterAccountId) Values ('A10012','',1,@newEventId,@newAccountId);
Insert Into Codes (CodeValue,Description,IsActive,DenormalizedEventId,MachineTwitterAccountId) Values ('A10020','',1,@newEventId,@newAccountId);
Insert Into Codes (CodeValue,Description,IsActive,DenormalizedEventId,MachineTwitterAccountId) Values ('A10031','',1,@newEventId,@newAccountId);
Insert Into Codes (CodeValue,Description,IsActive,DenormalizedEventId,MachineTwitterAccountId) Values ('A10042','',1,@newEventId,@newAccountId);
Insert Into Codes (CodeValue,Description,IsActive,DenormalizedEventId,MachineTwitterAccountId) Values ('A10050','',1,@newEventId,@newAccountId);
Insert Into Codes (CodeValue,Description,IsActive,DenormalizedEventId,MachineTwitterAccountId) Values ('A10061','',1,@newEventId,@newAccountId);
Insert Into Codes (CodeValue,Description,IsActive,DenormalizedEventId,MachineTwitterAccountId) Values ('A10072','',1,@newEventId,@newAccountId);
Insert Into Codes (CodeValue,Description,IsActive,DenormalizedEventId,MachineTwitterAccountId) Values ('A10080','',1,@newEventId,@newAccountId);
Insert Into Codes (CodeValue,Description,IsActive,DenormalizedEventId,MachineTwitterAccountId) Values ('A10091','',1,@newEventId,@newAccountId);
Insert Into Codes (CodeValue,Description,IsActive,DenormalizedEventId,MachineTwitterAccountId) Values ('A10102','',1,@newEventId,@newAccountId);
Insert Into Codes (CodeValue,Description,IsActive,DenormalizedEventId,MachineTwitterAccountId) Values ('A10110','',1,@newEventId,@newAccountId);
Insert Into Codes (CodeValue,Description,IsActive,DenormalizedEventId,MachineTwitterAccountId) Values ('A10121','',1,@newEventId,@newAccountId);
Insert Into Codes (CodeValue,Description,IsActive,DenormalizedEventId,MachineTwitterAccountId) Values ('A10132','',1,@newEventId,@newAccountId);
Insert Into Codes (CodeValue,Description,IsActive,DenormalizedEventId,MachineTwitterAccountId) Values ('A10151','',1,@newEventId,@newAccountId);
Insert Into Codes (CodeValue,Description,IsActive,DenormalizedEventId,MachineTwitterAccountId) Values ('A10162','',1,@newEventId,@newAccountId);
Insert Into Codes (CodeValue,Description,IsActive,DenormalizedEventId,MachineTwitterAccountId) Values ('A10170','',1,@newEventId,@newAccountId);
Insert Into Codes (CodeValue,Description,IsActive,DenormalizedEventId,MachineTwitterAccountId) Values ('A10181','',1,@newEventId,@newAccountId);
Insert Into Codes (CodeValue,Description,IsActive,DenormalizedEventId,MachineTwitterAccountId) Values ('A10192','',1,@newEventId,@newAccountId);
Insert Into Codes (CodeValue,Description,IsActive,DenormalizedEventId,MachineTwitterAccountId) Values ('A10200','',1,@newEventId,@newAccountId);
Insert Into Codes (CodeValue,Description,IsActive,DenormalizedEventId,MachineTwitterAccountId) Values ('A10211','',1,@newEventId,@newAccountId);
Insert Into Codes (CodeValue,Description,IsActive,DenormalizedEventId,MachineTwitterAccountId) Values ('A10222','',1,@newEventId,@newAccountId);

-- enable all constraints
exec sp_msforeachtable @command1="print '?'", @command2="ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all"

