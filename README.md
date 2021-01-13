# Social Media Vending Machine

----
####Overview
The solution was developed to handle code assignments for Twitter users to receive free items via special vending machines. It also performs reporting exports for the product client.

####Platform Requirements
- .Net framework 4.0
- Entity Framework 4.5
- MSSQL Server 8

####Deployment Environments
- Production
	- Hosted at Amazon
		- ec2-54-211-255-201.compute-1.amazonaws.com
	- Accessible via Remote Desktop
		- See Confluence for [log-in information](https://bluewater.atlassian.net/wiki/display/GM/Vending+Machine).  

####Further Information
- Specific Confluence area dedicated to the Vending Machine project
	- [https://bluewater.atlassian.net/wiki/display/GM/Vending+Machine](https://bluewater.atlassian.net/wiki/display/GM/Vending+Machine) 

-----

###Solution Projects

----------

####VendingMachine.Model
- Model Class Objects 

	#####Purpose
	All class objects being referenced by any Vending Machine project

	#####References
	- **NuGet Packages**
		- EntityFramework
			- Version: 5.0.0
			- Created by: Microsoft 

----------

####VendingMachine.Model.Tests
- Unit Tests for the Model Class objects

	#####Purpose
	Initial unit tests for the VendingMachine.Model class objects.

	#####Solution/Project References
	- VendingMachine.Model
	
	#####Notes

----------

####VendingMachine.Repository
- Database interface

	#####Purpose
	- Works as an interface/library for any database functionality required by the solution.
	- Creates the database and populates it with seed data.
		- */Migrations/Configuration.cs*
	- Updates database when database context changes.

	#####Technologies Used
	- Entity Framework
		- Code First 
	- Entity Migrations

	#####Solution/Project References
	- VendingMachine.Model

	#####Notes

----------

####VendingMachineReportExport
- Executable

	#####Purpose
	*Searches a specified folder for csv files created by the vending machine and inserts the values into an Excel template and saves it as a new spreadsheet.*

	#####Notes
	
----------

####VendingMachineReportExport.Installer
- Installer Project

	#####Purpose
	*Creates the installation package for the VendingMachineReportExport executable.*

	#####Notes

----------

####VendingMachineTwitter
- Executable

	#####Purpose
	*Monitors a selected Twitter account for specified #hashtags and responds with a random unique code that can be used on the vending machine to receive "swag".*

	#####Notes
	- Specific Instructions on how to [create a new event](https://bluewater.atlassian.net/wiki/display/GM/Creating+New+Event) are in Confluence.

----------

####VendingMachineTwitterMonitor.Installer
- Installer project

	#####Purpose
	*Creates the installer package for the VendingMachineTwitter executable.*

	#####Notes

