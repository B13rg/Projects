# mysite/routing.py
from channels.routing import ProtocolTypeRouter
from channels.auth import AuthMiddlewareStack
from channels.routing import ProtocolTypeRouter, URLRouter, ChannelNameRouter
import delta.routing
from delta.consumers import ChatConsumer

application = ProtocolTypeRouter({
    # (http->django views is added by default)
	'websocket': AuthMiddlewareStack(
        URLRouter(
            delta.routing.websocket_urlpatterns
        )
    ),
    #'http': AuthMiddlewareStack(
    #    URLRouter(
    #        delta.routing.http_urlpatterns
    #    )
    #),

    "channel": ChannelNameRouter({
        "chat": delta.consumers.ChatConsumer
    })
})

