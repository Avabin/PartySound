from flask import Flask


class GPMConnector(Flask):
    def run(self, host=None, port=None, debug=None, load_dotenv=True, **options):
        print("Google Play Music connector started!")
        super(GPMConnector, self).run(host=host, port=port, debug=debug, load_dotenv=load_dotenv, **options)