#!/usr/bin/python3

import string,cgi,time
import json

from http.server import BaseHTTPRequestHandler, HTTPServer
from os import curdir, sep, listdir
from os.path import isfile, join

ASSET_BUNDLE_FOLDER = "Assets/AssetBundles"

def GetAssetListing():
	assetBundles = [f for f in listdir(ASSET_BUNDLE_FOLDER) if isfile(join(ASSET_BUNDLE_FOLDER, f))]
	output = [];
	
	for bundle in assetBundles:
		if not bundle.endswith(".manifest") and not bundle.endswith(".meta"):
			# Find out which assets are contained in the current bundle
			assets = [];
			with open(ASSET_BUNDLE_FOLDER+"/"+bundle+".manifest", encoding="utf-8") as manifest:
				assetListReached = False
				for line in manifest:
					if line.startswith("Assets:"):
						assetListReached = True
					elif assetListReached:
						if line.startswith("- "):
							assets.append(line[2:].strip())
			output.append({"BundleName": bundle, "Assets": assets})
			
	return output

class MyHandler(BaseHTTPRequestHandler):

	def do_GET(self):
		output = ""
		if self.path == "/":
			
			self.send_response(200)
			self.send_header('Content-type', 'application/json')
			self.end_headers()
			self.wfile.write(json.dumps({"AssetBundleList": GetAssetListing()}).encode("utf-8"))
			self.wfile.flush()
		else:
			try:
				f = open(ASSET_BUNDLE_FOLDER + self.path, 'rb')

				self.send_response(200)
				self.send_header('Content-type', 'application/octet-stream')
				self.end_headers()

				self.wfile.write(f.read())
				self.wfile.close();
				
				f.close()
			except IOError:
				self.send_error(404,'File Not Found: %s' % self.path)

		return


def main():
	#print(json.dumps({"AssetBundleList": GetAssetListing()}))
	#return
	try:
		httpd = HTTPServer(('127.0.0.1', 80), MyHandler)
		print('started assetserver...')
		httpd.serve_forever()
		
	except KeyboardInterrupt:
		print ('^C received, shutting down server')
		httpd.socket.close()


if __name__ == '__main__':
	main()
