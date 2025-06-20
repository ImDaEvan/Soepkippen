import os
import cv2
import methods
import tkinter as tk
from tkinter import filedialog, messagebox
from PIL import Image
import math

# Global variables
imgtk = None
firstWebcam = True
location = None
datetime = None





# Displays the iamge on tkinter window
def predict_and_display_image(img):
    global imgtk, panel, window

    # Get aspect ratio of photo
    width = img.shape[1]
    height = img.shape[0]
    aspect_ratio = width / height

    if width / 1000 > height / 1000:
        new_width = min(width, 1000)
        new_height = new_width / aspect_ratio
    else:
        new_height = min(height, 1000)
        new_width = new_height * aspect_ratio

    # Final frame size as integers
    frame_size = (int(new_width), int(new_height))

    img = cv2.resize(img, frame_size)
    #Convert cv2 image to tkinter image
    imgtk = methods.cv2img_to_imgTK(img)

    # Show tkinter image (preview before showing the annotations)
    panel.config(image=imgtk)
    window.geometry(f"{str(imgtk.width())}x{str(imgtk.height() + 50)}")
    panel.update()
    window.update()
    
    # Classify the image
    ai_result = methods.classify_image(img)
    trashItems = methods.PredictionsToTrashItemList(ai_result['predictions'],img)
    if len(trashItems) > 0:
        methods.SendToApi(trashItems)

    # Show image boundary
    fig = methods.show_classification_boundary(img, ai_result, frame_size)
    print(ai_result["predictions"])
    img = methods.fig_to_img(fig, frame_size)
    imgtk = methods.cv2img_to_imgTK(img)

    # Show tkinter image (annotated view)
    panel.config(image=imgtk)
    panel.update()


# Opens and display image file
def open_image():
    global location
    image_path = filedialog.askopenfilename(title="Select Image File", filetypes=[("Image files", "*.png *.jpg *.jpeg *.gif *.bmp *.ico *.webp")])

    if(image_path != ""):
        # Convert image to cv2 image
        pic = cv2.imread(image_path)

        # Get position from image
        img = Image.open(image_path)
        location = methods.get_lonlat_from_photo(img)

        # Classify and display classifications
        predict_and_display_image(pic)

# Opens and displays shot photo
def open_camera():
    global firstWebcam
    # Open dialog telling user the controls
    if(firstWebcam):
        messagebox.showinfo("Controls", "Press space to take a photo, press esc to cancel")
        firstWebcam = False

    image = None
    frame_size = None
    while True:
        # Captures and shows current frame
        frame = methods.capture_camera()
        frame_size = (frame.shape[1], frame.shape[0])  # (width, height)
        frame = cv2.resize(frame, frame_size)
        cv2.imshow('Live Feed', frame)

        # Awaits keypress
        key = cv2.waitKey(1)
        if(key == 32): # Space key  
            image = methods.capture_camera()
            cv2.destroyAllWindows()
            predict_and_display_image(image)
            break

        if(key == 27): # Esc key
            cv2.destroyAllWindows()
            break

# Setup window
window = tk.Tk()
window.title("Image classifier")
window.geometry("300x400")
window.resizable(0,0)

# Button frame
top_frame = tk.Frame(window)
top_frame.pack(side="top", fill="x")

# Drag and drop image file


# Setup file image button
btn_file = tk.Button(top_frame, text="Open file...", command=open_image)
btn_file.pack(side=tk.LEFT, fill="x", expand=True)

# Setup live image button
btn_live = tk.Button(top_frame, text="Open camera...", command=open_camera)
btn_live.pack(side=tk.LEFT, fill="x", expand=True)

panel = tk.Label(window)
panel.pack(side="bottom")
window.mainloop()

exit()

# Check if results contain any classes
