from django.shortcuts import render
from django.http import HttpResponse
import random

# Create your views here.
def index(request):
	return render(request, 'chat/index.html', {})

def randomNum(request):
	return HttpResponse(str(random.randint(1,10)))

def websocket(request):
	return HttpResponse()