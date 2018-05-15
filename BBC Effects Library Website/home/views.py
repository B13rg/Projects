# -*- coding: utf-8 -*-
from __future__ import unicode_literals

from django.shortcuts import render
from django.template import RequestContext
from django.template import loader
from django.http import HttpResponse

# Create your views here.
def index(request):
    #return HttpResponse("Test tstirnsd bar")
    context = {'message': "This is memessage bar",}
    return render(request, 'index.html', context)
