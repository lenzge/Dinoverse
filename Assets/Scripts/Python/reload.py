# deletes an already loaded module
import sys

def delete_module(module):
    if module in sys.modules.keys():
        del sys.modules[module]