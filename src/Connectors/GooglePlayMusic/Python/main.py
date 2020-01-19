import gmusicapi

from gpm_connector import GPMConnector

app = GPMConnector(import_name=__name__)

manager = gmusicapi.Musicmanager()
IS_AUTHENTICATED: bool = False


@app.route('/')
def init() -> str:
    return "Hello! This is python module responsible for connecting with Google Play Music!"


@app.route('/auth', methods=['GET', 'POST'])
def auth():
    global IS_AUTHENTICATED
    if manager.is_authenticated():
        IS_AUTHENTICATED = True
        return "AUTHENTICATED", 201

    manager.perform_oauth(open_browser=True, storage_filepath="./_oauth")

    return "AUTHENTICATED", 201


@app.route('/health')
def health() -> (str, int):
    return "Healthy", 200


@app.route('/search', methods=['POST'])
def search() -> [str]:
    return []


if __name__ == '__main__':
    app.run(host="127.0.0.1", port=420_69, debug=False)
