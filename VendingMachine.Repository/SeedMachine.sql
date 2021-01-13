-- disable all constraints
EXEC sp_msforeachtable "ALTER TABLE ? NOCHECK CONSTRAINT all";

DECLARE @newEventId int;
DECLARE @newAccountId int;
DECLARE @newAccountId2 int;


-- Populate MachineTwitterAccounts Table --
Insert Into TwitterAccounts 
	(
		ScreenName
		,Password
		,ConsumerKey
		,ConsumerSecret
		,AccessToken
		,AccessTokenSecret
		,IsActive
	) 
VALUES 
	(
		'ChevyAutoShows'
		,'!Bluewater13!'
		,'rJtq6NCLlgo2B7UPoVQqVg'
		,'VlyxWYoUyCel04OHQfqEJiok8etYffqHeXJRnTdWg'
		,'1852450070-a6QSkTzbJ7ERrA4VABoTkF2VMkVBP8GDQBfxVff'
		,'h9AFlaexP1kQcwQrfr9efPXTGZXiXiSKWTZ0GdwvA'
		,1
	) 
select @newAccountId2 = SCOPE_IDENTITY();



SELECT top 1 @newEventId = Id FROM tableIds WHERE TableName = 'Events';

-- Populate MachineTwitterAccounts Table --
Insert Into MachineTwitterAccounts 
	(
		MachineName
		,MachineDescription
		,HashTag
		,CodeReplyMessage
		,IsActive
		,EventId
		,TwitterAccountId
	) 
VALUES 
	(
		'Machine2'
		,'Machine for OC Autoshow'
		,'chevrolet'
		,'Thank you for your interest in Chevrolet. Your vending machine code is {0}.'
		,1
		,@newEventId
		,@newAccountId2
	) 
select @newAccountId = SCOPE_IDENTITY();

insert into tableIds Select Id = @newAccountId, TableName = 'MachineTwitterAccounts';

-- enable all constraints
exec sp_msforeachtable @command1="print '?'", @command2="ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all"
