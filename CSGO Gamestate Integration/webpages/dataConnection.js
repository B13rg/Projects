var ws = new WebSocket("ws://127.0.0.1:5678/"),
    messages = document.createElement('ul');
ws.onmessage = function(event) {
    document.getElementById('dump').innerText = event.data
    var updateData = JSON.parse(event.data);
    setMatchData(updateData);
    setRoundData(updateData);
    setPlayerData(updateData);
    document.getElementById('position').innerText = updateData['playerItem'].position;
};

function setMatchData(inputData) {
    updateData = inputData['matchItem'];
    document.getElementById('mode').innerHTML = updateData.mode;
    document.getElementById('mapName').innerText = updateData.map_name;
    document.getElementById('phase').innerHTML = updateData.phase;
    document.getElementById('scoreCT').innerHTML = updateData.score_ct;
    document.getElementById('scoreT').innerHTML = updateData.score_t;
    document.getElementById('timestamp').innerHTML = updateData.timestamp;
}

function setRoundData(inputData) {
    updateData = inputData['roundItem'];
    document.getElementById('roundNum').innerHTML = updateData.roundNum;
    document.getElementById('phase').innerHTML = updateData.phase;
    document.getElementById('phaseTime').innerHTML = updateData.phaseTime;
}

function setPlayerData(inputData) {
    updateData = inputData['playerItem'];
    if (updateData.team == "ct") {
        document.getElementById('team').style = "background-color:#327cad;"
    } else {
        document.getElementById('team').style = "background-color:#bc8302;"
    }
    document.getElementById('name').innerHTML = updateData.name;

    document.getElementById('kills').innerHTML = updateData.kills;
    document.getElementById('assists').innerHTML = updateData.assists;
    document.getElementById('deaths').innerHTML = updateData.deaths;
    document.getElementById('mvps').innerHTML = updateData.mvps;
    document.getElementById('score').innerHTML = updateData.score;

    document.getElementById('health').innerHTML = updateData.health;
    document.getElementById('armor').innerHTML = updateData.armor;
    document.getElementById('helmet').innerHTML = updateData.helmet;
    document.getElementById('money').innerHTML = updateData.money;
    document.getElementById('round_kills').innerHTML = updateData.round_kills;
    document.getElementById('round_killhs').innerHTML = updateData.round_killhs;
    document.getElementById('round_totaldmg').innerHTML = updateData.round_totaldmg;
    document.getElementById('equip_value').innerHTML = updateData.equip_value;
    document.getElementById('position').innerHTML = updateData.position;
}

window.onbeforeunload = function() {
    ws.onclose = function() {}; // disable onclose handler first
    ws.close();
};