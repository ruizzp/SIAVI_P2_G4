import threading
import time
from voice.voice_recognition import start_voice_control
from gesture.gesture_handler import detect_gestures
from language_server import listen_for_language_change

from gesture.utils import send_to_unity

voice_thread = None
voice_thread_stop_flag = threading.Event()
current_language = "pt"

def process_gesture(gesture):
    if gesture == "pinça":
        #send_to_unity("fire")
        print("fire")
    elif gesture == "mão_aberta":
        #send_to_unity("idle")
        print("idle")

def start_voice(language):
    global voice_thread, voice_thread_stop_flag
    voice_thread_stop_flag.clear()
    model_path = f"../VoiceModels/vosk-model-{language}"
    voice_thread = threading.Thread(target=start_voice_control, args=(model_path, voice_thread_stop_flag))
    voice_thread.start()

def language_monitor():
    global current_language, voice_thread, voice_thread_stop_flag
    for new_language in listen_for_language_change():
        if new_language != current_language:
            print(f"🔁 Novo idioma recebido: {new_language}")
            current_language = new_language

            # Parar a thread de voz
            voice_thread_stop_flag.set()
            voice_thread.join()

            # Iniciar novamente com novo idioma
            start_voice(current_language)

if __name__ == "__main__":
    # Começa com o idioma padrão
    start_voice(current_language)

    # Thread para gestos
    gesture_thread = threading.Thread(target=detect_gestures, args=(process_gesture,))
    gesture_thread.start()

    # Thread para escutar alterações de idioma
    lang_thread = threading.Thread(target=language_monitor)
    lang_thread.start()

    # Espera todas as threads
    gesture_thread.join()
    lang_thread.join()