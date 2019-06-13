from http.server import BaseHTTPRequestHandler, HTTPServer
import json
import threading
import time

import asyncio
import websockets
import queue

PORT = 8000
dataQ = queue.Queue()

class reqHandler(BaseHTTPRequestHandler):
	def do_POST(self):
		content_len = int(self.headers['Content-Length'])
		body = self.rfile.read(content_len)
		#print("recieved data from csgo")
		dataQ.put(json.loads(body))

def startServer():
	serverAddress = ('',PORT)
	httpd = HTTPServer(serverAddress, reqHandler)
	httpd.serve_forever()

def generateMatch(data):
	match_id = "2",
	mode = data['map']['mode'],
	map_name = data['map']['name'],
	phase = data['map']['phase'],
	consec_loss_t = data['map']['team_t']['consecutive_round_losses'],
	consec_loss_ct = data['map']['team_ct']['consecutive_round_losses'],
	score_ct = data['map']['team_ct']['score'],
	score_t = data['map']['team_t']['score'],
	timeouts_ct = data['map']['team_ct']['timeouts_remaining'],
	timeouts_t = data['map']['team_t']['timeouts_remaining'],
	game = data['provider']['name'],
	version = data['provider']['version'],
	client_steamid = data['provider']['steamid'],
	timestamp = data['provider']['timestamp']

async def time(websocket, path):
	while True:
		item = dataQ.get()
		print("Got item from queue")
		print(item['map']['team_ct']['score'])
		ctscore=item['map']['team_ct']['score']
		tscore=item['map']['team_t']['score']
		await websocket.send("CT: "+str(ctscore)+" T: "+str(tscore))
		dataQ.task_done()
		print("item sent over websocket")

serveThread = threading.Thread(target=startServer,name="server")
serveThread.start()

start_server = websockets.serve(time, '127.0.0.1', 5678)
asyncio.get_event_loop().run_until_complete(start_server)
asyncio.get_event_loop().run_forever()