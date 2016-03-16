#!/bin/sh
echo "######## KODE INSTALL SCRIPT ########"

if [ -f /usr/bin/wine ]
	then
		echo "Wine is installed. Continuing"
	else
		echo "Wine is not installed. Please install to continue"
fi

#Lets get wine ready
echo "Setting up Wine"
WINE=$HOME/.kode/
WINEPREFIX=$WINE WINEARCH=win32 wineboot > /dev/null 2>&1
rm -f $WINE/drive_c/windows/system32/mscoree.dll
WINEPREFIX=$WINE wine uninstaller --remove '{E45D8920-A758-4088-B6C6-31DBB276992E}'
echo "Done"

#Now lets fetch the dotnet40 installer and install it
echo "Installing .Net 4"
wget http://download.microsoft.com/download/9/5/A/95A9616B-7A37-4AF6-BC36-D6EA96C8DAAE/dotNetFx40_Full_x86_x64.exe
WINEPREFIX=$WINE wine dotNetFx40_Full_x86_x64.exe /q > /dev/null 2>&1
echo "Done"

#Make a file that we need for the next bit
echo "Doing some config stuff"
cat > key.reg << EOF
[HKEY_CURRENT_USER\Software\Wine\DllOverrides]
"mscoree"="native"
EOF

#Set some registry entries to make things work
WINEPREFIX=$WINE wine reg add "HKLM\\Software\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full" /v Install /t REG_DWORD /d 0001 /f
WINEPREFIX=$WINE wine reg add "HKLM\\Software\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full" /v Version /t REG_SZ /d "4.0.30319" /f
WINEPREFIX=$WINE wine regedit key.reg

#Delete a file that we dont need anymore
rm -f key.reg
echo "Done"

#Get Kode ready to go
echo "Getting Kode ready"
sudo chmod +x ./kode
mkdir -p ~/.kode/drive_c/Program\ Files/Kode
mv ./app/* ~/.kode/drive_c/Program\ Files/Kode/
sudo mv ./Kode.desktop /usr/share/applications
sudo mv ./kode /bin

WINEPREFIX=$WINE wine $WINE/drive_c/windows/Microsoft.NET/Framework/v4.0.30319/ngen.exe executequeueditems > /dev/null 2>&1
echo "Done. You can launch Kode by typing 'kode' into the terminal or by launching it through your applications menu, found over Development Tools"