import json
import time
import paho.mqtt.client as mqttclient
import random
import geocoder
from utils import generate_random_location
print("Xin chào ThingsBoard")

# Server info
BROKER_ADDRESS = "demo.thingsboard.io"
PORT = 1883
with open("../../../Access Tokens/Lab1.txt", 'r') as f:
    THINGS_BOARD_ACCESS_TOKEN = f.readline()


def subscribed(client, userdata, mid, granted_qos):
    print("Subscribed...")


def recv_message(client, userdata, message):
    print("Received: ", message.payload.decode("utf-8"))
    temp_data = {'value': True}
    try:
        jsonobj = json.loads(message.payload)
        if jsonobj['method'] == "setValue":
            temp_data['value'] = jsonobj['params']
            client.publish('v1/devices/me/attributes',
                           json.dumps(temp_data), 1)
    except:
        pass


def connected(client, usedata, flags, rc):
    if rc == 0:
        print("Thingsboard connected successfully!!")
        client.subscribe("v1/devices/me/rpc/request/+")
    else:
        print("Connection is failed")


# provide the name of the client
client = mqttclient.Client("Gateway_Thingsboard")
client.username_pw_set(THINGS_BOARD_ACCESS_TOKEN)

client.on_connect = connected
client.connect(BROKER_ADDRESS, 1883)
client.loop_start()

client.on_subscribe = subscribed
client.on_message = recv_message

temp = 30
humi = 50
light_intesity = 100
latitude, longitude = geocoder.ip('me').latlng

counter = 0
while True:
    temp = temp + int(random.uniform(-9, 9)) if (4 <=
                                                 temp <= 36) else 30  # -5 -- 45
    humi = humi + int(random.uniform(-9, 9)) if (9 <=
                                                 humi <= 91) else 50  # 0 -- 100
    light_intesity = light_intesity + int(random.uniform(-9, 9)
                                          ) if (9 <= light_intesity <= 150) else 100  # 0 -- 150
    # generate random longitude and latitude within the radius 200 meters
    longitude, latitude = generate_random_location(longitude, latitude, 20)
    collect_data = {
        'temperature': temp,
        'humidity': humi,
        'light': light_intesity,
        'longitude': longitude,
        'latitude': latitude
    }
    client.publish('v1/devices/me/telemetry', json.dumps(collect_data), 1)
    time.sleep(10)
