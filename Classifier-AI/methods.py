import cv2
import matplotlib.pyplot as plt
import numpy as np
import requests
from inference_sdk import InferenceHTTPClient
from PIL import Image, ImageTk, ExifTags
from discord_webhook import DiscordEmbed,DiscordWebhook
import random
import datetime
import time
global cap

# Takes photo
cap = cv2.VideoCapture(0)
def capture_camera():
    if not cap.isOpened():
        raise("Cannot open camera")
    
    # Capture a single frame
    ret, frame = cap.read()

    if not ret:
        raise("Can't receive frame (stream end?). Exiting ...")
    else:
       return frame
    

# Displays the image
def show_img_from_fig(fig, frame_size):
    img = fig_to_img(fig, frame_size)
    cv2.imshow('Live Feed', img)


# cv2 to imageTK
def cv2img_to_imgTK(cv2Img):
    b,g,r = cv2.split(cv2Img)
    img = cv2.merge((r,g,b))
    img = Image.fromarray(img)
    return ImageTk.PhotoImage(image=img)


# Figure to image
def fig_to_img(fig, frame_size):
    fig.canvas.draw()
    img_plot = np.array(fig.canvas.renderer.buffer_rgba())
    img = cv2.resize(cv2.cvtColor(img_plot, cv2.COLOR_RGBA2BGR), frame_size)
    
    return img

# Gets position data from image
def get_spacetime_from_photo(img):
    try:
        exif_data = img._getexif()
    
        if not exif_data:
            return (None, None, None)
        
        # Map exif tag numbers to tag names
        exif = {
            ExifTags.TAGS.get(tag): value
            for tag, value in exif_data.items()
            if tag in ExifTags.TAGS
        }

        timestamp = exif["DateTime"]
        timestamp = datetime.datetime.strptime(timestamp, '%Y:%m:%d %H:%M:%S').strftime('%Y-%m-%dT%H:%M:%S.%f')[:-3] + 'Z'


        gps_info = exif.get("GPSInfo")
        if not gps_info:
            return (None, None, None)

        # map GPSInfo keys from integers to names
        gps_tags = {}
        for key in gps_info.keys():
            decoded = ExifTags.GPSTAGS.get(key)
            gps_tags[decoded] = gps_info[key]

        def convert_to_degrees(value):
            d, m, s = value
            return float(d) + float(m)/60 + float(s)/3600

        lat = convert_to_degrees(gps_tags["GPSLatitude"])
        lat_ref = gps_tags["GPSLatitudeRef"]
        lon = convert_to_degrees(gps_tags["GPSLongitude"])
        lon_ref = gps_tags["GPSLongitudeRef"]

        if lat_ref != 'N':
            lat = -lat
        if lon_ref != 'E':
            lon = -lon

        return lat, lon, timestamp
    except:
        return (None, None, None)

# Ai looks for objects
def classify_image(image):
    # Loading the model
    CLIENT = InferenceHTTPClient(
    api_url="https://serverless.roboflow.com",
    api_key=open("apikey.txt").read().split(',')[0]
    )

    # Using the model
    print("Checking image....")
    result = CLIENT.infer(image, model_id="trash-detection-qksx6/2")
    return result


