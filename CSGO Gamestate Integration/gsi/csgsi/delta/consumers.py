# chat/consumers.py
from channels.generic.websocket import WebsocketConsumer
from channels.generic.http import AsyncHttpConsumer
import json


class ChatConsumer(WebsocketConsumer):
    def connect(self):
        self.accept()
        

    def disconnect(self, close_code):
        pass

    def receive(self, text_data):
        text_data_json = json.loads(text_data)
        message = text_data_json['message']

        self.send(text_data=json.dumps({
            'message': message
        }))

    def process_Match(self, message):
        self.send(text_data=str(summary.timestamp) + "\t" + summary.map_name + "\t" + summary.mode + "\t" + str(summary.version))
        self.sent(text_data="CT: " + str(summary.score_ct) + "\tT: " + str(summary.score_t))

class Ingest(AsyncHttpConsumer):
    async def handle(self, body):
        print(self.scope['method'])
        #print(body)
        data = json.loads(body.decode('utf-8'))
        
        print("item:"+str(data[platform][timestamp]))
        #summary = generateMatch(data)
        #self.channel_layer.send(
        #    "process_Match",summary
        #)

