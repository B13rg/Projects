from http.server import BaseHTTPRequestHandler, HTTPServer
import json

PORT = 8000


class reqHandler(BaseHTTPRequestHandler):
	def do_POST(self):

		content_len = int(self.headers['Content-Length'])
		body = self.rfile.read(content_len)
		#data = json.loads(body)
		f = open( 'output', 'w' )
		f.write(body.decode('utf-8'))
		f.close()
		


def run():
	serverAddress = ('',PORT)
	httpd = HTTPServer(serverAddress, reqHandler)
	httpd.serve_forever()

run()