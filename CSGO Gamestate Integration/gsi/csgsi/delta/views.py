from django.shortcuts import render
import json
from django.utils.safestring import mark_safe

# Create your views here.

def index(request):
    if request.method == "POST":
        #Decode POST data into json to store in db
        data =  json.loads(request.body.decode('utf-8'))
        summary = generateMatch(data)
        print(str(summary.timestamp) + "\t" + summary.map_name + "\t" + summary.mode + "\t" + str(summary.version))
        print("CT: " + str(summary.score_ct) + "\tT: " + str(summary.score_t))
        return HttpResponse()
    return render(request, 'delta/index.html', {})

def room(request, room_name):
    return render(request, 'delta/random.html', {
        'room_name_json': mark_safe(json.dumps(room_name))
    })