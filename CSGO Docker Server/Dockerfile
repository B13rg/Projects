FROM ubuntu

ENV USER csgo
ENV HOME /home/$USER
ENV SERVER $HOME/hlserver

RUN apt-get -y update \
    && apt-get -y upgrade \
    && apt-get -y install lib32gcc1 curl net-tools lib32stdc++6 locales \
    && locale-gen en_US.UTF-8 \
    && update-locale LANG=en_US.UTF-8 LANGUAGE=en_US.UTF-8 LC_ALL=en_US.UTF-8 \
    && dpkg-reconfigure --frontend=noninteractive locales \
    && apt-get clean && rm -rf /var/lib/apt/lists/* /tmp/* /var/tmp/* \
	 && adduser --home $HOME $USER \
&& mkdir $SERVER

ENV LC_ALL en_US.UTF-8
ENV LANG en_US.UTF-8
ENV LANGUAGE en_US.UTF-8

ADD ./csgo_ds.txt $SERVER/csgo_ds.txt
ADD ./autoexec.cfg $SERVER/csgo/csgo/cfg/autoexec.cfg
ADD ./server.cfg $SERVER/csgo/csgo/cfg/server.cfg
ADD ./csgo.sh $SERVER/csgo.sh
ADD ./banned_user.cfg $SERVER/csgo/csgo/cfg/banned_user.cfg
ADD ./banned_ip.cfg $SERVER/csgo/csgo/cfg/banned_ip.cfg
Add ./gamestate_integration_Iceberg.cfg $SERVER/csgo/csgo/cfg/gamestate_integration_Iceberg.cfg
ADD ./update.sh $SERVER/update.sh

RUN chown -R $USER:$USER $SERVER
RUN chmod +x $SERVER/update.sh
RUN chmod +x $SERVER/csgo.sh

USER $USER
RUN curl -sQL "http://media.steampowered.com/client/steamcmd_linux.tar.gz" | tar -C $SERVER -xvz
RUN bash $SERVER/update.sh

EXPOSE 27015/udp

WORKDIR /home/$USER/hlserver
#ENTRYPOINT ["./csgo.sh"]