# %% necessary imports

import requests
import json
import time
import logging
import argparse
import os
import re

from flask import Flask, request, jsonify

import speech_recognition as sr

from pocketsphinx import LiveSpeech

# customised list of profanities in profanities.py file
from profanities import profanities as obscene_words
from credentials import credentials

# %% data for the API calls

url = "https://cloud.leonardo.ai/api/rest/v1/generations"
url_init = "https://cloud.leonardo.ai/api/rest/v1/init-image"

authorisation = f"Bearer: {credentials.get('auth_token_alt')}"

default_listen_keyword = "fire"

# %% additional stuff

# log everything needed (especially for debugging) in a file
logging.basicConfig(
    level=logging.DEBUG,  # Set the logging level
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s',  # Set the logging format
    datefmt='%Y-%m-%d %H:%M:%S',  # Set the date format
    handlers=[
        logging.FileHandler("app.log"),  # Log to a file
        # logging.StreamHandler()  # Log to the console
    ]
)

# %% flask app

app = Flask(__name__)
app.config['SECRET_KEY'] = 'secret!'

# %% speech recogniser

recogniser = sr.Recognizer()

# %% profanity filter

def contains_obscene_word(text: str):
    """Check if `text` string contains any of the obscene words.

    Args:
        `text` (str): String to check for profanity.

    Returns:
        bool: True if an obscene word was found in `text`, otherwise False.
    """

    text = text.lower()
    for word in obscene_words:
        if re.search(r'\b' + re.escape(word) + r'\b', text):
            return True
    return False


# %% generating an image

