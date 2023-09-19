# RTL_433 useful commands

## Show samples

rtl_433 -f 868M -R 0 -X 'n=Ecowitt,m=FSK_PCM,s=58,l=58,r=59392,bits=169' -F json:sample.json

## Send samples

rtl_433 -f 868M -R 0 -X 'n=Ecowitt,m=FSK_PCM,s=58,l=58,r=59392,bits=169' -F "mqtt://localhost,retain=0,events=rtl_433/Ecowitt"

## Connect dongle on WSL

https://www.xda-developers.com/wsl-connect-usb-devices-windows-11/

- usbipd wsl list

- usbipd wsl attach --busid 1-1

- usbipd wsl detach --busid 1-1

- sudo service udev start