# Draws a boundary box around the image
def show_classification_boundary(image, model_result, display_size):
    if image is None:
        raise FileNotFoundError("Image not found.")

    # Resize image to display size
    image_resized = cv2.resize(image, display_size)

    # Convert BGR to RGB for matplotlib
    image_rgb = cv2.cvtColor(image_resized, cv2.COLOR_BGR2RGB)

    # Create a figure with fixed pixel size (optional but cleaner)
    dpi = 100
    fig, ax = plt.subplots(figsize=(display_size[0]/dpi, display_size[1]/dpi), dpi=dpi)

    ax.imshow(image_rgb)

    # Get original model size (the size before resizing)
    orig_w = model_result['image']['width']
    orig_h = model_result['image']['height']

    # Calculate scaling factors
    scale_x = display_size[0] / orig_w
    scale_y = display_size[1] / orig_h

    for pred in model_result['predictions']:
        # Scale bbox coordinates
        xmin = int((pred['x'] - pred['width'] / 2) * scale_x)
        ymin = int((pred['y'] - pred['height'] / 2) * scale_y)
        xmax = int((pred['x'] + pred['width'] / 2) * scale_x)
        ymax = int((pred['y'] + pred['height'] / 2) * scale_y)

        ax.add_patch(plt.Rectangle((xmin, ymin), xmax - xmin, ymax - ymin,
                                   edgecolor='yellow', fill=False, linewidth=3))

        label = f"{pred['class']}: {pred['confidence']:.2f}"
        ax.text(xmin, ymin-1, label, color='yellow', fontsize=12, backgroundcolor='none')

    ax.axis('off')
    plt.tight_layout()
    return fig

def create_fake_location():
    lon = random.uniform(51.607389,51.564873)
    lat = random.uniform(4.727335,4.819141)
    location = (lon, lat)
    return location

def get_current_time():
    ts = time.time()
    dt = datetime.datetime.fromtimestamp(ts).strftime('%Y-%m-%dT%H:%M:%S.%f')[:-3] + 'Z'
    return dt

def PredictionsToTrashItemList(predictions,location,timestamp):
    trashItems = []
    if location == None or location == (None, None):
        location = create_fake_location()
    if timestamp == None:
        timestamp = get_current_time()
    for v in predictions:
        #[{'x': 421.5, 'y': 358.5, 'width': 251.0, 'height': 241.0, 'confidence': 0.5407358407974243, 'class': 'Hands', 'class_id': 18, 'detection_id': '4c342206-1d61-44ca-8e8e-1623d73ba99c'}]
        confidence = v['confidence']
        trashtype = v['class']
        
        #trashItems[len(trashItems)] = CreateTrashObject("",type,confidence,"","")
        trashItems.append(CreateTrashObject(timestamp,trashtype,confidence,location))
    return trashItems


# Generates dummy data
def GenerateTrashItems(count, hour_offset_from_now):
    trashItems = []
    for _ in range(count):
        trashItems.append(CreateTrashObject(
            (datetime.datetime.now() + datetime.timedelta(hours=int(random.uniform(hour_offset_from_now, 0)))).strftime('%Y-%m-%dT%H:%M:%S.%f')[:-3] + 'Z',
            random.choice(["can", "cardboard", "cigarette_butt", "paper_cup", "pet_bottle", "plastic"]),
            random.uniform(0.5, 0.99),
            create_fake_location()
        ))
    return trashItems

def CreateTrashObject(timestamp,trashtype,confidence,location):
    trashItem = {
        "timestamp" :               timestamp, # date object parsed to string
        "type":                     trashtype, # STRING
        "confidence":               confidence, # FLOAT
        "longitude":                location[0], # FLOAT
        "latitude":                 location[1], # FLOAT
    }
    return trashItem


def SendToApi(trashItems):
    url = "http://5.189.173.122:8080"
    api_key = open("apikey.txt").read().split(',')[1]

    # webhook = DiscordWebhook(url="https://discord.com/api/webhooks/1384960115274158240/aVAhNmaRVT-aR_OydhPMUH_mI81t8DRQdZilmnzSpU8Wy_migHj3DN9LpvQV40wo_uGt")
    # for v in trashItems:
    #     print(v)
    #     embed = DiscordEmbed(title=f"Trash item:{v['type']}", description=f"Timestamp:{v['timestamp']}\n Confidence score:{v['confidence']}\n Longitude:{v['longitude']}\n Latitude:{v['latitude']}",color="8efa46")
    #     webhook.add_embed(embed)
    
    jwt = requests.get(url + f"/api/Jwt?key={api_key}").text
    headers = {"Authorization": f"Bearer {jwt}"}
    #response = webhook.execute()
    request = requests.post(url + "/api/trash",headers=headers,json=trashItems)
    print(request.json)