
import json


fruits = {
    "trashType":"peuk",
    "latitude":0,
          "longitude":0
          }
#fruitsEncoded = json.dumps(fruits)
items = [fruits,fruits]
def SendToApi(trashItems):
    for l in trashItems:
        for i in l:
            print(i,l[i])

SendToApi(items)