from flask import Flask

app = Flask(import_name=__name__)


@app.route('/')
def init() -> str:
    return "Hello! This is python module responsible for connecting with Google Play Music!"


app.run(host="127.0.0.1", port=42069)
