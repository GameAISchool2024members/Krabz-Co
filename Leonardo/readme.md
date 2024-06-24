# Approaching Leonardo API and More

## Installation

Necessary Python modules:

```text
flask
speech_recognition
pocketsphinx
```

How to install them:

```bash
python -m pip install flask SpeechRecognition pocketsphinx
```

PocketSphinx might need something extra, as per their [web page](https://pypi.org/project/pocketsphinx/). In particular, it needs PortAudio installed.

Debian-based systems use the following, while others should check [PortAudio](http://www.portaudio.com/):

```bash
sudo apt-get install libportaudio2
```

## Setting up the Server

Before using Leonardo.AI API, authentication tokens are needed. In order to add your authentication token, rename the file `credentials template.py` or make your own copy, and add your authentication token under key `auth_token`. This is what it looks like in the end:

```python
credentials = {
    "auth_token": "<API key>",
}
```

For example, using Ubuntu:
```bash
cp "credentials template.py" "credentials.py"
sed -i 's/<API key>/Your-auth-token/g' credentials.py
```

## Running the Local Server

### Run the Server

Open the server running locally at `localhost:8008`:

```bash
python server.py --api
```

### Use Image Generating Endpoint

Endpoint `/get_image` takes JSON data as follows, and returns the absolute path to the generated downloaded image:

```json
{
    "prompt": "rainbow flower"
}
```

cURL example:

```bash
curl --request POST --url http://localhost:8008/get_image --header 'content-type: application/json' --data '{"prompt": "furry lobster"}'
```

In order to ensure smooth generation of images usable by the game Crab It, the applicable initial image must be uploaded first: [this one](init_image.jpg). Image ID (received after using acript agument `--init`) of the uploaded init image must be added to line `102` in `server.py`. Then that whole block (`server.py` lines 100-107, inclusive, the key `controlnets`), should be uncommented.

![alt text](init_image.jpg)

### Use Audio Transcription Endpoint

Endpoint `/transcribe` takes a single string which is a local path to the file to be transcribed (relative or absolute). Returns transcription text as a single string.

```json
{
    "file": "recording.wav"
}
```

cURL example:

```bash
curl --request POST --url http://localhost:8008/transcribe --header 'content-type: application/json' --data '{"file": "recording.wav"}'
```

### Use the Listening Endpoint

Endpoint `/listen` takes whatever gibberish, and returns "Go!" when the word "fire" is recognised.

```json
{
    "whatever": "whatever"
}
```

cURL example:

```bash
curl --request POST --url http://localhost:8008/listen --header 'content-type: application/json' --data '{"name": "Nemo"}'
```

## Using Functions Only

If you do not want to open the server, but generate and download a single image:

```bash
python server.py --image "prompt for the stuff in the cannonball"
```

Simply get an image, based on the Leonardo's generationID:

```bash
python server.py --getimage "Leonardo's generationID"
```
