from django.shortcuts import render
import json
from django.utils.safestring import mark_safe
from django.http import HttpRequest, HttpResponse

from django_redis import get_redis_connection

from . import models

# Create your views here.

def index(request):
    if request.method == "POST":
        #Decode POST data into json to store in db
        data =  json.loads(request.body.decode('utf-8'))
        summary = models.generateMatch(data)
        print(str(summary.timestamp) + "\t" + summary.map_name + "\t" + summary.mode + "\t" + str(summary.version))
        print("CT: " + str(summary.score_ct) + "\tT: " + str(summary.score_t))

        con = get_redis_connection("default")
        con.delete('map')
        con.set('map', summary)
        con.get('map')
        return HttpResponse()
    return render(request, 'delta/index.html', {})

def room(request, room_name):
    return render(request, 'delta/random.html', {
        'room_name_json': mark_safe(json.dumps(room_name))
    })