import cv2
import matplotlib.pyplot as plt
import numpy as np
import requests
from inference_sdk import InferenceHTTPClient

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
    fig.canvas.draw()
    img_plot = np.array(fig.canvas.renderer.buffer_rgba())
    img = cv2.resize(cv2.cvtColor(img_plot, cv2.COLOR_RGBA2BGR), frame_size)
    
    cv2.imshow('Live Feed', img)


# Ai looks for objects
def classify_image(image):
    # Loading the model
    CLIENT = InferenceHTTPClient(
    api_url="https://serverless.roboflow.com",
    api_key=open("apikey.txt").read().split(',')[0]
    )

    # Using the model
    print("Checking image....")
    result = CLIENT.infer(image, model_id="object-detection-cxgfe/3")
    return result




# Draws a boundary box around the image
def show_classification_boundary(image, model_result, display_size=None):
    if image is None:
        raise FileNotFoundError("Image not found.")
    
    # Determine output size
    if display_size is None:
        display_size = (image.shape[1], image.shape[0])  # (width, height)

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


def CreateTrashObject(timestamp,trashtype,confidence,longitude,latitude):
    trashItem = {
        "timestamp" :               timestamp, # date object parsed to string
        "type":                     trashtype, # STRING
        "confidence":               confidence, # FLOAT
        "longitude":                longitude, # FLOAT
        "latitude":                 latitude, # FLOAT
    }
    return trashItem


def SendToApi(trashItems):
    url = "http://5.189.173.122:8080";
    api_key = open("apikey.txt").read().split(',')[1]
    jwt = requests.get(url + f"/api/Jwt?key={api_key}").text

    headers = {"Authorization": f"Bearer {jwt}"}

    for l in trashItems:
        for i in l:
            print(i,l[i])