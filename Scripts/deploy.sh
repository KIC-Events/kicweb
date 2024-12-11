#!/bin/bash
#
#This script is in place to deploy the kicweb project
#This script should be run with sudo privileges

case $1 in
	"prod")
		echo "Deploying KIC and CURE sites to Production environments."
		echo "Stopping services..."
		systemctl stop kicweb.service
		systemctl stop cureweb.service
		echo "Updating repository to latest changes..."
		cd /srv/repo
		git switch main > /dev/null 2>&1
		git fetch > /dev/null 2>&1
		git pull > /dev/null 2>&1
		echo "Building applications..."
		dotnet build kicweb/KiCWeb.csproj --os linux -c Production -o /srv/kicweb/ > /dev/null 2>&1
		dotnet build cure/Cure.csproj --os linux -c Production -o /srv/cureweb/ > /dev/null 2>&1
		echo "Starting services..."
		cd /srv
		systemctl start kicweb.service
		systemctl start cureweb.service
		sleep 2
		;;
	"dev")
		echo "Deploying KIC and CURE sites to DEV environments."
		echo "Stopping services..."
		systemctl stop kicdev.service
		systemctl stop curedev.service
		echo "Updating repository to latest changes..."
		cd /srv/repo
		git switch dev > /dev/null 2>&1
		git fetch > /dev/null 2>&1
		git pull > /dev/null 2>&1
		echo "Building applications..."
		dotnet build kicweb/KiCWeb.csproj --os linux -c Development -o /srv/kicdev/ > /dev/null 2>&1
		dotnet build cure/Cure.csproj --os linux -c Development -o /srv/curedev/ > /dev/null 2>&1
		echo "Starting services..."
		cd /srv
		systemctl start kicdev.service
		systemctl start curedev.service
		sleep 2
		;;
	*)
		echo "This script requires one argument, either prod or dev."
		exit
		;;
esac

echo "Operations complete. Check of applications:"
systemctl -t service --state=running --no-legend --no-pager | egrep "kic|\bcure"
