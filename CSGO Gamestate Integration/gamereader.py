from http.server import BaseHTTPRequestHandler, HTTPServer
import json
import threading
import time

import asyncio
import websockets
import queue

PORT = 8000
dataQ = queue.Queue()

NOINSTANCE = True
dummyData = None
with open('bombPlant.json') as json_file:  
    dummyData = json.load(json_file)

class reqHandler(BaseHTTPRequestHandler):
	def do_POST(self):
		content_len = int(self.headers['Content-Length'])
		body = self.rfile.read(content_len)
		dataQ.put(json.loads(body))

def startServer():
	serverAddress = ('',PORT)
	httpd = HTTPServer(serverAddress, reqHandler)
	httpd.serve_forever()

def generateMatchObj(inputData):
	data = {}
	data['mode'] = inputData['map']['mode']
	data['map_name'] = inputData['map']['name']
	data['phase'] = inputData['map']['phase']
	data['consec_loss_t'] = inputData['map']['team_t']['consecutive_round_losses']
	data['consec_loss_ct'] = inputData['map']['team_ct']['consecutive_round_losses']
	data['score_ct'] = inputData['map']['team_ct']['score']
	data['score_t'] = inputData['map']['team_t']['score']
	data['timeouts_ct'] = inputData['map']['team_ct']['timeouts_remaining']
	data['timeouts_t'] = inputData['map']['team_t']['timeouts_remaining']
	data['game'] = inputData['provider']['name']
	data['version'] = inputData['provider']['version']
	data['client_steamid'] = inputData['provider']['steamid']
	data['timestamp'] = inputData['provider']['timestamp']
	return data

def generateRoundObj(inputData):
	data = {}
	data['roundNum'] = inputData['map']['round']
	data['phase'] = inputData['phase_countdowns']['phase']
	data['phaseTime'] = inputData['phase_countdowns']['phase_ends_in']
	return data

def generatePlayerObj(inputData):
	data = {}
	data['team'] = inputData['player']['team']
	data['name'] = inputData['player']['name']

	data['kills'] = inputData['player']['match_stats']['kills']
	data['assists'] = inputData['player']['match_stats']['assists']
	data['deaths'] = inputData['player']['match_stats']['deaths']
	data['mvps'] = inputData['player']['match_stats']['mvps']
	data['score'] = inputData['player']['match_stats']['score']
	
	data['health'] = inputData['player']['state']['health']
	data['armor'] = inputData['player']['state']['armor']
	data['helmet'] = inputData['player']['state']['helmet']
	data['money'] = inputData['player']['state']['money']
	data['round_kills'] = inputData['player']['state']['round_kills']
	data['round_killhs'] = inputData['player']['state']['round_killhs']
	data['round_totaldmg'] = inputData['player']['state']['round_totaldmg']
	data['equip_value'] = inputData['player']['state']['equip_value']
	data['position'] = inputData['player']['position']
	return data

async def sendLoop(websocket, path):
	i=0
	while True:
		item = None
		if NOINSTANCE:
			item = dummyData
		else:
			item = dataQ.get()
		gameDesc = {}
		gameDesc['matchItem'] = generateMatchObj(item)
		gameDesc['roundItem'] = generateRoundObj(item)
		gameDesc['playerItem'] = generatePlayerObj(item)
		gameDesc['matchItem']['timestamp'] = i
		i=i+1
		await websocket.send(json.dumps(gameDesc))
		if not NOINSTANCE:
			dataQ.task_done()
		time.sleep(0.2)

serveThread = threading.Thread(target=startServer,name="server")
serveThread.start()

start_server = websockets.serve(sendLoop, '127.0.0.1', 5678)
asyncio.get_event_loop().run_until_complete(start_server)
asyncio.get_event_loop().run_forever()