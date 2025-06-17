
import json
import random
import math
def CreateTrashObject(timestamp,trashtype,confidence,longitude,latitude,feel_temp,actual_temp,wind_force,wind_direction):
    trashItem = {
        "timestamp" :               timestamp or  , # date object parsed to string
        "type":                     trashtype, # STRING
        "confidence":               confidence, # FLOAT
        "longitude":                longitude, # FLOAT
        "latitude":                 latitude, # FLOAT
        "feels_like_temp_celsius":  feel_temp, # FLOAT?
        "actual_temp_celsius":      actual_temp, # FLOAT?
        "wind_force_bft":           wind_force, # FLOAT?
        "wind_direction_degrees":   wind_direction # FLOAT?
    }
    return trashItem

#fruitsEncoded = json.dumps(fruits)

def SendToApi(trashItems):
    for l in trashItems:
        for i in l:
            print(i,l[i])

SendToApi(items)

Images = ["1","2"]
for i in Images:
    trashItems = []
    trashItems[trashItems.count()+1] = CreateTrashObject()
