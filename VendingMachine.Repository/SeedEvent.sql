-- disable all constraints
EXEC sp_msforeachtable "ALTER TABLE ? NOCHECK CONSTRAINT all";

-- ADD IF exists drop table
Create Table tableIds
(	Id int,
	TableName varchar(75)
)

DECLARE @newEventId int;

-- Populate Events Table --
Insert Into Events (Name, Description, IsActive) VALUES ('Orange County Intl Auto Show','',1);
select @newEventId = SCOPE_IDENTITY()


insert into tableIds Select Id = @newEventId, TableName = 'Events';


Insert Into ReplyTweets (Tweet, EventId, IsActive) VALUES('Thank you for your interest in Chevrolet. Your Chevy Vending Machine code for the Orange County Autoshow is {0}.', @newEventId, 1);
Insert Into ReplyTweets (Tweet, EventId, IsActive) VALUES('Congratulations! Your vending machine code is {0}. Thanks for visiting Chevrolet at the Orange County Autoshow.', @newEventId, 1);
Insert Into ReplyTweets (Tweet, EventId, IsActive) VALUES('Success! Enter code {0} on the Chevrolet vending machine at the Orange County Autoshow to receive your prize.', @newEventId, 1);
Insert Into ReplyTweets (Tweet, EventId, IsActive) VALUES('Your Chevy Vending Machine code is {0}. Thank you for your interest in Chevrolet at the Orange County Autoshow.', @newEventId, 1);
Insert Into ReplyTweets (Tweet, EventId, IsActive) VALUES('Thanks for visiting Chevrolet at the Orange County Autoshow! Your vending machine code is {0}. Enjoy the show!', @newEventId, 1);



-- enable all constraints
exec sp_msforeachtable @command1="print '?'", @command2="ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all"