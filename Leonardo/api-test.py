# %% necessary imports

import requests
import json
import time
import logging
import argparse
import os
import re

# from PIL import Image
# import numpy as np

from flask import Flask, request, jsonify

# %% data for the API calls

url = "https://cloud.leonardo.ai/api/rest/v1/generations"
url_init = "https://cloud.leonardo.ai/api/rest/v1/init-image"

auth_token = "0f93149b-5a5f-41be-8bc3-47ae8010cd01"
auth_token_alt = "51bcc764-d533-4784-97f2-19c701939f8a"

authorisation = f"Bearer: {auth_token}"

# %% additional stuff

logging.basicConfig(
    level=logging.INFO,  # Set the logging level
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s',  # Set the logging format
    datefmt='%Y-%m-%d %H:%M:%S',  # Set the date format
    handlers=[
        logging.FileHandler("app.log"),  # Log to a file
        # logging.StreamHandler()  # Log to the console
    ]
)

# %% flask socket app

app = Flask(__name__)
app.config['SECRET_KEY'] = 'secret!'

# def start_tcp_server():
#     server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
#     server_socket.bind(('0.0.0.0', 8010))
#     server_socket.listen(5)
#     print("TCP server listening on port 8010")

#     while True:
#         client_socket, addr = server_socket.accept()
#         logging.info(f"Connection from {addr}")
#         handle_client(client_socket)

# def handle_client(client_socket):
#     with client_socket:
#         while True:
#             data = client_socket.recv(1024)
#             if not data:
#                 break
#             data = data.decode('utf-8')
#             logging.info(f"Received: {data}")
#             response = generate_image(data)
#             client_socket.sendall(response)

# %% profanity filter

obscene_words = [
    "fuck", "shit", "damn", "hell", "ass", "bitch", "bastard", "sex", "porn",
    "blowjob", "cock", "pussy", "tits", "dick", "dildo", "cum", "orgasm",
    "masturbation", "hentai", "anal", "fucker", "slut", "whore", "nigger",
    "chink", "spic", "kike", "gook", "cracker", "wetback", "faggot", "dyke",
    "weed", "cocaine", "heroin", "meth", "ecstasy", "crack", "LSD", "acid",
    "retard", "cripple", "midget", "fag", "queer", "homo", "tranny", "boobie", "boobs", "boob", "hentai", "boobies"
]

def contains_obscene_word(text):
    text = text.lower()
    for word in obscene_words:
        if re.search(r'\b' + re.escape(word) + r'\b', text):
            return True
    return False



# %% generating an image

def generate_image(prompt: str=None):

    logging.info("######### Starting image generation!")

    if prompt:
        if contains_obscene_word(prompt):
            prompt = "rainbow flower"
        prompt = f"{prompt} trapped in a glass ball"
    else:
        prompt = "skeleton monkey trapped in a glass ball"

    payload = {
        "num_images": 1,
        "height": 512,
        "width": 512,
        # "modelId": "2067ae52-33fd-4a82-bb92-c2c55e7d2786",
        "modelId": "b24e16ff-06e3-43eb-8d33-4416c2d75876",
        "sd_version": "SDXL_LIGHTNING",
        "prompt": prompt,
        "alchemy": True,
        "controlnets": [
                {
                    "initImageId": "63e9a774-baeb-4c11-93fc-09acf5b24db6",
                    "initImageType": "UPLOADED",
                    "preprocessorId": 19,
                    "weight": 1
                }   
            ],
        "elements": [
            {
                "akUUID": "d5d5ce55-5f3b-4bea-bb37-df6a4b5f3519",
                "weight": 1
            }
        ],
        "num_inference_steps": 10,
    }

    headers = {
        "accept": "application/json",
        "content-type": "application/json",
        "authorization": authorisation,
    }

    timeout = 15

    response = requests.post(url, json=payload, headers=headers, timeout=timeout)

    # time.sleep(timeout)

    logging.debug(response.text)

    data = json.loads(response.text)
    generationID = data.get('sdGenerationJob').get('generationId')

    logging.debug(f"Generation ID: {generationID}")

    return get_image(generationID)

