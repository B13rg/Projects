from websocket import create_connection
import json
from decimal import *

raw = 0
class c:
    HEADER = '\033[95m'
    OKBLUE = '\033[94m'
    OKGREEN = '\x1b[6;30;42m'
    WARNING = '\033[93m'
    FAIL = '\033[91m'
    ENDC = '\x1b[0m'
    BOLD = '\033[1m'
    UNDERLINE = '\033[4m'

ws = create_connection("ws://ws.blockchain.info/inv")
ws.send('{"op":"unconfirmed_sub"}')
while ( 1 ) :
	result = ws.recv()

	if raw :
		print result

	result = json.loads(result)
	marker = " "
	if 'out' in result['x'] :
		for out in result['x']['out'] :
			amountBTC= Decimal( out['value'] ) / Decimal(100000000.0)
			if amountBTC==0:
				continue
			if amountBTC>3:
				print c.OKGREEN + out['addr'] + ' got ' + str(amountBTC) + c.ENDC
			else:
				print out['addr'] + ' got ' + str(amountBTC) +marker
			marker = ''
		
print "Done"
ws.close()

""" {
    'x': {
        'inputs': [
            {
                'prev_out': {
                    'type': 0,
                    'addr': '14XGFnhBJQC2sKxwAvUqv8CAu43uezn4Xv',	//Sender address
                    'value': 7107830629								//Sender Amount
                }
            }
        ],
        'lock_time': 'Unavailable',
        'ver': 1,
        'tx_index': 4062121,
        'relayed_by': '65.49.73.51',
        'vin_sz': 1,
        'vout_sz': 2,
        'time': 1334803494,
        'hash': 'aabd272a9be5d6f2709da8e184f29b70d1f34f96aeead5a980a3cbde2863507e',
        'out': [
            {
                'type': 0,
                'addr': '16bEdESzzZA2975qe7egGDZbBcXnQNMJ8X',		//reciever address
                'value': 643133000									//reciever amount
            },
            {
                'type': 0,
                'addr': '13A7Sz4YnxpYYj8UEoiXbp2S1bTXUSFjpn',		//reciever address
                'value': 6464697629									//reciever amount
            }
        ],
        'size': 259
    },
    'op': 'utx'
}{
    'x': {
        'cc': 'us',
        'lat': 37.3842,
        'lon': -122.0196,
        'id': 1093749043
    },
    'op': 'marker'
} """