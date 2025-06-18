import os
import cv2
import methods

while True:
    # Capture photo
    image = None
    frame_size = None
    while True:
        frame = methods.capture_camera()
        frame_size = (frame.shape[1], frame.shape[0])  # (width, height)
        frame = cv2.resize(frame, frame_size)
        cv2.imshow('Live Feed', frame)

        key = cv2.waitKey(1)
        if(key == 32): # Space key  
            image = methods.capture_camera()
            break

        if(key == 27): # Space key
            print("exiting...")
            exit()


    # Classify the image
    ai_result = methods.classify_image(image)
    print(ai_result["predictions"])

    # Show image boundary
    fig = methods.show_classification_boundary(image, ai_result, frame_size)
    methods.show_img_from_fig(fig, frame_size) 


    # Wait untill user presses key again to restart the process
    while True:
        key = cv2.waitKey(1)

        if(key == 32): # Space key
            break

        if(key == 27): # Space key
            print("exiting...")
            exit()
    cv2.destroyAllWindows()
    continue # Repeat from the top of the program

    methods.SendToApi([])
    # Check if results contain any classes
    Images = ["1","2"]
    for i in Images:
        trashItems = []
        trashItems[trashItems.count()+1] = methods.CreateTrashObject()
