# -*- coding: utf-8 -*-
from __future__ import unicode_literals

from django.db import models

# Create your models here.
class tracks(models.Model):
	location	=models.CharField(max_length=500)
	description	=models.CharField(max_length=500)
	secs		=models.IntegerField(default=0)
	category	=models.CharField(max_length=500)
	CDNumber	=models.IntegerField(default=-1)
	CDName		=models.CharField(max_length=500)
	tracknum	=models.IntegerField(default=0)
