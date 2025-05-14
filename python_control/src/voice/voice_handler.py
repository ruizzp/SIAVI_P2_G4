from vosk import Model, KaldiRecognizer
import sounddevice as sd
import queue
import json

q = queue.Queue()

def callback(indata, frames, time, status):
    q.put(bytes(indata))

def listen_voice(model_path, on_command, stop_flag):
    model = Model(model_path)
    rec = KaldiRecognizer(model, 16000)

    with sd.RawInputStream(samplerate=16000, blocksize=8000, dtype='int16',
                           channels=1, callback=callback):
        print("🎙️ Escutando com modelo:", model_path)
        while not stop_flag.is_set():
            data = q.get()
            if rec.AcceptWaveform(data):
                result = json.loads(rec.Result())
                if 'text' in result and result['text']:
                    print(result['text'])
                    # Dividir o texto em palavras
                    words = result['text'].split()

                    # Verificar se o número de palavras é maior que 3
                    if len(words) <= 3:
                        on_command(result['text'])
                    else:
                        print("Ignorando comando, mais de 3 palavras detectadas:", result['text'])