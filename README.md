# RTL_433 useful commands

## How to see the subscriptions in mosquitto?`

First, connect to the container via terminal

`docker exec -it <mosquitto_container_name> /bin/sh`

Then, use this command to see the subscriptions in a specific topic

`mosquitto_sub -h localhost -t <topic_name>`

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

## Troubleshooting

"usbipd: error: WSL 'usbip' client not correctly installed" See following link
https://github.com/dorssel/usbipd-win/issues/399#issuecomment-1154864761

## How to extract the humidity and temperature

Ecowitt sends signals like this (hex)
ab5555555555545ba9b1e5368fffff54d8000000000

The humidity is placed before the four "ffff" that are in the middle and it's just to translate the two hexadecimals digits to binary and discard the last bit. Then transform the binary to decimal. In the example.
x8f = 10001111b => 1000111b = 71 %

The temperature is placed before the two humidity hexadecimals digits and uses 3 hexadecimals digits. First a translation from hexadecimal to binary is needed, then it's necessary to discard the first three digits and the last one. The next step it's transform the binary to integer and we obtain a number. As we observed the value of 118 corresponds to 23º, so we calculate the difference between 118 and the number obtained and finally it's splitted by 10 to obtain the difference. To obtain the temperature, we do the the sum between 23 and the difference.

In the example
x536 = 010100110110 => 10011011 = 155
155 - 118 = 37
37/10 = 3,7
23 + 3,7 = 26,7
