#!/bin/sh
export MONO_IOMAP=all
VSToolsPath=msbuild
Configuration=Release
DeployTo=UNIX.SERVER

set -e

PLATFORM=""
if [ `python -mplatform | grep Ubuntu` ]; then
	PLATFORM="ubuntu"
elif [ `python -mplatform | grep centos` ]; then
	PLATFORM="centos"
else
	echo "Unknown platform"
	exit 1
fi

echo "Platform: $PLATFORM"

msbuild msbuild/build.proj /flp:LogFile=Build.log
msbuild msbuild/deploy.proj /flp:LogFile=Deploy.log /p:DeployTo=$DeployTo

if [ "$PLATFORM" = "ubuntu" ]; then
	cd install/deb
	rm -R -f ../*.deb ../*.changes Files/Services Files/WebStudio Files/sql Files/licenses
	mkdir Files/sql
	cp -R ../../../licenses Files/licenses
	cp -R ../../deploy/$DeployTo/Services Files
	cp -R ../../deploy/$DeployTo/WebStudio Files/WebStudio
	cp ../../sql/EKTAIOFFICE* Files/sql

	if [ -t 0 ]; then
		# if in terminal
		dpkg-buildpackage -b
	else
		dpkg-buildpackage -b -p"$SIGN_COMMAND"
	fi
	mv -f ../EKTAIOFFICE* ../../deploy/
else
	cd install/rpm
	rm -R -f Files/EKTAIOFFICE/Services Files/EKTAIOFFICE/WebStudio Files/EKTAIOFFICE/Sql Files/EKTAIOFFICE/Licenses
	mkdir Files/EKTAIOFFICE/Sql
	cp -R ../../../licenses Files/EKTAIOFFICE/Licenses
	cp -R ../../deploy/$DeployTo/Services Files/EKTAIOFFICE/Services
	cp -R ../../deploy/$DeployTo/WebStudio Files/EKTAIOFFICE/WebStudio
	cp ../../sql/EKTAIOFFICE* Files/EKTAIOFFICE/Sql

	sh buildhelper.sh -b -u
	mv -f builddir/RPMS/noarch/EKTAIOFFICE* ../../deploy/
fi
