from voice.voice_handler import listen_voice
from voice.utils import send_to_unity

def process_voice_command(text):
    comandos_pt = {
        "cima": "move_up",
        "baixo": "move_down",
        "direita": "move_right",
        "esquerda": "move_left",
        "começar": "start",
        "parar" : "stop",
        "jogo um":"game one",
        "um": "one"
        
    }

    comandos_en = {
        "up": "move_up",
        "down": "move_down",
        "left": "move_left",
        "right": "move_right",
        "start": "start",
        "stop" : "stop",
        "game one":"game one",
        "one": "one"
    }

    comandos = {**comandos_pt, **comandos_en}

    for palavra, comando in comandos.items():
        if palavra in text:
            send_to_unity(comando)
            break

def start_voice_control(model_path, stop_flag):
    listen_voice(model_path, process_voice_command, stop_flag)