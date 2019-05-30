# chat/consumers.py
from channels.generic.websocket import WebsocketConsumer
from channels.generic.http import AsyncHttpConsumer
from asgiref.sync import async_to_sync
from channels.layers import get_channel_layer
import json
from . import models


class ChatConsumer(WebsocketConsumer):
    def connect(self):
        async_to_sync(self.channel_layer.group_add)("map", self.channel_name)
        self.accept()
        

    def disconnect(self, close_code):
        async_to_sync(self.channel_layer.group_discard)(
            'map',
            self.channel_name
        )
        self.close()

    def receive(self, text_data):
        text_data_json = json.loads(text_data)
        message = text_data_json['message']

        self.send(text_data=json.dumps({
            'message': message
        }))

    def update_map(self, event):
        print("Updateing map data...")
        self.send(text_data=event['map'])

    def process_Match(self, message):
        self.send(text_data=str(summary.timestamp) + "\t" + summary.map_name + "\t" + summary.mode + "\t" + str(summary.version))
        self.sent(text_data="CT: " + str(summary.score_ct) + "\tT: " + str(summary.score_t))

class Ingest(AsyncHttpConsumer):
    def handle(self, body):
        print(self.scope['method'])
        thing = body.decode('utf-8')
        data = json.loads(thing)
        print("item2:"+str(data['provider']['timestamp']))
        models.generateMatch(data)
        print("item3:"+str(data['provider']['timestamp']))
        #self.channel_layer.send(
        #    "process_Match",summary
        #)