def generate_image(prompt: str="a cute otter", numer_of_images: int=1) -> str:
    """Use Leonardo.ai API to generate an image based on `prompt`. The prompt is added to the predefined string `trapped in a glass ball` to ensure a ball is received every time, regardless of the prompt.

    Args:
        `prompt` (str, optional): the prompt for the generated image. Defaults to `a cute otter`.

    Returns:
        str: ID of the generation task of Leonardo.ai, which is a unique identifier of the generation task, and necessary for retrieving the generated image via API.
    """

    logging.info("Starting image generation!")

    if contains_obscene_word(prompt):
        prompt = "rainbow flower"

    prompt = f"{prompt} trapped in a glass ball"

    payload = {
        "num_images": numer_of_images,
        "height": 512,
        "width": 512,
        "modelId": "b24e16ff-06e3-43eb-8d33-4416c2d75876",
        "sd_version": "SDXL_LIGHTNING",
        "scheduler": "LEONARDO",
        "prompt": prompt,
        # "controlnets": [
        #         {
        #             "initImageId": "63e9a774-baeb-4c11-93fc-09acf5b24db6",
        #             "initImageType": "UPLOADED",
        #             "preprocessorId": 19,
        #             "weight": 1
        #         }   
        #     ],
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

    logging.debug(response.text)

    data = json.loads(response.text)
    generationID = data.get('sdGenerationJob').get('generationId')

    logging.debug(f"Generation ID: {generationID}")

    return generationID

# %% download the image

import requests
# generationID = "0cd82f30-ddaa-4faf-bb65-a960c9df1cbf"

def download_file(url: str, local_filename: str) -> str:
    """Download file from the `url` and save it using the `local_filename`. Return the absolute path of the file.

    Args:
        url (str): URL of the file to be downloaded.
        local_filename (str): Name of the file to be saved.

    Returns:
        str: Absolute path to the saved file, including the saved file's name and extension.
    """

    with requests.get(url, stream=True) as r:
        r.raise_for_status()
        with open(local_filename, 'wb') as f:
            for chunk in r.iter_content(chunk_size=8192): 
                f.write(chunk)

    absolute_file_path = os.path.abspath(local_filename)

    logging.info(f"Image {absolute_file_path} saved!")
    
    return absolute_file_path

# %% get the image data

def get_image(generationID: str) -> list:
    """Use Leonardo.ai API to get details of the image generation process identified by the given `generationID`. Image URL of only the first image is retrieved by default.

    Args:
        generationID (str): ID of the generation task to retrieve images of.

    Returns:
        list: List of image URLs of the referenced generation task.
    """

    headers = {
        "accept": "application/json",
        "authorization": authorisation,
    }

    response = requests.get(url + f"/{generationID}", headers=headers)

    data = json.loads(response.text)

    images = data.get("generations_by_pk").get("generated_images")

    while not images:
        time.sleep(1)

        response = requests.get(url + f"/{generationID}", headers=headers)

        data = json.loads(response.text)


        images = data.get("generations_by_pk").get("generated_images")

    logging.debug(json.dumps(data, indent=2))

    image_urls = [image.get("url") for image in images]
    logging.debug(f"Images in the generation task: {image_urls}")

    # output_image_name = f"leo-{generationID}.jpg"

    return image_urls

    return download_file(image_url, output_image_name)

# %% upload init image

def upload_init_image():
    """Upload the init image to Leonardo.ai API. Init image can be used as a guide in generating images. This function uses a hardcoded name of the image to be uploaded. Image ID of the uploaded image is logged for purposes of later using the uploaded init image. A special URL is received upon contacting the API. This URL is then used for uploading the init image.
    """

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
def generate_and_download_image():
    """Receive a prompt as JSON-formatted `data` in form of `{"prompt": "rubber ducks"}`. Return the absolute path of the downloaded generated image.

    Raises:
        e: Log error if no key `prompt` is provided in `data` or if anything happens in the process of generating the image.

    Returns:
        str: Returns the absolute path of the downloaded generated image. If key `prompt` is not present in `data`, or there was an error in working with the API, return JSON with key `error` and an error message.
    """    
    data = request.json
    if 'prompt' in data:
        try:
            generationID = generate_image(data['prompt'])
            image_urls = get_image(generationID)
            image_paths = map(download_file, image_urls, [f"leo-{generationID}-{num}.jpg" for num in range(len(image_urls))])
        except Exception as e:
            logging.error(e)
            return jsonify({'error': 'Error in working with API'}), 400
            # raise e
        return list(image_paths)[0]
    else:
        return jsonify({'error': 'No message found'}), 400

@app.route("/transcribe", methods=["POST"])
def transcribe():
    """Transcribe an audio file and return the transcription. Local path to the file is provided as a JSON-formatted `data` using key `file`, e.g. `{"file": "recording.wav"}`. The transcription is returned as a simple string.

    Raises:
        e: Logs the error in the log file, if an error occurs.

    Returns:
        str: Simple string containing the transcript of the referenced audio file.
    """    
    data = request.json

    if not data.get('file'):
        return jsonify({"error": "No file path provided"}), 400

    if not os.path.isfile(data.get('file')):
        return jsonify({"error": "Provided is not an existing file path"}), 400

    file_path = data.get("file")
    file_path = os.path.abspath(file_path)
    logging.info(f"File to transcribe: {file_path}")

    try:
        sound_file = sr.AudioFile(file_path)
        with sound_file as source:
            audio = recogniser.record(source)

        result = recogniser.recognize_google(audio)
        logging.info(f"Transcription: {result}")
        
    except Exception as e:
        logging.error(e)
        # raise e
    
    return result

@app.route("/listen", methods=["POST"])
def listen():
    """Open the microphone, and listen to the sounds. Transcribe those sounds automatically. If keyword is recognised, return simple string `Go!`.

    Returns:
        str: "Go!"
    """    
    speech = LiveSpeech(keyphrase=default_listen_keyword, kws_threshold=1e-9)
    for phrase in speech:
        logging.info(phrase.segments(detailed=True))
        return "Go!"


# %% main part

parser = argparse.ArgumentParser(description="Example script for Leonardo API")

parser.add_argument('--generate', action='store_true', help='Generate an image, but do not download it')
parser.add_argument('--init', action='store_true', help="Upload init image")
parser.add_argument('--api', action='store_true', help="Start the API")
parser.add_argument('--getimage', type=str, required=False, help='Download the image/s based on the ID of the generation process of the image')
parser.add_argument('--image', type=str, required=False, help='Generate image based on the prompt and download it')

args = parser.parse_args()

if __name__ == "__main__":
    if args.api:
        app.run(debug=False, host="0.0.0.0", port="8008")
    
    if args.generate:
        generate_image()

    if args.image:
        generationID = generate_image(args.image)
        image_urls = get_image(generationID)
        image_paths = map(download_file, image_urls, [f"leo-{generationID}-{num}.jpg" for num in range(len(image_urls))])
        logging.debug(list(image_paths))


    if args.getimage:
        image_urls = get_image(args.getimage)
        image_paths = map(download_file, image_urls, [f"leo-{generationID}-{num}.jpg" for num in range(len(image_urls))])
        logging.debug(list(image_paths))

    if args.init:
        upload_init_image()
