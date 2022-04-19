import serial.tools.list_ports
import json
import time
import paho.mqtt.client as mqttclient
from pprint import pprint
print("IoT Gateway")

BROKER_ADDRESS = "demo.thingsboard.io"
PORT = 1883
mess = ""
LED = False
FAN = False

# TODO: Add your token and your comport
# Please check the comport in the device manager
THINGS_BOARD_ACCESS_TOKEN = "XKSj05MKL2k5GLXOIgqL"
bbc_port = "COM3"
if len(bbc_port) > 0:
    ser = serial.Serial(port=bbc_port, baudrate=115200)


def processData(data):
    data = data.replace("!", "")
    data = data.replace("#", "")
    splitData = data.split(":")
    print(splitData)
    # TODO: Add your source code to publish data to the server
    if (splitData[1] == 'LIGHT'):
        data = {
            'light': splitData[2]
        }
    if (splitData[1] == 'TEMP'):
        data = {
            'temperature': splitData[2]
        }

    client.publish('v1/devices/me/telemetry', json.dumps(data), 1)


def readSerial():
    bytesToRead = ser.inWaiting()
    if (bytesToRead > 0):
        global mess
        mess = mess + ser.read(bytesToRead).decode("UTF-8")
        while ("#" in mess) and ("!" in mess):
            start = mess.find("!")
            end = mess.find("#")
            processData(mess[start:end + 1])
            if (end == len(mess)):
                mess = ""
            else:
                mess = mess[end+1:]


def subscribed(client, userdata, mid, granted_qos):
    print("Subscribed...")


def recv_message(client, userdata, message):
    print("Received: ", message.payload.decode("utf-8"))
    temp_data = {'value': True}
    cmd = 1
    # TODO: Update the cmd to control 2 devices
    global FAN, LED
    try:
        jsonobj = json.loads(message.payload)
        if jsonobj['method'] == "setLED":
            temp_data['value'] = jsonobj['params']
            LED = temp_data['value']
            client.publish('v1/devices/me/attributes',
                           json.dumps(temp_data), 1)
        if jsonobj['method'] == "setFAN":
            temp_data['value'] = jsonobj['params']
            FAN = temp_data['value']
            client.publish('v1/devices/me/attributes',
                           json.dumps(temp_data), 1)
    except:
        pass

    if FAN and LED:
        cmd = 0
    if not FAN and LED:
        cmd = 1
    if FAN and not LED:
        cmd = 2
    if not FAN and not LED:
        cmd = 3

    if len(bbc_port) > 0:
        ser.write((str(cmd) + "#").encode())


def connected(client, usedata, flags, rc):
    if rc == 0:
        print("Thingsboard connected successfully!!")
        client.subscribe("v1/devices/me/rpc/request/+")
        print(client._userdata)
    else:
        print("Connection is failed")


client = mqttclient.Client("Gateway_Thingsboard")
client.username_pw_set(THINGS_BOARD_ACCESS_TOKEN)

client.on_connect = connected
client.connect(BROKER_ADDRESS, 1883)
client.loop_start()

client.on_subscribe = subscribed
client.on_message = recv_message
# pprint(dir(client))


while True:

    if len(bbc_port) > 0:
        readSerial()

    time.sleep(1)
