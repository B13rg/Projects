# chat/routing.py
from django.conf.urls import url

from . import consumers

websocket_urlpatterns = [
    url(r'^ws/delta/(?P<room_name>[^/]+)/$', consumers.ChatConsumer),
]

http_urlpatterns = [
    url('', consumers.Ingest),
]