# Approaching Leonardo API

Needed Python modules:

```text
flask
```

Can have these Python modules, but they're commented out ATM:

```text
numpy
pillow
```

How to install them:

```bash
python -m pip install flask pillow numpy
```

## Running the Local Server

Open the server running locally at `localhost:8008`:

```bash
python api-test.py --api
```

Only one open endpoint `/get_image` that takes JSON data as follows:

```json
{
    "prompt": "rainbow flower"
}
```

cURL example:

```bash
curl --request POST --url http://localhost:8008/get_image --header 'content-type: application/json' --data '{"prompt": "furry lobster"}'
```

The response is the absolute path to the generated image.

Do not open the server, but generate and download a single image:

```bash
python api-test.py --image "prompt for the stuff in the cannonball"
```

Simply get an image, based on the Leonardo's generationID:

```bash
python api-test.py --getimage "Leonardo's generationID"
```
