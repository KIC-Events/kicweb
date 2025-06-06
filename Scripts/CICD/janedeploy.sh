#!/bin/bash
#
#This script is in place to deploy the jane project
#This script should be run with sudo privileges

case $1 in
	"prod")
		echo "Deploying Jane to Production environment."
		echo "Stopping services..."
		systemctl stop jane.service
		echo "Updating repository to latest changes..."
		cd /srv/repo
		git switch main > /dev/null 2>&1
		git fetch > /dev/null 2>&1
		git pull > /dev/null 2>&1
		echo "Updating services..."
		cp /srv/repo/Scripts/systemd/jane.service /etc/systemd/system/jane.service
		systemctl daemon-reload
		echo "Building application..."
		dotnet build jane/Jane.csproj --os linux -c Production -o /srv/jane/ > /dev/null 2>&1
		echo "Adding config files..."
		cp -u /srv/config/jane/*.json /srv/jane/
		echo "Starting services..."
		cd /srv
		systemctl start jane.service
		sleep 2
		;;
	"dev")
		echo "Deploying Jane to Development environment."
		echo "Stopping services..."
		systemctl stop janedev.service
		echo "Updating repository to latest changes..."
		cd /srv/repo
		git switch dev > /dev/null 2>&1
		git fetch > /dev/null 2>&1
		git pull > /dev/null 2>&1
		echo "Updating services..."
		cp /srv/repo/Scripts/systemd/janedev.service /etc/systemd/system/janedev.service
		systemctl daemon-reload
		echo "Building application..."
		dotnet build jane/Jane.csproj --os linux -c Development -o /srv/janedev/ > /dev/null 2>&1
		echo "Adding config files..."
		cp -u /srv/config/jane/*.json /srv/janedev/
		echo "Starting services..."
		cd /srv
		systemctl start janedev.service
		sleep 2
		;;	
	*)
		echo "This script requires one argument, either prod or dev."
		exit
		;;		
esac

echo "Operations complete. Check of applications:"	
systemctl -t service --state=running --no-legend --no-pager | egrep "jane"