# StarC

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
- Git

## Setup
1. Setup OpenVPN Server
Refer https://gist.github.com/koike/420a27b0c8c7df24fef3fcccefed7f7d

2. Install Virtual Box on Host
- Please set PATH so that VBoxManage.exe can be used

3. Install Git for Windows on Host
- Create repository to store the data
- Repository's name is "starc.log"
- Please also set ssh key
- Please clone directly below the drive (C:\starc.log)

4. Install Windows on VM
- VM's name is "starc"
- No Login Password
- No UAC
- Create very vulnerable VM

5. Install OpenVPN Client on VM
- Be sure to connect confirmation!
- Please put a config file named vpn.ovpn under %UserProfile%\OpenVPN\config

6. Install Wireshark on VM
- Please set PATH so that tshark.exe can be used

7. Make initial setting of IE

8. Delete all files under %temp% as much as possible

9. Set starc.clirnt.exe to startup

10. Change VM settings
- Delete clipboard share setting
- Delete drag and drop setting
- Delete share folder setting

11. Create snapshot of VM
- Snapshot's name is "setuped"

## Usage
```
$ starc.exe [URL]
```
