#!/usr/bin/python3

import string,cgi,time
import argparse
import json
import inspect
import base64
import datetime
import calendar

from http.server import BaseHTTPRequestHandler, HTTPServer
from os import curdir, sep, listdir
from os.path import isfile, join, dirname
import os

ASSET_BUNDLE_FOLDER = "Assets/AssetBundles"
THUMBNAIL_FOLDER	= "Assets/Thumbnails"
SCENE_FOLDER		= "Assets/SceneSaves"
SCENE_SAVE_EXT		= "scsav"

SERVER_ADDR			= ""

def GetAssetListing():
	assetBundles = [f for f in listdir(ASSET_BUNDLE_FOLDER) if isfile(join(ASSET_BUNDLE_FOLDER, f))]
	output = []

	for bundle in assetBundles:
		if bundle.startswith("AssetBundles"):
			continue

		if not bundle.endswith(".manifest") and not bundle.endswith(".meta"):
			# Check if a Thumbnail exists
			thumbURI = THUMBNAIL_FOLDER + "/" + bundle + ".png"

			item = {"BundleName": bundle}

			if os.path.isfile(thumbURI):
				with open(thumbURI, "rb") as thumb:
					item["Thumbnail"] = base64.b64encode(thumb.read()).decode("ascii")

			output.append(item)

			'''
			# Find out which assets are contained in the current bundle
			assets = []
			with open(ASSET_BUNDLE_FOLDER+"/"+bundle+".manifest", encoding="utf-8") as manifest:
				assetListReached = False
				for line in manifest:
					if line.startswith("Assets:"):
						assetListReached = True
					elif assetListReached:
						if line.startswith("- "):
							assets.append(line[2:].strip())
			output.append({"BundleName": bundle, "Assets": assets})
			'''
			
	return output

def GetSceneListing():
	scenes = [f for f in listdir(SCENE_FOLDER) if isfile(join(SCENE_FOLDER, f))]
	output = []

	for scene in scenes:

		if not scene.endswith(".manifest") and not scene.endswith(".meta"):
			output.append({"LastModified": os.path.getmtime(SCENE_FOLDER + "/" + scene), "Name": scene[:-6]})

	return output

class MyHandler(BaseHTTPRequestHandler):

	def do_GET(self):
		paths = self.path[1:].split('/')
		GET_ROOT = paths[0]

		if self.path == "/":
			methods = inspect.getmembers(self, predicate=inspect.ismethod)
			possibleMethods = {}

			# Get all HTTP-METHODs in this class (e.g. GET, PUT, DELETE etc.)
			for meth in methods:
				if meth[0].startswith("do_"):
					httpMethod = meth[0][3:]
					possibleMethods[httpMethod] = []
					# Now cycle through all the methods again and see if we have servlets
					for servlet in methods:
						if servlet[0].startswith(httpMethod.lower() + "_"):
							possibleMethods[httpMethod].append(servlet[0][servlet[0].index("_")+1:])

			self.send_response(200)
			self.send_header('Content-type', 'application/json')
			self.end_headers()
			self.wfile.write(json.dumps({"Server-Methods": possibleMethods}).encode("utf-8"))
			self.wfile.flush()

		else:
			request = "get_" + GET_ROOT
			if hasattr(self, request):
				method = getattr(self, request)
				if inspect.ismethod(method):
					method("".join(paths[1:]))
			else:
				self.send_error(501,"%s-Module not implemented" % GET_ROOT)
		return

	def do_PUT(self):
		paths = self.path
		paths = self.path[1:].split('/')
		PUT_ROOT = paths[0]

		if self.path.endswith('/'):
			self.send_response(405, "Method Not Allowed")
			self.end_headers()
			self.wfile.write("PUT not allowed on a directory\n".encode())
			self.wfile.flush()
		elif PUT_ROOT == "":
			self.send_response(405, "Method Not Allowed")
			self.end_headers()
			self.wfile.write("PUT not allowed on directory root\n".encode())
			self.wfile.flush()
		else:
			request = "put_" + PUT_ROOT
			if hasattr(self, request):
				method = getattr(self, request)
				if inspect.ismethod(method):
					method("".join(paths[1:]))
			else:
				self.send_error(501,"%s-Module not implemented" % GET_ROOT)

	def get_Bundle(self, path):
		if path == "":
			self.send_response(200)
			self.send_header('Content-type', 'application/json')
			self.end_headers()
			self.wfile.write(json.dumps({"AssetBundleList": GetAssetListing()}).encode("utf-8"))
			self.wfile.flush()
		else:
			try:
				f = open(ASSET_BUNDLE_FOLDER + "/" + path, 'rb')

				self.send_response(200)
				self.send_header('Content-type', 'application/octet-stream')
				self.end_headers()

				self.wfile.write(f.read())
				f.close()

				self.wfile.flush()
			except IOError:
				self.send_error(404,'File Not Found: %s' % path)
		return

	def get_Scene(self, path):
		if path == "":
			self.send_response(200)
			self.send_header('Content-type', 'application/json')
			self.end_headers()
			self.wfile.write(json.dumps({"SceneList": GetSceneListing()}).encode("utf-8"))
			self.wfile.flush()
		else:
			try:
				f = open(SCENE_FOLDER + "/" + path + "." + SCENE_SAVE_EXT, 'rb')

				self.send_response(200)
				self.send_header('Content-type', 'application/json')
				self.end_headers()

				self.wfile.write(f.read())
				f.close()

				self.wfile.flush()
			except IOError:
				self.send_error(404,'File Not Found: %s' % path)
		return

	def put_Scene(self, path):
		global SERVER_ADDR

		try:
			os.makedirs(SCENE_FOLDER)
		except FileExistsError: pass

		length = int(self.headers['Content-Length'])
			
		with open(SCENE_FOLDER + "/" + path + "." + SCENE_SAVE_EXT, 'wb') as f:
			f.write(self.rfile.read(length))
			
		now = datetime.datetime.now()

		print("%s - - [%d/%s/%d %02d:%02d:%02d] \"PUT %s with %d bytes\"" % (
			SERVER_ADDR, now.day, calendar.month_abbr[now.month], now.year, now.hour, now.minute, now.second, path, length
		))
		
		self.send_response(201, "Created")
		self.end_headers()

		return

def main():
	global SERVER_ADDR

	parser = argparse.ArgumentParser()
	parser.add_argument("--bind", help="Set the IP from which the server waits for incoming requests")
	args = parser.parse_args()
	
	ipAddr = ""
	if args.bind:
		SERVER_ADDR = args.bind
	else:
		SERVER_ADDR = "127.0.0.1"
		print( "Warning! You didn specify an IP to bind the HTTP-Server on." )
		print( "This could potentially lead to an unreachable Server, if localhost isn't mapped to the preferred IPv4-Address." )
		print( "The bound IP-Address is in most cases the same address which gets queried for Assets.\n" )
	
	try:
		httpd = HTTPServer((SERVER_ADDR, 80), MyHandler)
		print('AssetServer started on %s. Serving...' % (SERVER_ADDR))
		httpd.serve_forever()
		
	except KeyboardInterrupt:
		print ('^C received, shutting down server')
		httpd.socket.close()


if __name__ == '__main__':
	main()