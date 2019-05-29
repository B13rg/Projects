from django.db import models

def generateMatch(data):
    thing = Match(
        match_id = "2",
        mode = data['map']['mode'],
        map_name = data['map']['name'],
        phase = data['map']['phase'],
        consec_loss_t = data['map']['team_t']['consecutive_round_losses'],
        consec_loss_ct = data['map']['team_ct']['consecutive_round_losses'],
        score_ct = data['map']['team_ct']['score'],
        score_t = data['map']['team_t']['score'],
        timeouts_ct = data['map']['team_ct']['timeouts_remaining'],
        timeouts_t = data['map']['team_t']['timeouts_remaining'],
        game = data['provider']['name'],
        version = data['provider']['version'],
        client_steamid = data['provider']['steamid'],
        timestamp = data['provider']['timestamp']
    )
    return thing

# Create your models here.
class Match(models.Model):
    match_id = models.CharField(max_length=32)
    mode = models.CharField(max_length=50)
    map_name = models.CharField(max_length=50)
    phase = models.CharField(max_length=25)
    consec_loss_t = models.IntegerField()
    consec_loss_ct = models.IntegerField()
    score_ct = models.IntegerField()
    score_t = models.IntegerField()
    timeouts_ct = models.IntegerField()
    timeouts_t = models.IntegerField()
    game = models.CharField(max_length=50)
    version = models.IntegerField()
    client_steamid = models.CharField(max_length=25)
    timestamp = models.IntegerField()

class Round(models.Model):
    match = models.ForeignKey(Match, on_delete=models.CASCADE)
    round =  models.IntegerField()
    win_event = models.CharField(
        max_length=50,
        choices=[
            ('ct_win_defuse','Defuse'),
            ('t_win_bomb','Bomb'),
            ('t_win_elimination','T-Elim'),
            ('ct_win_elimination','CT-Elim'),
        ],
        default=None
    )
    phase = models.CharField(max_length=25)
    phase_time = models.CharField(max_length=25)

class Performance(models.Model):
    match = models.ForeignKey(Match, on_delete=models.CASCADE)
    round =  models.IntegerField()
    steamid = models.CharField(max_length=25)
    round_kills = models.IntegerField()
    round_hs = models.IntegerField()
    round_damage = models.IntegerField()
    equip_val_start = models.IntegerField()
    equip_val_end = models.IntegerField()

#Current state of the scoreboard
class Scoreboard(models.Model):
    match = models.ForeignKey(Match, on_delete=models.CASCADE)
    steamid = models.CharField(max_length=25)
    name = models.CharField(max_length=256)
    team = models.CharField(
        max_length=2,
        choices=[('CT','CT'),('T','T')],
        default='CT',
    )
    kills = models.IntegerField()
    assists = models.IntegerField()
    deaths = models.IntegerField()
    mvps = models.IntegerField()
    score = models.IntegerField()
    adr = models.FloatField()
    money = models.IntegerField()

#Deals with current state of player.  Data stored between rounds in the Round table.
class Player(models.Model):
    steamid = models.CharField(max_length=25)
    name = models.CharField(max_length=256)
    match = models.ForeignKey(Match, on_delete=models.CASCADE)
    spectating = models.BooleanField()
    observer_slot = models.IntegerField()
    team = models.CharField(
        max_length=2,
        choices=[('CT','CT'),('T','T')],
        default='CT',
    )
    health = models.IntegerField()
    armor = models.IntegerField()
    helmet = models.BooleanField()
    flashed = models.IntegerField()
    smoked = models.IntegerField()
    burning = models.IntegerField()
    money = models.IntegerField()    
    equip_val_cur = models.IntegerField()
    round_kills = models.IntegerField()
    round_hs = models.IntegerField()
    round_damage = models.IntegerField()