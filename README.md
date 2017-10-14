# StarC
```Simple``` High-interactive client Honeypot

## Require
- Windows
  - Host
  - Guest
    - Windows 7
- Virtual Box
- OpenVPN
  - Server
  - Client Software
- Wireshark
- Fiddler
- Git

## Setup
1. Setup OpenVPN Server  

2. Install Virtual Box on Host  
Please set PATH so that VBoxManage.exe can be used  

3. Install Git for Windows on Host  
Create repository to store the data  
Repository's name is "```starc.log```"  
Please also set ssh key  
Please clone directly below the drive (```C:\starc.log```)  

4. Install Windows on VM  
VM's name is "```starc```"  
No Login Password  
No UAC  
Create very vulnerable VM  

5. Install OpenVPN Client on VM  
Be sure to connect confirmation!  
Please put a config file named vpn.ovpn under ```C:\starc.log\config``` on Host  

6. Install Wireshark on VM  
Please set PATH so that ```tshark.exe``` can be used  

7. Install Fiddler on VM  
Please set PATH so that ```fiddler.exe``` & ```execaction.exe``` can be used  
Please make the appropriate settings, ```Decrypt HTTPS traffic```  

8. Make initial setting of Internet Explorer on VM

9. Delete all files under ```%temp%``` as much as possible on VM

10. Set ```starc.client.exe``` to startup on VM

11. Change VM settings  
Delete clipboard share setting  
Delete drag and drop setting  
Delete share folder setting  

12. Create snapshot of VM  
Snapshot's name is "```setuped```"  

## Usage
```
$ starc.exe [URL]
```
