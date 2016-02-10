#!/bin/sh
echo "######## KODE INSTALL SCRIPT ########"

if [ -f /usr/bin/wine ]
	then
		echo "##Wine is installed. Continuing"
	else
		echo "##Installing Wine"
		sudo apt-get install wine
		echo "##Wine is installed. Continuing"
fi

echo "##Creating folders"
mkdir -p ~/.cache/winetricks
echo "##Moving dependancies"
mv ./packages/* ~/.cache/winetricks/

echo "##Creating a clean Wine environment"
echo "##You will need to press Ok on dialog to continue"
WINEARCH=win32 winecfg

echo "##We are ready to install Kode. This will take some time and there will be little visual feedback."
echo "##This script will tell you when install is finished"
env WINEARCH=win32 winetricks -q dotnet45

sudo chmod +x ./kode
sudo mkdir -p ~/.wine/drive_c/Program\ Files/Kode
sudo mv ./app/* ~/.wine/drive_c/Program\ Files/Kode/
sudo mv ./Kode.desktop /usr/share/applications
sudo mv ./kode /bin
sudo ln -s ~/.steam/steam ~/.wine/drive_c/Program\ Files/Kode/

echo "Install complete"