# %% getting the image

# generationID = "0cd82f30-ddaa-4faf-bb65-a960c9df1cbf"

def download_file(url, local_filename):
    with requests.get(url, stream=True) as r:
        r.raise_for_status()
        with open(local_filename, 'wb') as f:
            for chunk in r.iter_content(chunk_size=8192): 
                f.write(chunk)

    absolute_file_path = os.path.abspath(local_filename)

    logging.info(f"##### Image {absolute_file_path} saved!")
    
    return absolute_file_path

def get_image(generationID):
    headers = {
        "accept": "application/json",
        "authorization": authorisation,
    }

    response = requests.get(url + f"/{generationID}", headers=headers)

    data = json.loads(response.text)

    logging.debug(data)

    images = data.get("generations_by_pk").get("generated_images")

    while not images:
        time.sleep(1)

        response = requests.get(url + f"/{generationID}", headers=headers)

        data = json.loads(response.text)

        logging.debug(data)

        images = data.get("generations_by_pk").get("generated_images")

    image_url = images[0].get("url")

    logging.debug(image_url)

    output_image_name = f"leo-{generationID}.jpg"

    logging.debug(json.dumps(data, indent=2))

    return download_file(image_url, output_image_name)

    return crop_image_with_mask(output_image_name, "mask.png", "masked_" + output_image_name.split(".")[0] + ".png")

# %% apply mask

def crop_image_with_mask(image_path, mask_path, output_path):
    # Open the image and the mask
    image = Image.open(image_path).convert("RGBA")
    mask = Image.open(mask_path).convert("L")  # Convert mask to grayscale

    # Ensure the mask is binary (0 or 255)
    mask = np.array(mask)
    mask = np.where(mask > 128, 255, 0).astype(np.uint8)

    # Create a new image with an alpha channel to apply the mask
    image_array = np.array(image)
    alpha_channel = mask

    # Add the alpha channel to the image
    image_array[:, :, 3] = alpha_channel

    # Convert the result back to an image
    cropped_image = Image.fromarray(image_array)

    # Save the result
    cropped_image.save(output_path)

    return output_path


# %% upload init image

def upload_init_image():

    payload = {"extension": "jpg"}

    headers = {
        "accept": "application/json",
        "content-type": "application/json",
        "authorization": authorisation
    }

    response = requests.post(url_init, json=payload, headers=headers)

    logging.debug(response.status_code)

    fields = json.loads(response.json()['uploadInitImage']['fields'])
    url = response.json()['uploadInitImage']['url']
    image_id = response.json()['uploadInitImage']['id']  # For getting the image later
    logging.debug(f"Init image ID: {image_id}")

    image_file_path = "init_image.jpg"
    files = {'file': open(image_file_path, 'rb')}

    response = requests.post(url, data=fields, files=files) # Header is not needed

    logging.debug(response.status_code)

# %% API endpoints

@app.route('/get_image', methods=['POST'])
def echo():
    data = request.json
    if 'prompt' in data:
        image_path = generate_image(data['prompt'])
        # return jsonify({'response': image_path})
        return image_path
        # return jsonify(image_path)
    else:
        return jsonify({'error': 'No message found'}), 400


# %% main part

parser = argparse.ArgumentParser(description="Example script for Leonardo API")

parser.add_argument('--generate', action='store_true', help='Generate and get an image')
parser.add_argument('--init', action='store_true', help="Upload init image")
parser.add_argument('--api', action='store_true', help="Start the API")
parser.add_argument('--getimage', type=str, required=False, help='ID of the image to get')
parser.add_argument('--image', type=str, required=False, help='Prompt for the image to generate')

args = parser.parse_args()

if __name__ == "__main__":
    if args.api:
        # tcp_thread = threading.Thread(target=start_tcp_server)
        # tcp_thread.daemon = True
        # tcp_thread.start()

        # app.run(debug=True)
        app.run(debug=True, host="0.0.0.0", port="8008")
    
    if args.generate:
        generate_image()

    if args.image:
        generate_image(args.image)

    if args.getimage:
        get_image(args.getimage)

    if args.init:
        upload_init_image()
