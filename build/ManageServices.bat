echo off

set basepath=%cd%
for %%D in ("%CD%") do set "parentdir=%%~nxD"
set version=
if not "%~2" == "" set version=%~2

SET parent=%~dp0
FOR %%a IN ("%parent:~0,-1%") DO SET grandparent=%%~dpa

if "%~1" == "--install" (
	sc create EKTAIOFFICE%parentdir%%version%      start= auto binPath= "\"%basepath%\TeamLabSvc.exe\"
	goto Exit
)

if "%~1" == "--install-all" (
	sc create EKTAIOFFICENotify%version%           start= auto binPath= "\"%basepath%\TeamLabSvc.exe\" --service \"ASC.Notify.NotifyServiceLauncher, ASC.Notify\" --log Notify"
	sc create EKTAIOFFICEJabber%version%           start= auto binPath= "\"%basepath%\TeamLabSvc.exe\" --service \"ASC.Xmpp.Host.XmppServerLauncher, ASC.Xmpp.Host\" --log Jabber"
	sc create EKTAIOFFICEIndex%version%            start= auto binPath= "\"%basepath%\TeamLabSvc.exe\" --service \"ASC.ElasticSearch.Launcher, ASC.ElasticSearch\" --log Index"
	sc create EKTAIOFFICERadicale%version%         start= auto binPath= "\"%basepath%\TeamLabSvc.exe\" --service \"ASC.Radicale.Launcher, ASC.Radicale\" --log Radicale"
	sc create EKTAIOFFICEStorageMigrate%version%   start= auto binPath= "\"%basepath%\TeamLabSvc.exe\" --service \"ASC.Data.Storage.Migration.Launcher,ASC.Data.Storage.Migration\" --log StorageMigrate"
	sc create EKTAIOFFICEFeed%version%             start= auto binPath= "\"%basepath%\TeamLabSvc.exe\" --service \"ASC.Feed.Aggregator.FeedAggregatorLauncher, ASC.Feed.Aggregator\" --log Feed"
	sc create EKTAIOFFICEBackup%version%           start= auto binPath= "\"%basepath%\TeamLabSvc.exe\" --service \"ASC.Data.Backup.Service.BackupServiceLauncher, ASC.Data.Backup\" --log Backup"
	sc create EKTAIOFFICESocketIO%version%         start= auto binPath= "\"%basepath%\TeamLabSvc.exe\" --service \"ASC.Socket.IO.Svc.Launcher, ASC.Socket.IO.Svc\" --log SocketIO"
	sc create EKTAIOFFICEMailAggregator%version%   start= auto binPath= "\"%grandparent%\MailAggregator\ASC.Mail.Aggregator.CollectionService.exe\""
	sc create EKTAIOFFICEMailWatchdog%version%     start= auto binPath= "\"%grandparent%\MailWatchdog\ASC.Mail.Watchdog.Service.exe\""
	
	goto Exit
)
if "%~1" == "--uninstall" (
	net stop  EKTAIOFFICE%parentdir%%version%
	sc delete EKTAIOFFICE%parentdir%%version%
	goto Exit
)

if "%~1" == "--uninstall-all" (
	net stop  EKTAIOFFICENotify%version%
	sc delete EKTAIOFFICENotify%version%
	net stop  EKTAIOFFICEJabber%version%
	sc delete EKTAIOFFICEJabber%version%
	net stop  EKTAIOFFICEIndex%version%
	sc delete EKTAIOFFICEIndex%version%
	net stop  EKTAIOFFICEFeed%version%
	sc delete EKTAIOFFICEFeed%version%
	net stop  EKTAIOFFICEBackup%version%
	sc delete EKTAIOFFICEBackup%version%	
	net stop  EKTAIOFFICESocketIO%version%
	sc delete EKTAIOFFICESocketIO%version%	
	net stop  EKTAIOFFICEMailAggregator%version%
	sc delete EKTAIOFFICEMailAggregator%version%	
	net stop  EKTAIOFFICEMailWatchdog%version%
	sc delete EKTAIOFFICEMailWatchdog%version%	
	goto Exit
)

:Help
echo Usage: ManageServices.bat COMMAND [VERSION]
echo        COMMAND        install or unistall services:
echo                       --install         - install service
echo                       --uninstall       - uninstall service
echo                       --install-all     - install all services from this folder
echo                       --uninstall-all   - uninstall all services from this folder
echo        VERSION        service version, example: 8.5.1, can be empty

:Exit
echo on