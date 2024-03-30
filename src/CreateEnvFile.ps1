<#
.SYNOPSIS
	Generate .env file for docker-compose.yml
.DESCRIPTION
	This script Generates a .env file that will be used in the docker-compose file. It can also used by a build server to populate data for automatic testing.
.PARAMETER Version
	The version of the docker image, default is 'latest'
.PARAMETER PersistanceMode
	Mode for persistance of the history, allowed values are 'None', 'Database', and 'Json'. Default is 'None'
.PARAMETER Registry
	The docker registry for the image, default is 'docker.io/ramoy/'
.PARAMETER DbName
	Name of the database that will be created, default is 'BulkRename_Web_DB_Live'
.PARAMETER DbPort
	The external port of the SQL Server, default is '14333'
.PARAMETER DbUser
	The database user, default is 'sa', it's not recommended to use sa in production'
.PARAMETER SqlServerSaPassword
	Password for the SA user of the SQL Server, it doesn't have any default value
.PARAMETER SeqUrl
	URL and port of your Seq installation, leave it empty if you don't have any
.PARAMETER SeqApiKey
	The API Key of your Seq installation, leave it empty if you don't have any
.PARAMETER BulkRenamePort
	Port of the bulkrename application, default is '8383'
.PARAMETER BulkRenameFolder
	Folder outside docker that will be mapped to use for the files to rename, default is 'D:\\BulkRename\\Files'
.PARAMETER LogFolder
	Folder where logs will be stored if seq is not configured, default is 'D:\\BulkRename\\Logs'
.PARAMETER HistoryFolder
	The history file folder if you decide to use the file history instead of database, default is 'D:\\BulkRename\\RenamingHistory'
.PARAMETER SupportedFileEndings
	File endings that are used for videos or the files you want to rename, default is 'mkv;mp4;m4v;avi'
.PARAMETER FoldersToIgnore
	Folders which should be ignored, default is '.@__thumb'
#>

[CmdletBinding()]
Param (
	[Parameter(Mandatory = $false)][string]$Version = "latest",
	[Parameter(Mandatory = $false)][string]$Registry = "docker.io/ramoy/",
	[Parameter(Mandatory = $false)][string]$PersistanceMode = "None",
	[Parameter(Mandatory = $false)][string]$DbName = "BulkRename_Web_DB_Live",
	[Parameter(Mandatory = $false)][string]$DbPort = "14333",
	[Parameter(Mandatory = $false)][string]$DbUser = "sa",
	[Parameter(Mandatory = $false)][string]$SqlServerSaPassword,
	[Parameter(Mandatory = $false)][string]$SeqUrl,
	[Parameter(Mandatory = $false)][string]$SeqApiKey,
	[Parameter(Mandatory = $false)][string]$BulkRenamePort = "8383",
	[Parameter(Mandatory = $false)][string]$BulkRenameFolder = "D:\\BulkRename\\Files",
	[Parameter(Mandatory = $false)][string]$LogFolder = "D:\\BulkRename\\Logs",
	[Parameter(Mandatory = $false)][string]$HistoryFolder = "D:\\BulkRename\\RenamingHistory",
	[Parameter(Mandatory = $false)][string]$SupportedFileEndings = "mkv;mp4;m4v;avi",
	[Parameter(Mandatory = $false)][string]$FoldersToIgnore = ".@__thumb"
)

# Creates file .env
New-Item -ItemType File -Name ".env"

# Fills sample values for the created files
Set-Content .\.env "VERSION=$Version"
Add-Content .\.env "PERSITANCE_MODE=$PersistanceMode"
Add-Content .\.env "DOCKER_REGISTRY=$Registry"
Add-Content .\.env "DB_NAME=$DbName"
Add-Content .\.env "SQL_SERVER_EXTERNAL_PORT=$DbPort"
Add-Content .\.env "DB_USER=$DbUser"
Add-Content .\.env "SQL_SERVER_SA_PASSWORD=$SqlServerSaPassword"
Add-Content .\.env "SEQ_URL=$SeqUrl"
Add-Content .\.env "SEQ_API_KEY=$SeqApiKey"
Add-Content .\.env "BULK_RENAME_PORT=$BulkRenamePort"
Add-Content .\.env "BULK_RENAME_FOLDER=$BulkRenameFolder"
Add-Content .\.env "LOG_FOLDER=$LogFolder"
Add-Content .\.env "HISTORY_FOLDER=$HistoryFolder"
Add-Content .\.env "SUPPORTED_FILE_ENDINGS=$SupportedFileEndings"
Add-Content .\.env "FOLDERS_TO_IGNORE=$FoldersToIgnore"

Write-Host "File has been created, listing all files in directory:"
$Files = Get-ChildItem -Force
foreach ($File in $Files) {
	Write-Host $File
}
